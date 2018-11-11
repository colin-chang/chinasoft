/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.knowledgeController', ['global', 'ngCordova', 'app.factory', 'app.devInfo.factory'])
        .controller('devInfoKnowledgeController', ['$scope', '$state', '$stateParams', '$cordovaKeyboard', 'appConfig', 'appFactory', 'konwledgeService', function ($scope, $state, $stateParams, $cordovaKeyboard, appConfig, appFactory, service) {
            //绘图
            (function () {
                let stage = new Konva.Stage({
                    container: 'container',
                    width: innerWidth,
                    height: document.querySelector('ion-content').clientHeight,
                    draggable: true
                });

                let layer = new Konva.Layer();
                stage.add(layer);

                let cenY = stage.height() / 2;
                let map = new KnowledgeMap({
                    x: 10,
                    y: cenY,
                    rootText: appConfig.equipmentName,
                    service: service,
                    redraw: function () {
                        layer.draw();
                    },
                    appFactory: appFactory,
                });
                map.add2LayerOrGroup(layer);

                layer.draw();
                $scope.map = map;
            })();

            //搜索
            $scope.search = function () {
                $cordovaKeyboard.close();
                $scope.map.search($scope.keyword);
            };

            //搜索faultCode
            let code = $stateParams.faultCode;
            if (code) {
                $scope.keyword = code;
                $scope.search();
            }

            //取消搜索
            $scope.cancel = function () {
                $scope.keyword = '';
                $scope.search();
            };

            $scope.goBack = function () {
                if (code)
                    $state.go('warning.fault');
                else
                    $state.go('indexTab.devInfo');
            };
        }]);
})(angular);
