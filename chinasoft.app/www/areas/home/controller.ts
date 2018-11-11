(function (angular) {
    'use strict';

    angular.module('app.home.controller', ['global', 'app.factory', 'app.home.factory'])
        .controller('homeController', ['$scope', 'appConfig', 'workOrderService', 'equipmentService', 'appFactory', 'homeFactory', function ($scope, appConfig, orderService, equService, appFct, homeFactory) {

            homeFactory.placeEquInfoSection();

            orderService.get(function (data) {
                data.obj.workOrderShortNo = appFct.omitString(data.obj.workOrderNo, 8);
                $scope.order = data.obj;
            }, function (err) {
            });
            $scope.equ = equService.get(function (data) {
                appConfig.equipmentName = data.equipmentName;
            }, function (err) {
                console.log(err.statusText);
            });
        }])
    ;
})(angular);