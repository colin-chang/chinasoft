/**
 * Created by zhangcheng on 2016/12/6.
 */
(function (angular, $) {
    'use strict';
    angular.module('app.home.factory', ['app.factory', 'ngResource', 'global'])
        .factory('homeFactory', ['appFactory', function (appFactory) {
            return {
                //定位设备信息Tab模块
                placeEquInfoSection: function () {
                    let content = $('.content-banner');
                    let imgPath = content.css('background-image').replace(/url\("(.+)"\)/, "$1");
                    let src = appFactory.getUrlParts(imgPath).pathname;
                    let img = $(".img-banner");
                    img.attr('src', src);
                    let top = 0;
                    let timer = setInterval(function () {
                        top = img.height();
                        if (top > 0) {
                            content.height(top);
                            let section = $('.section-equinfo');
                            let h = $('#home').height() - $('ion-header-bar').height() - top - section.find('.tabs').height() * 2;
                            section.css({'top': top, 'height': h, 'display': 'block'});
                            clearInterval(timer);
                        }
                    }, 50);
                },
            };
        }])
        //设备概况
        .factory('equipmentService', ['$resource', 'appConfig', function ($resource, appConfig) {
            return $resource(`${appConfig.apiUrl}/equipment/info/detail/:equipmentId`, {equipmentId: appConfig.equipmentId});
        }])
        .factory('workOrderService', ['$resource', 'appConfig', function ($resource, appConfig) {
            return $resource(`${appConfig.apiUrl}/equipment/info/parms/:equipmentId`, {equipmentId: appConfig.equipmentId});
        }])
    ;
})(angular, $);