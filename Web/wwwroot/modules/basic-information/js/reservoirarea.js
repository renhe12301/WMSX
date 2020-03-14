function areaQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        areaName:$("#area-name").val()
    }
}
function operateFormatter(value, row, index) {
    return [
        '<a class="assign-location" href="javascript:void(0)" title="分配货位">',
        '<i class="fa fa-arrow-right"></i>',
        '</a>'
    ].join('')
}
operateEvents = {
    'click .assign-location': function (e, value, row, index)
    {
        typeRow=row;
        $('#assign-loc-dlg').modal('show');
        $("#dic-sel").empty();
        $('#dic-sel').bootstrapDualListbox(
            {
                nonSelectedListLabel: '物理货位',
                selectedListLabel: row.AreaName,
                preserveSelectionOnMove: 'moved',
                moveOnSelect: false,
                infoText:'总记录数： {0}',
                infoTextEmpty:'空记录',
                filterPlaceHolder:'过滤',
                infoTextFiltered:'<span class="label label-warning">找到记录：</span> {0} 总记录： {1}',
                filterTextClear:""
            });
        $('#dic-sel').bootstrapDualListbox('refresh',true);
    }
};
var typeRow=null;
var ouId=null;
var whId=null;
var areaId=null;
var areaNameClick;
$(function () {
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').overlayScrollbars({});

    asynTask({
        type:'get',
        url:controllers["phy-warehouse"]["get-phy-warehouses"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.PhyName});
                });
                $("#phy-sel").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
                phyId=$("#phy-sel").val();
                loadFRC(phyId);
            }
        }
    });

   loadingShow();
    areaNameClick = function (id) {
        areaId = id;
        $('#material-type-table').bootstrapTable('refresh');
    };
    renderTree({
        rootId: 0, renderTarget: 'jsTree', depthTag: 'warehouse', url: controllers.ou["get-ou-trees"],
        successCallback: function () {
            loadingClose();
        },
        selectNodeCall: function (node, data) {
            if (data.type == "ou") {
                ouId = data.id;
                whId = null;
            } else if (data.type == "warehouse") {
                ouId = null;
                whId = data.id;
            }
            $('#area-table').bootstrapTable('refresh');

        },
        showRoot: true
    });
    $('#area-table').bootstrapTable({
        ajax: function (request) {
            var rd = request.data;
            if (ouId) rd.ouId = ouId;
            if (whId) rd.wareHouseId = whId;
            asynTask({
                type: 'get',
                url: controllers["reservoir-area"]["get-areas"],
                jsonData: rd,
                successCallback: function (response) {
                    $('#area-table').bootstrapTable('load', response.Data);
                    $('#area-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: 600,
        queryParams: 'areaQueryParams',
        pagination: true,
        pageNumber: 1,
        sidePagination: "server",
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        smartDisplay: false,
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
                    title: '库区名称',
                    field: 'AreaName',
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
                    field: 'operate',
                    title: '操作',
                    align: 'center',
                    clickToSelect: false,
                    formatter: operateFormatter,
                    events:operateEvents
                }
            ]
    });

    $("#query-btn").click(function () {
        $('#area-table').bootstrapTable('refresh');
    });

    $("#phy-query-btn").click(function () {

        var rd={};
        phyId = $("#phy-sel").val();
        rd.phyId=phyId;
        var row=$("#row-sel").val();
        var rank=$("#rank-sel").val();
        var col=$("#col-sel").val();
        if(row!="0")
            rd.floors=row;
        if(rank!="0")
            rd.items=rank;
        if(col!="0")
            rd.cols=col;
        asynTask({ 
            type:'get', 
            url:controllers.location["get-locations"], 
            jsonData: rd, 
            successCallback:function(response) 
            {
                $("#dic-sel").empty();
                $.each(response.Data, function(key, val)
                {
                    var o = document.createElement("option");
                    o.value = val.Id;
                    o.text = val.SysCode;
                    if(val.ReservoirAreaId==typeRow.Id)
                        o.selected='selected';
                    $("#dic-sel").append(o);
                });
                $('#dic-sel').bootstrapDualListbox('refresh',true);
            } 
        });

    });

    $("#phy-sel").change(function () {
        phyId = $("#phy-sel").val();
        loadFRC(phyId);
    });

    var loadFRC=function (phyId) {
        asynTask({
            type:'get',
            url:controllers.location["get-max-floor-item-col"],
            jsonData:{phyId:phyId},
            successCallback:function(response)
            {
                if(response.Code==200)
                {
                    var frl=response.Data;
                    data=[];
                    for (var i=0;i<frl[0];i++) data.push({id:i+1,text:i+1});
                    $("#row-sel").select2({
                        data: data,
                        theme: 'bootstrap4'
                    });
                    data=[];
                    for (var i=0;i<frl[1];i++) data.push({id:i+1,text:i+1});
                    $("#rank-sel").select2({
                        data: data,
                        theme: 'bootstrap4'
                    });
                    data=[];
                    for (var i=0;i<frl[2];i++) data.push({id:i+1,text:i+1});
                    $("#col-sel").select2({
                        data: data,
                        theme: 'bootstrap4'
                    });
                }
            }
        });
    };

    $("#assign-loc-btn").click(function () {
        var dicSelects=$("#dic-sel").val(); 
        var dicIds=[]; 
        $.each(dicSelects, function(key, val) { 
            dicIds.push(parseInt(val));
         });
        if(dicIds.length==0)
        {
            toastr.error("请选择左边需要划分的货位！", '错误信息', {timeOut: 3000});
            return;
        }
        asynTask({
            type:'post',
            url:controllers["reservoir-area"]["assign-location"],
            jsonData: {ReservoirAreaId:typeRow.Id,LocationIds:dicIds},
            successCallback:function(response)
            {
                $('#assign-loc-dlg').modal('hide');
            }
        });
    });

});