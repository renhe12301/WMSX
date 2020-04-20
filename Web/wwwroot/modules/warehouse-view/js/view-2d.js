var phyId=null;
var types=null;
var showLocationCargo=function (id) {
    $('#location-detail-dlg').modal('show');
    asynTask({
        type:'get',
        url:controllers["warehouse-material"]["get-materials"],
        jsonData: {locationId:id},
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                if(response.Data.length>0)
                {
                    var data = response.Data[0];
                    $("#location-code").text(data.LocationCode);
                    $("#tray-code").text(data.TrayCode);
                    $("#material-code").text(data.Code);
                    $("#material-name").text(data.MaterialName);
                    $("#material-count").text(data.MaterialCount);
                    $("#area-name").text(data.ReservoirAreaName);
                    $("#warehouse-name").text(data.WarehouseName);
                    $("#ou-name").text(data.OUName);
                }

            }
        }
    });
};

$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 100;
    $("#sidebar").css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('.card card-primary card-outline card-tabs').css("height", parentHeight);
    asynTask({
        type:'get',
        url:controllers["phy-warehouse"]["get-phy-warehouses"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.PhyName});
                });
                $(".select2").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
                $(".select2").change(function () {
                    phyId=parseInt($("#phy-select").val());
                    loadPlanData();
                });
                if(data.length>0)
                {
                    phyId=parseInt(data[0].id);
                    loadPlanData();
                }
            }
        }
    });

    var loadPlanData = function()
    {
        loadingShow();
        planTable=$("#planTable");
        planTable.html('');
        asynTask({type:'get',url:controllers["location"]["get-max-floor-item-col"],
            jsonData: {phyId:phyId},
            successCallback:function (response) {
                if(response.Code==200)
                {
                    var rowRanCols=response.Data;

                    asynTask({type:'get',url:controllers["location"]["get-locations"],
                        jsonData: {phyId:phyId},
                        successCallback:function (response2) {
                            if(response2.Code==200)
                            {
                                var sl = [];
                                $.each( response2.Data, function( key, value ) {
                                    if(value.Row==0)
                                        sl.push(value);
                                });
                                var content="";

                                if(sl.length>0) {
                                    content += "<tr style='border-bottom-width: 3px;border-bottom-style: solid;border-bottom-color: grey'><td style='background-color: grey;color: white;font-size: 50px;text-align: center;'>出入口</td><td>";
                                    content += "<table>";

                                    for (var i = 0; i < sl.length; i++) {
                                        if (i % parseInt(rowRanCols[1]) <= 0) {
                                            if (i > 0)
                                                content += "</tr>";
                                            content += "<tr>";
                                        }

                                        var icon = "fa fa-grin-beam";
                                        var btnSty = "whitesmoke";
                                        var title = "系统编码：" + sl[i].SysCode + "\n用户编码：" + sl[i].UserCode + "\n";
                                        if (sl[i].InStock == "有货")
                                            btnSty = "orangered";
                                        else if (sl[i].InStock == "空托盘")
                                            btnSty = "dodgerblue";
                                        if (sl[i].Status == "正常") {
                                            icon = "fa fa-grin-beam";
                                            title += "货位状态：正常\n";
                                        } else if (sl[i].Status == "锁定") {
                                            icon = "fa fa-cog fa-spin";
                                            title += "货位状态：执行任务\n";
                                        } else if (sl[i].Status == "禁用") {
                                            icon = "fa fa-skull-crossbones";
                                            title += "货位状态：禁用\n";
                                            btnSty = "gray";
                                        }
                                        title += "货载：" + sl[i].InStock + "\n";
                                        if (sl[i].ReservoirAreaName)
                                            title += "库区：" + sl[i].ReservoirAreaName + "\n";
                                        if (sl[i].WarehouseName)
                                            title += "仓库：" + sl[i].WarehouseName + "\n";
                                        if (sl[i].OUName)
                                            title += "业务实体：" + sl[i].OUName + "\n";

                                        content += "<td>";
                                        content += "<a class='btn btn-app'  title='" + title + "' style='cursor: default;background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + sl[i].SysCode + "</a>";
                                        content += "</td>";
                                    }

                                    content += "</table>";
                                    content += "</td></tr>";
                                }
                                var row=rowRanCols[0];
                                for (var floor=1;floor<=row;floor++)
                                {
                                    if(floor<row)
                                        content+="<tr style='border-bottom-width: 3px;border-bottom-style: solid;border-bottom-color: grey'><td style='background-color: grey;color: white;font-size: 50px;text-align: center;'>第"+floor+"层</td><td>";
                                    else
                                        content+="<tr><td style='background-color: grey;color: white;font-size: 50px;text-align: center;'>第"+floor+"层</td><td>";
                                    content+="<table>";
                                    for (var item=1;item<=rowRanCols[1];item++)
                                    {
                                        content+="<tr>";
                                        for (var col=1;col<=rowRanCols[2];col++)
                                        {
                                            var l = response2.Data.find(x => x.Row == floor&&x.Rank==item&&x.Col==col);
                                            if(!l)continue;
                                            var icon="fa fa-grin-beam";
                                            var btnSty="whitesmoke";
                                            var title="系统编码："+l.SysCode+"\n用户编码："+l.UserCode+"\n";
                                            if(l.InStock=="有货")
                                                btnSty="orangered";
                                            else if(l.InStock=="空托盘")
                                                btnSty="dodgerblue";
                                            if(l.Status=="正常")
                                            {
                                                icon="fa fa-grin-beam";
                                                title+="货位状态：正常\n";
                                            }
                                            else if(l.Status=="锁定")
                                            {
                                                icon="fa fa-cog fa-spin";
                                                title+="货位状态：执行任务\n";
                                            }
                                            else if(l.Status=="禁用")
                                            {
                                                icon="fa fa-skull-crossbones";
                                                title+="货位状态：禁用\n";
                                                btnSty="gray";
                                            }
                                            title+="货载："+l.InStock+"\n";
                                            if(l.ReservoirAreaName)
                                                title+="库区："+l.ReservoirAreaName+"\n";
                                            if(l.WarehouseName)
                                                title+="仓库："+l.WarehouseName+"\n";
                                            if(l.OUName)
                                                title+="业务实体："+l.OUName+"\n";

                                            content+="<td>";
                                            var codes=[];
                                            var scodes=l.UserCode.split('-');
                                            for (var i=1;i<scodes.length;i++) codes.push(parseInt(scodes[i]));
                                            if(l.InStock=="有货"||l.InStock=="空托盘")
                                            {
                                                content+="<a class='btn btn-app' onclick='showLocationCargo("+l.Id+")' title='"+title+"' style='background-color:"+btnSty+";font-size: 2px;'><i class='"+icon+"'></i>"+codes.join('')+"</a>";
                                            }
                                            else {
                                                content+="<a class='btn btn-app'  title='"+title+"' style='cursor: default;background-color:"+btnSty+";font-size: 2px;'><i class='"+icon+"'></i>"+codes.join('')+"</a>";
                                            }

                                            content+="</td>";
                                        }
                                        content+="</tr>";
                                    }
                                    content+="</table>";
                                    content+="</td></tr>";
                                }
                                planTable.append(content);
                                loadingClose();
                            }
                            else {
                                toastr.error(response2.Data, '错误信息', {timeOut: 3000});
                            }
                        }
                    });
                }
                else {
                    toastr.error(response.Data, '错误信息', {timeOut: 3000});
                }
            }
        });
    };

    // $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //     tabType=$("#"+e.target.id).text();
    // });
    //

});