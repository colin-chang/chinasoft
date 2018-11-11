/**
 * Created by zhangcheng on 2016/11/30.
 */
(function (angular) {
    'use strict';

    angular.module('app.summary.warningController', [])
        .controller('summaryWarningController', ['$scope', '$ionicHistory', function ($scope, $ionicHistory) {
            $scope.goBack = function () {
                $ionicHistory.goBack();
            };

            let pie = echarts.init(document.getElementById('pie'));
            let pieOpt = {
                title: {
                    text: '报警统计饼图',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: ['故障报警', '质量报警', '设备报警', '其他报警']
                },
                color: ['#276791', '#4cb7e6', '#33cd5f', '#ffc900'],
                series: [
                    {
                        name: '报警种类',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        data: [
                            {value: 35, name: '故障报警'},
                            {value: 18, name: '质量报警'},
                            {value: 6, name: '设备报警'},
                            {value: 2, name: '其他报警'},
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
            pie.setOption(pieOpt);

            let histogram = echarts.init(document.getElementById('histogram'));
            let histogramOpt = {
                title: {
                    text: '故障报警柱状图'
                },
                tooltip: {},
                color: ['#40cee5'],
                xAxis: {
                    data: ["故障1", "故障2", "故障3", "故障4", "故障5", "故障6"]
                },
                yAxis: {},
                series: [{
                    name: '故障',
                    type: 'bar',
                    data: [5, 20, 36, 10, 10, 20]
                }]
            };
            histogram.setOption(histogramOpt);
        }]);
})(angular);