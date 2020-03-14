function logQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        logTypes:"2,3,4"
    }
}

var sCreateTime=null;
var eCreateTime=null;

$(function () {

    $(".select2").select2({
        theme: 'bootstrap4'
    });

    rangeTime("reservationtime",function (s,e) {
        sCreateTime=s;
        eCreateTime=e;
    });

    $('#log-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            var logDesc = $("#log-desc").text();
            if(logDesc!="")
                rd.logDesc = logDesc;
            var typeSels=$("#type-select").val();
            if(typeSels.length>0)
                rd.logTypes = typeSels;
            if(sCreateTime)rd.sCreateTime=sCreateTime;
            if(eCreateTime)rd.eCreateTime=eCreateTime;
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
        height:600,
        queryParams:'logQueryParams',
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
                    title: '日志描述',
                    field: 'LogDesc',
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