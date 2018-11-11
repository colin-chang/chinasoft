/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular, $) {
    'use strict';

    angular.module('app.imgViewerController', ['global', 'app.factory'])
        .controller('imgViewerController', ['$scope', '$stateParams', '$ionicHistory', 'appConfig', 'appFactory', function ($scope, $stateParams, $ionicHistory, appConfig, appFct) {

            $scope.title = $stateParams.title;
            $scope.path = !$stateParams.path ? '/img/default.png' : `${appConfig.fileUrl}${appFct.toUrl($stateParams.path)}`;

            //图片纵向居中
            document.querySelector('.img-model').addEventListener('load', function () {
                let parent = $('ion-content');
                let imgHeight = parent.width() * this.height / this.width;
                let top = (parent.height() - imgHeight) / 2 + 'px';
                this.style.top = top;
                $scope.loaded = true;
            });

            $scope.goBack = function () {
                $ionicHistory.goBack();
            };
        }]);
})(angular, $);