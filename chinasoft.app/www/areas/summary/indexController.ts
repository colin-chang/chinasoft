/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.summary.indexController', [])
        .controller('summaryIndexController', ['$scope', '$state', function ($scope, $state) {
            $state.go('summary.list');
        }]);
})(angular);