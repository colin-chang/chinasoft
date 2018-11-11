/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.videosController', ['global', 'app.factory', 'app.devInfo.factory', 'ngCordova'])
        .controller('devInfoVideosController', ['$scope', 'appConfig', 'appFactory', 'videoService', '$cordovaKeyboard', function ($scope, appConfig, appFactory, service, $cordovaKeyboard) {
            //基本参量
            const ROWSIZE = 2, PAGESIZE = 10;
            let pageIndex = 1;
            $scope.videoGroups = [];
            $scope.classId = '';
            $scope.keyword = '';

            //加载分类列表
            service.videoClassResource.query(function (data) {
                $scope.classList = [{'text': '全部', 'value': ''}].concat(data);
            }, function (err) {
            });

            //初始化
            loadVideos();

            //滚动加载
            $scope.loadMore = function () {
                pageIndex++;
                loadVideos();
            };

            //分类过滤
            $scope.queryClass = function (id: string) {
                if ($scope.classId === id)
                    return;
                pageIndex = 1;
                $scope.classId = id;
                $scope.videoGroups = [];
                loadVideos();
            };

            //复合搜索
            $scope.search = function () {
                $cordovaKeyboard.close();
                pageIndex = 1;
                $scope.videoGroups = [];
                loadVideos();
            };

            //取消
            $scope.cancel = function () {
                $scope.keyword = '';
                $scope.search();
            };

            // //打开文件
            // $scope.openVideo = function (video) {
            //     console.log(video);
            // };

            /*
             * 工具方法
             */
            //构建查询参数
            function createParams(): Object {
                //{"query":{"classId":"","name":"","equipmentId":"01"},"pager":{"pageIndex":1,"pageSize":10},"sort":{}}
                let params = `{"query":{"equipmentId":"${appConfig.equipmentId}","classId":"${$scope.classId}","name":"${$scope.keyword}"},"pager":{"pageIndex":${pageIndex},"pageSize":${PAGESIZE}},"sort":{}}`;
                return {params: encodeURI(params)};
            }

            //加载文档
            function loadVideos() {
                service.videoResource.get(createParams(), function (data) {
                    let videos = data.item;
                    let group = [];
                    while (videos.length > 0) {
                        let video = videos.pop();
                        if (video.type !== 'video')
                            continue;
                        //TODO:处理非视频文件
                        video.thumbnailUrl = `${appConfig.fileUrl}${appFactory.toUrl(video.thumbnail.path)}`;
                        video.shortName = appFactory.omitString(video.name, 10);
                        group.push(video);
                        if (group.length >= ROWSIZE) {
                            $scope.videoGroups.push(group);
                            group = [];
                        }
                    }
                    if (group.length > 0) {
                        for (let i = 0; i <= ROWSIZE - group.length; i++)
                            group.push({});
                        $scope.videoGroups.push(group);
                    }
                    $scope.$broadcast('scroll.infiniteScrollComplete');
                    $scope.canLoadMore = Math.ceil(data.total / PAGESIZE) > pageIndex;
                }, function (err) {
                });
            }
        }]);
})(angular);
