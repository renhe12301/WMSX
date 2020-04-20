
function dicQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}
$(function () {

    asynTask({
        type:'get',
        url:controllers["material-type"]["get-material-types"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.TypeName});
                });
                $(".select2").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
            }
        }
    });

    $('#material-dic-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            materialCode=$("#material-dic-code").val();
            if(materialCode!="")
                rd.materialCode=materialCode;
            materialName=$("#material-dic-name").val();
            if(materialName!="")
                rd.materialName=materialName;
            spec=$("#material-dic-spec").val();
            if(spec!="")
                rd.spec=spec;
            var typeId=$("#type-select").val();
            if(typeId!="0")
                rd.typeId=typeId;
            asynTask({
                type:'get',
                url:controllers["material-dic"]["get-material-dics"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#material-dic-table').bootstrapTable('load', response.Data);
                    $('#material-dic-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'dicQueryParams',
        pagination: true,
        pageNumber:1,
        sidePagination: "server",
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        smartDisplay:false,
        toolbar: '#toolbar',
        showColumns: true,
        showRefresh: true,
        clickToSelect: true,
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
                    title: '规格型号',
                    field: 'Spec',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '创建时间',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                }

            ]
    });
    $("#query-btn").click(function () {
        $('#material-dic-table').bootstrapTable('refresh');
    });


});