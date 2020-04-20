var cookieObj = $.parseJSON($.cookie('wms-user'));
function orderQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
};
function orderRowDetailQueryParams() {
    return {};
};
function orderRowQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
};

var orderNumberClick=function(id)
{
    orderId=id;
    $('#order-row-table').bootstrapTable('refresh');
};

function whMaterialQueryParams(params) {
    return {
        ouId:9999999,
        traySteps:"5"   //【入库完成】物料
    };
}

function editWhMaterialQueryParams(params) {
    return {
        traySteps:"5"   //【入库完成】物料
    };
}

var getTKCount=function(input,row,field)
{
    $("#"+input).val($("#"+input).val().replace(/\D/gi,""));
    row[field] = $("#"+input).val();
};

var setTKCount=function(table,field,input,index)
{
    $("#"+table).bootstrapTable('updateCell', {
        index: index,
        field: field,
        value: $("#"+input).val()
    });
};

function operateFormatter(value, row, index) {
    return [
        '<a class="edit-order" href="javascript:void(0)" title="修改订单">',
        '<i class="fa fa-edit"></i>',
        '</a>  ',
        '<a class="trash-order" href="javascript:void(0)" title="关闭订单">',
        '<i class="fa fa-trash"></i>',
        '</a>  '
    ].join('')
}

operateEvents = {
    'click .edit-order': function (e, value, row, index) {
        rowOUId = row.OUId;
        rowWHId = row.WarehouseId;
        rowOrderId = row.Id;
        rowOrderNumber = row.OrderNumber;
        $('#edit-order-dlg').unbind("shown.bs.modal");
        $('#edit-order-dlg').on('shown.bs.modal', function ()
        {
            $('#order-row-material-table').bootstrapTable('destroy').bootstrapTable({
                ajax:function(request)
                {
                    var rd=request.data;
                    if(rowOrderId)
                        rd.orderId = rowOrderId;
                    asynTask({
                        type:'get',
                        url:controllers["order-row"]["get-order-rows"],
                        jsonData: rd,
                        successCallback:function(response)
                        {
                            $('#order-row-material-table').bootstrapTable('load', response.Data);
                            $('#order-row-material-table').bootstrapTable('hideLoading');
                            $('#order-row-material-table').bootstrapTable("resetView");
                        }
                    });
                },
                height:300,
                queryParams:'editWhMaterialQueryParams',
                clickEdit: true,
                columns:
                    [
                        {
                            field: 'state',
                            checkbox: true,
                            align: 'center',
                            valign: 'middle',
                            formatter : function(value, row, index) {
                                return{
                                    disabled: true,
                                    checked : true
                                };

                            }
                        },
                        {
                            title: '子库区',
                            field: 'ReservoirAreaName',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '物料编号',
                            field: 'MaterialDicId',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '行编码',
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
                            title: '单价',
                            field: 'Price',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '金额',
                            field: 'Amount',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '子库存编号',
                            field: 'ReservoirAreaId',
                            valign: 'middle',
                            align: 'center',
                            visible : false
                        },
                        {
                            title: '行编号',
                            field: 'Id',
                            valign: 'middle',
                            align: 'center',
                            visible : false
                        },
                        {
                            title: '订单编号',
                            field: 'OrderId',
                            valign: 'middle',
                            align: 'center',
                            visible : false
                        },
                        {
                            title: '行数量',
                            field: 'PreCount',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index)
                            {
                                return "<input value='"+value+"' id='order-row-material-table_"+row.MaterialDicId+"' style='width: 80px' onblur='setTKCount(\"order-row-material-table\",\"PreCount\",this.id,"+index+")' onkeyup='getTKCount(this.id,"+JSON.stringify(row).replace(/"/g, '&quot;') +",\"PreCount\")'/>";
                            }
                        }

                    ]

            });

            $('#edit-warehouse-material-table').bootstrapTable('destroy').bootstrapTable({
                ajax:function(request)
                {
                    var rd=request.data;
                    if(rowOUId)
                        rd.ouId = rowOUId;
                    if(rowWHId)
                        rd.warehouseId = rowWHId;
                    asynTask({
                        type:'get',
                        url:controllers.order["get-tkorder-materials"],
                        jsonData: rd,
                        successCallback:function(response)
                        {
                            $('#edit-warehouse-material-table').bootstrapTable('load', response.Data);
                            $('#edit-warehouse-material-table').bootstrapTable('hideLoading');
                            $('#edit-warehouse-material-table').bootstrapTable("resetView");
                            mergeCells(response.Data, "AreaName", 1, $('#edit-warehouse-material-table'));
                        }
                    });
                },
                height:200,
                queryParams:'editWhMaterialQueryParams',
                clickEdit: true,
                columns:
                    [
                        {
                            field: 'state',
                            checkbox: true,
                            align: 'center',
                            valign: 'middle',
                            formatter : function(value, row, index) {
                                if (row.RemainingCount<0)
                                    return {disabled: true};
                                return value;

                            }
                        },
                        {
                            title: '子库区',
                            field: 'AreaName',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '物料编号',
                            field: 'MaterialId',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '物料编码',
                            field: 'MaterialCode',
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
                            field: 'MaterialSpec',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '单价',
                            field: 'Price',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '金额',
                            field: 'Amount',
                            valign: 'middle',
                            align: 'center'
                        },

                        {
                            title: '子库存编号',
                            field: 'AreaId',
                            valign: 'middle',
                            align: 'center',
                            visible : false
                        },
                        {
                            title: '业务实体编号',
                            field: 'OUId',
                            valign: 'middle',
                            align: 'center',
                            visible : false
                        },
                        {
                            title: '库存组织编号',
                            field: 'WarehouseId',
                            valign: 'middle',
                            align: 'center',
                            visible : false
                        },
                        {
                            title: '库存数量',
                            field: 'MaterialCount',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '已占用数量',
                            field: 'OccCount',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '剩余数量',
                            field: 'RemainingCount',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index)
                            {
                                if(value>0)return value;
                                return  '<span class="badge bg-red">库存不足</span>';
                            }
                        },
                        {
                            title: '退库数量',
                            field: 'TKCount',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index)
                            {
                                return "<input value='"+value+"' id='edit-warehouse-material-table_"+row.MaterialId+"' style='width: 80px' onblur='setTKCount(\"edit-warehouse-material-table\",\"TKCount\",this.id,"+index+")' onkeyup='getTKCount(this.id,"+JSON.stringify(row).replace(/"/g, '&quot;') +",\"TKCount\")'/>";
                            }
                        }

                    ]

            });
        });
        $('#edit-order-dlg').modal('show');
    },
    'click .trash-order': function (e, value, row, index) {
        confirmShow(function () {
            asynTask({type:'post',url:controllers.order["close-order"],
                jsonData:
                    {
                        Id:row.Id,
                        Tag:cookieObj.loginName
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#order-table').bootstrapTable('refresh');
                        $('#order-row-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });
    }

};


var rowOrderId=null;
var rowOrderNumber=null;
var rowWHId=null;
var rowOUId=null;
var ouId=null;
var whId=null;
var treeNode=null;
var sCreateTime=null;
var eCreateTime=null;
var sFinishTime=null;
var eFinishTime=null;
var type=2;
var tstatus=[0,1,2,3];
var orderId=0;
var ouId2=0;
var whId2=0;
var areaId2=0;
var materialName=null;
$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').overlayScrollbars({ });
    $(".select2").select2({
        theme: 'bootstrap4'
    });

    var loadAreaSel = function (wareHouseId) {
        removeOptions("area-sel",[0]);
        asynTask({
            type:'get',
            url:controllers["reservoir-area"]["get-areas"],
            jsonData:{wareHouseId:wareHouseId},
            successCallback:function(response)
            {
                if(response.Code==200)
                {
                    var data=[];
                    $.each(response.Data, function(key, val) {
                        data.push({id:val.Id,text:val.AreaName});
                    });
                    $("#area-sel").select2({
                        data: data,
                        theme: 'bootstrap4'
                    });
                }
            }
        });
    };

    var loadWarehouseSel=function(ouId)
    {
        removeOptions("warehouse-sel",[0]);
        asynTask({
            type:'get',
            url:controllers.warehouse["get-warehouses"],
            jsonData:{ouId:ouId},
            successCallback:function(response)
            {
                if(response.Code==200)
                {
                    var data=[];
                    $.each(response.Data, function(key, val) {
                        data.push({id:val.Id,text:val.WhName});
                    });
                    $("#warehouse-sel").select2({
                        data: data,
                        theme: 'bootstrap4'
                    });
                    if(data.length>0)
                    {
                        loadAreaSel( $("#warehouse-sel").val());
                    }
                    else
                        {
                            removeOptions("area-sel",[0]);
                        }
                    $("#warehouse-sel").change(function () {
                      loadAreaSel( $("#warehouse-sel").val());
                    });
                }
            }
        });
    };

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
                $("#ou-sel").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
                if(data.length>0)
                {
                    loadWarehouseSel($("#ou-sel").val());
                }
                $("#ou-sel").change(function () {
                    loadWarehouseSel($("#ou-sel").val());
                });
            }
        }
    });

    loadingShow();

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
            loadingClose();
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
            $('#order-row-table').bootstrapTable('load', []);
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
            if(tstatus)rd.status=tstatus;
            if(type)rd.orderTypeId=type;
            if(sCreateTime)rd.sCreateTime=sCreateTime;
            if(eCreateTime)rd.eCreateTime=eCreateTime;
            if(sFinishTime)rd.sFinishTime=sFinishTime;
            if(eFinishTime)rd.eFinishTime=eFinishTime;
            asynTask({
                type:'get',
                url:controllers.order["get-orders"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#order-table').bootstrapTable('load', response.Data);
                    $('#order-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: (parent.document.getElementById("contentFrame").height - 10) / 2,
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
                    title: '订单编码',
                    field: 'OrderNumber',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        var e = '<a  href="#"  onclick="orderNumberClick('+row.Id+')">'+row.OrderNumber+'</a> ';
                        return e;
                    }
                },
                {
                    title: '类型',
                    field: 'OrderType',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '创建人',
                    field: 'EmployeeName',
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
                    title: '完成日期',
                    field: 'FinishTime',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '库存组织',
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
                },
                {
                    field: 'operate',
                    title: '操作',
                    align: 'center',
                    clickToSelect: false,
                    events:operateEvents,
                    formatter: operateFormatter
                }
            ]
    });

    $('#order-row-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            rd.orderId=orderId;
            asynTask({
                type:'get',
                url:controllers["order-row"]["get-order-rows"],
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
            var subTable1 = $detail.html('<table></table>').find('table');
            subTable1.bootstrapTable({
                ajax:function(request)
                {
                    var srd=request.data;
                    srd.orderRowId=row.Id;
                    asynTask({
                        type:'get',
                        url:controllers["order-row-batch"]["get-order-row-batchs"],
                        jsonData: srd,
                        successCallback:function(response)
                        {
                            subTable1.bootstrapTable('load', response.Data);
                            subTable1.bootstrapTable('hideLoading');
                        }
                    });
                },
                height:200,
                queryParams:'orderRowDetailQueryParams',
                pagination: false,
                detailView: true,
                onExpandRow: function (index, row, $detail) {
                    var subTable1 = $detail.html('<table></table>').find('table');
                    subTable1.bootstrapTable({
                        ajax:function(request)
                        {
                            var srd=request.data;
                            srd.orderRowBatchId=row.Id;
                            asynTask({
                                type:'get',
                                url:controllers["in-out-record"]["get-in-out-records"],
                                jsonData: srd,
                                successCallback:function(response)
                                {
                                    subTable1.bootstrapTable('load', response.Data);
                                    subTable1.bootstrapTable('hideLoading');
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
                                    title: '托盘',
                                    field: 'TrayCode',
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
                                    title: '出库数量',
                                    field: 'InOutCount',
                                    valign: 'middle',
                                    align: 'center'
                                },
                                {
                                    title: '执行状态',
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
                                },
                                {
                                    title: '数据同步',
                                    field: 'IsSyncStr',
                                    valign: 'middle',
                                    align: 'center',
                                    formatter : function(value, row, index) {
                                        if(value=="已同步")
                                        {
                                            return '<span class="badge bg-green">已同步</span>';
                                        }
                                        else if(value=="未同步")
                                        {
                                            return '<span class="badge bg-yellow">未同步</span>';
                                        }
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
                            title: '批次数量',
                            field: 'BatchCount',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '实际数量',
                            field: 'RealityCount',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '出库类型',
                            field: 'TypeStr',
                            valign: 'middle',
                            align: 'center'
                        },
                        {
                            title: '执行状态',
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
                        },
                        {
                            title: '数据上报',
                            field: 'IsReadStr',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index) {
                                if(value=="已读")
                                {
                                    return '<span class="badge bg-green">已上报</span>';
                                }
                                else if(value=="未读")
                                {
                                    return '<span class="badge bg-yellow">未上报</span>';
                                }
                            }
                        },
                        {
                            title: '数据同步',
                            field: 'IsSyncStr',
                            valign: 'middle',
                            align: 'center',
                            formatter : function(value, row, index) {
                                if(value=="已同步")
                                {
                                    return '<span class="badge bg-green">已同步</span>';
                                }
                                else if(value=="未同步")
                                {
                                    return '<span class="badge bg-yellow">未同步</span>';
                                }
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
                    title: '出库数量',
                    field: 'PreCount',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '实际数量',
                    field: 'RealityCount',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '累积数量',
                    field: 'Sorting',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '单价',
                    field: 'Price',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '金额',
                    field: 'Amount',
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
                },
                {
                    field: 'operate',
                    title: '操作',
                    align: 'center',
                    clickToSelect: false,
                    events:{
                        'click .trash-order-row': function (e, value, row, index) {
                            confirmShow(function () {

                                asynTask({type:'post',url:controllers.order["close-order-row"],
                                    jsonData:
                                        {
                                            Id:row.Id,
                                            Tag:cookieObj.loginName
                                        },
                                    successCallback:function (response) {
                                        if(response.Code==200)
                                        {
                                            toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                                            $('#order-row-table').bootstrapTable('refresh');
                                        }
                                        else {
                                            toastr.error(response.Data, '错误信息', {timeOut: 3000});
                                        }
                                    }
                                });
                            });
                        }
                    },
                    formatter: function (value, row, index) {
                        return [
                            '<a class="trash-order-row" href="javascript:void(0)" title="关闭订单行">',
                            '<i class="fa fa-trash"></i>',
                            '</a>  '
                        ].join('')
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

    $('#create-order-dlg').on('shown.bs.modal', function () {
        $('#warehouse-material-table').bootstrapTable('destroy').bootstrapTable({
            ajax:function(request)
            {
                var rd=request.data;
                ouId2 = $("#ou-sel").val();
                whId2 = $("#warehouse-sel").val();
                areaId2 = $("#area-sel").val();
                if(ouId2>0)rd.ouId=parseInt(ouId2);
                if(whId2>0)rd.warehouseId=parseInt(whId2);
                if(areaId2>0)rd.areaId=parseInt(areaId2);
                if(materialName)rd.materialName=materialName;
                asynTask({
                    type:'get',
                    url:controllers.order["get-tkorder-materials"],
                    jsonData: rd,
                    successCallback:function(response)
                    {
                        $('#warehouse-material-table').bootstrapTable('load', response.Data);
                        $('#warehouse-material-table').bootstrapTable('hideLoading');

                        mergeCells(response.Data, "AreaName", 1, $('#warehouse-material-table'));
                    }
                });
            },
            height:350,
            queryParams:'whMaterialQueryParams',
            clickEdit: true,
            columns:
                [
                    {
                        field: 'state',
                        checkbox: true,
                        align: 'center',
                        valign: 'middle',
                        formatter : function(value, row, index) {
                            if (row.RemainingCount<0)
                                return {disabled: true};
                            return value;

                        }
                    },
                    {
                        title: '子库区',
                        field: 'AreaName',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '物料编号',
                        field: 'MaterialId',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '物料编码',
                        field: 'MaterialCode',
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
                        field: 'MaterialSpec',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '单价',
                        field: 'Price',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '金额',
                        field: 'Amount',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '子库存编号',
                        field: 'AreaId',
                        valign: 'middle',
                        align: 'center',
                        visible : false
                    },
                    {
                        title: '业务实体编号',
                        field: 'OUId',
                        valign: 'middle',
                        align: 'center',
                        visible : false
                    },
                    {
                        title: '库存组织编号',
                        field: 'WarehouseId',
                        valign: 'middle',
                        align: 'center',
                        visible : false
                    },
                    {
                        title: '库存数量',
                        field: 'MaterialCount',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '已占用数量',
                        field: 'OccCount',
                        valign: 'middle',
                        align: 'center'
                    },
                    {
                        title: '剩余数量',
                        field: 'RemainingCount',
                        valign: 'middle',
                        align: 'center',
                        formatter : function(value, row, index)
                        {
                            if(value>0)return value;
                            return  '<span class="badge bg-red">库存不足</span>';
                        }
                    },
                    {
                        title: '退库数量',
                        field: 'TKCount',
                        valign: 'middle',
                        align: 'center',
                        formatter : function(value, row, index)
                        {
                            return "<input value='"+value+"' id='warehouse-material-table_"+row.MaterialId+"' style='width: 80px' onblur='setTKCount(\"warehouse-material-table\",\"TKCount\",this.id,"+index+")' onkeyup='getTKCount(this.id,"+JSON.stringify(row).replace(/"/g, '&quot;') +",\"TKCount\")'/>";

                        }
                    }

                ]

        });
    });

    $("#apply-btn").click(function () {
        $('#create-order-dlg').modal('show');
    });


    $("#material-query-btn").click(function () {
        var ouSels = $("#ou-sel").val();
        if(ouSels.length==0)
        {
            toastr.error("请选择业务实体！", '错误信息', {timeOut: 3000});
            return;
        }
        var warehouseSels = $("#warehouse-sel").val();
        if(warehouseSels.length==0)
        {
            toastr.error("请选择库存组织！", '错误信息', {timeOut: 3000});
            return;
        }
        $('#warehouse-material-table').bootstrapTable('refresh');
    });

    $("#add-order-btn").click(function () {
        var tableSelects=$("#warehouse-material-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要退库的物料！", '错误信息', {timeOut: 3000});
            return;
        }

        var order = {
            OrderTypeId : 2,
            CallingParty : "wms",
            ApplyUserCode : cookieObj.loginName,
            Tag : cookieObj.loginName,
            OUId : parseInt($("#ou-sel").val()),
            WarehouseId : parseInt($("#warehouse-sel").val()),
            EmployeeId : parseInt(cookieObj.userId)
            };
        var orderRows=[];
        var errormsg1="操作失败,编号 ";
        var errormsg2="操作失败,编号 ";
        var error1 = false;
        var error2 = false;
        $.each( tableSelects, function( key, value ) {
            if(parseInt(value.TKCount)>0) {
                if (parseInt(value.TKCount) > parseInt(value.RemainingCount)) {
                    errormsg2+=(" "+value.MaterialId);
                    error2 = true;
                } else {
                    var orderRow = {
                        ReservoirAreaId: parseInt(value.AreaId),
                        MaterialDicId: parseInt(value.MaterialId),
                        PreCount: parseInt(value.TKCount),
                        Price: parseFloat(value.Price)
                    };
                    orderRows.push(orderRow);
                }
            }
            else
                {
                    errormsg1+=(" "+value.MaterialId);
                    error1 = true;
                }
        });
        if(error1)
        {
            toastr.error(errormsg1+",退库数量输入错误！", '错误信息', {timeOut: 3000});
            return;
        }
        if(error2)
        {
            toastr.error(errormsg2+",退库数量不能大于剩余数量！", '错误信息', {timeOut: 3000});
            return;
        }
        order.OrderRows = orderRows;
        console.log(order);
        asynTask({
            type:'post',
            url:controllers.order["create-out-order"],
            jsonData: order,
            successCallback:function(response) {
                if (response.Code == 200) {
                    toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                    $('#create-order-dlg').modal('hide');
                    $('#order-table').bootstrapTable('refresh');
                } else {
                    toastr.error(response.Data, '系统信息', {timeOut: 3000});
                }
            }
        });
    });

    $("#edit-order-btn").click(function () {
        var orderRowSelects=$("#order-row-material-table").bootstrapTable('getSelections');
        var tableSelects=$("#edit-warehouse-material-table").bootstrapTable('getSelections');
        var order = {
            Id : rowOrderId,
            RowNumber : rowOrderNumber,
            OrderTypeId : 2,
            CallingParty : "wms",
            ApplyUserCode : cookieObj.loginName,
            Tag : cookieObj.loginName,
            OUId : parseInt(rowOUId),
            WarehouseId : parseInt(rowWHId),
            EmployeeId : parseInt(cookieObj.userId)
        };
        var orderRows=[];
        var errormsg1="操作失败,编号 ";
        var errormsg2="操作失败,编号 ";
        var error1 = false;
        var error2 = false;
        $.each( tableSelects, function( key, value ) {
            if(parseInt(value.TKCount)>0) {
                if (parseInt(value.TKCount) > parseInt(value.RemainingCount)) {
                    errormsg2+=(" "+value.MaterialId);
                    error2 = true;
                } else {
                    var orderRow = {
                        ReservoirAreaId: parseInt(value.AreaId),
                        MaterialDicId: parseInt(value.MaterialId),
                        PreCount: parseInt(value.TKCount),
                        Price: parseFloat(value.Price)
                    };
                    orderRows.push(orderRow);
                }
            }
            else
            {
                errormsg1+=(" "+value.MaterialId);
                error1 = true;
            }
        });
        if(error1)
        {
            toastr.error(errormsg1+",退库数量输入错误！", '错误信息', {timeOut: 3000});
            return;
        }
        if(error2)
        {
            toastr.error(errormsg2+",退库数量不能大于剩余数量！", '错误信息', {timeOut: 3000});
            return;
        }
        $.each( orderRowSelects, function( key, value ) {
            var orderRow = {
                Id:parseInt(value.Id),
                RowNumber : value.RowNumber,
                Price : parseFloat(value.Price),
                ReservoirAreaId: parseInt(value.ReservoirAreaId),
                MaterialDicId: parseInt(value.MaterialDicId),
                PreCount: parseInt(value.PreCount)

            };
            orderRows.push(orderRow);
        });
        order.OrderRows = orderRows;
        console.log(order);
        asynTask({
            type:'post',
            url:controllers.order["create-out-order"],
            jsonData: order,
            successCallback:function(response) {
                if (response.Code == 200) {
                    toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                    $('#edit-order-dlg').modal('hide');
                    $('#order-table').bootstrapTable('refresh');
                } else {
                    toastr.error(response.Data, '系统信息', {timeOut: 3000});
                }

            }
        });
    });
});