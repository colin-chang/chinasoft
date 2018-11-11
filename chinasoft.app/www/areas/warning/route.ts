(function (angular) {
    'use strict';
    angular.module('app.warning.route',
        [
            'app.warning.indexController',
            'app.warning.faultController',
            'app.warning.repairController',
            'app.warning.renewalsController',
            'app.warning.lifetimeController',
        ])
        .config(function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('indexTab.warning', {
                    url: '/warning',
                    views: {
                        'indexTab-warning': {
                            controller: 'warningIndexController'
                        }
                    }
                })
                //带导航一级页面
                .state('warning', {
                    url: "/warning",
                    cache: false,
                    abstract: true,
                    templateUrl: 'areas/indexTab/index.html',
                })
                .state('warning.fault', {
                    url: '/fault',
                    views: {
                        'indexTab-warning': {
                            templateUrl: 'areas/warning/fault.html',
                            controller: 'warningFaultController'
                        }
                    }
                })
                .state('warning.lifetime', {
                    url: '/lifetime',
                    views: {
                        'indexTab-warning': {
                            templateUrl: 'areas/warning/lifetime.html',
                            controller: 'warningLifetimeController'
                        }
                    }
                })
                //无导航子页面
                .state('sub-warning', {
                    url: "/sub-warning",
                    cache: false,
                    abstract: true,
                    template: '<ion-nav-view name="sub-warning" animation="slide-left-right"></ion-nav-view>',
                })
                .state('sub-warning.repair', {
                    url: '/repair/:id/:alertId',
                    views: {
                        'sub-warning': {
                            templateUrl: 'areas/warning/repair.html',
                            controller: 'warningRepairController'
                        }
                    }
                })
                .state('sub-warning.renewals', {
                    url: '/renewals/:id',
                    views: {
                        'sub-warning': {
                            templateUrl: 'areas/warning/renewals.html',
                            controller: 'warningRenewalsController'
                        }
                    }
                })
            ;
        });
})(angular);
