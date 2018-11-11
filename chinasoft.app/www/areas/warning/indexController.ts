/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.warning.indexController', [])
        .controller('warningIndexController', ['$scope', '$state', function ($scope, $state) {
            $state.go('warning.fault');
        }]);
})(angular);