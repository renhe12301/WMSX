function whQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        whName:$("#warehouse-name").val()
    }
}
var ouId=null;
$(function () {
    asynTask({
        type:'get',
        url:controllers.ou["get-ous"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.OUName});
                });
                $(".select2").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
            }
        }
    });

    $('#wh-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(ouId)rd.ouId=ouId;
            asynTask({
                type:'get',
                url:controllers.warehouse["get-warehouses"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#wh-table').bootstrapTable('load', response.Data);
                    $('#wh-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
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
                    title: '编号',
                    field: 'Id',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '库存组织',
                    field: 'WhName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '业务实体',
                    field: 'OUName',
                    valign: 'middle',
                    align: 'center'
                },
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

    $("#sync-btn").click(function () {
        asynTask({
            type: 'post',
            url: controllers["sys-config"]["update-config"],
            jsonData: { KName: "库存组织同步", KVal: "1" },
            successCallback: function (response) {
                if (response.Code == 200)
                    toastr.success("操作成功！", '系统信息', { timeOut: 3000 });
                else
                    toastr.success(response.Data, '系统信息', { timeOut: 3000 });
            }
        });
    });
});