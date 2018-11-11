/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.devInfo.videoPlayerController',
        [
            "ngSanitize",
            "com.2fdevs.videogular",
            "com.2fdevs.videogular.plugins.controls",
            "com.2fdevs.videogular.plugins.overlayplay",
            "com.2fdevs.videogular.plugins.poster",
            "com.2fdevs.videogular.plugins.buffering"
        ])
        .controller('devInfoVideoPlayerController', ['$scope', '$state', '$stateParams', '$sce', function ($scope, $state, $stateParams, $sce) {
            // if (!$stateParams.id)
            //     $state.go('devInfo.strategy');
            //
            // console.log($stateParams.id);

            // $scope.goBack = function () {
            // $state.goBack();
            //TODO:goBack
            // };

            this.config = {
                sources: [
                    {
                        src: $sce.trustAsResourceUrl("http://219.238.7.71/mp4files/308700000991D6E7/static.videogular.com/assets/videos/videogular.mp4"),
                        type: "video/mp4"
                    },
                    {src: $sce.trustAsResourceUrl("http://static.videogular.com/assets/videos/videogular.webm"), type: "video/webm"},
                    {src: $sce.trustAsResourceUrl("http://static.videogular.com/assets/videos/videogular.ogg"), type: "video/ogg"}
                ],
                tracks: [
                    {
                        src: "http://www.videogular.com/assets/subs/pale-blue-dot.vtt",
                        kind: "subtitles",
                        srclang: "en",
                        label: "English",
                        default: ""
                    }
                ],
                theme: "lib/videogular-themes-default/videogular.css",
                plugins: {
                    poster: "http://www.videogular.com/assets/images/videogular.png"
                }
            };
        }]);
})(angular);