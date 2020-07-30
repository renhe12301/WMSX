var pyId = 1;
var ouId=null;
var whId=null;
var areaId=null;
var sysCode=null;
var userCode=null;
var types=null;
var lstatus=null;
var inStocks=null;
var isTasks=null;
var treeNode = null;
var floor = null;
var rank = null;
var col = null;
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
function locationQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}

$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 100;
    $('#sidebar2').overlayScrollbars({ });
    $('#sidebar3').overlayScrollbars({ });
    $('.card-body box-profile').overlayScrollbars({ });
    $(".select2").select2({
        theme: 'bootstrap4'
    });
    
    $('#location-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(sysCode)rd.sysCode=sysCode;
            if(types)rd.types=types;
            if(lstatus) rd.status=lstatus;
            if(inStocks)rd.inStocks=inStocks;
            if(isTasks)rd.isTasks=isTasks;
            if (pyId) rd.phyId = pyId;
            if (floor) rd.floors = floor;
            if (rank) rd.items = rank;
            if (col) rd.cols = col;
            asynTask({
                type:'get',
                url:controllers.location["get-locations"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#location-table').bootstrapTable('load', response.Data);
                    $('#location-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parentHeight,
        queryParams:'locationQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: parseInt(parentHeight / 55),
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
        showColumns: true,
        showRefresh: true,
        toolbar: '#toolbar',
        columns:
            [
                {
                    field: 'state',
                    checkbox: true,
                    align: 'center',
                    valign: 'middle'
                },
                {
                    title: '编号',
                    field: 'Id',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '货位编码',
                    field: 'SysCode',
                    valign: 'middle',
                    align: 'center'
                    
                },
                {
                    title: '货载',
                    field: 'InStock',
                    valign: 'middle',
                    align: 'center',

                    formatter : function(value, row, index) {
                        if(row.InStock=="空托盘")
                        {
                            e = '<a  href="javascript:void(0)" onclick=\'showLocationCargo("' + row.Id + '",' + JSON.stringify(row.UserCode).replace(/"/g, '&quot;')+',\"空托盘\")\' title="'+row.InStock+'">'+
                                '<i class="fa fa-dice-four"></i>'+
                                '</a>  ';
                            return e;
                        }
                        else if(row.InStock=="无货")
                        {
                            return '无';
                        }
                        else if(row.InStock=="有货")
                        {
                            e = '<a  href="javascript:void(0)" onclick=\'showLocationCargo("' + row.Id + '",' + JSON.stringify(row.UserCode).replace(/"/g, '&quot;') +',\"有货\")\' title="'+row.InStock+'">'+
                                '<i class="fa fa-gift"></i>'+
                                '</a>  ';
                            return e;
                        }
                    }
                },
                {
                    title: '状态',
                    field: 'Status',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        if(value=="锁定")
                            return '<i title="锁定" class="fa fa-lock"></i>';
                        if(value=="禁用")
                            return '<i title="禁用" class="fa fa-skull-crossbones"></i>';
                        else return '<i title="正常" class="fa fa-grin-beam"></i>';
                    }
                },
                {
                    title: '任务',
                    field: 'IsTask',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        if(value=="有任务")
                        {
                            e='<a  href="javascript:void(0)" style="cursor: pointer;" title="正在任务...">'+
                                '<i class="fa fa-cog fa-spin"></i>'+
                                '</a>  ';
                            return e;
                        }
                        else return '无';
                    }
                }
            ]
    });
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        tabType=$("#"+e.target.id).text().trim();
        if(tabType=="李家峡")
        {
            pyId = 1;
        }
        else if(tabType=="共和")
        {
            pyId = 2;
        }
        $("#sys-code").val("");
        sysCode = null;
        $('#location-table').bootstrapTable("refresh");
    });

    $("#more-query-btn").click(function () {
        $('#more-query-dlg').modal('show');
        asynTask({
            type: 'get', url: controllers["location"]["get-max-floor-item-col"],
            jsonData:
            {
                phyId: pyId
            },
            successCallback: function (response) {
                if (response.Code == 200) {
                    var data = response.Data;
                    if (data.length > 0) {
                        $("#floor-select").empty();
                        $("#floor-select").append($("<option value=" + -1 + ">全部</option>"));
                        $("#rank-select").empty();
                        $("#rank-select").append($("<option value=" + -1 + ">全部</option>"));
                        $("#col-select").empty();
                        $("#col-select").append($("<option value=" + -1 + ">全部</option>"));
                        var f = data[0];
                        var r = data[1];
                        var c = data[2];
                        for (var i = 1; i <= f; i++) {
                            var opt = $("<option value=" + i + ">" + i + "</option>")
                            $("#floor-select").append(opt);
                        }
                        for (var j = 1; j <= r; j++) {
                            var opt = $("<option value=" + j + ">" + j + "</option>")
                            $("#rank-select").append(opt);
                        }
                        for (var k = 1; k <= c; k++) {
                            var opt = $("<option value=" + k + ">" + k + "</option>")
                            $("#col-select").append(opt);
                        }
                    }
                }
                else {
                    toastr.error(response.Data, '错误信息', { timeOut: 3000 });
                }
            }
        });

       
       
    });

    $("#query-btn").click(function () {
        sysCode=$("#sys-code").val();
        var inStockSelects=$('#instock-select').val();
        var isTaskSelects=$("#istask-select").val();
        if(inStockSelects.length>0)inStocks=inStockSelects.join(',');
        if(isTaskSelects.length>0)isTasks=isTaskSelects.join(',');
        $('#location-table').bootstrapTable("refresh");
    });

    $("#disable-btn").click(function () {

        var tableSelects=$("#location-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要禁用的货位！", '错误信息', {timeOut: 3000});
            return;
        }
        confirmShow(function () {
            var selectIds=[];
            $.each( tableSelects, function( key, value ) {
                selectIds.push(parseInt(value.Id));
            });
            asynTask({type:'post',url:controllers["location"]["disable"],
                jsonData:
                    {
                        LocationIds:selectIds
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#location-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });
    });

    $("#enable-btn").click(function () {
        var tableSelects=$("#location-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要启用的货位！", '错误信息', {timeOut: 3000});
            return;
        }
        confirmShow(function () {
            var selectIds=[];
            $.each( tableSelects, function( key, value ) {
                selectIds.push(parseInt(value.Id));
            });
            asynTask({type:'post',url:controllers["location"]["enable"],
                jsonData:
                    {
                        LocationIds:selectIds
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#location-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });

    });

    $("#clear-btn").click(function () {
        var tableSelects=$("#location-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要清空的货位！", '错误信息', {timeOut: 3000});
            return;
        }
        confirmShow(function () {
            var selectIds=[];
            $.each( tableSelects, function( key, value ) {
                selectIds.push(parseInt(value.Id));
            });
            asynTask({type:'post',url:controllers["location"]["clear"],
                jsonData:
                    {
                        LocationIds:selectIds
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#location-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });

    });

    $("#add-location-form").validate({
        rules: {
            "add-sys-code": {
                required: true,
                maxlength:5,
                digits:true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {
            if(orgNode==null)
            {
                toastr.error("请选择左边树中的公司节点", '错误信息', {timeOut: 3000});
                return;
            }
            asynTask({type:'post',url:controllers["location"]["add-location"],
                jsonData:
                    {
                        SysCode:$('#add-sys-code').val(),
                        UserCode: $("#add-user-code").val(),
                        OrganizationId:orgNode.id
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#add-location-dlg').modal('hide');
                        $('#location-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#edit-syscode-form").validate({
        rules: {
            "edit-sys-code": {
                required: true,
                maxlength:5,
                digits:true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {
            asynTask({type:'post',url:controllers["location"]["update-location"],
                jsonData:
                    {
                        SysCode:$('#edit-sys-code').val(),
                        Id:parseInt($("#edit-location-id").val())
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#edit-syscode-dlg').modal('hide');
                        $('#location-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#edit-usercode-form").validate({
        rules: {
            "edit-user-code": {
                required: true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {
            asynTask({type:'post',url:controllers["location"]["update-location"],
                jsonData:
                    {
                        UserCode:$('#edit-user-code').val(),
                        Id:parseInt($("#edit-location-id").val())
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#edit-usercode-dlg').modal('hide');
                        $('#location-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#build-location-form").validate({
        rules: {
            "build-row": {
                required: true,
                maxlength:3,
                digits:true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            },
            "build-rank": {
                required: true,
                maxlength:3,
                digits:true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            },
            "build-col": {
                required: true,
                maxlength:3,
                digits:true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {
            if(orgNode==null)
            {
                toastr.error("请选择左边树中的公司节点", '错误信息', {timeOut: 3000});
                return;
            }
            $('body').loading({
                loadingWidth: 240,
                title: '请稍等!',
                name: 'loadWindow',
                discription: '正在生成货位数据...',
                direction: 'column',
                type: 'origin',
                originDivWidth: 40,
                originDivHeight: 40,
                originWidth: 6,
                originHeight: 6,
                smallLoading: false,
                loadingMaskBg: 'rgba(0,0,0,0.2)'
            });
            asynTask({type:'post',url:controllers["location"]["build-location"],
                jsonData:
                    {
                        OrganizationId:orgNode.id,
                        Row:parseInt($("#build-row").val()),
                        Rank:parseInt($("#build-rank").val()),
                        Col:parseInt($("#build-col").val())
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#build-location-dlg').modal('hide');
                        $('#location-table').bootstrapTable('refresh');
                        removeLoading('loadWindow');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#saveBtn").click(function () {
        if ($("#instock-select").val() == -1) {
            inStocks = null;
        }
        if ($("#istask-select").val() == -1) {
            isTasks = null;
        }
        if ($("#floor-select").val() == -1) {
            floor = null;
            floors = null;
        }
        if ($("#rank-select").val() == -1) {
            rank = null;
            items = null;
        }
        if ($("#col-select").val() == -1) {
            col = null;
            cols = null;
        }
        if ($("#instock-select").val() != "" && $("#instock-select").val() != -1)
        {
            inStocks = $("#instock-select").val();
        }
        if ($("#istask-select").val() != "" && $("#istask-select").val() !=-1) {
            isTasks = $("#istask-select").val();
        }
        if ($("#floor-select").val() != "" && $("#floor-select").val() !=-1) {
            floor = $("#floor-select").val();
        }
        if ($("#rank-select").val() != "" && $("#rank-select").val() !=-1) {
            rank = $("#rank-select").val();
        }
        if ($("#col-select").val() != "" && $("#col-select").val() !=-1) {
            col = $("#col-select").val();
        }
    });
});