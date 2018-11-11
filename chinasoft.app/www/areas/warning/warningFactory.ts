/**
 * Created by zhangcheng on 2016/12/16.
 */
(function (angular) {
    'use strict';
    angular.module('app.warning.factory', ['ngResource', 'global', 'app.factory'])
        .factory('warningService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return {
                //预警(故障预警、寿命预警)列表
                listResource: $resource(`${appConfig.apiUrl}warning/list?params=:params`, {params: '@params'}),
                //处理预警(置为已处理)
                disposeResource: $resource(`${appConfig.apiUrl}warning/dispose/:id`, {id: '@id'}, {put: {method: 'PUT'}})
            };
        }])
        //故障预警
        .factory('faultService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return {
                //故障详情
                detailResource: $resource(`${appConfig.apiUrl}warning/strategy/fault/detail/:id`, {id: '@id'}),
                //新增保修单
                repairBillResource: $resource(`${appConfig.apiUrl}sercenter/repairbill/add`, {params: '@params'}),
                //故障种类
                typeResource: $resource(`${appConfig.skyUrl}dict/getDictList/dict_rb_type`),
            };
        }])
        //寿命预警
        .factory('lifetimeService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return {
                //预警零件
                elementResource: $resource(`${appConfig.apiUrl}component/bom/detail/:id`, {id: '@id'}),
                //新增换件记录
                changeInfoResource: $resource(`${appConfig.apiUrl}component/changeinfo/add`, {params: '@params'}),
            };
        }])
    ;
})(angular);