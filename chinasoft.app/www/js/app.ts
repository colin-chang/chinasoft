(function (angular) {
    'use strict';

    angular.module('app', ['ionic', 'ionicLazyLoad', 'ngCordova', 'route', 'global', 'config', 'app.controller', 'app.factory'])
        .run(function ($ionicPlatform) {
            $ionicPlatform.ready(function () {
                if (window.cordova && window.cordova.plugins && window.cordova.plugins.Keyboard) {
                    cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
                    cordova.plugins.Keyboard.disableScroll(true);
                }
                // if (window.StatusBar) {
                    // StatusBar.styleDefault();
                    // StatusBar.styleLightContent();
                    // console.log(123);
                    // console.log(StatusBar);

                    // console.log(StatusBar);
                    // StatusBar.backgroundColorByHexString('ff0000');
                    // StatusBar.backgroundColorByName('red');
                    // StatusBar.styleLightContent();
                    // StatusBar.hide();

                //     StatusBar.styleBlackTranslucent();
                //     if (cordova.platformId == 'android') {
                //         StatusBar.backgroundColorByHexString("#f00");
                //     }
                // }
            });
        })
})(angular);

