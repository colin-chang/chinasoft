## 常用命令
1.添加iOS/Android平台

`ionic platform add ios/android`
 
2.编译iOS/Android平台APP

`ionic build ios/android`

3.运行iOS/Android平台APP
+ 直接运行<br/>
`ionic/cordova run ios/android`
+ 指定模拟器和系统版本运行<br>
`ionic/cordova run ios -target='iPhone-6s,10.0'`
 
 4.查看iOS模拟器设备列表

`ios-sim showdevicetypes`

## API示例
### 一机一档
1.设备概况
+ 资产信息<br>
`http://115.28.209.159:8081/devinfo/api/equipment/info/detail/01`

+ 当前工单信息(APP专用)
`http://115.28.209.159:8081/devinfo/api/equipment/info/parms/01`

2.设备结构
+ 虚拟目录<br>
`http://115.28.209.159:8081/devinfo/api/equipment/bom/tree?equipmentId=01`

+ 子结构<br>
`http://115.28.209.159:8081/devinfo/api/equipment/bom/tree?equipmentId=01&parentId=01`

+ 零部件 *参数：{"query": {"parentBomId": "15"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/equipment/bom/list?params=%257b%2522query%2522%253a%2b%257b%2522parentBomId%2522%253a%2b%252215%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 搜索<br>
`http://115.28.209.159:8081/devinfo/api/equipment/bom/tree?equipmentId=01&queryKey=%25e7%2583%259f%25e6%2594%25af`

+ 单条搜索结果结构<br>
`http://115.28.209.159:8081/devinfo/api/equipment/bom/tree?equipmentId=01&queryCode=001001005002`

+ 部位详情   *参数：busid:10(磨刀装置)*<br>
`http://115.28.209.159:8081/devinfo/api/position/detail/10`

+ 获取部位文档列表<br>
`http://115.28.209.159:8081/devinfo/api/equipment/techdoc/bom?eid=01&bid=17`

+ 获取零件详情<br>
`http://115.28.209.159:8081/devinfo/api/component/bom/detail/46`

+ 获取电路图元件 *参数：{"query":{"circuitCode":"01"},"pager":{"pageIndex":1,"pageSize":10},"sort":{}}*<br>
`http://115.28.209.159:8081/devinfo/api/equipment/circuit/resource/list?params=%257b%2522query%2522%253a%257b%2522circuitCode%2522%253a%252201%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a1%252c%2522pageSize%2522%253a10%257d%252c%2522sort%2522%253a%257b%257d%257d`

+ 获取电路图元件功能列表<br>
`http://115.28.209.159:8081/devinfo/api/equipment/circuit/resource/=MG+MR-A200-A1/functions?rcode=%253dMG%252f1A023`

+ 获取QueryCode<br>
`http://115.28.209.159:8081/devinfo/api/equipment/bom/bom/qc?eid=01&bid=02`

3.随机文档 *参数：{"query":{"equipmentId":"01"},"pager":{"pageIndex":1,"pageSize":10},"sort":{}}*

`http://115.28.209.159:8081/devinfo/api/equipment/techdoc/list?params=%257b%2522query%2522%253a%257b%2522equipmentId%2522%253a%252201%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a1%252c%2522pageSize%2522%253a10%257d%252c%2522sort%2522%253a%257b%257d%257d`

4.培训资料 

+ 培训资料列表 *参数：{"query":{"classId":"","name":"","equipmentId":"01"},"pager":{"pageIndex":1,"pageSize":10},"sort":{}}*
`http://115.28.209.159:8081/devinfo/api/equipment/trainvideo/list?params=%257b%2522query%2522%253a%257b%2522classId%2522%253a%2522%2522%252c%2522name%2522%253a%2522%2522%252c%2522equipmentId%2522%253a%252201%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a1%252c%2522pageSize%2522%253a10%257d%252c%2522sort%2522%253a%257b%257d%257d`

+ 培训资料分类<br>
`http://115.28.209.159:8080/skydev/api/dict/getDictList/doc`

5.维护策略

+ 一级Bom结构<br>
`http://115.28.209.159:8081/devinfo/api/standard/strategymap/list?mid=01&pid=`

+ 子结构<br>
`http://115.28.209.159:8081/devinfo/api/standard/strategymap/list?mid=01&pid=08`

+ 获取点检列表 *参数：{"query": {"modelId": "01","queryCode": "001002"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/check/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001002%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取润滑列表 *参数：{"query": {"modelId": "01","queryCode": "001001"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/lubrication/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001001%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取保养列表 *参数：{"query": {"modelId": "01","queryCode": "001001"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/maintain/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001001%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取维修列表 *参数：{"query": {"modelId": "01","queryCode": "001001"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/repair/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001001%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取项修列表 *参数：{"query": {"modelId": "01","queryCode": "001001","extendField1":"0"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/techimpr/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001001%2522%252c%2522extendField1%2522%253a%25220%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取大修列表 *参数：{"query": {"modelId": "01","queryCode": "001002","extendField1":"1"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/techimpr/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001002%2522%252c%2522extendField1%2522%253a%25221%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取换件列表 *参数：{"query": {"modelId": "01","queryCode": "001003"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/changepart/list?params=%2b%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001003%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

+ 获取监测列表 *参数：{"query": {"modelId": "01","queryCode": "001001"},"pager": {"pageIndex": 1,"pageSize": 10},"sort": {}}*<br>
`http://115.28.209.159:8081/devinfo/api/standard/detectpoint/list?params=%257b%2522query%2522%253a%2b%257b%2522modelId%2522%253a%2b%252201%2522%252c%2522queryCode%2522%253a%2b%2522001001%2522%257d%252c%2522pager%2522%253a%2b%257b%2522pageIndex%2522%253a%2b1%252c%2522pageSize%2522%253a%2b10%257d%252c%2522sort%2522%253a%2b%257b%257d%257d`

6.专家经验库

+ 获取一级Bom结构<br>
`http://115.28.209.159:8081/devinfo/api/model/bom/list?moduleId=01&pid=`
+ 获取层级Bom结构<br>
`http://115.28.209.159:8081/devinfo/api/model/bom/list?moduleId=01&pid=01`
+ 获取现象<br>
`http://115.28.209.159:8081/devinfo/api/knowledge/listfaultfailure?moduleId=01&partId=10`
+ 获取原因<br>
`http://115.28.209.159:8081/devinfo/api/knowledge/listfaultcause?moduleId=01&partId=10&failureId=01`
+ 获取解决方案<br>
`http://115.28.209.159:8081/devinfo/api/knowledge/listfaulttrentment?moduleId=01&partId=10&failureId=01&causeId=01`
+ 搜索Bom结构（磨刀装置|卡纸）<br>
`http://115.28.209.159:8081/devinfo/api/model/bom/treekl?modelId=01&partNm=%25E7%25A3%25A8%25E5%2588%2580%25E8%25A3%2585%25E7%25BD%25AE&failureNm=%25E5%258D%25A1%25E7%25BA%25B8`
+ 搜索Bom结构（卡纸[app]）<br>
`http://115.28.209.159:8081/devinfo/api/model/bom/treekl?modelId=01&failureNm=%25E5%258D%25A1%25E7%25BA%25B8`
+ 搜索现象  （卡纸）<br>
`http://115.28.209.159:8081/devinfo/api/knowledge/listfaultfailure?moduleId=01&partId=10&kw=%25e5%258d%25a1%25e7%25ba%25b8`

7.备件清单

+ 备件清单列表 *参数：{"query":{"equipmentId":"01","startDate":"","endDate":"","queryKey":""},"pager":{"pageIndex":1,"pageSize":10},"sort":{}}*<br>
`http://115.28.209.159:8081/devinfo/api/component/spearparts/list?params=%257b%2522query%2522%253a%257b%2522equipmentId%2522%253a%252201%2522%252c%2522startDate%2522%253a%2522%2522%252c%2522endDate%2522%253a%2522%2522%252c%2522queryKey%2522%253a%2522%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a1%252c%2522pageSize%2522%253a10%257d%252c%2522sort%2522%253a%257b%257d%257d`

+ 获取备件详情<br>
`http://115.28.209.159:8081/devinfo/api/component/bom/detail/46`

+ 换件历史记录 *参数：{"query":{"bomId":"46"},"pager":{"pageIndex":1,"pageSize":10},"sort":{}}*<br>
`http://115.28.209.159:8081/devinfo/api/component/changeinfo/list?params=%257b%2522query%2522%253a%257b%2522bomId%2522%253a%252246%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a1%252c%2522pageSize%2522%253a10%257d%252c%2522sort%2522%253a%257b%257d%257d`

### 预警警告

1.警告预警列表

+ 故障预警列表 *参数：{"query":{"equipmentId":"01","StrategyType":"0","status":"0"},"pager":{"pageIndex":"1","pageSize":"10"},"sort":{}}*<br>
`http://115.28.209.159:8081/devinfo/api/warning/list?params=%257b%2522query%2522%253a%257b%2522equipmentId%2522%253a%252201%2522%252c%2522StrategyType%2522%253a%25220%2522%252c%2522status%2522%253a%25220%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a%25221%2522%252c%2522pageSize%2522%253a%252210%2522%257d%252c%2522sort%2522%253a%257b%257d%257d`

+ 寿命预警列表 *参数：{"query":{"equipmentId":"01","StrategyType":"1","status":"0"},"pager":{"pageIndex":"1","pageSize":"10"},"sort":{}}*<br>
`http://115.28.209.159:8081/devinfo/api/warning/list?params=%257b%2522query%2522%253a%257b%2522equipmentId%2522%253a%252201%2522%252c%2522StrategyType%2522%253a%25221%2522%252c%2522status%2522%253a%25220%2522%257d%252c%2522pager%2522%253a%257b%2522pageIndex%2522%253a%25221%2522%252c%2522pageSize%2522%253a%252210%2522%257d%252c%2522sort%2522%253a%257b%257d%257d`

+ 处理预警(置为已处理) *method:PUT,Content-Type:application/json;charset=UTF-8* <br>
`http://115.28.209.159:8081/devinfo/api/warning/dispose/01`


2.故障预警

+ 故障详情<br>
`http://115.28.209.159:8081/devinfo/api/warning/strategy/fault/detail/01`

+ 新增保修单 *method:POST*<br>
`http://115.28.209.159:8081/devinfo/api/sercenter/repairbill/add`

+ 故障类型<br>
`http://115.28.209.159:8080/skydev/api/dict/getDictList/dict_rb_type`

+ 专家经验<br>
`http://115.28.209.159:8081/devinfo/api/model/bom/treekl?modelId=01&failureNm=D-001`

3.寿命预警

+ 预警零件详情<br>
`http://115.28.209.159:8081/devinfo/api/component/bom/detail/46`

+ 新增换件记录 *method:POST body:{bomId: "46", changeNum: 1, equmentId: "01", inputUser: "张三", inputTime: "2017-01-13T07:14:59.372Z"}*<br>
`http://115.28.209.159:8081/devinfo/api/component/changeinfo/add`

### 班报统计

1.班报列表<br>
`http://115.28.209.159:8081/devinfo/api/workorder/report/list?eid=01&date=20170114`

2.班报详情
`http://115.28.209.159:8081/devinfo/api/workorder/report/detial/19`


### 其他

+ API根地址<br>
`http://115.28.209.159:8081/devinfo/api`

+ API WIKI *参数：http://115.28.209.159:8080/SwaggerUI/json/devinfo_1.0.json*<br>
`http://115.28.209.159:8080/SwaggerUI/`

## 其他

1.Bom结构

|Bom类型|展示内容|
|:-|:-|
|虚拟目录|暂无|
|部位|爆炸图、3D、文件|
|零件|3D、2D（零件详情列表）、文件|
|电路图|电路图|


## 真机或模拟器调试白屏
可能在浏览器中调试时一切正常，当你在真机或模拟器上测试时不正常，当app启动时只有空白的页面。

导致“死亡白屏”的原因多种多样，不能确切的说明是什么原因，经常是因为缺失了JavaScript文件，这就中断了Angular初始化app的过程，从而不能加载我们的视图模板，所以显示空白的页面。因为在真机或模拟器上没有控制台，我们推荐以下两种方式进行调试。

1.ionic Live Reload

当使用真机或模拟器时，即使用ionic run或ionic emulate命令时加上–consolelogs参数，–consolelogs参数不能单独使用，需配合–livereload参数使用，即可在终端中输出app的相关信息。

`ionic run/emulate android/ios --livereload --consolelogs`<br>
或简写为<br>
`ionic run/emulate android/ios -l -c`<br>
Live Reload option的作用和浏览器调试下的ionic serve作用类似，当调试过程中改变了代码，真机/模拟器上的app会自动更新。但是需要注意的是，对于插件的任何改变都会导致app的重新构建(rebuild)。Live Reload option起作用还有个前提：你的主机和测试设备或模拟器需要处于同一局域网，并且测试设备或模拟器必须支持web sockets。

2.Chrome Developer Tools

对于ionic app来说，Chrome Developer Tools只能对Android真机或android模拟器进行调试。
+ 使用USB线连接你的主机和android测试机，确保你的测试机已经打开了USB调试并已经允许你的主机对你的android测试机进行调试;
+ 打开Chrome，地址栏输入chrome://inspect，回车;
+ 网页中应该能看到你的设备<br>
![Chrome Developer Tools 
效果图](http://img.blog.csdn.net/20151102000237566)
+ 点击inspect，Chrome会新建一个调试窗口，剩下的就和网页的调试一样了。看一眼Console，可能会看到如下结果：<br>
![调试效果图](http://img.blog.csdn.net/20151102000517318)

### 小结
最简单实用的方式是使用Live Reload参数，适用于Android和iOS，如果只是调试Android，那么使用Chrome Developer Tools将是最好的选择。
