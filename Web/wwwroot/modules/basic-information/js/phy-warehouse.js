function whQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        phyName:$("#warehouse-name").val()
    }
}

$(function () {
    $('#wh-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;

            asynTask({
                type:'get',
                url:controllers["phy-warehouse"]["get-phy-warehouses"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#wh-table').bootstrapTable('load', response.Data);
                    $('#wh-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height-10,
        queryParams:'whQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
        toolbar: '#toolbar',
        showColumns: true,
        showRefresh: true,
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
                    title: '仓库名称',
                    field: 'PhyName',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });
    $("#query-btn").click(function () {
        var select=$('.select2').val();
        if(select!='0')
            ouId=parseInt(select);
        else
            ouId=null;
        $('#wh-table').bootstrapTable('refresh');
    });


    $("#add-btn").click(function () {
        $('#add-location-dlg').modal('show');

    });
    $("#build-btn").click(function () {
        $('#build-location-dlg').modal('show');

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
            var tableSelects=$("#wh-table").bootstrapTable('getSelections');
            if(tableSelects.length==0)
            {
                toastr.error("请选择仓库！", '错误信息', {timeOut: 3000});
                return;
            }
            if(tableSelects.length>1)
            {
                toastr.error("只能选择一个仓库！", '错误信息', {timeOut: 3000});
                return;
            }
            asynTask({type:'post',url:controllers["location"]["add-location"],
                jsonData:
                    {
                        SysCode:$('#add-sys-code').val(),
                        UserCode: $("#add-user-code").val(),
                        PhyWarehouseId:tableSelects[0].Id
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#add-location-dlg').modal('hide');

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

            var tableSelects=$("#wh-table").bootstrapTable('getSelections');
            if(tableSelects.length==0)
            {
                toastr.error("请选择仓库！", '错误信息', {timeOut: 3000});
                return;
            }
            if(tableSelects.length>1)
            {
                toastr.error("只能选择一个仓库！", '错误信息', {timeOut: 3000});
                return;
            }
            asynTask({type:'post',url:controllers["location"]["build-location"],
                jsonData:
                    {
                        PhyWarehouseId:tableSelects[0].Id,
                        Row:parseInt($("#build-row").val()),
                        Rank:parseInt($("#build-rank").val()),
                        Col:parseInt($("#build-col").val())
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#build-location-dlg').modal('hide');

                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });
});