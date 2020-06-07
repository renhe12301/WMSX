
var ouId=null;
var treeNode=null;
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    parentWidth = parent.document.getElementById("contentFrame").clientWidth;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#myTabContent').css("height", parentHeight-70);
    $('#wChart').css("height", parentHeight-100);
    $('#wChart').css("width", parentWidth-$('#sidebar').css("width"));
    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'ou',url:controllers.ou["get-ou-trees"],
        selectNodeCall:function (node, data) {
            treeNode=data;
            if(data.type=="ou")
            {
                ouId=data.id;
            }
            loadChart(ouId,4);
        },
        showRoot:true
    });

    var loadChart=function(ouId,queryType)
    {
        loadingShow();

        asynTask({
            type: 'get',
            url: controllers.statistical["in-order-chart"],
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
                            data:['入库','退料'],
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
                                name:'入库',
                                type:'bar',
                                data: response.Data.warehouseDatas,
                                itemStyle: {
                                    normal: {
                                        color:"#45c0ff"
                                    }
                                }
                            },
                            {
                                name:'退料',
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


        loadingClose();
    };
    
    $("#today-btn").click(function () {
        if(!ouId) 
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        loadChart(ouId,5)
    });
    $("#week-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        loadChart(ouId,1)
    });
    $("#month-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        loadChart(ouId,2)
    });
    $("#season-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        loadChart(ouId,3)
    });
    $("#year-btn").click(function () {
        if(!ouId)
        {
            toastr.error("请选择左边业务对象!", '错误信息', {timeOut: 3000});
            return;
        }
        loadChart(ouId,4)
    });
});