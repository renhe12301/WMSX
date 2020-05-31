function logQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        logTypes:"0",
        founder : $("#op-name").val()
    }
}

var sCreateTime=null;
var eCreateTime=null;

$(function () {

    rangeTime("reservationtime",function (s,e) {
        sCreateTime=s;
        eCreateTime=e;
    });

    $('#log-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            asynTask({
                type:'get',
                url:controllers["log-record"]["get-log-records"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#log-table').bootstrapTable('load', response.Data);
                    $('#log-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'logQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: parseInt((parent.document.getElementById("contentFrame").height - 10) / 55),
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
                    title: '日志描述',
                    field: 'LogDesc',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '登录名',
                    field: 'Founder',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '记录时间',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });

    $("#query-btn").click(function () {
        if($("#reservationtime").val()=="")
        {
            sCreateTime=null;
            eCreateTime=null;
        }
        $('#log-table').bootstrapTable('refresh');
    });
});