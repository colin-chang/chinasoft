/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.summary.listController', ['app.summary.factory'])
        .controller('summaryListController', ['$scope', 'summaryService', function ($scope, service) {
            let date = new Date();
            loadData();

            $scope.siblingDay = function (next: boolean) {
                date = new Date(date.valueOf() + 24 * 60 * 60 * 1000 * (next ? 1 : -1));
                loadData();
            };

            function loadData() {
                let dateStr = date.getFullYear().toString() +
                    (date.getMonth() < 9 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1) +
                    (date.getDate() < 9 ? '0' + date.getDate() : date.getDate());
                service.listResource.get({date: dateStr}, function (data) {
                    $scope.orders = data.item;
                }, function (err) {
                });
            }
        }]);
})(angular);