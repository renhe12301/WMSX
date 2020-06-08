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
                    if(typeRow.PhyName)
                    {
                        $("#phy-sel").val(typeRow.PhyWarehouseId).select2({
                            data: data,
                            theme: 'bootstrap4'
                        });
                    }
                    phyId=$("#phy-sel").val();
                    loadFRC(phyId);
                    loadLocs();
                }
            }
        });
        $('#assign-loc-dlg').modal('show');
        $("#dic-sel").empty();
      
        var selectLab=typeRow.PhyName?typeRow.AreaName+"("+typeRow.PhyName+")":typeRow.AreaName;
         $('#dic-sel').bootstrapDualListbox(
            {
                nonSelectedListLabel: '物理货位',
                selectedListLabel: selectLab,
                preserveSelectionOnMove: 'moved',
                moveOnSelect: false,
                infoText:'总记录数： {0}',
                infoTextEmpty:'空记录',
                filterPlaceHolder:'过滤',
                infoTextFiltered:'<span class="label label-warning">找到记录：</span> {0} 总记录： {1}',
                filterTextClear:""
            });
       // dlist.refresh();
        $('#dic-sel').bootstrapDualListbox("refresh", true);
        $('#dic-sel').bootstrapDualListbox("setSelectedListLabel",selectLab,true);
    
    }
};
var typeRow=null;
var ouId=null;
var whId=null;
var areaId=null;
var areaNameClick;


var loadLocs=function () {
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
}

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

$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 30;
    $('#sidebar').css("height", parentHeight);
    $('#sidebar').overlayScrollbars({});
    $('#sidebar2').overlayScrollbars({});
    

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
                    console.log(response.Data);
                    $('#area-table').bootstrapTable('load', response.Data);
                    $('#area-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 30,
        queryParams: 'areaQueryParams',
        pagination: true,
        pageNumber: 1,
        sidePagination: "server",
        pageSize: parseInt((parent.document.getElementById("contentFrame").height - 10) / 55),
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
        loadLocs();
    });

    $("#phy-sel").change(function () {
        phyId = $("#phy-sel").val();
        loadLocs();
        loadFRC(phyId);
    });
    
    $("#assign-loc-btn").click(function () {
        var dicSelects=$("#dic-sel").val(); 
        var dicIds=[]; 
        $.each(dicSelects, function(key, val) { 
            dicIds.push(parseInt(val));
                 });
        // if(dicIds.length==0)
        // {
        //     toastr.error("请选择左边需要划分的货位！", '错误信息', {timeOut: 3000});
        //     return;
        // }
        
        if(typeRow.PhyWarehouseId&&$("#phy-sel").val()!=typeRow.PhyWarehouseId)
        {

            confirmShow(function () {

                asynTask({
                    type: 'post',
                    url: controllers["reservoir-area"]["assign-location"],
                    jsonData: {ReservoirAreaId: typeRow.Id, LocationIds: dicIds},
                    successCallback: function (response) {
                        $('#assign-loc-dlg').modal('hide');
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#area-table').bootstrapTable('refresh');
                    }
                });
            },"子库区["+typeRow.AreaName+"]绑定仓库与当前仓库不一致,操作后会覆盖原有绑定信息");
        }
        else {
            asynTask({
                type: 'post',
                url: controllers["reservoir-area"]["assign-location"],
                jsonData: {ReservoirAreaId: typeRow.Id, LocationIds: dicIds},
                successCallback: function (response) {
                    $('#assign-loc-dlg').modal('hide');
                    toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                    $('#area-table').bootstrapTable('refresh');
                }
            });
        }

       
    });

   

    $("#sync-btn").click(function () {
        asynTask({
            type: 'post',
            url: controllers["sys-config"]["update-config"],
            jsonData: { KName: "子库存同步", KVal: "1" },
            successCallback: function (response) {
                if (response.Code == 200)
                    toastr.success("操作成功！", '系统信息', { timeOut: 3000 });
                else
                    toastr.error(response.Data, '系统信息', { timeOut: 3000 });
            }
        });
    });

});