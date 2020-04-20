function whTrayQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
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
                rd.rangeMaterialCount="0,0";
            else if(typeSelect=="2")
                rd.rangeMaterialCount="1,9999999";
            console.log(rd);
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
        $('#wh-material-table').bootstrapTable('refresh');
    })
});