/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.warning.lifetimeController', ['global', 'app.warning.factory'])
        .controller('warningLifetimeController', ['$scope', '$state', 'appConfig', 'warningService', 'lifetimeService', function ($scope, $state, appConfig, warningService, lifetimeService) {
            $scope.viewImage = function () {
                $state.go('imgViewer', {title: '寿命预警', path: 'devinfo\\equipment\\bom\\photo\\dj.jpg'});
            };

            //基本参量
            const PAGESIZE = 10;
            let pageIndex = 1;
            $scope.warnings = [];

            loadData();

            //滚动加载
            $scope.loadMore = function () {
                pageIndex++;
                loadData();
            };

            //查看详情
            $scope.viewImage = function (id) {
                lifetimeService.elementResource.get({id: id}, function (data) {
                    if (!data.obj || !data.obj.appendfile2dId)
                        return;
                    $state.go('imgViewer', {title: '预警零件详情', path: data.obj.appendfile2d.path});
                }, function (err) {
                })
            };

            /*
             * 工具方法
             */
            //构建查询参数
            function createParams(): Object {
                let params = `{"query":{"equipmentId":"${appConfig.equipmentId}","StrategyType":"1","status":"0"},"pager":{"pageIndex":"${pageIndex}","pageSize":"${PAGESIZE}"},"sort":{}}`;
                return {params: encodeURI(params)};
            }

            //加载列表
            function loadData() {
                warningService.listResource.get(createParams(), function (data) {
                    if (!data.total)
                        return;
                    $scope.warnings = $scope.warnings.concat(data.item);
                    $scope.$broadcast('scroll.infiniteScrollComplete');
                    $scope.canLoadMore = Math.ceil(data.total / PAGESIZE) > pageIndex;
                }, function (err) {
                });
            }
        }]);
})(angular);