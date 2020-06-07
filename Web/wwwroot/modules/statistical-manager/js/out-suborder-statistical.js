var queryType = 4;
var ouId=0;
var treeNode=null;
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    parentWidth = parent.document.getElementById("contentFrame").clientWidth;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').css("height", parentHeight);
    $('#sidebar2').overlayScrollbars({});
    $('#wChart').css("height", parentHeight/2-20);
    $('#wChart').css("width", parentWidth-$('#sidebar').css("width"));
    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'ou',url:controllers.ou["get-ou-trees"],
        successCallback:function()
        {
            loadChart(ouId);
        },
        selectNodeCall:function (node, data) {
            treeNode=data;
            if(data.type=="ou")
            {
                ouId=data.id;
            }
            loadChart(ouId);
            $('#statistical-table').bootstrapTable('refresh',true);
        },
        showRoot:true
    });

    $('#statistical-table').bootstrapTable({
        ajax: function (request) {
            var rd = request.data;
            if (ouId) rd.ouId = ouId;
            rd.queryType = queryType
            asynTask({
                type: 'get',
                url: controllers.statistical["out-sub-order-sheet"],
                jsonData: rd,
                successCallback: function (response) {
                    $('#statistical-table').bootstrapTable('load', response.Data);
                    $('#statistical-table').bootstrapTable('hideLoading');
                    mergeCells(response.Data, "WarehouseName", 1, $('#statistical-table'));
                    mergeCells(response.Data, "TotalStatisticalCount", 1, $('#statistical-table'));
                }
            });
        },
        height: parentHeight/2-20,
        smartDisplay: false,
        columns:
            [
                {
                    title: '库存组织',
                    field: 'WarehouseName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '订单类型',
                    field: 'OrderType',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '订单数量',
                    field: 'StatisticalCount',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '合计',
                    field: 'TotalStatisticalCount',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });

    var loadChart=function(ouId)
    {
        asynTask({
            type: 'get',
            url: controllers.statistical["out-sub-order-chart"],
            jsonData:
                {
                    ouId: ouId,
                    queryType:queryType
                },
            successCallback: function (response) {
                if (response.Code == 200) {
                    var wChart = echarts.init($("#wChart")[0]);
                    woption = {
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
                                color:"#000000"

                            }
                        },
                        xAxis : [
                            {
                                type : 'category',
                                data : response.Data.warehouseLabels,
                                splitLine: {
                                    show: false
                                },
                                axisLabel: {
                                    show: true,
                                    textStyle: {
                                        color: '#000000',
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
                                data: response.Data.warehouseDatas,
                                itemStyle: {
                                    normal: {
                                        color:"#45c0ff"
                                    }
                                }
                            },
                            {
                                name:'退库',
                                type:'bar',
                                data: response.Data.warehouseDatas2,
                                itemStyle: {
                                    normal: {
                                        color:"#feb602"
                                    }
                                }
                            }
                        ]
                    };
                    wChart.setOption(woption);

                }

            }
        });


    };

    $("#today-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        queryType = 5;
        loadChart(ouId);
        $('#statistical-table').bootstrapTable('refresh',true);
    });
    $("#week-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        queryType = 1;
        loadChart(ouId);
        $('#statistical-table').bootstrapTable('refresh',true);
    });
    $("#month-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        queryType = 2;
        loadChart(ouId);
        $('#statistical-table').bootstrapTable('refresh',true);
    });
    $("#season-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        queryType = 3;
        loadChart(ouId);
        $('#statistical-table').bootstrapTable('refresh',true);
    });
    $("#year-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        queryType = 4;
        loadChart(ouId);
        $('#statistical-table').bootstrapTable('refresh',true);
    });
});