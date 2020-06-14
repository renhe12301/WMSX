function taskQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}
function orderRowDetailQueryParams() {
    return {};
}
var ouId=null;
var whId=null;
var areaId=null;
var treeNode=null;
var sCreateTime=null;
var eCreateTime=null;
var sFinishTime=null;
var eFinishTime=null;
var types=null;
var tstatus=null;
var steps=null;
var trayCode=null;
var materialCode = null;

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

    renderTree({rootId: 0,renderTarget:'jsTree',depthTag: 'area',url:controllers.ou["get-ou-trees"],
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
                areaId=null;
            }
            else if(data.type=="org")
            {
                orgNode=data;
                ouId=null;
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
            $('#task-table').bootstrapTable('refresh');
        },
        showRoot:true
    });

    $('#task-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(ouId)rd.ouId=parseInt(ouId);
            if(whId)rd.wareHouseId=parseInt(whId);
            if(areaId)rd.areaId=parseInt(areaId);
            if(sCreateTime)rd.sCreateTime=sCreateTime;
            if(eCreateTime)rd.eCreateTime=eCreateTime;
            if(sFinishTime)rd.sFinishTime=sFinishTime;
            if(eFinishTime)rd.eFinishTime=eFinishTime;
            
            var typeSels=$("#type-select").val();
            if(typeSels.length>0)types=typeSels;
            var statusSels=$("#status-select").val();
            if(statusSels.length>0)tstatus=statusSels;
            var stepSels=$("#step-select").val();
            if(stepSels.length>0)steps=stepSels;
            if(tstatus)rd.status=tstatus;
            if(types)rd.types=types;
            if(steps)rd.steps=steps;


           if($("#suborder-id").val()!="")
           {
               rd.subOrderId=parseInt($("#suborder-id").val());
           }
            if($("#suborderrow-id").val()!="")
            {
               rd.subOrderRowId=parseInt($("#suborderrow-id").val());
            }
            
            trayCode=$("#tray-code").val();
            if(trayCode)rd.trayCode=trayCode;
            materialCode=$("#material-code").val();
            if(materialCode)rd.materialCode=materialCode;
            
            asynTask({
                type:'get',
                url:controllers["in-out-task"]["get-in-out-tasks"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#task-table').bootstrapTable('load', response.Data);
                    $('#task-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'taskQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: parseInt((parent.document.getElementById("contentFrame").height - 10) / 55),
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
        toolbar: '#toolbar',
        showColumns: true,
        showRefresh: true,
        detailView: true,
        onExpandRow: function (index, row, $detail) {
            if(row.Status!=0&&row.Status!=1)return;
            if(!row.TrayCode)return;
            var subTable = $detail.html('<table></table>').find('table');
            subTable.bootstrapTable({
                ajax:function(request)
                {
                    var srd=request.data;
                    srd.Id=row.WarehouseTrayId;
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
                                    return '<span class="badge bg-blue">'+value+'</span>';
                                }
                                else if(value=="入库中已执行"||value=="出库中已执行")
                                {
                                    return '<span class="badge bg-green">'+value+'</span>';
                                }
                                else if(value=="待入库"||value=="入库申请"||value=="入库中未执行"
                                    ||value=="待出库"||value=="出库中未执行")
                                {
                                    return '<span class="badge bg-yellow">'+value+'</span>';
                                }
                                else if(value=="入库完成"||value=="出库完成等待确认")
                                    return '<span class="badge bg-gray">'+value+'</span>';

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
                    title: '订单Id',
                    field: 'SubOrderId',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '行Id',
                    field: 'SubOrderRowId',
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
                    title: '物料编码',
                    field: 'MaterialCode',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '类型',
                    field: 'TypeStr',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '步骤',
                    field: 'StepStr',
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
                },
                {
                    title: 'WCS读取',
                    field: 'IsReadStr',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                         if(value=="已读")
                        {
                            return '<span class="badge bg-green">已读</span>';
                        }
                        else if(value=="未读")
                            return '<span class="badge bg-red">未读</span>';
                    }
                },
                {
                    title: '子库区',
                    field: 'ReservoirAreaName',
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
        if($("#suborder-id").val()!="")
        {
            var ival = parseInt($("#suborder-id").val());
            if(isNaN(ival))
            {
                toastr.error("订单Id只能输入数字!", '错误信息', {timeOut: 3000});
                return;
            }
        }
        if($("#suborderrow-id").val()!="")
        {
            var ival = parseInt($("#suborderrow-id").val());
            if(isNaN(ival))
            {
                toastr.error("订单行Id只能输入数字!", '错误信息', {timeOut: 3000});
                return;
            }
        }
        $('#task-table').bootstrapTable('refresh');
    });
    $("#more-query-btn").click(function () {
        $('#more-query-dlg').modal('show');
    });
    $("#send-btn").click(function () {
        confirmShow(function () {
            $('#send-dlg').modal('show');
        });
    });
    $("#save-btn").click(function () {
         var pickCode = $("#pick-code").val();
         var dropCode = $("#drop-code").val();
         if(pickCode.trim()=="")
         {
             $("#pick-code").focus();
             toastr.error("请输入取货位置编码!", '错误信息', {timeOut: 3000});
             return;
         }
        if(dropCode.trim()=="")
        {
            $("#drop-code").focus();
            toastr.error("请输入放货位置编码!", '错误信息', {timeOut: 3000});
            return;
        }

        asynTask({
            type:'post',
            url:controllers["in-out-task"]["send-wcs"],
            jsonData: 
                {
                    Type:parseInt($("#type-sel").val()),
                    SrcId:pickCode,
                    TargetId:dropCode,
                    PhyWarehouseId:parseInt($("#py-sel").val())
                },
            successCallback:function(response) {
                if (response.Code == 200) {
                    toastr.success("操作成功!", '系统信息', {timeOut: 3000});
                    $('#send-dlg').modal('hide');
                    $('#task-table').bootstrapTable('refresh');
                } else {
                    toastr.error(response.Data, '系统信息', {timeOut: 3000});
                }

            }
        });
        
    });
});