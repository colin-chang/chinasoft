(function (angular) {
    'use strict';
    angular.module('app.devInfo.route',
        [
            'app.devInfo.indexController',
            'app.devInfo.bomController',
            'app.devInfo.docsController',
            'app.devInfo.videosController',
            'app.devInfo.videoPlayerController',
            'app.devInfo.strategyController',
            'app.devInfo.strategyDetailController',
            'app.devInfo.spareController',
            'app.devInfo.knowledgeController',
        ])
        .config(function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('indexTab.devInfo', {
                    url: '/devinfo',
                    views: {
                        'indexTab-devInfo': {
                            templateUrl: 'areas/devInfo/index.html',
                            controller: 'devInfoIndexController'
                        }
                    }
                })
                .state('devInfo', {
                    url: "/devinfo",
                    cache: false,
                    abstract: true,
                    template: '<ion-nav-view name="devInfo-childView" animation="slide-left-right"></ion-nav-view>',
                })
                .state('devInfo.bom', {
                    url: '/bom',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/bom.html',
                            controller: 'devInfoBomController'
                        }
                    }
                })
                .state('devInfo.docs', {
                    url: '/docs',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/docs.html',
                            controller: 'devInfoDocsController'
                        }
                    }
                })
                .state('devInfo.videos', {
                    url: '/videos',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/videos.html',
                            controller: 'devInfoVideosController'
                        }
                    }
                })
                .state('devInfo.videoPlayer', {
                    url: '/videoplayer/:id',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/videoPlayer.html',
                            controller: 'devInfoVideoPlayerController'
                        }
                    }
                })
                .state('devInfo.strategy', {
                    url: '/strategy',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/strategy.html',
                            controller: 'devInfoStrategyController'
                        }
                    }
                })
                .state('devInfo.strategyDetail', {
                    url: '/strategydetail/:type/:queryCode',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/strategyDetail.html',
                            controller: 'devInfoStrategyDetailController'
                        }
                    }
                })
                .state('devInfo.spare', {
                    url: '/spare',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/spare.html',
                            controller: 'devInfoSpareController'
                        }
                    }
                })
                .state('devInfo.knowledge', {
                    url: '/knowledge/:faultCode',
                    views: {
                        'devInfo-childView': {
                            templateUrl: 'areas/devInfo/knowledge.html',
                            controller: 'devInfoKnowledgeController'
                        }
                    }
                })
            ;
        });
})(angular);
