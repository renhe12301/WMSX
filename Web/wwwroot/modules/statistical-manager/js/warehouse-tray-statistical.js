
var ouId=null;
var treeNode=null;
$(function () {
    $('#sidebar').overlayScrollbars({ });
    $('#sidebar2').overlayScrollbars({ });

    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'ou',url:controllers.ou["get-ou-trees"],
        successCallback:function(rdata)
        {
            if(rdata.Children.length>0)
            {
                var id = rdata.Children[0].Id;
                var name = rdata.Children[0].Name;
                $("#warehouse-title").text(name+"-"+"库存组织托盘统计");
                $("#area-title").text(name+"-"+"子库存托盘统计");
                loadChart(id);
            }

        },
        selectNodeCall:function (node, data) {
            treeNode=data;
            if(data.type=="ou")
            {
                ouId=data.id;
            }
            $("#warehouse-title").text(data.text+"-"+"库存组织物料统计");
            $("#area-title").text(data.text+"-"+"子库存物料统计");
            loadChart(ouId);
        },
        showRoot:true
    });

    function getRandomColors(len) {
        var colors = [];
        for (var i = 0;i<len;i++)
        {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for(var j=0;j<6;j++)
            {
                color+=letters[Math.floor(Math.random()*16)];
            }
            colors.push(color);
        }
        return colors;
    }
    var loadChart=function(ouId)
    {
        loadingShow();
        asynTask({type:'get',url:controllers.warehouse["warehouse-tray-chart"],
            jsonData:
                {
                    ouId:ouId
                },
            successCallback:function (response) {
                if(response.Code==200)
                {
                    var donutChartData = {
                        labels  : response.Data.labels,
                        datasets: [
                            {
                                label               : '空托盘',
                                backgroundColor     : 'rgba(60,141,188,0.9)',
                                borderColor         : 'rgba(60,141,188,0.8)',
                                pointRadius          : false,
                                pointColor          : '#3b8bba',
                                pointStrokeColor    : 'rgba(60,141,188,1)',
                                pointHighlightFill  : '#fff',
                                pointHighlightStroke: 'rgba(60,141,188,1)',
                                data                : response.Data.datas1
                            },
                            {
                                label               : '非空托盘',
                                backgroundColor     : 'rgba(210, 214, 222, 1)',
                                borderColor         : 'rgba(210, 214, 222, 1)',
                                pointRadius         : false,
                                pointColor          : 'rgba(210, 214, 222, 1)',
                                pointStrokeColor    : '#c1c7d1',
                                pointHighlightFill  : '#fff',
                                pointHighlightStroke: 'rgba(220,220,220,1)',
                                data                : response.Data.datas2
                            }
                        ]
                    };
                    var donutChartCanvas = $('#donutChart').get(0).getContext('2d');
                    var donutChartData = jQuery.extend(true, {}, donutChartData);
                    var temp1 = donutChartData.datasets[0];
                    var temp2 = donutChartData.datasets[1];
                    donutChartData.datasets[0] = temp1;
                    donutChartData.datasets[1] = temp2;
                    var donutOptions     = {
                        responsive              : true,
                        maintainAspectRatio     : false,
                        datasetFill             : false
                    };
                    var donutChart = new Chart(donutChartCanvas, {
                        type: 'bar',
                        data: donutChartData,
                        options: donutOptions
                    });
                }

            }
        });

        asynTask({type:'get',url:controllers["reservoir-area"]["area-tray-chart"],
            jsonData:
                {
                    ouId:ouId
                },
            successCallback:function (response) {
                if(response.Code==200)
                {
                    var barChartData = {
                        labels  : response.Data.labels,
                        datasets: [
                            {
                                label               : '空托盘',
                                backgroundColor     : 'rgba(60,141,188,0.9)',
                                borderColor         : 'rgba(60,141,188,0.8)',
                                pointRadius          : false,
                                pointColor          : '#3b8bba',
                                pointStrokeColor    : 'rgba(60,141,188,1)',
                                pointHighlightFill  : '#fff',
                                pointHighlightStroke: 'rgba(60,141,188,1)',
                                data                : response.Data.datas1
                            },
                            {
                                label               : '非空托盘',
                                backgroundColor     : 'rgba(210, 214, 222, 1)',
                                borderColor         : 'rgba(210, 214, 222, 1)',
                                pointRadius         : false,
                                pointColor          : 'rgba(210, 214, 222, 1)',
                                pointStrokeColor    : '#c1c7d1',
                                pointHighlightFill  : '#fff',
                                pointHighlightStroke: 'rgba(220,220,220,1)',
                                data                : response.Data.datas2
                            }
                        ]
                    };
                    var barChartCanvas = $('#barChart').get(0).getContext('2d');
                    var barChartData = jQuery.extend(true, {}, barChartData);
                    var temp1 = barChartData.datasets[0];
                    var temp2 = barChartData.datasets[1];
                    barChartData.datasets[0] = temp1;
                    barChartData.datasets[1] = temp2;
                    var barChartOptions = {
                        responsive              : true,
                        maintainAspectRatio     : false,
                        datasetFill             : false
                    };
                    var barChart = new Chart(barChartCanvas, {
                        type: 'bar',
                        data: barChartData,
                        options: barChartOptions
                    });

                    loadingClose();
                }

            }
        });


    };

});