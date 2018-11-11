/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.summary.detailController', ['app.summary.factory'])
        .controller('summaryDetailController', ['$scope', '$state', '$stateParams', 'summaryService', function ($scope, $state, $stateParams, service) {
            let id = $stateParams.id;
            if (!id) {
                $state.go('summary.list');
                return;
            }
            service.detailResource.get({id: id}, function (data) {
                $scope.data = data.obj;
            }, function (err) {
            });
        }]);
})(angular);