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
var type=[3];
var tstatus=[0,1,2,3];
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
          
            corderId=$("#order-id").val();
            if(userName!="")rd.employeeName=userName;
            if(orderCode!="")rd.orderNumber=orderCode;
            if(supplierName!="")rd.supplierName=supplierName;
           
            if(corderId!="")rd.id=corderId;

            var statusSels=$("#status-select").val();
            if(statusSels.length>0)tstatus=statusSels;
            else tstatus=null;
            
            if(tstatus)rd.status=tstatus;
            if(type)rd.orderTypeIds=type;
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
                    title: '集约化单据类型',
                    field: 'SourceOrderType',
                    valign: 'middle',
                    align: 'center'
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
                            return '<span class="badge bg-green">执行中</span>';
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
            rd.subOrderId = orderId;
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
                        url:controllers["warehouse-tray"]["get-trays"],
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
                detailView: true,
                onExpandRow: function (index, row, $detail) {
                    var subTable2 = $detail.html('<table></table>').find('table');
                    subTable2.bootstrapTable({
                        ajax:function(request)
                        {
                            var srd=request.data;
                            srd.warehouseTrayId=row.Id;
                            asynTask({
                                type:'get',
                                url:controllers["warehouse-material"]["get-materials"],
                                jsonData: srd,
                                successCallback:function(response)
                                {
                                    subTable2.bootstrapTable('load', response.Data);
                                    subTable2.bootstrapTable('hideLoading');
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
                                    title: '物料编码',
                                    field: 'Code',
                                    valign: 'middle',
                                    align: 'center'
                                },
                                {
                                    title: '物料名称',
                                    field: 'MaterialName',
                                    valign: 'middle',
                                    align: 'center'
                                },
                                {
                                    title: '物料属性',
                                    field: 'Spec',
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
                                    title: '所在货位',
                                    field: 'LocationCode',
                                    valign: 'middle',
                                    align: 'center'
                                },
                                {
                                    title: '所在托盘',
                                    field: 'TrayCode',
                                    valign: 'middle',
                                    align: 'center'
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
                            title: '托盘编码',
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
                            title: '所在货位',
                            field: 'LocationCode',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '载体',
                            field: 'Carrier',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '状态',
                            field: 'TrayStepStr',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index) {
                                if(value=="初始化")
                                {
                                    return '<span class="badge bg-blue">value</span>';
                                }
                                else if(value=="入库中已执行"||value=="出库中已执行")
                                {
                                    return '<span class="badge bg-green">value</span>';
                                }
                                else if(value=="待入库"||value=="入库申请"||value=="入库中未执行"
                                    ||value=="待出库"||value=="出库中未执行")
                                {
                                    return '<span class="badge bg-yellow">value</span>';
                                }
                                else if(value=="入库完成"||value=="出库完成等待确认")
                                    return '<span class="badge bg-gray">value</span>';

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
                    title: '行物料',
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
                            return '<span class="badge bg-green">执行中</span>';
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
   
});