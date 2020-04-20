function orgQueryParams(params) {
   return {
      pageIndex: params.offset,
      itemsPage: params.limit
   }
}
var ouId=null;
$(function () {
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
                $(".select2").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
            }
        }
    });
   $('#org-table').bootstrapTable({
         ajax:function(request)
         {
             var rd=request.data;
             if(ouId)rd.ouId=ouId;
             var orgName=$("#org-name").val();
             if(orgName!="")rd.orgName=orgName;
              asynTask({
                                  type:'get',
                                  url:controllers.organization["get-organizations"],
                                  jsonData: rd,
                                   successCallback:function(response)
                                   {
                                      $('#org-table').bootstrapTable('load', response.Data);
                                      $('#org-table').bootstrapTable('hideLoading');
                                   }
                       });
         },
         height: parent.document.getElementById("contentFrame").height - 10,
         queryParams:'orgQueryParams',
         pagination: true,
         pageNumber:1,
         sidePagination: "server",
         pageSize: parseInt((parent.document.getElementById("contentFrame").height - 10) / 55),
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
                     title: '公司编号',
                     field: 'OrgCode',
                     valign: 'middle',
                     align: 'center'
                 },
                {
                   title: '公司名称',
                   field: 'OrgName',
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
        var select=$('.select2').val();
        if(select!='0')
            ouId=parseInt(select);
        else
            ouId=null;
        $('#org-table').bootstrapTable('refresh');
    });

    $("#sync-btn").click(function () {
        asynTask({
            type: 'post',
            url: controllers["sys-config"]["update-config"],
            jsonData: {KName:"公司部门同步",KVal:"1"},
            successCallback: function (response) {
                if (response.Code == 200)
                    toastr.success("操作成功！", '系统信息', { timeOut: 3000 });
                else
                    toastr.success(response.Data, '系统信息', { timeOut: 3000 });
            }
        });
    });
});