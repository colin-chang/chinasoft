/**
 * Created by zhangcheng on 2017/1/12.
 */
(function (angular) {
    'use strict';
    angular.module('app.summary.route',
        [
            'app.summary.indexController',
            'app.summary.listController',
            'app.summary.detailController',
            'app.summary.haltController',
            'app.summary.warningController'
        ])
        .config(function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('indexTab.summary', {
                    url: '/summary',
                    views: {
                        'indexTab-summary': {
                            controller: 'summaryIndexController'
                        }
                    }
                })
                .state('summary', {
                    url: "/summary",
                    cache: false,
                    abstract: true,
                    templateUrl: 'areas/indexTab/index.html',
                })
                .state('summary.list', {
                    url: '/list',
                    views: {
                        'indexTab-summary': {
                            templateUrl: 'areas/summary/list.html',
                            controller: 'summaryListController'
                        }
                    }
                })
                .state('sub-summary', {
                    url: "/sub-summary",
                    cache: false,
                    abstract: true,
                    template: '<ion-nav-view name="sub-summary" animation="slide-left-right"></ion-nav-view>',
                })
                .state('sub-summary.detail', {
                    url: '/detail/:id',
                    views: {
                        'sub-summary': {
                            templateUrl: 'areas/summary/detail.html',
                            controller: 'summaryDetailController'
                        }
                    }
                })
                .state('sub-summary.halt', {
                    url: '/halt/:id',
                    views: {
                        'sub-summary': {
                            templateUrl: 'areas/summary/halt.html',
                            controller: 'summaryHaltController'
                        }
                    }
                })
                .state('sub-summary.warning', {
                    url: '/warning/:id',
                    views: {
                        'sub-summary': {
                            templateUrl: 'areas/summary/warning.html',
                            controller: 'summaryWarningController'
                        }
                    }
                })
            ;
        });
})(angular);
