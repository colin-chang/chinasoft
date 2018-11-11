(function (angular) {
    'use strict';
    angular.module('app.home.route', ['app.home.controller'])
        .config(function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('indexTab.home', {
                    url: '/home',
                    views: {
                        'indexTab-home': {
                            templateUrl: 'areas/home/index.html',
                            controller: 'homeController'
                        }
                    }
                });
        });
})(angular);
