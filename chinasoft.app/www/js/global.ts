/**
 * Created by zhangcheng on 2016/11/29.
 */
// 全局配置
(function (angular) {
    const domain = 'http://115.28.209.159';
    angular.module("global", []).constant('appConfig', {
        'equipmentId': '01',
        'equipmentName': '',
        'domain': `${domain}`,
        'apiUrl': `${domain}:8081/devinfo/api/`,
        'fileUrl': `${domain}/files/`,
        //平台服务地址
        'skyUrl': `${domain}:8080/skydev/api/`,
        //电路图根地址
        'circuitUrl': `${domain}/devinfo/app/component/circuitViewer_wpf.html?busid=`,
        //培训资料分类API地址
        'videoClassUrl': `${domain}:8080/skydev/api/dict/getDictList/doc/`
    });
})(angular);
