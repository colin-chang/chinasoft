(function () {
    /**
     * 作者: furw
     * 时间: 2016-06-30
     * 描述: 拦截器
     */

    'use strict'
    angular.module('interceptor', [])
        .factory('httpInterceptor', ['$q', 'utilService', function ($q, utilService) {
            var interceptor = {
                'request': function (config) {

                    //将PUT请求方式的请求参数转换为字符串
                    if (config.method == "PUT") {
                        var params = JSON.stringify(config.params);
                        config.data = params;
                        config.params = "";
                    }
                    //追加accessToken
                    var _accessToken = utilService.session().get("skydev-access-token");
                    if (null != _accessToken) {
                        config.headers.accessToken = _accessToken;
                    }
                    return config;
                },
                'response': function (resp) {
                    return resp;
                },
                'requestError': function (rejection) {
                    return $q.reject(rejection);
                },
                'responseError': function (rejection) {
                    if (rejection.status == 500) { //页面跳转到异常页面，显示异常信息
                        if (undefined != rejection.data) {
                            sessionStorage.setItem("exception_message", rejection.data.errorMsg);
                            sessionStorage.setItem("exception_detial", rejection.data.others[0].value);
                            var _path = utilService.getRootPath();
                            window.location.href = _path + "/lib/view/error/exception.html";
                            return;
                        }
                    }
                    if (rejection.status == 0 || rejection.status == -1 || rejection.status == 401 || rejection.status == 403 || rejection.status == 404) {
                        //抛出异常
                        if (rejection.status == 401) {
                            if (rejection.data.errorCode == "SD-001") {
                                window.location.href = utilService.getRootPath() + "/lib/view/error/sexpire.html";

                            }
                        }
                    }

                    return $q.reject(rejection);
                }
            }
            return interceptor;
        }]);
})();