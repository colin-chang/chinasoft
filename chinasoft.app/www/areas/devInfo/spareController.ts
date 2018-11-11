/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.spareController', ['ngCordova', 'percentage', 'global', 'app.factory', 'app.devInfo.factory'])
        .controller('devInfoSpareController', ['$scope', '$state', '$filter', 'appConfig', 'appFactory', 'spareService', '$cordovaKeyboard', function ($scope, $state, $filter, appConfig, appFactory, service, $cordovaKeyboard) {

            //基本参量
            const PAGESIZE = 13;
            let pageIndex = 1;
            $scope.keyword = '';
            $scope.list = [];

            //初始化
            loadData();

            //查看模型
            $scope.viewModel = function (id: string) {
                service.detailResource.get({id: id}, function (data) {
                    let title = `${data.obj.elementName} - 模型`;
                    let path = (!data.obj.appendfile2dId) ? '' : data.obj.appendfile2d.path;
                    $state.go('imgViewer', {title: title, path: path});
                }, function (err) {
                });
            };

            //滚动加载
            $scope.loadMore = function () {
                pageIndex++;
                loadData();
            };

            //搜索
            $scope.search = function () {
                $cordovaKeyboard.close();
                pageIndex = 1;
                $scope.list = [];
                loadData();
            };

            //取消
            $scope.cancel = function () {
                $scope.keyword = '';
                $scope.search();
            };

            /*
             * 工具方法
             * */
            //构建查询参数
            function createParams(): Object {
                let params = `{"query":{"equipmentId":"${appConfig.equipmentId}","startDate":"","endDate":"","queryKey":"${$scope.keyword}"},"pager":{"pageIndex":${pageIndex},"pageSize":${PAGESIZE}},"sort":{}}`;
                return {params: encodeURI(params)};
            }

            //加载列表
            function loadData() {
                service.listResource.get(createParams(), function (data) {
                    for (let i = 0; i < data.item.length; i++) {
                        let item = data.item[i];
                        item.shortName = appFactory.omitString(item.elementName, 10);
                        let cd = $filter('date')(item.changeDate, 'yyyy-MM-dd');
                        item.changeDate = !cd ? '暂无' : cd;
                        $scope.list.push(item);
                    }
                    $scope.$broadcast('scroll.infiniteScrollComplete');
                    $scope.canLoadMore = Math.ceil(data.total / PAGESIZE) > pageIndex;
                }, function (err) {
                });
            }
        }]);
})(angular);