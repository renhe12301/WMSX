
function roleQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        roleName: $("#role-name").val()
    }
}
var selectNode=null;
var roleRow=null;
function operateFormatter(value, row, index) {
    return [
        '<a class="edit-role" href="javascript:void(0)" title="修改角色">',
        '<i class="fa fa-edit"></i>',
        '</a>  ',
        '<a class="assign-menu" href="javascript:void(0)" title="分配菜单">',
        '<i class="fa fa-arrow-right"></i>',
        '</a>'
    ].join('')
}
operateEvents = {
    'click .edit-role': function (e, value, row, index) {
        if(row.RoleName=="超级管理员")
        {
            toastr.error("不能修改系统默认管理员信息！", '错误信息', {timeOut: 3000});
            return;
        }
        $('#edit-role-dlg').modal('show');
        $('#edit-role-name').val(row.RoleName);
        $('#edit-role-id').val(row.Id);
    },
    'click .assign-menu': function (e, value, row, index) {
        if(row.RoleName=="超级管理员")
        {
            toastr.error("不能修改系统默认管理员信息！", '错误信息', {timeOut: 3000});
            return;
        }
        $('#assign-menu-dlg').modal('show');
        roleRow=row;
        asynTask({
            type:'get',
            url:controllers["sys-menu"]["get-menus"],
            jsonData:{roleId:roleRow.Id},
            successCallback:function (response) {
                var selectIds=[];
                $.each( response.Data, function( key, value ) {
                    selectIds.push(parseInt(value.MenuId));
                });
                renderTree({rootId: 1,renderTarget:'jsTree',showCheckbox:true,
                    defaultSelecteds:selectIds,depthTag: 'menu',url:controllers["sys-menu"]["get-menu-trees"],
                    selectNodeCall:function (node, data) {


                    }
                });
            }
        });

    }
};

$(function () {
    $('#sidebar').overlayScrollbars({ });
    $('#role-table').bootstrapTable({
        ajax:function(request)
        {
            asynTask({
                type:'get',
                url:controllers["sys-role"]["get-roles"],
                jsonData: request.data,
                successCallback:function(response)
                {
                    $('#role-table').bootstrapTable('load', response.Data);
                    $('#role-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'roleQueryParams',
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
                    field: 'state',
                    checkbox: true,
                    align: 'center',
                    valign: 'middle'
                },
                {
                    title: '编号',
                    field: 'Id',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '角色名',
                    field: 'RoleName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '创建时间',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '角色状态',
                    field: 'Status',
                    valign: 'middle',
                    align: 'center'
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

    $("#add-role-form").validate({
        rules: {
            addRoleName: {
                required: true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {
            asynTask({type:'post',url:controllers["sys-role"]["add-role"],
                jsonData:
                    {
                        RoleName:$('#add-role-name').val(),
                        Type:0,
                        ParentId:selectNode==null?1:(selectNode.Type=="dir"?selectNode.Id:1)
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        $('#add-role-name').val('');
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#role-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#edit-role-form").validate({
        rules: {
            editRoleName: {
                required: true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {
            asynTask({type:'post',url:controllers["sys-role"]["update-role"],
                jsonData:
                    {
                        Id:parseInt($('#edit-role-id').val()),
                        RoleName:$('#edit-role-name').val()
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#role-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#assign-btn").click(function () {
       var selecteds= $('#jsTree').jstree('get_selected',true);
       var selectIds=[];
        $.each( selecteds, function( key, value ) {
             selectIds.push(parseInt(value.data.id));
        });
        asynTask({type:'post',url:controllers["sys-role"]["assign-menu"],
            jsonData:
                {
                    RoleId:parseInt(roleRow.Id),
                    MenuIds:selectIds
                },
            successCallback:function (response) {
                if(response.Code==200)
                {
                    toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                    $('#assign-menu-dlg').modal('hide');
                }
                else {
                    toastr.error(response.Data, '错误信息', {timeOut: 3000});
                }
            }
        });
    });
    $("#enable-btn").click(function () {
         var tableSelects=$("#role-table").bootstrapTable('getSelections');
         if(tableSelects.length==0)
         {
             toastr.error("请选择需要启用的角色！", '错误信息', {timeOut: 3000});
             return;
         }
        var selectIds=[];
        var existAdmin = false;
        $.each( tableSelects, function( key, value ) {
            selectIds.push(parseInt(value.Id));
            if(value.RoleName=="超级管理员")
                existAdmin = true;
        });
        if(existAdmin)
        {
            toastr.error("不能修改系统默认管理员信息！", '错误信息', {timeOut: 3000});
            return;
        }
         confirmShow(function () {

             asynTask({type:'post',url:controllers["sys-role"]["enable"],
                 jsonData:
                     {
                         RoleIds:selectIds
                     },
                 successCallback:function (response) {
                     if(response.Code==200)
                     {
                         toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                         $('#role-table').bootstrapTable('refresh');
                     }
                     else {
                         toastr.error(response.Data, '错误信息', {timeOut: 3000});
                     }
                 }
             });
         });

    });
    $("#disable-btn").click(function () {

        var tableSelects=$("#role-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要禁用的角色！", '错误信息', {timeOut: 3000});
            return;
        }
        var selectIds=[];
        var existAdmin = false;
        $.each( tableSelects, function( key, value ) {
            selectIds.push(parseInt(value.Id));
            if(value.RoleName=="超级管理员")
                existAdmin = true;
        });
        if(existAdmin)
        {
            toastr.error("不能修改系统默认管理员信息！", '错误信息', {timeOut: 3000});
            return;
        }
        confirmShow(function () {

            asynTask({type:'post',url:controllers["sys-role"]["logout"],
                jsonData:
                    {
                        RoleIds:selectIds
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#role-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });

    });
    $("#query-btn").click(function () {
        $('#role-table').bootstrapTable('refresh');
    });
});