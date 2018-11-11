/**
 * Created by zhangcheng on 2016/12/16.
 */
(function (angular) {
    'use strict';
    angular.module('app.summary.factory', ['ngResource', 'global', 'app.factory'])
        .factory('summaryService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return {
                //预警(故障预警、寿命预警)列表
                listResource: $resource(`${appConfig.apiUrl}workorder/report/list?eid=:eid&date=:date`, {
                    eid: appConfig.equipmentId || '01',
                    date: '@date'
                }),
                //处理预警(置为已处理)
                detailResource: $resource(`${appConfig.apiUrl}workorder/report/detial/:id`, {id: '@id'})
            };
        }]);
})(angular);