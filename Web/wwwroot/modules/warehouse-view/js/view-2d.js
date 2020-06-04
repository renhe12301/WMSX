var phyId=1;
var types=null;
var showLocationCargo=function (id,sysCode,cargoType) {
    $('#location-detail-dlg').modal('show');
    $("#location-code").text(sysCode);
    $("#tray-code").text("");
    $("#material-code").val("");
    $("#material-name").val("");
    $("#material-count").val("");
    $("#area-name").val("");
    $("#warehouse-name").val("");
    $("#ou-name").val("");
    $("#material-spec").val("");
    if (cargoType == "有货") {
        asynTask({
            type: 'get',
            url: controllers["warehouse-material"]["get-materials"],
            jsonData: { locationId: id },
            successCallback: function (response) {
                if (response.Code == 200) {
                    if (response.Data.length > 0) {
                        var data = response.Data[0];
                        $("#location-code").text(data.LocationCode);
                        $("#tray-code").text(data.TrayCode);
                        $("#material-code").val(data.Code);
                        $("#material-name").val(data.MaterialName);
                        $("#material-count").val(data.MaterialCount);
                        $("#area-name").val(data.ReservoirAreaName);
                        $("#warehouse-name").val(data.WarehouseName);
                        $("#ou-name").val(data.OUName);
                        $("#material-spec").val(data.Spec);
                    }

                }
            }
        });
    }
    else if (cargoType == "空托盘") {
        asynTask({
            type: 'get',
            url: controllers["warehouse-tray"]["get-trays"],
            jsonData: { locationId: id },
            successCallback: function (response) {
                if (response.Code == 200) {
                    if (response.Data.length > 0) {
                        var data = response.Data[0];
                        $("#location-code").text(data.LocationCode);
                        $("#tray-code").text(data.TrayCode);
                    }

                }
            }
        });
    }
};

$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 100;
    $("#sidebar").css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar3').overlayScrollbars({});
    $('.tab-content border border-top-0').css("height", parentHeight);

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
                                        var title = "货位编码：" + sl[i].SysCode + "\n";
                                        if (sl[i].InStock == "有货")
                                            btnSty = "orangered";
                                        else if (sl[i].InStock == "空托盘")
                                            btnSty = "dodgerblue";
                                        if (sl[i].Status == "正常") {
                                            icon = "fa fa-grin-beam";
                                            title += "货位状态：正常\n";
                                        } else if (sl[i].Status == "锁定") {
                                            icon = "fa fa-lock";
                                            title += "货位状态：锁定\n";
                                        } else if (sl[i].Status == "禁用") {
                                            icon = "fa fa-skull-crossbones";
                                            title += "货位状态：禁用\n";
                                            btnSty = "gray";
                                        }
                                        title += "货载：" + sl[i].InStock + "\n";
                                        title+="任务："+sl[i].IsTask+"\n";
                                        if (sl[i].ReservoirAreaName)
                                            title += "库区：" + sl[i].ReservoirAreaName + "\n";
                                        if (sl[i].WarehouseName)
                                            title += "仓库：" + sl[i].WarehouseName + "\n";
                                        if (sl[i].OUName)
                                            title += "业务实体：" + sl[i].OUName + "\n";

                                        content += "<td>";
                                        
                                        if (sl[i].InStock == "有货" && sl[i].Status != "禁用") {
                                            content += "<a class='btn btn-app' onclick='showLocationCargo(" + sl[i].Id + "," + JSON.stringify(sl[i].UserCode).replace(/"/g, '&quot;') + ",\"有货\")' title='" + title + "' style='background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + sl[i].SysCode + "</a>";
                                        }
                                        else if (sl[i].InStock == "空托盘" && sl[i].Status != "禁用") {
                                            content += "<a class='btn btn-app' onclick='showLocationCargo(" + sl[i].Id + "," + JSON.stringify(sl[i].UserCode).replace(/"/g, '&quot;') + ",\"空托盘\")' title='" + title + "' style='background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + sl[i].SysCode + "</a>";
                                        }
                                        else if (sl[i].InStock == "无货" && sl[i].InStock != "禁用") {
                                            content += "<a class='btn btn-app'  title='" + title + "' style='background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + sl[i].SysCode + "</a>";
                                        }
                                        else {
                                            content += "<a class='btn btn-app'  title='" + title + "' style='cursor: default;background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + sl[i].SysCode + "</a>";
                                        }
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
                                            var title="货位编码："+l.SysCode+"\n";
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
                                                icon="fa fa-lock";
                                                title+="货位状态：锁定\n";
                                            }
                                            else if(l.Status=="禁用")
                                            {
                                                icon="fa fa-skull-crossbones";
                                                title+="货位状态：禁用\n";
                                                btnSty="gray";
                                            }
                                            title+="货载："+l.InStock+"\n";
                                            title+="任务："+l.IsTask+"\n";
                                            if(l.ReservoirAreaName)
                                                title+="库区："+l.ReservoirAreaName+"\n";
                                            if(l.WarehouseName)
                                                title+="仓库："+l.WarehouseName+"\n";
                                            if(l.OUName)
                                                title+="业务实体："+l.OUName+"\n";

                                            content+="<td>";
                                           
                                            if ((l.InStock == "有货") && l.Status!="禁用")
                                            {
                                                content += "<a class='btn btn-app' onclick='showLocationCargo(" + l.Id + "," + JSON.stringify(l.UserCode).replace(/"/g, '&quot;') + ",\"有货\")' title='" + title + "' style='background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + l.SysCode + "</a>";
                                            }
                                            else if (( l.InStock == "空托盘") && l.Status != "禁用") {
                                                content += "<a class='btn btn-app' onclick='showLocationCargo(" + l.Id + "," + JSON.stringify(l.UserCode).replace(/"/g, '&quot;') + ",\"空托盘\")' title='" + title + "' style='background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + l.SysCode + "</a>";
                                            }
                                            else if (l.InStock == "无货" && l.Status != "禁用"){
                                                content += "<a class='btn btn-app'   title='" + title + "' style='background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + l.SysCode+"</a>";
                                            }
                                            else {
                                                content += "<a class='btn btn-app'  title='" + title + "' style='cursor: default;background-color:" + btnSty + ";font-size: 2px;'><i class='" + icon + "'></i>" + l.SysCode + "</a>";
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
    
    
    loadPlanData();
    
    $("#ljx-tab").click(function () {
        phyId = 1;
        loadPlanData()
    });
    $("#gh-tab").click(function () {
        phyId = 2;
        loadPlanData()
    });
});