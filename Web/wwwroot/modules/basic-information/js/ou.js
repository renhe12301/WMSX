function ouQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}
$(function () {
    $('#ou-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            var ouName=$("#ou-name").val();
            if(ouName!="")rd.ouName=ouName;
            var companyName=$("#company-name").val();
            if(companyName!="")rd.companyName=companyName;
            asynTask({
                type:'get',
                url:controllers.ou["get-ous"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#ou-table').bootstrapTable('load', response.Data);
                    $('#ou-table').bootstrapTable('hideLoading');
                }
            });
        },
        height:600,
        queryParams:'ouQueryParams',
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
                    title: '法人',
                    field: 'CompanyName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '业务实体编码',
                    field: 'OUCode',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '业务实体名称',
                    field: 'OUName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '产业板块编码',
                    field: 'PlateCode',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '产业板块名称',
                    field: 'PlateName',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });

    $("#query-btn").click(function () {
        $('#ou-table').bootstrapTable('refresh');
    });
});
