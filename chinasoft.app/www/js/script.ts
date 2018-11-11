(function (angular) {
    'use strict';

    $script([
        //框架
        'lib/ionic/js/ionic.bundle.min.js',
        'lib/angular-resource/angular-resource.min.js',
        'lib/ngCordova/dist/ng-cordova.min.js',
        'cordova.js',

        //插件
        'lib/jquery/dist/jquery.min.js',
        'lib/angular-sanitize/angular-sanitize.min.js',
        'lib/angular-percentage-directive/percentage.js',
        'lib/swiper/dist/js/swiper.min.js',
        'lib/videogular/videogular.min.js',
        'lib/videogular-controls/vg-controls.min.js',
        'lib/videogular-overlay-play/vg-overlay-play.min.js',
        'lib/videogular-poster/vg-poster.min.js',
        'lib/videogular-buffering/vg-buffering.min.js',
        'lib/ionic-image-lazy-load/ionic-image-lazy-load.js',
        'lib/konva/konva.min.js',
        'lib/echarts/dist/echarts.min.js',

        //全局
        'js/global.js',
        'js/config.js',
        'js/route.js',
        'js/app.js',
        'js/controller.js',
        'js/service.js',
        // 'js/interceptor.js',

        //模块
        'areas/indexTab/route.js',
        'areas/indexTab/controller.js',
        'areas/imgViewerController.js',
        'areas/home/route.js',
        'areas/home/controller.js',
        'areas/home/factory.js',
        'areas/devInfo/route.js',
        'areas/devInfo/devInfoFactory.js',
        'areas/devInfo/indexController.js',
        'areas/devInfo/indexFactory.js',
        'areas/devInfo/bomController.js',
        'areas/devInfo/docsController.js',
        'areas/devInfo/videosController.js',
        'areas/devInfo/videoPlayerController.js',
        'areas/devInfo/strategyController.js',
        'areas/devInfo/strategyDetailController.js',
        'areas/devInfo/spareController.js',
        'areas/devInfo/knowledgeMap.js',
        'areas/devInfo/knowledgeController.js',
        'areas/warning/route.js',
        'areas/warning/warningFactory.js',
        'areas/warning/indexController.js',
        'areas/warning/faultController.js',
        'areas/warning/repairController.js',
        'areas/warning/lifetimeController.js',
        'areas/warning/renewalsController.js',
        'areas/summary/route.js',
        'areas/summary/summaryFactory.js',
        'areas/summary/indexController.js',
        'areas/summary/listController.js',
        'areas/summary/detailController.js',
        'areas/summary/haltController.js',
        'areas/summary/warningController.js',
    ], function () {
        // angular.bootstrap(document, ['app']);
    });
})(angular);
