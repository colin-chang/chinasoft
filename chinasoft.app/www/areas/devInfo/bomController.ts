/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.bomController', ['ngCordova', 'global', 'app.factory', 'app.devInfo.factory'])
        .controller('devInfoBomController', ['$scope', '$ionicPlatform', '$cordovaFileOpener2', '$cordovaToast', 'appConfig', 'appFactory', 'bomTreeService', function ($scope, $ionicPlatform, $cordovaFileOpener2, $cordovaToast, appConfig, appFct, service) {
            //swiper初始化
            let swiper;
            $scope.$on('$ionicView.afterEnter', function (e) {
                swiper = new Swiper('#bomSlide', {
                    sliderPerView: 'auto',//每屏显示几个视图
                    pagination: '.swiper-pagination',//分页器显示位置
                    paginationClickable: true,//分页器是否可以点击
                    centeredSlides: true,
                    // autoplay: 3000,//自动播放毫秒数
                    autoplayDisableOnInteraction: false,//用户操作swiper之后，是否禁止autoplay
                    // loop: true,//循环播放
                    // loopedSlides :4,
                    // loopAdditionalSlides : 1,
                    //改变内容后自动更新
                    observer: true,
                    observeParents: true,
                });
            });

            //设备名称
            $scope.equipmentName = appConfig.equipmentName;

            //初始化Bom列表
            initEquipmentBom();

            //选中Bom节点
            $scope.selectBom = function (id: string) {
                $scope.cancel();
                let bom = appFct.arrayFirstOrDefault($scope.boms, {'id': id});

                //虚拟目录
                if (bom.bomEntryCode === CATALOG) {
                    //虚拟目录叶子节点
                    if (!!bom.isLeaf) {
                    }
                    //虚拟目录非叶子节点
                    else
                        loadChildren(bom);
                    refreshHasNoting(true);
                }
                //部位
                else if (bom.bomEntryCode === POSITION) {
                    //部位非叶子节点
                    if (!bom.isLeaf)
                        loadChildren(bom);
                    //爆炸图/3D
                    loadPositionDetail(bom.busId);
                    //文档
                    loadBomDocs(bom.id);
                    $scope.has2D = false;
                    $scope.hasCircuit = false;
                }
                //零件
                else if (bom.bomEntryCode === ELEMENT) {
                    //3D/2D
                    service.positionDetailResource.get({busId: bom.busId}, function (data) {
                        $scope.has3D = !!data && !!data.obj && !!data.obj.appendfile3dId;
                        $scope.has2D = !!data && !!data.obj && !!data.obj.appendfile2dId;
                        refreshHasNoting();
                        if ($scope.has3D) {
                            //TODO:3D图路径
                        }
                        if ($scope.has2D)
                            $scope._2D = `${appConfig.fileUrl}${appFct.toUrl(data.obj.appendfile2d.path)}`;
                    }, function (err) {
                    });
                    //文档
                    loadBomDocs(bom.id);
                    $scope.hasExplode = false;
                    $scope.hasCircuit = false;
                }
                //电路图
                else {
                    $scope.hasExplode = false;
                    $scope.has3D = false;
                    $scope.has2D = false;
                    $scope.hasDocs = false;
                    $scope.hasCircuit = true;
                    refreshHasNoting(false);
                    $scope.circuit = `${appConfig.circuitUrl}${bom.busId}`;
                }
            };

            //选中面包屑导航
            $scope.selectCrumb = function (bom, index, isLast) {
                $scope.cancel();

                //顶级
                if (index < 0) {
                    initEquipmentBom();
                    return;
                }
                //最后一级
                if (isLast)
                    return;

                $scope.crumbs = $scope.crumbs.slice(0, index);
                swiper.slideTo(0, 0, true);
                loadChildren(bom);
                if (bom.bomEntryCode !== POSITION) {
                    refreshHasNoting(true);
                    return;
                }
                //爆炸图/3D
                loadPositionDetail(bom.busId);
                //文档
                loadBomDocs(bom.id);
            };

            //取消搜索
            $scope.cancel = function () {
                $scope.keyword = '';
            };

            //打开文件
            $scope.openFile = function (file) {
                // console.log(file);
                //TODO:文件预览
                // let path = 'http://115.28.209.159/files/devinfo/equipment/techdoc/zl_pdf.pdf';
                // $cordovaToast.showShortCenter('ionic中文网').then(function(success) {
                //     // success
                // }, function (error) {
                //     // error
                // });
                // console.log('wocao');
                // console.log($cordovaFileOpener2);
                // console.log(!!$cordovaFileOpener2.open);
                // $ionicPlatform.ready(function () {
                //     console.log($cordovaFileOpener2);
                //     if (!$cordovaFileOpener2)
                //         return;
                //     $cordovaFileOpener2.open(
                //         path,
                //         'application/pdf'
                //     ).then(function () {
                //         console.log('OK');
                //         // file opened successfully
                //     }, function (err) {
                //         console.log('shit');
                //         // An error occurred. Show a message to the user
                //     });
                // });


                // http://115.28.209.159/files/devinfo/equipment/cover/v01.jpg
                // http://115.28.209.159/files/devinfo/equipment/techdoc/bwaz.mp4;
                // http://115.28.209.159/files/devinfo/equipment/techdoc/zl_html/index.html
                // http://115.28.209.159/files/devinfo/equipment/techdoc/zl_pdf.pdf
                // http://115.28.209.159/files/devinfo/equipment/techdoc/zl_word.doc
            };

            /**
             * 工具方法
             */
            const CATALOG = 'catalog', POSITION = 'position', ELEMENT = 'element', CIRCUIT = 'circuit';
            let bomEntryDict = {};
            bomEntryDict[CATALOG] = '虚拟目录';
            bomEntryDict[POSITION] = '部位';
            bomEntryDict[ELEMENT] = '零件';
            bomEntryDict[CIRCUIT] = '电路图';

            //初始化设别Bom(顶级)
            function initEquipmentBom() {
                $scope.crumbs = [];
                refreshHasNoting(true);

                $scope.boms = service.equipmentResource.query(function (data) {
                    for (let i = 0; i < data.length; i++) {
                        let bom = data[i];
                        bom.bomEntryName = bomEntryDict[bom.bomEntryCode];
                    }
                }, function (err) {
                    console.log(err.statusText);
                });
            }

            //附加面包屑
            function appendCrumbs(bom) {
                $scope.crumbs.push(bom);
            }

            //加载子节点
            function loadChildren(bom) {
                $scope.boms = service.childStructureResource.query({parentId: bom.id}, function (data) {
                    for (let i = 0; i < data.length; i++) {
                        let bom = data[i];
                        bom.bomEntryName = bomEntryDict[bom.bomEntryCode];
                    }
                });
                appendCrumbs(bom);
            }

            //部位爆炸图/3D
            function loadPositionDetail(busId: string) {
                service.positionDetailResource.get({busId: busId}, function (data) {
                    $scope.hasExplode = !!data && !!data.obj && !!data.obj.explodeFileId;
                    $scope.has3D = !!data && !!data.obj && !!data.obj.appendfile3dId;
                    refreshHasNoting();
                    if ($scope.hasExplode) {
                        $scope.explode = `${appConfig.fileUrl}${appFct.toUrl(data.obj.explodeFile2d.path)}`;
                    }
                    if ($scope.has3D) {
                        //TODO:3D图路径
                    }
                }, function (err) {
                });
            }

            //加载部位或零件文档
            function loadBomDocs(bomId: string) {
                service.bomDocsResource.get({bid: bomId}, function (data) {
                    $scope.hasDocs = !!data && data.total > 0;
                    refreshHasNoting();
                    if (!$scope.hasDocs)
                        return;

                    $scope.docs = [];
                    for (let i = 0; i < data.item.length; i++) {
                        let item = data.item[i];
                        if (item.type !== 'zip')
                            $scope.docs.push(item);
                    }
                }, function (err) {
                });
            }

            //刷新hasNothing
            function refreshHasNoting(hasNothing: boolean) {
                if (hasNothing === true) {
                    $scope.hasExplode = false;
                    $scope.has3D = false;
                    $scope.has2D = false;
                    $scope.hasDocs = false;
                    $scope.hasCircuit = false;
                    $scope.hasNothing = true;
                }
                else if (hasNothing === false)
                    $scope.hasNothing = false;
                else
                    $scope.hasNothing = !$scope.hasExplode && !$scope.has3D && !$scope.has2D && !$scope.hasDocs && !$scope.hasCircuit;
            }
        }]);
})(angular);