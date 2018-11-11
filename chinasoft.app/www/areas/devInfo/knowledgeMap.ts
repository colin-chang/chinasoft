interface KonvaElement {
    on: (events: string, handler: Function) => void;
    rotation: (angle: number) => void;
    show: Function;
    hide: Function;
}

interface KonvaLine extends KonvaElement {
    points: (points?: number[]) => number[];
}

interface KnowledgeNodeGroup {
    y: (y?: number) => number;
    add: (ele: Object, ...rest: Object[]) => void;
    getNode: () => KnowledgeNode;
    on: (events: string, handler: Function) => void;
    findOne: (selector: string) => KonvaElement;
    destroyChildren: Function;
    getLayer: Function;
    children: KnowledgeNodeGroup[];
}
interface KnowledgeNodeOption {
    x?: number;
    y?: number;
    model: KnowledgeNodeModel;
    width?: number;
    maxWidth?: number;
    height?: number;
    hSeper?: number;
    vSeper?: number;
    expFill?: string;
    radius?: number;
    fontSize?: number;
    fontColor?: string;
    lineColor?: string;
    presuffixFill?: string;//前后缀背景填充
    presuffixContentFill?: string;//前后缀内容填充
    presuffixPadding?: number;//前后缀填充
    expandRoot?: Function;//展开根节点事件处理函数
    collapseRoot?: Function;//折叠根节点事件处理函数
    drawBoms?: Function;//绘制Boms
    destroyBoms?: Function;//销毁Boms
    drawExps?: Function;//绘制Exps
    destroyExps?: Function;//销毁Exps
    parentNode?: KnowledgeNode;//父节点
}
interface KnowledgeNodeModel {
    name?: string;
    content?: string;
    shortName?: string;
    hasExp: boolean;
    leaf: boolean;
    collapseStatus: CollapseStatusEnum;
    type: NodeTypeEnum;
}
interface AppFacotry {
    omitString: (str: string, maxLength: number) => string;
    alert: (title: string, content: string) => void;
    arrayFirstOrDefault: (array: Object[], filter: Object) => Object;
}
interface Promise {
    get: Function;
}
interface KnowledgeService {
    bomResource: Promise;
    failureResource: Promise;
    causeResource: Promise;
    treatmentResource: Promise;
    searchBoms: Promise;
    searchFailure: Promise;
}
interface KnowledgeMapOption {
    lineColor: string;
    nodeHeight: number;
    rootText: string;
    redraw: Function;
    service: KnowledgeService;
    appFactory: AppFacotry;
}

/*
 * 自定义元素抽象基类
 * */
abstract class CustomElement {
    x: number;
    y: number;
    group: KnowledgeNodeGroup;

    constructor(option) {
        this.x = option.x || 0;
        this.y = option.y || 0;

        this.initGroup();
    }

    initGroup() {
        //以当前元素坐标为当前组坐标
        this.group = new Konva.Group({
            x: this.x,
            y: this.y
        });
    }

    add2LayerOrGroup(layerOrGroup) {
        layerOrGroup.add(this.group);
    }
}


/*
 * 专家经验库节点折叠状态
 * */
enum CollapseStatusEnum{
    // 全部折叠
    Collapsed,
        // 仅Bom结构展开
    BomExpanded,
        // 仅专家经验节点展开
    ExpExpanded,
}

/*
 * 节点类型
 * */
enum NodeTypeEnum{
    Root,
    Bom,
        // 现象
    Failure,
        //原因
    Cause,
        //解决方案
    Treatment
}

/*
 * 专家经验库地图节点
 * */
class KnowledgeNode extends CustomElement {
    width: number;
    height: number;
    hSeper: number;
    vSeper: number;
    private _colFill: string;
    private _expFill: string;
    private _radius: number;
    private _fontSize: number;
    private _fontColor: string;
    private _lineColor: string;

    private _presuffixFill: string;//前后缀背景填充
    private _presuffixContentFill: string;//前后缀内容填充
    private _presuffixPadding: number;//前后缀填充
    private _expandRoot: Function;//展开根节点事件处理函数
    private _collapseRoot: Function;//折叠根节点事件处理函数
    private _drawBoms: Function;//绘制Boms
    private _destroyBoms: Function;//销毁Boms
    private _drawExps: Function;//绘制Exps
    private _destroyExps: Function;//销毁Exps

    private prefixGroup: KnowledgeNodeGroup;//前缀组
    private suffixGroup: KnowledgeNodeGroup;//后缀组

    model: KnowledgeNodeModel;//数据模型
    parentNode: KnowledgeNode;//父节点
    detailsGroup: KnowledgeNodeGroup;//Boms+Exps+Lines
    bomsGroup: KnowledgeNodeGroup;//Bom子节点
    expsGroup: KnowledgeNodeGroup;//Exp子节点
    linesGroup: KnowledgeNodeGroup;//连接线(不含节点前水平短线)

    static redraw: Function;//重绘
    static omitString: (str: string, maxLength: number) => string;//省略字符串
    static alert: (title: string, content: string) => void;//提示框

    constructor(option: KnowledgeNodeOption) {
        super(option);
        this.model = option.model;
        this.width = this.model.type === NodeTypeEnum.Treatment ? (option.maxWidth || 150) : (option.width || 120);
        this.height = option.height || 30;
        this.hSeper = option.hSeper || 50;
        this.vSeper = option.vSeper || 28;
        this._colFill = option.colFill || '#33CD5F';
        this._expFill = option.expFill || '#11C1F3';
        this._radius = option.radius || this.height / 2;
        this._fontSize = option.fontSize || 13;
        this._fontColor = option.fontColor || '#fff';
        this._lineColor = option.lineColor || 'orange';
        this._presuffixFill = option.presuffixFill || '#f8f8f8';
        this._presuffixContentFill = option.presuffixContentFill || '#387EF5';
        this._presuffixPadding = option.presuffixPadding || 2;
        this._expandRoot = option.expandRoot;
        this._collapseRoot = option.collapseRoot;
        this._drawBoms = option.drawBoms;
        this._destroyBoms = option.destroyBoms;
        this._drawExps = option.drawExps;
        this._destroyExps = option.destroyExps;

        this.parentNode = option.parentNode;

        //boms/exps/lines坐标均相对于当前节点
        this.detailsGroup = new Konva.Group({x: 0, y: 0});
        this.bomsGroup = new Konva.Group({x: 0, y: 0});
        this.expsGroup = new Konva.Group({x: 0, y: 0});
        this.linesGroup = new Konva.Group({x: 0, y: 0});
        this.detailsGroup.add(this.bomsGroup, this.expsGroup, this.linesGroup);
        this.group.add(this.detailsGroup);

        //通过当前节点组拿到当前节点对象
        let self = this;
        this.group.getNode = function () {
            return self;
        };

        //处理文本
        this.model.name = this.model.name || this.model.content;
        let maxLength = 0, offsetX = 0;
        if (this.model.leaf === false && this.model.hasExp)
            maxLength = 4;
        else if (this.model.leaf !== false && !this.model.hasExp)
            maxLength = 8;
        else {
            maxLength = 5;
            if (this.model.name.length > maxLength)
                offsetX = this.model.leaf === false ? -5 : 5;
        }
        if (this.model.type === NodeTypeEnum.Treatment)
            maxLength = 10;
        this.model.shortName = KnowledgeNode.omitString(this.model.name, maxLength) || '';

        //矩形
        let rect = new Konva.Rect({
            //矩形坐标为当前节点组坐标
            x: 0,
            y: 0,
            width: this.width,
            height: this.height,
            fill: this._expFill,
            cornerRadius: this._radius,
        });
        this.group.add(rect);

        //水平连接线
        if (this.model.type !== NodeTypeEnum.Root)
            this._drawLine();

        //文本
        let opt = {
            x: 0,
            y: 0,
            offsetX: offsetX,
            offsetY: (this._fontSize - this.height) / 2,
            fontSize: this._fontSize,
            text: this.model.shortName,
            fill: this._fontColor,
            width: this.width,
            align: 'center',
        };
        let text = new Konva.Text(opt);
        if (this.model.shortName.endsWith('…')) {
            text.on('mouseup touchend', function () {
                KnowledgeNode.alert('专家经验详情', self.model.name);
            });
        }
        this.group.add(text);

        //前后缀
        if (this.model.leaf === false)
            this._drawPrefix();
        if (this.model.hasExp)
            this._drawSuffix();
    }

    //连接线
    private _drawLine() {
        let line = new Konva.Line({
            points: [-this.hSeper, this.height / 2, 0, this.height / 2],
            stroke: this._lineColor
        });
        this.group.add(line);
    };

    //前缀
    private _drawPrefix() {
        this.prefixGroup = new Konva.Group({
            x: 0,
            y: 0
        });
        let prefix = {
            x: this.height / 2,
            y: this.height / 2,
            radius: this.height / 2 - this._presuffixPadding,
            contentRadius: this.height / 2 - this._presuffixPadding * 3
        };
        //前缀圆
        let prefixCircle = new Konva.Circle({
            x: prefix.x,
            y: prefix.y,
            radius: prefix.radius,
            fill: this._presuffixFill
        });
        this.prefixGroup.add(prefixCircle);
        // 前缀内容图形
        let prefixContent = new Konva.RegularPolygon({
            x: prefix.x,
            y: prefix.y,
            sides: 3,
            radius: prefix.contentRadius,
            fill: this._presuffixContentFill,
            name: 'prefixContent'
        });
        this.prefixGroup.add(prefixContent);

        let self = this;
        //点击前缀
        this.prefixGroup.on('mouseup touchend', function () {
            if (self.model.collapseStatus !== CollapseStatusEnum.BomExpanded) {
                self._drawBoms();
                self._expandBoms();
            }
            else {
                self._destroyBoms();
                self._collapseBoms();
            }
            KnowledgeNode.redraw();
        });

        this.group.add(this.prefixGroup);
    }

    //后缀
    private _drawSuffix() {
        this.suffixGroup = new Konva.Group({
            x: 0,
            y: 0
        });
        let suffix = {
            x: this.width - this.height / 2,
            y: this.height / 2,
            radius: this.height / 2 - this._presuffixPadding,
            contentRadius: this.height / 2 - this._presuffixPadding * 4
        };
        //后缀圆
        let suffixCircle = new Konva.Circle({
            x: suffix.x,
            y: suffix.y,
            radius: suffix.radius,
            fill: this._presuffixFill
        });
        this.suffixGroup.add(suffixCircle);
        //后缀内容图形
        let suffixShapeCollapse = new Konva.Line({
            points: [suffix.x - suffix.contentRadius, suffix.y, suffix.x + suffix.contentRadius, suffix.y],
            stroke: this._presuffixContentFill,
            strokeWidth: 4,
        });
        this.suffixGroup.add(suffixShapeCollapse);
        let suffixShapeExpand = new Konva.Line({
            points: [suffix.x, suffix.y - suffix.contentRadius, suffix.x, suffix.y + suffix.contentRadius,],
            stroke: this._presuffixContentFill,
            strokeWidth: 4,
            name: 'suffixShapeExpand'
        });
        this.suffixGroup.add(suffixShapeExpand);

        let self = this;
        //点击后缀
        this.suffixGroup.on('mouseup touchend', function () {
            if (self.model.collapseStatus !== CollapseStatusEnum.ExpExpanded) {
                //展开根节点
                if (self.model.type === NodeTypeEnum.Root)
                    self._expandRoot();
                //非根节点
                else
                    self._drawExps();

                self._expandExps();
            }
            else {
                // 折叠根节点
                if (self.model.type === NodeTypeEnum.Root)
                    self._collapseRoot();
                else
                    self._destroyExps();
                self._collapseExps();
            }
            KnowledgeNode.redraw();
        });

        this.group.add(this.suffixGroup);
    }

    //展开Bom
    private _expandBoms() {
        let prefixContent = this.prefixGroup.findOne('.prefixContent');
        if (!prefixContent)
            return;
        prefixContent.rotation(180);
        this.model.collapseStatus = CollapseStatusEnum.BomExpanded;
    }

    //折叠Bom
    private _collapseBoms() {
        let prefixContent = this.prefixGroup.findOne('.prefixContent');
        if (!prefixContent)
            return;
        prefixContent.rotation(0);
        this.model.collapseStatus = CollapseStatusEnum.Collapsed;
    }

    //展开Exp
    private _expandExps() {
        let suffixShapeExpand = this.suffixGroup.findOne('.suffixShapeExpand');
        if (!suffixShapeExpand)
            return;
        suffixShapeExpand.hide();
        this.model.collapseStatus = CollapseStatusEnum.ExpExpanded;
    }

    //折叠Exp
    private _collapseExps() {
        let suffixShapeExpand = this.suffixGroup.findOne('.suffixShapeExpand');
        if (!suffixShapeExpand)
            return;
        suffixShapeExpand.show();
        this.model.collapseStatus = CollapseStatusEnum.Collapsed;
    }

    expandRoot() {
        this._expandRoot();
        this._expandExps();
    }

    collapseRoot() {
        this._collapseRoot();
        this._collapseExps();
    }

    expandBoms() {
        this._drawBoms();
        this._expandBoms();
    }

    collapseBoms() {
        this._destroyBoms();
        this._collapseBoms();
    }

    expandExps() {
        this._drawExps();
        this._expandExps();
    }

    collapseExps() {
        this._destroyExps();
        this._collapseExps();
    }
}

/*
 * 专家经验库地图
 * */
class KnowledgeMap extends CustomElement {
    private _lineColor: string;
    private _nodeHeight: number;//节点高度
    private _service: KnowledgeService;//数据查询服务
    private _redraw: Function;//重绘
    private _rootText: string;//根节点文本
    private _searched: boolean;//是否处于搜索结果状态
    private _searchedBoms: Array<Object>;//Boms搜索结果集
    private _appFactory: AppFacotry;
    private _kw: string;

    constructor(option: KnowledgeMapOption) {
        //Map组坐标原点为(0,centY)
        super(option);
        this._lineColor = option.lineColor || 'orange';
        this._nodeHeight = option.nodeHeight || 30;
        this._rootText = option.rootText;
        this._searched = false;
        this._searchedBoms = [];
        this._service = option.service;
        this._redraw = function (group?) {
            option.redraw();
            if (!group)
                return;
            group.opacity(0);
            group.to({
                duration: .5,
                opacity: 1,
            });
        };
        this._appFactory = option.appFactory;

        KnowledgeNode.redraw = this._redraw;
        KnowledgeNode.omitString = this._appFactory.omitString;
        KnowledgeNode.alert = this._appFactory.alert;
        this._drawRoot();
    }

    //搜索
    search(keyword: string) {
        this._kw = keyword;
        this._searchedBoms = [];
        if (!keyword) {
            this._searched = false;
            this._collapseRoot()();
            this._redraw();
        }
        else {
            this._searched = true;
            let self = this;
            this._service.searchBoms.get({failureNm: encodeURI(keyword)}, function (data) {
                if (!data || !data.total) {
                    self.group.destroyChildren();
                    let text = new Konva.Text({
                        x: 0,
                        y: 0,
                        fontSize: 18,
                        text: '没有符合条件的内容',
                        fill: '#ccc',
                        width: self.group.getLayer().width(),
                        align: 'center',
                    });
                    self.group.add(text);
                    self._redraw();
                    return;
                }

                let items = data.item;

                items.forEach(function (item, index) {
                    cachedSearchedBomsLine(item);

                    //展开第一条搜索结果
                    if (index === 0) {
                        self._collapseRoot()();
                        let root = self.group.children[0].getNode();
                        root.expandRoot();
                        drawSearchedBoms(item.items[0], root.expsGroup.children[0].getNode());
                    }
                });

                //缓存搜索结果
                function cachedSearchedBomsLine(searchedBom) {
                    if (!self._appFactory.arrayFirstOrDefault(self._searchedBoms, {id: searchedBom.id})) {
                        self._searchedBoms.push(searchedBom);
                    }
                    if (!searchedBom.items)
                        return;
                    cachedSearchedBomsLine(searchedBom.items[0]);
                }

                //绘制Node
                function drawSearchedBoms(item, node: KnowledgeNode) {
                    node.expandBoms();
                    let nd = node.bomsGroup.children[0].getNode();
                    if (!item.items) {
                        //展开现象
                        nd.expandExps();
                        return;
                    }
                    drawSearchedBoms(item.items[0], nd);
                }
            }, function (err) {
            });
        }
    }

    //绘制根节点
    private _drawRoot() {
        let root = new KnowledgeNode({
            //坐标相对于Map组
            x: 0,
            y: -this._nodeHeight / 2,
            height: this._nodeHeight,

            model: {
                name: this._rootText || 'ZJ119-01',
                hasExp: true,
                leaf: true,
                type: NodeTypeEnum.Root,
                collapseStatus: CollapseStatusEnum.Collapsed
            },
            expandRoot: this._expandRoot(),
            collapseRoot: this._collapseRoot(),
        });
        root.add2LayerOrGroup(this.group);
    }

    //展开根节点
    private _expandRoot() {
        let map = this;
        return function () {
            let self = this;

            function drawBoms(boms) {
                let vly0, vly1;//连接竖线开始结束y坐标
                let x = self.width + self.hSeper * 2;
                boms.forEach(function (item, index) {
                    item.type = NodeTypeEnum.Bom;
                    //绘制节点
                    let y = (self.height + self.vSeper) * ((1 - boms.length) / 2 + index);
                    if (index === 0)
                        vly0 = y + self.height / 2;
                    if (index === boms.length - 1)
                        vly1 = y + self.height / 2;
                    let opt: KnowledgeNodeOption = {
                        x: x,
                        y: y,
                        model: item,
                        parentNode: self
                    };
                    if (item.leaf === false) {
                        opt.drawBoms = map._drawBoms();
                        opt.destroyBoms = map._destroyBoms();
                    }
                    if (item.hasExp) {
                        opt.drawExps = map._drawExps();
                        opt.destroyExps = map._destroyExps();
                    }
                    let node = new KnowledgeNode(opt);
                    node.add2LayerOrGroup(self.expsGroup);
                });
                //连接竖线
                let vl = new Konva.Line({
                    points: [self.width + self.hSeper, vly0, self.width + self.hSeper, vly1],
                    stroke: self._lineColor,
                    name: 'vl'
                });
                self.linesGroup.add(vl);
                //连接横线
                let hl = new Konva.Line({
                    points: [self.width, self.height / 2, self.width + self.hSeper, self.height / 2],
                    stroke: self._lineColor,
                });
                self.linesGroup.add(hl);
                KnowledgeNode.redraw(self.detailsGroup);
            }

            if (map._searched) {
                let catalog = map._appFactory.arrayFirstOrDefault(map._searchedBoms, {parentId: null});
                drawBoms([catalog]);
            }
            else {
                map._service.bomResource.get(function (data) {
                    if (data.total <= 0)
                        return;
                    drawBoms(data.item);
                }, function (err) {
                });
            }
        }
    }

    //折叠根节点
    private _collapseRoot() {
        let map = this;
        return function () {
            map.group.destroyChildren();
            map._drawRoot();
            if (map._searched)
                map._searchedBoms.forEach(function (item: KnowledgeNodeModel, index) {
                    item.collapseStatus = CollapseStatusEnum.Collapsed;
                })
        };
    }

    //绘制Boms
    private _drawBoms() {
        let map = this;
        return function () {
            let self = this;
            map._collapseSiblings(this);
            if (map._searched) {
                let item = map._appFactory.arrayFirstOrDefault(map._searchedBoms, {parentId: this.model.id});
                if (!item)
                    return;
                drawBoms([item]);
            }
            else {
                map._service.bomResource.get({pid: self.model.id}, function (data) {
                    if (data.total <= 0)
                        return;
                    drawBoms(data.item);
                }, function (err) {
                });
            }
            function drawBoms(boms) {
                //Boms
                let x = self.width / 2 + self.hSeper;
                boms.forEach(function (item, index) {
                    item.type = NodeTypeEnum.Bom;
                    let opt = {
                        x: x,
                        y: (self.height + self.vSeper) * (index + 1),
                        model: item,
                        parentNode: self,
                    };
                    if (item.leaf === false) {
                        opt.drawBoms = map._drawBoms();
                        opt.destroyBoms = map._destroyBoms();
                    }
                    if (item.hasExp) {
                        opt.drawExps = map._drawExps();
                        opt.destroyExps = map._destroyExps();
                    }
                    let node = new KnowledgeNode(opt);
                    node.add2LayerOrGroup(self.bomsGroup);
                });
                //连接竖线
                let vl = new Konva.Line({
                    points: [self.width / 2, self.height, self.width / 2, (self.height + self.vSeper) * boms.length + self.height / 2],
                    stroke: self._lineColor,
                    name: 'vl'
                });
                self.linesGroup.add(vl);

                //下移关联元素
                let delta = (self.height + self.vSeper) * boms.length;
                map._moveSiblings(self, delta);

                KnowledgeNode.redraw(self.detailsGroup);
            }
        };
    }

    //销毁Boms
    private _destroyBoms() {
        let map = this;

        //获取子孙Boms数量
        function descendantBomsCount(node: KnowledgeNode) {
            let count = 0;

            function traverseDescendantBoms(nd: KnowledgeNode) {
                let children = nd.bomsGroup.children;
                if (children.length <= 0)
                    return;
                count += children.length;
                children.forEach(function (item, index) {
                    let node = item.getNode();
                    if (map._searched)
                        node.model.collapseStatus = CollapseStatusEnum.Collapsed;
                    traverseDescendantBoms(node);
                });
            }

            traverseDescendantBoms(node);
            return count;
        }

        return function () {
            let delta = -(this.height + this.vSeper) * descendantBomsCount(this);
            this.bomsGroup.destroyChildren();
            this.linesGroup.destroyChildren();
            map._moveSiblings(this, delta);
        };
    }

    //绘制Exps
    private _drawExps() {
        let map = this;
        return function () {
            let service, params, type;
            switch (this.model.type) {
                case NodeTypeEnum.Bom:
                    if (map._searched) {
                        service = map._service.searchFailure;
                        params = {partId: this.model.busId, kw: encodeURI(map._kw)};
                    }
                    else {
                        service = map._service.failureResource;
                        params = {partId: this.model.busId};
                    }
                    type = NodeTypeEnum.Failure;
                    break;
                case NodeTypeEnum.Failure:
                    service = map._service.causeResource;
                    params = {partId: this.model.partId, failureId: this.model.id};
                    type = NodeTypeEnum.Cause;
                    break;
                case NodeTypeEnum.Cause:
                    service = map._service.treatmentResource;
                    params = {partId: this.model.partId, failureId: this.model.failureId, causeId: this.model.id};
                    type = NodeTypeEnum.Treatment;
                    break;
                default:
                    return;
            }

            map._collapseSiblings(this);
            service.get(params, function (data) {
                drawExps(data);
            }, function (err) {
            });
            let self = this

            function drawExps(data) {
                if (data.total <= 0)
                    return;
                let exps = data.item;
                let vly0, vly1;//连接竖线开始结束y坐标
                let x = self.width + self.hSeper * 2;
                exps.forEach(function (item, index) {
                    item.type = type;
                    item.hasExp = type !== NodeTypeEnum.Treatment;
                    //绘制节点
                    let y = (self.height + self.vSeper) * ((1 - exps.length) / 2 + index);
                    if (index === 0)
                        vly0 = y + self.height / 2;
                    if (index === exps.length - 1)
                        vly1 = y + self.height / 2;
                    let opt: KnowledgeNodeOption = {
                        x: x,
                        y: y,
                        model: item,
                        parentNode: self,
                    };
                    if (item.hasExp) {
                        opt.drawExps = map._drawExps();
                        opt.destroyExps = map._destroyExps();
                    }
                    let node = new KnowledgeNode(opt);
                    node.add2LayerOrGroup(self.expsGroup);
                });
                //连接竖线
                let vl = new Konva.Line({
                    points: [self.width + self.hSeper, vly0, self.width + self.hSeper, vly1],
                    stroke: self._lineColor,
                    name: 'vl'
                });
                self.linesGroup.add(vl);
                //连接横线
                let hl = new Konva.Line({
                    points: [self.width, self.height / 2, self.width + self.hSeper, self.height / 2],
                    stroke: self._lineColor,
                });
                self.linesGroup.add(hl);
                KnowledgeNode.redraw(self.detailsGroup);
            }
        }
    }

    //销毁Exps
    private _destroyExps() {
        return function () {
            this.expsGroup.destroyChildren();
            this.linesGroup.destroyChildren();
        }
    }

    //折叠同级节点(含自己)
    private _collapseSiblings(node: KnowledgeNode) {
        let parentNode = node.parentNode;
        if (!parentNode)
            return;
        let boms = parentNode.bomsGroup.children;
        let exps = parentNode.expsGroup.children;
        //折叠同级Bom节点
        if (boms.length > 0)
            boms.forEach(function (item, index) {
                let bom = item.getNode();
                //折叠同级Bom的Boms
                if (bom.model.collapseStatus === CollapseStatusEnum.BomExpanded)
                    bom.collapseBoms();
                //折叠同级Bom的Exps
                else if (bom.model.collapseStatus === CollapseStatusEnum.ExpExpanded)
                    bom.collapseExps()
            });
        //折叠同级Exp节点
        if (exps.length > 0)
            exps.forEach(function (item, index) {
                let exp = item.getNode();
                if (exp.model.collapseStatus === CollapseStatusEnum.ExpExpanded)
                    exp.collapseExps();
            });
    }

    //上下移动关联元素
    private _moveSiblings(node: KnowledgeNode, delta: number) {
        let parentNode = node.parentNode;
        if (!parentNode)
            return;
        let parentBomsGroup = parentNode.model.type === NodeTypeEnum.Root ? parentNode.expsGroup : parentNode.bomsGroup;
        let behind = false, isNotLast = false;
        //下移兄弟节点
        parentBomsGroup.children.forEach(function (nd: KnowledgeNodeGroup, index) {
            if (behind) {
                nd.y(nd.y() + delta);
                isNotLast = true;
            }
            else if (nd === node.group)
                behind = true;
        });
        //延长父级连接竖线
        if (isNotLast) {
            let v: KonvaLine = parentNode.linesGroup.findOne('.vl') as KonvaLine;
            let points = v.points();
            let p_y = points.pop() + delta;
            let p_x = points.pop();
            v.points(v.points().concat([p_x, p_y]));
        }
        this._moveSiblings(parentNode, delta);
    }
}