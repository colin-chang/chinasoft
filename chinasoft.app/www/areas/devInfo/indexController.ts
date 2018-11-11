(function (angular) {
    'use strict';

    angular.module('app.devInfo.indexController', ['app.devInfo.indexFactory'])
        .controller('devInfoIndexController', ['$scope', 'devInfoIndexFactory', function ($scope, fct) {
            fct.setBannerSrc();
        }]);
})(angular);