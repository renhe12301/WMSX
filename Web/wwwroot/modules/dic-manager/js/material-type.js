function typeQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,

    }
}
$(function () {

    $('#type-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            typeName=$("#type-name").val();
            if(typeName!="")
            {
                rd.typeName=typeName;
            }
            asynTask({
                type:'get',
                url:controllers["material-type"]["get-material-types"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#type-table').bootstrapTable('load', response.Data);
                    $('#type-table').bootstrapTable('hideLoading');
                }
            });
        },
        height:600,
        queryParams:'typeQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
        toolbar: '#toolbar',
        showColumns: true,
        showRefresh: true,
        clickToSelect: true,
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
                    title: '物料类型',
                    field: 'TypeName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '创建时间',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                }

            ]
    });
$("#query-btn").click(function () {
    $('#type-table').bootstrapTable('refresh');
});
});