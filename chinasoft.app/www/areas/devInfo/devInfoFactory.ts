/**
 * Created by zhangcheng on 2016/12/16.
 */
(function (angular) {
    'use strict';
    angular.module('app.devInfo.factory', ['ngResource', 'global', 'app.factory'])
    //设备Bom
        .factory('bomTreeService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            let baseBomUrl = `${appConfig.apiUrl}equipment/bom/tree?equipmentId=:equipmentId`;

            return {
                //设备Bom(顶级虚拟目录)
                equipmentResource: $resource(baseBomUrl, {equipmentId: appConfig.equipmentId}),
                //子结构
                childStructureResource: $resource(`${baseBomUrl}&parentId=:parentId`, {equipmentId: appConfig.equipmentId, parentId: '@parentId'}),
                //Bom搜索
                searchResource: $resource(`${baseBomUrl}&queryKey=:queryKey`, {
                    equipmentId: appConfig.equipmentId,
                    queryKey: '@queryKey'
                }),
                //单条搜索结果结构
                searchItemResource: $resource(`${baseBomUrl}&queryCode=:queryCode`, {equipmentId: appConfig.equipmentId, queryCode: '@queryCode'}),
                //部位详情(爆炸图/3D/2D)
                positionDetailResource: $resource(`${appConfig.apiUrl}position/detail/:busId`, {busId: '@busId'}),
                //部位/零件 文档列表
                bomDocsResource: $resource(`${appConfig.apiUrl}equipment/techdoc/bom?eid=:eid&bid=:bid`, {eid: appConfig.equipmentId, bid: '@bid'}),
            }
        }])
        //随机文档
        .factory('docService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return $resource(`${appConfig.apiUrl}equipment/techdoc/list?params=:params`, {params: '@params'});
        }])
        //培训资料
        .factory('videoService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return {
                //资料列表
                videoResource: $resource(`${appConfig.apiUrl}equipment/trainvideo/list?params=:params`, {params: '@params'}),
                //资料分类
                videoClassResource: $resource(appConfig.videoClassUrl)
            };
        }])
        //维护策略
        .factory('strategyService', ['$resource', 'appConfig', 'appFactory', function ($resource, appConfig, appFactory) {
            return {
                //Map
                strategymap: $resource(`${appConfig.apiUrl}standard/strategymap/list?mid=:mid&pid=:pid`, {mid: appConfig.equipmentId, pid: '@pid'}),
                // 点检列表
                checkResource: $resource(`${appConfig.apiUrl}standard/check/list?params=:params`, {params: '@params'}),
                // 润滑列表
                lubricationResource: $resource(`${appConfig.apiUrl}standard/lubrication/list?params=:params`, {params: '@params'}),
                // 保养列表
                maintainResource: $resource(`${appConfig.apiUrl}standard/maintain/list?params=:params`, {params: '@params'}),
                // 维修列表
                repairResource: $resource(`${appConfig.apiUrl}standard/repair/list?params=:params`, {params: '@params'}),
                // 项修/大修 列表
                imprsResource: $resource(`${appConfig.apiUrl}standard/techimpr/list?params=:params`, {params: '@params'})
            };
        }])
        //专家经验
        .factory('konwledgeService', ['$resource', 'appConfig', function ($resource, appConfig) {
            return {
                //Bom结构
                bomResource: $resource(`${appConfig.apiUrl}model/bom/list?moduleId=:mid&pid=:pid`, {mid: appConfig.equipmentId, pid: '@pid'}),
                //现象
                failureResource: $resource(`${appConfig.apiUrl}knowledge/listfaultfailure?moduleId=:mid&partId=:partId`, {
                    mid: appConfig.equipmentId,
                    partId: '@partId'
                }),
                //原因
                causeResource: $resource(`${appConfig.apiUrl}knowledge/listfaultcause?moduleId=:mid&partId=:partId&failureId=:failureId`, {
                    mid: appConfig.equipmentId,
                    partId: '@partId',
                    failureId: '@failureId'
                }),
                //解决方案
                treatmentResource: $resource(`${appConfig.apiUrl}knowledge/listfaulttrentment?moduleId=:mid&partId=:partId&failureId=:failureId&causeId=:causeId`, {
                    mid: appConfig.equipmentId,
                    partId: '@partId',
                    failureId: '@failureId',
                    causeId: '@causeId'
                }),
                //搜索Boms层级结构
                searchBoms: $resource(`${appConfig.apiUrl}model/bom/treekl?modelId=:mid&failureNm=:failureNm`, {
                    mid: appConfig.equipmentId,
                    failureNm: '@failureNm'
                }),
                //搜索现象
                searchFailure: $resource(`${appConfig.apiUrl}knowledge/listfaultfailure?moduleId=:mid&partId=:partId&kw=:kw`, {
                    mid: appConfig.equipmentId,
                    partId: '@partId',
                    kw: '@kw'
                }),
            };
        }])
        //备件
        .factory('spareService', ['$resource', 'appConfig', function ($resource, appConfig) {
            return {
                //备件清单
                listResource: $resource(`${appConfig.apiUrl}component/spearparts/list?params=:params`, {params: '@params'}),
                //备件详情
                detailResource: $resource(`${appConfig.apiUrl}component/bom/detail/:id`, {id: '@id'})
            };
        }])
    ;
})(angular);