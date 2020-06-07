
var ouId=0;
var treeNode=null;
var queryType = 4;
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    parentWidth = parent.document.getElementById("contentFrame").clientWidth;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').css("height", parentHeight);
    $('#sidebar2').overlayScrollbars({});
    $('#wChart').css("height", parentHeight/3-15);
    $('#wChart').css("width", parentWidth-$('#sidebar').css("width"));
    $('#aChart').css("height", parentHeight/3-15);
    $('#aChart').css("width", parentWidth-$('#sidebar').css("width"));
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

    var loadChart=function(ouId)
    {

        asynTask({
            type: 'get',
            url: controllers.statistical["out-record-chart"],
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
                            data:['库存组织入库记录'],
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
                                name:'库存组织入库记录',
                                type:'bar',
                                data: response.Data.warehouseDatas,
                                itemStyle: {
                                    normal: {
                                        color:"#2e7cff"
                                    }
                                }
                            }
                        ]
                    };
                    wChart.setOption(woption);


                    var aChart = echarts.init($("#aChart")[0]);
                    aoption = {
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
                            data:['子库区入库记录'],
                            textStyle:{
                                color:"#000000"

                            }
                        },
                        xAxis : [
                            {
                                type : 'category',
                                data : response.Data.areaLabels,
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
                                name:'子库区入库记录',
                                type:'bar',
                                data: response.Data.areaDatas,
                                itemStyle: {
                                    normal: {
                                        color:"#00cc33"
                                    }
                                }
                            }
                        ]
                    };
                    aChart.setOption(aoption);
                }

            }
        });

    };

    $('#statistical-table').bootstrapTable({
        ajax: function (request) {
            var rd = request.data;
            if (ouId) rd.ouId = ouId;
            rd.queryType = queryType
            asynTask({
                type: 'get',
                url: controllers.statistical["out-record-sheet"],
                jsonData: rd,
                successCallback: function (response) {
                    $('#statistical-table').bootstrapTable('load', response.Data);
                    $('#statistical-table').bootstrapTable('hideLoading');
                    mergeCells(response.Data, "WarehouseName", 1, $('#statistical-table'));
                    mergeCells(response.Data, "TotalStatisticalCount", 1, $('#statistical-table'));
                }
            });
        },
        height: parentHeight/3-15,
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
                    title: '库区名称',
                    field: 'AreaName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '入库记录',
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