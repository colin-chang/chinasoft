(function (angular) {
    'use strict';

    //定义全局路由（使用AngularUi-Router）
    angular.module('route',
        [
            'app.indexTab.route',
            'app.home.route',
            'app.devInfo.route',
            'app.warning.route',
            'app.summary.route',
        ])
        .config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/indextab/home');
        });
})(angular);
