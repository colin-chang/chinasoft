/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.strategyDetailController', ['global', 'app.devInfo.factory'])
        .controller('devInfoStrategyDetailController', ['$scope', '$state', '$stateParams', 'appConfig', 'strategyService', function ($scope, $state, $stateParams, appConfig, service) {
            if (!$stateParams.type || !$stateParams.queryCode)
                $state.go('devInfo.strategy');

            $scope.type = $stateParams.type;
            $scope.list = [];
            let pageIndex = 1, pageSize = 2, resource;

            //初始化参数
            initTypeParams();

            //加载数据
            loadList();

            //滚动加载
            $scope.loadMore = function () {
                pageIndex++;
                loadList();
            };

            /*
             * 工具方法
             * */
            //创建查询参数
            function createParams() {
                let impr = '';
                if ($scope.type === 'proImpr')
                    impr = ',"extendField1":"0"';
                else if ($scope.type === 'bigImpr')
                    impr = ',"extendField1":"1"';

                let result = `{"query":{"modelId":"${appConfig.equipmentId}","queryCode":"${$stateParams.queryCode}"${impr}},"pager":{"pageIndex":${pageIndex},"pageSize":${pageSize}},"sort":{}}`;
                return {params: encodeURI(result)};
            }

            //初始化类型相关参数
            function initTypeParams() {
                switch ($scope.type) {
                    case 'check':
                        resource = service.checkResource;
                        $scope.typeName = '点检';
                        break;
                    case 'lubrication':
                        resource = service.lubricationResource;
                        $scope.typeName = '润滑';
                        break;
                    case 'maintain':
                        resource = service.maintainResource;
                        $scope.typeName = '保养';
                        break;
                    case 'repair':
                        resource = service.repairResource;
                        $scope.typeName = '维修';
                        break;
                    case 'proImpr':
                        resource = service.imprsResource;
                        $scope.typeName = '项修';
                        break;
                    case 'bigImpr':
                        resource = service.imprsResource;
                        $scope.typeName = '大修';
                        break;
                    default:
                        break;
                }
            }

            //加载数据
            function loadList() {
                if (!resource)
                    initTypeParams();

                resource.get(createParams(), function (data) {
                    $scope.list = $scope.list.concat(data.item);
                    $scope.$broadcast('scroll.infiniteScrollComplete');
                    $scope.canLoadMore = Math.ceil(data.total / pageSize) > pageIndex;
                }, function (err) {
                });
            }
        }]);
})(angular);