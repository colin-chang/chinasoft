(function (angular) {
    'use strict';

    angular.module('app.factory', ['ngCordova'])
        .factory('appFactory', ['$cordovaDialogs', function ($cordovaDialogs) {
            return {
                //解析Url
                getUrlParts: function (url: string) {
                    let lk = document.createElement("a");
                    lk.setAttribute("href", url);
                    return {
                        protocol: lk.protocol,//网络请求协议
                        host: lk.host,//主机（域名+端口）
                        hostname: lk.hostname,//主机名（域名或ip地址）
                        port: lk.port,//端口
                        pathname: lk.pathname,//url相对路径部分
                        search: lk.search,//url参数
                        hash: lk.hash//url锚点部分
                    };
                },
                //格式化URL路径
                toUrl: function (url: string) {
                    while (url.indexOf('\\') > 0)
                        url = url.replace('\\', '/');
                    return url;
                },
                //省略字符串(截取固定长度,超长显示省略号)
                omitString: function (str: string, maxLength: number): string {
                    if (!str || str.length <= maxLength)
                        return str;
                    return `${str.substr(0, maxLength - 1)}…`;
                },
                //判断一个对象是否为空
                isEmptyObject: function (e: Object) {
                    let t;
                    for (t in e)
                        return !1;
                    return !0
                },
                //过滤对象数组元素
                arrayWhere: function (array: Object[], filter: Object): Array<Object> {
                    if (!array || !array.length)
                        return [];
                    if (this.isEmptyObject(filter))
                        return array;

                    let result: Object[];
                    for (let i = 0; i < array.length; i++) {
                        let item = array[i];
                        let confirm = true;
                        for (let p in filter) {
                            if (!item[p] || item[p] !== filter[p]) {
                                confirm = false;
                                break;
                            }
                        }
                        if (confirm)
                            result.push(item);
                    }
                    return result;
                },
                //获取数组中满足条件的第一个元素
                arrayFirstOrDefault: function (array: Object[], filter: Object): Object {
                    if (!array || !array.length)
                        return null;
                    if (this.isEmptyObject(filter))
                        return array[0];

                    for (let i = 0; i < array.length; i++) {
                        let item = array[i];
                        let confirm = true;
                        for (let p in filter) {
                            if (item[p] !== filter[p]) {
                                confirm = false;
                                break;
                            }
                        }
                        if (confirm)
                            return item;
                    }
                    return null;
                },
                //提示框
                alert: function (title: string, content: string) {
                    $cordovaDialogs.alert(content, title, '关闭')
                        .then(function () {
                            // callback success
                        });
                }
            };
        }]);
})(angular);