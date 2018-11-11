/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.docsController', ['global', 'app.factory', 'app.devInfo.factory'])
        .controller('devInfoDocsController', ['$scope', 'appConfig', 'appFactory', 'docService', function ($scope, appConfig, appFactory, service) {
            //基本参量
            const ROWSIZE = 3, PAGESIZE = 18;
            let pageIndex = 1;
            let docTypeClassDict = {
                'word': 'icon-fileword positive',
                'pdf': 'icon-filepdf assertive',
                'html': 'icon-809geshi_wendanghtml calm',
                'photo': 'icon-filepicture calm',
                'zip': 'icon-filezip energized'
            };
            $scope.docGroups = [];

            //初始化
            loadDocs();

            //滚动加载
            $scope.loadMore = function () {
                pageIndex++;
                loadDocs();
            };

            //打开文件
            $scope.openFile = function (doc) {
                console.log(doc);
            };

            /*
             * 工具方法
             */
            //构建查询参数
            function createParams(): Object {
                let params = `{"query":{"equipmentId":"${appConfig.equipmentId}","docTypes":"word,pdf,html,photo"},"pager":{"pageIndex":${pageIndex},"pageSize":${PAGESIZE}},"sort":{}}`;
                return {params: encodeURI(params)};
            }

            //加载文档
            function loadDocs() {
                service.get(createParams(), function (data) {
                    let docs = data.item;
                    let group = [];
                    while (docs.length > 0) {
                        let doc = docs.pop();
                        doc.iconClass = docTypeClassDict[doc.type];
                        doc.shortName = appFactory.omitString(doc.name, 6);
                        group.push(doc);
                        if (group.length >= ROWSIZE) {
                            $scope.docGroups.push(group);
                            group = [];
                        }
                    }
                    if (group.length > 0) {
                        for (let i = 0; i <= ROWSIZE - group.length; i++)
                            group.push({});
                        $scope.docGroups.push(group);
                    }
                    $scope.$broadcast('scroll.infiniteScrollComplete');
                    $scope.canLoadMore = Math.ceil(data.total / PAGESIZE) >= pageIndex;
                }, function (err) {
                });
            }
        }]);
})(angular);
