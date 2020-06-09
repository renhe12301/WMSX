function whTrayQueryParams(params) {
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

            $('#wh-material-table').bootstrapTable('refresh');
        },
        showRoot:true
    });

    $('#wh-tray-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(ouId)rd.ouId=parseInt(ouId);
            if(whId)rd.wareHouseId=parseInt(whId);
            if(areaId)rd.areaId=parseInt(areaId);
            trayCode=$("#tray-code").val();
            if(trayCode!="")
                rd.trayCode=trayCode;
            typeSelect=$("#type-select").val();
            if(typeSelect=="1")
                rd.rangeMaterialCount="0.000000001,999999999";
            else if(typeSelect=="2")
                rd.rangeMaterialCount="0,0";
            
            if($("#type-step").val()!="-1")
            {
                rd.trayTaskStatus=$("#type-step").val();
            }
            if($("#suborder-id").val()!="")
            {
                rd.subOrderId=parseInt($("#suborder-id").val());
            }
            if($("#suborderrow-id").val()!="")
            {
                rd.subOrderRowId=parseInt($("#suborderrow-id").val());
            }
            asynTask({
                type:'get',
                url:controllers["warehouse-tray"]["get-trays"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#wh-tray-table').bootstrapTable('load', response.Data);
                    $('#wh-tray-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'whTrayQueryParams',
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
                            title: '订单Id',
                            field: 'SubOrderId',
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
                    title: '物料编码',
                    field: 'Code',
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
                },
                {
                    title: '子库区',
                    field: 'ReservoirAreaName',
                    valign: 'middle',
                    align: 'center',
                    visible:false
                },
                {
                    title: '库组织',
                    field: 'WarehouseName',
                    valign: 'middle',
                    align: 'center',
                    visible:false
                },
                {
                    title: '业务实体',
                    field: 'OUName',
                    valign: 'middle',
                    align: 'center',
                    visible:false
                }
            ]
    });
    $("#query-btn").click(function () {
        $('#wh-tray-table').bootstrapTable('refresh');
    })
});