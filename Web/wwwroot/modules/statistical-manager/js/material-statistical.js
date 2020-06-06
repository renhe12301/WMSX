
var ouId=null;
var treeNode=null;
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    parentWidth = parent.document.getElementById("contentFrame").clientWidth;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').css("height", parentHeight);
    $('#sidebar2').overlayScrollbars({});

    $('#donutChart').css("height", parentHeight / 2 - 105);
    $('#donutChart').css("width", parentWidth - 200);
    $('#barChart').css("height", parentHeight / 2 - 105);
    $('#barChart').css("width", parentWidth - 200);
    // $('#barChart2').css("height", parentHeight / 2 - 105);
    // $('#barChart2').css("width", parentWidth - 200);

    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'ou',url:controllers.ou["get-ou-trees"],
        successCallback:function(rdata)
        {
            if(rdata.Children.length>0)
            {
               var id = rdata.Children[0].Id;
                var name = rdata.Children[0].Name;
                $("#warehouse-title").text(name+"-"+"库存组织物料统计");
                $("#area-title").text(name+"-"+"子库存物料统计");
                // $("#type-title").text(name+"-"+"物料类型统计");
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
            // $("#type-title").text(data.text+"-"+"物料类型统计");
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
        asynTask({type:'get',url:controllers.warehouse["warehouse-material-chart"],
            jsonData:
                {
                    ouId:ouId
                },
            successCallback:function (response) {
                if(response.Code==200)
                {
                    
                    var donutChartCanvas = $('#donutChart').get(0).getContext('2d');
                    var donutData        = {
                        labels: response.Data.labels,
                        datasets: [
                            {
                                data:  response.Data.datas,
                                backgroundColor : getRandomColors(response.Data.labels.length)
                            }
                        ]
                    };
                    var donutOptions     = {
                        maintainAspectRatio : false,
                        responsive : true,
                    };
                    new Chart(donutChartCanvas, {
                        type: 'doughnut',
                        data: donutData,
                        options: donutOptions
                    });
                }

            }
        });

        asynTask({type:'get',url:controllers["reservoir-area"]["area-material-chart"],
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
                                label               : '子库存统计',
                                backgroundColor     : 'rgba(60,141,188,0.9)',
                                borderColor         : 'rgba(60,141,188,0.8)',
                                pointRadius          : false,
                                pointColor          : '#3b8bba',
                                pointStrokeColor    : 'rgba(60,141,188,1)',
                                pointHighlightFill  : '#fff',
                                pointHighlightStroke: 'rgba(60,141,188,1)',
                                data                : response.Data.datas
                            }
                        ]
                    };
                    var barChartCanvas = $('#barChart').get(0).getContext('2d');
                    var barChartData = jQuery.extend(true, {}, barChartData);
                    var temp1 = barChartData.datasets[0];
                    barChartData.datasets[0] = temp1;

                    var barChartOptions = {
                        responsive              : true,
                        maintainAspectRatio     : false,
                        datasetFill             : false
                    };
                    new Chart(barChartCanvas, {
                        type: 'bar',
                        data: barChartData,
                        options: barChartOptions
                    });

                }

            }
        });
        
        // asynTask({type:'get',url:controllers["material-type"]["material-type-chart"],
        //     jsonData:
        //         {
        //             ouId:ouId
        //         },
        //     successCallback:function (response) {
        //         if(response.Code==200)
        //         {
        //             var barChartData = {
        //                 labels  : response.Data.labels,
        //                 datasets: [
        //                     {
        //                         label               : '物料类型统计',
        //                         backgroundColor     : 'rgba(60,141,188,0.9)',
        //                         borderColor         : 'rgba(60,141,188,0.8)',
        //                         pointRadius          : false,
        //                         pointColor          : '#3b8bba',
        //                         pointStrokeColor    : 'rgba(60,141,188,1)',
        //                         pointHighlightFill  : '#fff',
        //                         pointHighlightStroke: 'rgba(60,141,188,1)',
        //                         data                : response.Data.datas
        //                     }
        //                 ]
        //             };
        //             var barChartCanvas = $('#barChart2').get(0).getContext('2d');
        //             var barChartData = jQuery.extend(true, {}, barChartData);
        //             var temp1 = barChartData.datasets[0];
        //             barChartData.datasets[0] = temp1;
        //
        //             var barChartOptions = {
        //                 responsive              : true,
        //                 maintainAspectRatio     : false,
        //                 datasetFill             : false
        //             };
        //             var barChart = new Chart(barChartCanvas, {
        //                 type: 'bar',
        //                 data: barChartData,
        //                 options: barChartOptions
        //             });
        //             loadingClose();
        //
        //         }
        //
        //     }
        // });
        loadingClose();
    };






});