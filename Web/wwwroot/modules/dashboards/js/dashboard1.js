$(function(){
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/dashboard")
        .build();
    setInterval(function () {
        connection.invoke("SendHeart").catch(function (err) {
        });
    }, 1000);
    
    var inRecordChart = echarts.init($("#buyTime")[0]);
    initInRecordAnalysis(inRecordChart);
    connection.on("ShowInRecordAnalysis", (datas) => {
        var option = inRecordChart.getOption();
        option.series[0].data = datas[0];
        inRecordChart.setOption(option);
    });

    var outRecordChart = echarts.init($("#bookAret")[0]);
    initOutRecordAnalysis(outRecordChart);
    connection.on("ShowOutRecordAnalysis", (datas) => {
        var option = outRecordChart.getOption();
        option.series[0].data = datas[0];
        outRecordChart.setOption(option);
    });

    var inOrderChart = echarts.init($("#bookBmonth")[0]);
    initInOrderAnalysis(inOrderChart);
    connection.on("ShowInOrderAnalysis", (datas) => {
        var option = inOrderChart.getOption();
        option.series[0].data = datas[0];
        option.series[1].data = datas[1];
        inOrderChart.setOption(option);
    });

    var orderChronergyChart = echarts.init($("#orderChronergy")[0]);
    initOrderChronergyAnalysis(orderChronergyChart);
    connection.on("ShowOrderChronergyChartAnalysis", (datas) => {
        var option = orderChronergyChart.getOption();
        option.series[0].data = datas[0];
        option.series[1].data = datas[1];
        option.series[2].data = datas[2];
        orderChronergyChart.setOption(option);
    });

    var outOrderChart = echarts.init($("#whearAbook")[0]);
    initOutOrderAnalysis(outOrderChart);
    connection.on("ShowOutOrderAnalysis", (datas) => {
        var option = outOrderChart.getOption();
        option.series[0].data = datas[0];
        option.series[1].data = datas[1];
        outOrderChart.setOption(option);
    });

    var orderTypeChart = echarts.init($("#rode01")[0]);
    initOrderTypeAnalysis(orderTypeChart);
    connection.on("ShowOrderTypeAnalysis", (datas) => {
        var option = orderTypeChart.getOption();
        option.series[0].data[0].value = datas[0];
        option.series[0].data[1].value = datas[1];
        option.series[0].data[2].value = datas[2];
        option.series[0].data[3].value = datas[3];
        orderTypeChart.setOption(option);
    });

    var stockCountChart = echarts.init($("#Package01")[0]);
    initStockCountAnalysis(stockCountChart);
    connection.on("ShowStockCountAnalysis", (data) => {
        var option = stockCountChart.getOption();
        option.series[0].data = data;
        stockCountChart.setOption(option);
    });


    var inWeekOrderChart = echarts.init($("#rodeAbook")[0]);
    initWeekOrderAnalysis(inWeekOrderChart);
    connection.on("ShowWeekOrderAnalysis", (datas) => {
        
        var option = inWeekOrderChart.getOption();
        option.series[0].data = datas[0];
        option.series[1].data = datas[1];
        option.series[2].data = datas[2];
        option.series[3].data = datas[3];
        inWeekOrderChart.setOption(option);
    });

    var stockAssestChart = echarts.init($("#seaAbook01")[0]);
    initStockAssestAnalysis(stockAssestChart);
    connection.on("ShowStockAssestAnalysis", (data) => {

        var option = stockAssestChart.getOption();
        option.series[0].data = data;
        stockAssestChart.setOption(option);
    });

    var stockUtilizationChart = echarts.init($("#actionBook")[0]);
    initStockUtilization(stockUtilizationChart)
    connection.on("ShowStockUtilizationAnalysis", (datas) => {
        var option = stockUtilizationChart.getOption();
        option.series[0].data[0].value = datas[0];
        option.series[0].data[1].value = datas[1];
        option.series[0].data[2].value = datas[2];
        stockUtilizationChart.setOption(option);
    });

    var mapChart = echarts.init($("#main")[0]);
    initMap(mapChart);
    
    connection.start().catch(err => console.error(err));
    
    
});

//入库记录图表
function initInRecordAnalysis(chart) {
    option = {
        tooltip: {
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y: 30,
            x2: 30,
            y2: 20,
            borderWidth: 0
        },

        calculable: false,
        legend: {
            data: ['物料入库'],
            textStyle: {
                color: "#e9ebee"

            }
        },
        xAxis: [
            {
                type: 'category',
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }

            }
        ],
        yAxis: [
            {
                type: 'value',
                axisLabel: {
                    formatter: '{value} ',
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                },
                splitLine: {
                    show: false
                },
            }

        ],
        series: [
            {
                name: '物料入库',
                type: 'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#00cc33"
                    }
                }
            }
        ]
    };
    chart.setOption(option);
}

//出库记录图表
function initOutRecordAnalysis(chart) {
    option = {
        tooltip: {
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y: 30,
            x2: 30,
            y2: 20,
            borderWidth: 0
        },

        calculable: false,
        legend: {
            data: ['物料出库'],
            textStyle: {
                color: "#e9ebee"

            }
        },
        xAxis: [
            {
                type: 'category',
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }

            }
        ],
        yAxis: [
            {
                type: 'value',
                axisLabel: {
                    formatter: '{value} ',
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                },
                splitLine: {
                    show: false
                },
            }

        ],
        series: [
            {
                name: '物料出库',
                type: 'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#FFD700"
                    }
                }
            }
        ]
    };
    chart.setOption(option);
}

// 入库单分析
function initInOrderAnalysis(chart) {

    option = {
        tooltip : {
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y: 30,
            x2: 30,
            y2: 20,
            borderWidth: 0
        },

        calculable : false,
        legend: {
            data:['入库','退料'],
            textStyle:{
                color:"#e9ebee"

            }
        },
        xAxis : [
            {
                type : 'category',
                data : ['1月','2月','3月','4月','5月','6月','7月','8月','9月','10月','11月','12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }

            }
        ],
        yAxis : [
            {
                type : 'value',
                axisLabel : {
                    formatter: '{value} ',
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                },
                splitLine: {
                    show: false
                },
            }
        ],
        series : [

            {
                name:'入库',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#45c0ff"
                    }
                }
            },
            {
                name:'退料',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#ff81cb"
                    }
                }
            }
        ]
    };

    chart.setOption(option);
}

// 出库单分析
function initOutOrderAnalysis(chart) {
    option = {
        tooltip : {
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y:30,
            x2:30,
            y2:20,
            borderWidth: 0
        },

        calculable : false,
        legend: {
            data:['领料','退库'],
            textStyle:{
                color:"#e9ebee"

            }
        },
        xAxis : [
            {
                type : 'category',
                data : ['1月','2月','3月','4月','5月','6月','7月','8月','9月','10月','11月','12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }

            }
        ],
        yAxis : [
            {
                type : 'value',
                axisLabel : {
                    formatter: '{value} ',
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                },
                splitLine: {
                    show: false
                },
            }

        ],
        series : [

            {
                name:'领料',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#2e7cff"
                    }
                }
            },
            {
                name:'退库',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#e15828"
                    }
                }
            }
        ]
    };

    chart.setOption(option);
}

//订单类型分析
function initOrderTypeAnalysis(chart) {
    option = {
        legend: {
            orient : 'vertical',
            x : 'left',
            data:['入库','退库','退料','领料'], textStyle:{
                color:"#e9ebee"

            }
        },

        calculable : false,
        series : [

            {
                type:'pie',
                radius : '60%',
                center: ['50%', '60%'],
                roseType : 'area',
                x: '50%',               // for funnel
                max: 40,                // for funnel
                sort : 'ascending',     // for funnel
                data:[
                    {
                        value:0, 
                        name:'入库',
                        itemStyle: {
                            normal: {
                                color:"#45c0ff"
                            }
                        }},
                    {
                        value:0, 
                        name:'退库',
                        itemStyle: {
                            normal: {
                                color:"#e15828"
                            }
                        }},
                    {
                        value:0, 
                        name:'退料',
                        itemStyle: {
                            normal: {
                                color:"#ff81cb"
                            }
                        }},
                    {
                        value:0, 
                        name:'领料',
                        itemStyle: {
                            normal: {
                                color:"#2e7cff"
                            }
                        }
                    }
                    
                ]
            }
        ]
    };
    chart.setOption(option);
}

//库存量分析
function initStockCountAnalysis(chart) {
    var option = {
        tooltip: {
            show: true,
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y:30,
            x2:30,
            y2:40,
            borderWidth: 0
        },
        legend: {
            data: [],
            orient: 'vertical',
            textStyle: { fontWeight: 'bold', color: '#a4a7ab' }
        },

        calculable: false,
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }
            }

        ],
        yAxis: [
            {
                type: 'value',
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                }
            }
        ],
        series: [
            {
                name: '',
                type: 'line',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color: '#02bcbc'
                    }
                }
            }
        ]
    };

    chart.setOption(option);

}

// 一周订单分析
function initWeekOrderAnalysis(chart) {
    option = {
        tooltip : {
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y:30,
            x2:30,
            y2:20,
            borderWidth: 0
        },
        calculable : false,
        legend: {
            data:['入库','退库','退料','领料'],
            textStyle:{
                color:"#e9ebee"

            }
        },
        xAxis : [
            {
                type : 'category',
                data : ['周一','周二','周三','周四','周五','周六','周日'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }

            }
        ],
        yAxis : [
            {
                type : 'value',
                axisLabel : {
                    formatter: '{value} ',
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                },
                splitLine: {
                    show: false
                },
            }

        ],
        series : [

            {
                name:'入库',
                type:'bar',
                data:[0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#45c0ff"
                    }
                }
            },
            {
                name:'退库',
                type:'bar',
                data:[0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#e15828"
                    }
                }
            },
            {
                name:'退料',
                type:'bar',
                data:[0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#ff81cb"
                    }
                }
            },
            {
                name:'领料',
                type:'bar',
                data:[0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#2e7cff"
                    }
                }
            }
        ]
    };

    chart.setOption(option);
}

// 库存资产分析
function initStockAssestAnalysis(chart) {
    var option = {
        tooltip: {
            show: true,
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y:30,
            x2:30,
            y2:40,
            borderWidth: 0
        },
        legend: {
            data: [],
            orient: 'vertical',
            textStyle: { fontWeight: 'bold', color: '#a4a7ab' }
        },

        calculable: false,
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }
            }

        ],
        yAxis: [
            {
                type: 'value',
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                }
            }
        ],
        series: [
            {
                name: '',
                type: 'line',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color: '#02bcbc'
                    }
                }
            }
        ]
    };

    chart.setOption(option);
}

// 库存利用率
function initStockUtilization(chart) {
    option = {
        legend: {
            orient : 'vertical',
            x : 'left',
            data:['空货位','空托盘','物料'], textStyle:{
                color:"#e9ebee"

            }
        },

        calculable : false,
        series : [

            {
                type:'pie',
                radius : '70%',
                center: ['50%', '60%'],
                splitLine:{show: false},
                roseType : 'area',
                x: '50%',               // for funnel
                max: 40,                // for funnel
                sort : 'ascending',     // for funnel
                data:[
                    {
                        value:0,
                        name:'空货位',
                        itemStyle: {
                            normal: {
                                color:"#2e7cff"
                            }
                        }},
                    {
                        value:0,
                        name:'空托盘',
                        itemStyle: {
                            normal: {
                                color:"#ffcb89"
                            }
                        }},
                    {
                        value:0,
                        name:'物料',
                        itemStyle: {
                            normal: {
                                color:"#005ea1"
                            }
                        }
                    }
                ]
            }
        ]
    };
    chart.setOption(option);
}


function initOrderChronergyAnalysis(chart) {

    option = {
        tooltip : {
            trigger: 'axis'
        },
        grid: {
            x: 46,
            y: 30,
            x2: 30,
            y2: 20,
            borderWidth: 0
        },

        calculable : false,
        legend: {
            data:['待处理','执行中','已完成'],
            textStyle:{
                color:"#e9ebee"

            }
        },
        xAxis : [
            {
                type : 'category',
                data : ['1月','2月','3月','4月','5月','6月','7月','8月','9月','10月','11月','12月'],
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'center'
                    }
                }

            }
        ],
        yAxis : [
            {
                type : 'value',
                axisLabel : {
                    formatter: '{value} ',
                    textStyle: {
                        color: '#a4a7ab',
                        align: 'right'
                    }
                },
                splitLine: {
                    show: false
                },
            }
        ],
        series : [

            {
                name:'待处理',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#45c0ff"
                    }
                }
            },
            {
                name:'执行中',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#feb602"
                    }
                }
            },
            {
                name:'已完成',
                type:'bar',
                data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                itemStyle: {
                    normal: {
                        color:"#ff81cb"
                    }
                }
            }
        ]
    };

    chart.setOption(option);
}

function initMap(chart) {
    var dataList=[
        {name:"南海诸岛",value:0},
        {name: '北京', value: 0},
        {name: '天津', value: 0},
        {name: '上海', value: 0},
        {name: '重庆', value: 0},
        {name: '河北', value: 0},
        {name: '河南', value: 0},
        {name: '云南', value: 0},
        {name: '辽宁', value: 0},
        {name: '黑龙江', value: 0},
        {name: '湖南', value: 0},
        {name: '安徽', value: 0},
        {name: '山东', value: 0},
        {name: '新疆', value: 0},
        {name: '江苏', value: 0},
        {name: '浙江', value: 0},
        {name: '江西', value: 0},
        {name: '湖北', value: 0},
        {name: '广西', value: 0},
        {name: '甘肃', value: 0},
        {name: '山西', value: 0},
        {name: '内蒙古', value: 0},
        {name: '陕西', value: 0},
        {name: '吉林', value: 0},
        {name: '福建', value: 0},
        {name: '贵州', value: 0},
        {name: '广东', value: 0},
        {name: '青海', value: 2},
        {name: '西藏', value: 0},
        {name: '四川', value: 0},
        {name: '宁夏', value: 0},
        {name: '海南', value: 0},
        {name: '台湾', value: 0},
        {name: '香港', value: 0},
        {name: '澳门', value: 0}
    ];
    
    option = {
        tooltip: {
            formatter:function(params,ticket, callback){
                return params.seriesName+'<br />'+params.name+'：'+params.value
            }//数据格式化
        },
        visualMap: {
            min: 0,
            max: 20,
            left: 'left',
            top: 'bottom',
            text: ['高','低'],//取值范围的文字
            inRange: {
                color: ['#eab44a', '#ff0000']//取值范围的颜色
            },
            show:true//图注
        },
        geo: {
            map: 'china',
            roam: true,//不开启缩放和平移
            zoom: 1.23,//视角缩放比例
            label: {
                normal: {
                    show: true,
                    fontSize:'10',
                    color: 'rgba(0,0,0,0.7)'
                }
            },
            itemStyle: {
                normal:{
                    borderColor: 'rgba(0, 0, 0, 0.2)'
                },
                emphasis:{
                    areaColor: '#68bff9',
                    shadowOffsetX: 0,
                    shadowOffsetY: 0,
                    shadowBlur: 20,
                    borderWidth: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        },
        series : [
            {
                name: '实体仓库',
                type: 'map',
                geoIndex: 0,
                data:dataList
            }
        ]
    };
    chart.setOption(option);
}

