function taskQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
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
            if(tstatus)rd.status=tstatus;
            if(types)rd.types=types;
            if(steps)rd.steps=steps;
            if(trayCode)rd.trayCode=trayCode;
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
                    title: '类型',
                    field: 'Type',
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
        $('#task-table').bootstrapTable('refresh');
    });
    $("#more-query-btn").click(function () {
        $('#more-query-dlg').modal('show');
    });
    $("#save-btn").click(function () {
       var typeSels=$("#type-select").val();
       if(typeSels.length>0)types=typeSels;
        var statusSels=$("#status-select").val();
        if(statusSels.length>0)tstatus=statusSels;
        var stepSels=$("#step-select").val();
        if(stepSels.length>0)steps=stepSels;
        trayCode=$("#tray-code").val();

    });
});