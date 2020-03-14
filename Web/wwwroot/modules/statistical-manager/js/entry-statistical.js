
var ouId=null;
var treeNode=null;
var queryType = 0;
$(function () {
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').overlayScrollbars({});

    renderTree({
        rootId: 0, renderTarget: 'jsTree', depthTag: 'ou', url: controllers.ou["get-ou-trees"],
        successCallback: function (rdata) {
            if (rdata.Children.length > 0) {
                var id = rdata.Children[0].Id;
                var name = rdata.Children[0].Name;
                $("#warehouse-title").text(name+"-"+"库存组织入库统计");
                $("#area-title").text(name+"-"+"子库存入库统计");
                ouId = id;
                loadChart(ouId);
            }
        },
        selectNodeCall: function (node, data) {
            treeNode = data;
            if (data.type == "ou") {
                ouId = data.id;
            }
            $("#warehouse-title").text(data.text+"-"+"库存组织入库统计");
            $("#area-title").text(data.text+"-"+"子库存入库统计");
            loadChart(ouId);
        },
        showRoot: true
    });

    function getRandomColors(len) {
        var colors = [];
        for (var i = 0; i < len; i++) {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for (var j = 0; j < 6; j++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            colors.push(color);
        }
        return colors;
    }

    var loadChart = function (ouId) {
        loadingShow();
        asynTask({
            type: 'get', url: controllers.warehouse["warehouse-entry-out-record-chart"],
            jsonData:
                {
                    ouId: ouId,
                    inOutType: 0,
                    queryType: queryType
                },
            successCallback: function (response) {
                if (response.Code == 200) {

                    var donutChartCanvas = $('#donutChart').get(0).getContext('2d');
                    var donutData = {
                        labels: response.Data.labels,
                        datasets: [
                            {
                                data: response.Data.datas,
                                backgroundColor: getRandomColors(response.Data.labels.length)
                            }
                        ]
                    };
                    var donutOptions = {
                        maintainAspectRatio: false,
                        responsive: true,
                    };
                    var donutChart = new Chart(donutChartCanvas, {
                        type: 'doughnut',
                        data: donutData,
                        options: donutOptions
                    });
                }

            }
        });

        asynTask({
            type: 'get', url: controllers["reservoir-area"]["area-entry-out-record-chart"],
            jsonData:
                {
                    ouId: ouId,
                    inOutType: 0,
                    queryType: queryType
                },
            successCallback: function (response) {
                if (response.Code == 200) {
                    var barChartData = {
                        labels: response.Data.labels,
                        datasets: [
                            {
                                label: '子库存入库统计',
                                backgroundColor: 'rgba(60,141,188,0.9)',
                                borderColor: 'rgba(60,141,188,0.8)',
                                pointRadius: false,
                                pointColor: '#3b8bba',
                                pointStrokeColor: 'rgba(60,141,188,1)',
                                pointHighlightFill: '#fff',
                                pointHighlightStroke: 'rgba(60,141,188,1)',
                                data: response.Data.datas
                            }
                        ]
                    };
                    var barChartCanvas = $('#barChart').get(0).getContext('2d');
                    var barChartData = jQuery.extend(true, {}, barChartData);
                    var temp1 = barChartData.datasets[0];
                    barChartData.datasets[0] = temp1;

                    var barChartOptions = {
                        responsive: true,
                        maintainAspectRatio: false,
                        datasetFill: false
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

    $("#today-btn").click(function () {
        queryType = 0;
        loadChart(ouId);
    });
    $("#week-btn").click(function () {
        queryType = 1;
        loadChart(ouId);
    });
    $("#month-btn").click(function () {
        queryType = 2;
        loadChart(ouId);
    });
    $("#season-btn").click(function () {
        queryType = 3;
        loadChart(ouId);
    });
    $("#year-btn").click(function () {
        queryType = 4;
        loadChart(ouId);
    });

});