/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.summary.haltController', [])
        .controller('summaryHaltController', ['$scope', '$ionicHistory', function ($scope, $ionicHistory) {
            $scope.goBack = function () {
                $ionicHistory.goBack();
            };

            let myChart = echarts.init(document.getElementById('pie'));
            let option = {
                title: {
                    text: '停机时间饼图',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: ['故障', '待料', '换牌', '换班']
                },
                color: ['#276791', '#4cb7e6', '#33cd5f', '#ffc900'],
                series: [
                    {
                        name: '停机原因',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        data: [
                            {value: 35, name: '故障'},
                            {value: 18, name: '待料'},
                            {value: 6, name: '换牌'},
                            {value: 2, name: '换班'},
                        ],
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };

            myChart.setOption(option);
        }]);
})(angular);