/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.warning.renewalsController', ['global', 'app.warning.factory'])
        .controller('warningRenewalsController', ['$scope', '$state', '$stateParams', 'appConfig', 'lifetimeService', function ($scope, $state, $stateParams, appConfig, service) {
            let eleId = $stateParams.id;
            if (!eleId) {
                $state.go('warning.lifetime');
                return;
            }

            service.elementResource.get({id: eleId}, function (data) {
                let ele = data.obj;
                if (!ele)
                    return;
                $scope.element = ele;
            }, function (err) {
            });

            $scope.changeNums = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            $scope.changeInfo = {
                bomId: eleId,
                changeNum: 1,
                equmentId: appConfig.equipmentId || '01',
                inputUser: '张三',
                inputTime: new Date(),
            };

            $scope.submit = function () {
                service.changeInfoResource.save($scope.changeInfo);
            };
        }]);
})(angular);