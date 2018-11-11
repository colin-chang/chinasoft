(function (angular) {
    'use strict';
    angular.module('app.controller', ['ngCordova'])
        .controller('appController', ['$ionicPlatform', '$cordovaStatusbar', function ($ionicPlatform, $cordovaStatusbar) {
            $ionicPlatform.ready(function () {
// $cordovaStatusbar.styleColor('red');
                if (window.StatusBar)
                    $cordovaStatusbar.styleHex('#f00');
            });

            // $cordovaStatusbar.overlaysWebView(true);
            //
            // 样式: 无 : 0, 白色不透明: 1, 黑色半透明: 2, 黑色不透明: 3
            // $cordovaStatusbar.style(2);
            //
            // // 背景颜色名字 : black, darkGray, lightGray, white, gray, red, green,
            // // blue, cyan, yellow, magenta, orange, purple, brown 注:需要开启状态栏占用视图.
            // $cordovaStatusbar.styleColor('red');

            // $cordovaStatusbar.styleHex('#f00');
            //
            // $cordovaStatusbar.hide();
            //
            // $cordovaStatusbar.show();
            //
            // var isVisible = $cordovaStatusbar.isVisible();


        }]);
})(angular);
