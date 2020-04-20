var ouId=null;
var whId=null;
var areaId=null;
var sysCode=null;
var userCode=null;
var types=null;
var lstatus=null;
var inStocks=null;
var isTasks=null;
var treeNode=null;
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
                    $("#org-name").text(data.OrgName);
                }

            }
        }
    });
};
function locationQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').overlayScrollbars({ });
    $('#sidebar3').overlayScrollbars({ });
    $('.card-body box-profile').overlayScrollbars({ });
    $(".select2").select2({
        theme: 'bootstrap4'
    });
    $('body').loading({
        loadingWidth: 240,
        title: '请稍等!',
        name: 'loadWindow',
        discription: '正在加载数据...',
        direction: 'column',
        type: 'origin',
        originDivWidth: 40,
        originDivHeight: 40,
        originWidth: 6,
        originHeight: 6,
        smallLoading: false,
        loadingMaskBg: 'rgba(0,0,0,0.2)'
    });

    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'area',url:controllers.ou["get-ou-trees"],
        successCallback:function()
        {
            removeLoading('loadWindow');
        },
        selectNodeCall:function (node, data) {
            treeNode=data;
            if(data.type=="ou")
            {
                if(data.id==0)
                    ouId=null;
                else
                    ouId=data.id;
                whId=null;
                areaId=null;
            }
            else if(data.type=="warehouse")
            {
                ouId=null;
                whId=data.id;
                areaId=null;
            }
            else if(data.type=="area")
            {
                ouId=null;
                whId=null;
                areaId=data.id;
            }
            sysCode=null;
            userCode=null;
            types=null;
            lstatus=null;
            inStocks=null;
            isTasks=null;
            $('#location-table').bootstrapTable("refresh");
        },
        showRoot:true
    });

    $('#location-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(ouId!=null)rd.ouId=parseInt(ouId);
            if(whId!=null)rd.wareHouseId=parseInt(whId);
            if(areaId!=null)rd.areaId=parseInt(areaId);
            if(sysCode)rd.sysCode=sysCode;
            if(userCode)rd.userCode=userCode;
            if(types)rd.types=types;
            if(lstatus) rd.status=lstatus;
            if(inStocks)rd.inStocks=inStocks;
            if(isTasks)rd.isTasks=isTasks;
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
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'locationQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: 10,
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
                    title: '系统编码',
                    field: 'SysCode',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index)
                    {
                        if(value.length>5)return value;
                        return  '<a class="edit-sys-code" href="#">'+value+'</a> ';
                    },
                    events:
                        {
                            'click .edit-sys-code':function(e, value, row, index) {
                                $("#edit-location-id").val(row.Id);
                                $('#edit-syscode-dlg').modal('show');
                                $("#edit-sys-code").val(row.SysCode);
                            }
                        }
                },
                {
                    title: '用户编码',
                    field: 'UserCode',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index)
                    {
                        value=value==null?'-':value;
                        return '<a class="edit-sys-code" href="#">'+value+'</a> ';
                    },
                    events:
                        {
                            'click .edit-sys-code':function(e, value, row, index) {
                                $("#edit-location-id").val(row.Id);
                                $('#edit-usercode-dlg').modal('show');
                                $("#edit-user-code").val(row.UserCode);
                            }
                        }
                },
                {
                    title: '货载',
                    field: 'InStock',
                    valign: 'middle',
                    align: 'center',

                    formatter : function(value, row, index) {
                        if(row.InStock=="空托盘")
                        {
                            e='<a  href="javascript:void(0)" onclick=\'showLocationCargo("'+row.Id+'")\' title="'+row.InStock+'">'+
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
                            e='<a  href="javascript:void(0)" onclick=\'showLocationCargo("'+row.Id+'")\' title="'+row.InStock+'">'+
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
                            e='<a  href="javascript:void(0)" title="正在任务...">'+
                                '<i class="fa fa-cog fa-spin"></i>'+
                                '</a>  ';
                            return e;
                        }
                        else return '无';
                    }
                },
                {
                    title: '库区',
                    field: 'ReservoirAreaName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '仓库',
                    field: 'WarehouseName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '业务实体',
                    field: 'OUName',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });
    var tableInit=true;
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        tabType=$("#"+e.target.id).text();
        if(tabType=="列表"&&tableInit)
        {
            $('#location-table').bootstrapTable("refresh");
            tableInit=false;
        }
        else if(tabType=='列表'&&!tableInit)
            $('#location-table').bootstrapTable("resetView");
    });

    $("#more-query-btn").click(function () {
        $('#more-query-dlg').modal('show');
    });

    $("#query-btn").click(function () {
        sysCode=$("#sys-code").val();
        userCode=$("#user-code").val();
        var inStockSelects=$('#instock-select').val();
        var isTaskSelects=$("#istask-select").val();
        if(inStockSelects.length>0)inStocks=inStockSelects.join(',');
        if(isTaskSelects.length>0)isTasks=isTaskSelects.join(',');
        loadTabData();
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

});