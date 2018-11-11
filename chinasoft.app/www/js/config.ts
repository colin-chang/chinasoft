/**
 * Created by zhangcheng on 2016/11/29.
 */


(function (angular) {
    'use strict';

    angular.module('config', ['global'])
        .config(function ($ionicConfigProvider) {
            //ionic打包发布到android和iOS平台时存在一定差异，需要在此处进行平台的差异化配置

            //$ionicConfigProvider.platform.ios.tabs.style('standard');
            //$ionicConfigProvider.platform.ios.tabs.position('bottom');
            //$ionicConfigProvider.platform.android.tabs.style('standard');
            $ionicConfigProvider.platform.android.tabs.position('bottom');
            //
            //$ionicConfigProvider.platform.ios.navBar.alignTitle('center');
            $ionicConfigProvider.platform.android.navBar.alignTitle('center');
            //
            //$ionicConfigProvider.platform.ios.backButton.previousTitleText('').icon('ion-ios-arrow-thin-left');
            //$ionicConfigProvider.platform.android.backButton.previousTitleText('').icon('ion-android-arrow-back');
            //
            //$ionicConfigProvider.platform.ios.views.transition('ios');
            //$ionicConfigProvider.platform.android.views.transition('android');


            //全局页面缓存配置 ionic中有四种配置页面缓存的方式，详见http://ionicframework.com/docs/api/directive/ionNavView/
            $ionicConfigProvider.views.forwardCache(false);//禁用缓存
        })
        .config(function ($sceDelegateProvider, appConfig) {
            //非安全资源访问白名单
            $sceDelegateProvider.resourceUrlWhitelist([
                'self',
                //exp: 'http://srv*.assets.example.com/**'
                `${appConfig.domain}/**`,
            ]);
        });
    ;
})(angular);


