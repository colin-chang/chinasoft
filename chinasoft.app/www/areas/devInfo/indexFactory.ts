/**
 * Created by zhangcheng on 2016/12/6.
 */
(function (angular, $) {
    'use strict';
    angular.module('app.devInfo.indexFactory', ['app.factory'])
        .factory('devInfoIndexFactory', ['appFactory', function (appFactory) {
            return {
                setBannerSrc: function () {
                    let content = $('.content-banner');
                    let imgPath = content.css('background-image').replace(/url\("(.+)"\)/, "$1");
                    $(".img-banner").attr('src', appFactory.getUrlParts(imgPath).pathname);
                },
            };
        }]);
})(angular, $);