/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';
    angular.module('app.indexTab.route', [
        'app.indexTab.controller',
        'app.imgViewerController'
    ])
        .config(function ($stateProvider, $urlRouterProvider) {
            $stateProvider
            // 首页tab的抽象路由
                .state('indexTab', {
                    url: "/indextab",
                    cache: false,
                    abstract: true,
                    templateUrl: "areas/indexTab/index.html",
                    controller: 'indexTabController'
                })
                //图片查看器
                .state('imgViewer', {
                    url: "/imgviewer/:title/:path",
                    cache: false,
                    templateUrl: "areas/imgViewer.html",
                    controller: 'imgViewerController'
                })
            ;
        });
})(angular);
