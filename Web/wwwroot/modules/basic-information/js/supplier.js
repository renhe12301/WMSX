function supplierQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}

function siteQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        supplierId:0
    }
}

var supplierId=null;

var supplierNameClick=function(id)
{
    supplierId=id;
    $('#site-table').bootstrapTable('refresh');
};

$(function () {
    $('#supplier-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            var supplierName=$("#supplier-name").val();
            if(supplierName!="")
                rd.supplierName=supplierName;
            asynTask({
                type:'get',
                url:controllers.supplier["get-suppliers"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#supplier-table').bootstrapTable('load', response.Data);
                    $('#supplier-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: (parent.document.getElementById("contentFrame").height - 10) / 2,
        queryParams:'supplierQueryParams',
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
                    title: '供应商编码',
                    field: 'SupplierCode',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        var e = '<a  href="#"  onclick="supplierNameClick('+row.Id+')">'+row.SupplierCode+'</a> ';
                        return e;
                    }
                },
                {
                    title: '供应商名称',
                    field: 'SupplierName',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        var e = '<a  href="#"  onclick="supplierNameClick('+row.Id+')">'+row.SupplierName+'</a> ';
                        return e;
                    }
                },
                {
                    title: '纳税人识别号',
                    field: 'TaxpayerCode',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });


    $('#site-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(supplierId)
                rd.supplierId=supplierId;
            asynTask({
                type:'get',
                url:controllers.supplier["get-supplier-sites"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#site-table').bootstrapTable('load', response.Data);
                    $('#site-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: (parent.document.getElementById("contentFrame").height - 20) / 2,
        queryParams:'siteQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
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
                    title: '供应商名称',
                    field: 'SupplierName',
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
                    title: '地址',
                    field: 'Address',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                title: '联系人',
                field: 'Contact',
                valign: 'middle',
                align: 'center'
                },
                {
                    title: '联系电话',
                    field: 'TelPhone',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });

    $("#query-btn").click(function () {
        $('#supplier-table').bootstrapTable('refresh');
    });

    $("#sync-btn").click(function () {
        asynTask({
            type: 'post',
            url: controllers["sys-config"]["update-config"],
            jsonData: { KName: "供应商同步", KVal: "1" },
            successCallback: function (response) {
                if (response.Code == 200)
                    toastr.success("操作成功！", '系统信息', { timeOut: 3000 });
                else
                    toastr.success(response.Data, '系统信息', { timeOut: 3000 });
            }
        });
    });

});