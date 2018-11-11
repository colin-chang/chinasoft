/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.warning.repairController', ['ngCordova', 'global', 'app.warning.factory'])
        .controller('warningRepairController', ['$scope', '$state', '$stateParams', '$cordovaCamera', 'appConfig', 'faultService', function ($scope, $state, $stateParams, $cordovaCamera, appConfig, service) {
            let id = $stateParams.id;
            let alertId = $stateParams.alertId;
            if (!id || !alertId) {
                $state.go('warning.fault');
                return;
            }

            //表单模型
            $scope.data = {
                equipmentId: appConfig.equipmentId,
                inputTime: new Date(),
                recardUserName: '王勇',
                fbWay: 0,
                alertId: alertId,
                linkman: '',
                linktel: '',
                content: '',
                status: 1
            };
            //故障详情
            service.detailResource.get({id: id}, function (data) {
                let fault = data.obj;
                if (!fault)
                    return;
                $scope.fault = {
                    equipmentName: appConfig.equipmentName,
                    shiftName: fault.shiftName,
                    faultCode: fault.knowledge ? fault.knowledge.faultCode : '',
                    partName: fault.partName,
                    failure: fault.knowledge ? fault.knowledge.failureContent : '',
                    isSendFac: true
                };
                $scope.data.title = $scope.fault.failure;
                //故障种类
                service.typeResource.query(function (data) {
                    $scope.data.typeId = data[0].value;
                    $scope.fault.types = data;
                }, function (err) {
                });
            }, function (err) {
            });

            //拍照
            $scope.takePicture = function () {
                document.addEventListener("deviceready", function () {
                    let options = {
                        quality: 50,
                        destinationType: Camera.DestinationType.DATA_URL,
                        sourceType: Camera.PictureSourceType.CAMERA,
                        allowEdit: true,
                        encodingType: Camera.EncodingType.JPEG,
                        // targetWidth: 100,
                        targetHeight: 150,
                        popoverOptions: CameraPopoverOptions,
                        saveToPhotoAlbum: false,
                        correctOrientation: true
                    };

                    $cordovaCamera.getPicture(options).then(function (imageData) {
                        let image = document.getElementById('img');
                        image.src = "data:image/jpeg;base64," + imageData;
                    }, function (err) {
                        // error
                    });

                }, false);
            };

            //新增报修
            $scope.submit = function () {
                $scope.data.isSendFac = $scope.fault.isSendFac ? 1 : 0;
                service.repairBillResource.save($scope.data);
            };
        }]);
})(angular);