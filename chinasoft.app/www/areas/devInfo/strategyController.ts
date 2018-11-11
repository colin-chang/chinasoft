/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.strategyController', ['global', 'app.devInfo.factory'])
        .controller('devInfoStrategyController', ['$scope', 'appConfig', 'strategyService', function ($scope, appConfig, service) {
            //设备名称
            $scope.equipmentName = appConfig.equipmentName;
            //面包屑导航
            $scope.crumbs = [];
            let pid = '';

            //初始化
            loadMaps();

            //选中Bom
            $scope.selectBom = function (map) {
                if (map.childrenCount <= 0)
                    return;
                pid = map.id;
                loadMaps();
                $scope.crumbs.push(map);
            };

            //选中面包屑导航
            $scope.selectCrumb = function (id, index, isLast) {
                if (isLast)
                    return;

                $scope.crumbs = $scope.crumbs.slice(0, index + 1);
                pid = id;
                loadMaps();
            };

            /*
             * 工具方法
             * */
            //加载维护策略地图
            function loadMaps() {
                service.strategymap.get({pid: pid}, function (data) {
                    $scope.maps = data.item;
                }, function (err) {
                });
            }
        }]);
})(angular);