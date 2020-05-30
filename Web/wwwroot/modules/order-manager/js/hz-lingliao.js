function orderQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    };
}
function orderRowDetailQueryParams() {
   return {};
}

function orderRowQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    };
}

var orderNumberClick=function(id)
{
    orderId=id;
    $('#order-row-table').bootstrapTable('refresh');
};

var ouId=null;
var whId=null;
var treeNode=null;
var sCreateTime=null;
var eCreateTime=null;
var sFinishTime=null;
var eFinishTime=null;
var type=3;
var tstatus=[0,1];
var orderId=0;
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
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

    rangeTime("reservationtime",function (s,e) {
        sCreateTime=s;
        eCreateTime=e;
    });
    rangeTime("reservationtime2",function (s,e) {
        sFinishTime=s;
        eFinishTime=e;
    });

    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'warehouse',url:controllers.ou["get-ou-trees"],
        successCallback:function()
        {
            removeLoading('loadWindow');
        },
        selectNodeCall:function (node, data) {
            treeNode=data;
            if(data.type=="ou")
            {
                ouId=data.id;
                whId=null;
            }
            else if(data.type=="warehouse")
            {
                ouId=null;
                whId=data.id;
            }
            $('#order-table').bootstrapTable('refresh');
        },
        showRoot:true
    });

    $('#order-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(ouId)rd.ouId=parseInt(ouId);
            if(whId)rd.wareHouseId=parseInt(whId);
            userName=$("#user-name").val();
            orderCode=$("#order-code").val();
            supplierName=$("#supplier-name").val();
            sourceId=$("#source-id").val();
            corderId=$("#order-id").val();
            if(userName!="")rd.employeeName=userName;
            if(orderCode!="")rd.orderNumber=orderCode;
            if(supplierName!="")rd.supplierName=supplierName;
            if(sourceId!="")rd.sourceId=sourceId;
            if(corderId!="")rd.id=corderId;
            if(tstatus)rd.status=tstatus;
            if(type)rd.orderTypeId=type;
            if(sCreateTime)rd.sCreateTime=sCreateTime;
            if(eCreateTime)rd.eCreateTime=eCreateTime;
            if(sFinishTime)rd.sFinishTime=sFinishTime;
            if(eFinishTime)rd.eFinishTime=eFinishTime;
            asynTask({
                type:'get',
                url:controllers["sub-order"]["get-orders"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#order-table').bootstrapTable('load', response.Data);
                    $('#order-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: (parent.document.getElementById("contentFrame").height - 10)/2,
        queryParams:'orderQueryParams',
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
                    title: '编码',
                    field: 'OrderNumber',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        var e = '<a  href="#"  onclick="orderNumberClick('+row.Id+')">'+row.OrderNumber+'</a> ';
                        return e;
                    }
                },
                {
                    title: '创建日期',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '库组织',
                    field: 'WarehouseName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '业务实体',
                    field: 'OUName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '状态',
                    field: 'StatusStr',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        if(value=="执行中")
                        {
                            e='<a  href="javascript:void(0)" title="执行中...">'+
                                '<i class="fa fa-circle-notch fa-spin"></i>'+
                                '</a>  ';
                            return e;
                        }
                        else if(value=="待处理")
                        {
                            return '<span class="badge bg-yellow">待处理</span>';
                        }
                        else if(value=="完成")
                            return '<span class="badge bg-gray">完成</span>';
                        else if(value=="关闭")
                            return '<span class="badge bg-gray-dark">关闭</span>';
                    }
                }
            ]
    });

    $('#order-row-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            rd.orderId = orderId;
            asynTask({
                type:'get',
                url:controllers["sub-order"]["get-order-rows"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#order-row-table').bootstrapTable('load', response.Data);
                    $('#order-row-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: (parent.document.getElementById("contentFrame").height - 20) / 2,
        queryParams:'orderRowQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
        showColumns: true,
        showRefresh: true,
        detailView: true,
        onExpandRow: function (index, row, $detail) {
            var subTable = $detail.html('<table></table>').find('table');
            subTable.bootstrapTable({
                ajax:function(request)
                {
                    var srd=request.data;
                    srd.subOrderRowId=row.Id;
                    asynTask({
                        type:'get',
                        url:controllers["in-out-task"]["get-in-out-tasks"],
                        jsonData: srd,
                        successCallback:function(response)
                        {
                            subTable.bootstrapTable('load', response.Data);
                            subTable.bootstrapTable('hideLoading');
                        }
                    });
                },
                height:200,
                queryParams:'orderRowDetailQueryParams',
                pagination: false,
                columns:
                    [
                        {
                            title: '编号',
                            field: 'Id',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '取货',
                            field: 'SrcId',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '放货',
                            field: 'TargetId',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '托盘',
                            field: 'TrayCode',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '物料数量',
                            field: 'MaterialCount',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '步骤',
                            field: 'Step',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '状态',
                            field: 'StatusStr',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index) {
                                if(value=="执行中")
                                {
                                    e='<a  href="javascript:void(0)" title="执行中...">'+
                                        '<i class="fa fa-circle-notch fa-spin"></i>'+
                                        '</a>  ';
                                    return e;
                                }
                                else if(value=="待处理")
                                {
                                    return '<span class="badge bg-yellow">待处理</span>';
                                }
                                else if(value=="完成")
                                    return '<span class="badge bg-gray">完成</span>';
                                else if(value=="关闭")
                                    return '<span class="badge bg-gray-dark">关闭</span>';
                            }
                        }
                    ]
            });
        },
        columns:
            [
                {
                    title: '编号',
                    field: 'Id',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '编码',
                    field: 'RowNumber',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '物料名称',
                    field: 'MaterialDicName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '行数量',
                    field: 'PreCount',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '出库数量',
                    field: 'RealityCount',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '状态',
                    field: 'StatusStr',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        if(value=="执行中")
                        {
                            e='<a  href="javascript:void(0)" title="执行中...">'+
                                '<i class="fa fa-circle-notch fa-spin"></i>'+
                                '</a>  ';
                            return e;
                        }
                        else if(value=="待处理")
                        {
                            return '<span class="badge bg-yellow">待处理</span>';
                        }
                        else if(value=="完成")
                            return '<span class="badge bg-gray">完成</span>';
                        else if(value=="关闭")
                            return '<span class="badge bg-gray-dark">关闭</span>';
                    }
                }
            ]
    });


    $("#query-btn").click(function () {
        if($("#reservationtime").val()=="")
        {
            sCreateTime=null;
            eCreateTime=null;
        }
        if($("#reservationtime2").val()=="")
        {
            sFinishTime=null;
            eFinishTime=null;
        }
        $('#order-table').bootstrapTable('refresh');
    });
    $("#more-query-btn").click(function () {
        $('#more-query-dlg').modal('show');
    });
    $("#save-btn").click(function () {
        var statusSels=$("#status-select").val();
        if(statusSels.length>0)tstatus=statusSels;
        else tstatus=null;
    });
});