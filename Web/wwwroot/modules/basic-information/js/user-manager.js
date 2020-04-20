function userQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}

function operateFormatter(value, row, index) {
    return [
        '<a class="query-detail" href="javascript:void(0)" title="查看详情">',
        '<i class="fa fa-address-card"></i>',
        '</a>  ',
        '<a class="edit-user" href="javascript:void(0)" title="修改用户">',
        '<i class="fa fa-edit"></i>',
        '</a>  ',
        '<a class="assign-role" href="javascript:void(0)" title="分配角色">',
        '<i class="fa fa-arrow-right"></i>',
        '</a>'
    ].join('')
}
operateEvents = {
    'click .assign-role': function (e, value, row, index) {
        $('#assign-role-dlg').modal('show');
        $('#user-id').val(row.Id);
        asynTask({
            type:'get',
            url:controllers["employee"]["get-roles"],
            jsonData:{employeeId:row.Id},
            successCallback:function(response)
            {
                if(response.Code==200)
                {
                    var data=[];
                    $.each(response.Data, function(key, val) {
                        data.push(val.Id);
                    });
                    if(data.length>0)
                       $(".select2").val(data).trigger('change');
                }

            }
        });
    },
    'click .query-detail': function (e, value, row, index) {
        $('#query-detail-dlg').modal('show');
        $("#login-name").val(row.LoginName);
        $("#login-pwd").val(row.LoginPwd);
        $("#user-code").val(row.UserCode);
        $("#user-name").val(row.UserName);
        $("#user-sex").val(row.Sex);
        $("#user-phone").val(row.Telephone);
        $("#user-email").val(row.Email);
        $("#user-address").val(row.Address);
        $("#user-status").val(row.Status);
    },
    'click .edit-user': function (e, value, row, index) {
        userRow=row;
        $('#edit-user-dlg').modal('show');
        $("#edit-login-name").val(row.LoginName);
        $("#edit-login-pwd").val(row.LoginPwd);
        $("#edit-user-id").val(row.Id);
    }
};
var userRow=null;
var orgId=null;
$(function () {
    $('#sidebar').overlayScrollbars({ });
    asynTask({
        type:'get',
        url:controllers["sys-role"]["get-roles"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.RoleName});
                });
                $("#role-select").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
            }

        }
    });

    asynTask({
        type:'get',
        url:controllers.organization["get-organizations"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.OrgName});
                });
                $("#org-select").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
            }
        }
    });

    $('#user-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            employeeName = $("#employee-name").val();
            if(employeeName!="")
                rd.employeeName=employeeName;
            if(orgId)
                rd.orgId=orgId;
            asynTask({
                type:'get',
                url:controllers.employee["get-employees"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#user-table').bootstrapTable('load', response.Data);
                    $('#user-table').bootstrapTable('hideLoading');
                }
            });
        },
        height: parent.document.getElementById("contentFrame").height - 10,
        queryParams:'userQueryParams',
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
                    title: '登录名',
                    field: 'LoginName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '登录密码',
                    field: 'LoginPwd',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '用户编码',
                    field: 'UserCode',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '用户名',
                    field: 'UserName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '用户状态',
                    field: 'Status',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '公司部门',
                    field: 'OrgName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '系统角色',
                    field: 'RoleName',
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

    $("#enable-btn").click(function () {
        var tableSelects=$("#user-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要启用的用户！", '错误信息', {timeOut: 3000});
            return;
        }
        confirmShow(function () {
            var selectIds=[];
            $.each( tableSelects, function( key, value ) {
                selectIds.push(parseInt(value.Id));
            });
            asynTask({type:'post',url:controllers["employee"]["enable"],
                jsonData:
                    {
                        UserIds:selectIds
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#user-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });

    });
    $("#disable-btn").click(function () {
        var tableSelects=$("#user-table").bootstrapTable('getSelections');
        if(tableSelects.length==0)
        {
            toastr.error("请选择需要禁用的用户！", '错误信息', {timeOut: 3000});
            return;
        }
        confirmShow(function () {
            var selectIds=[];
            $.each( tableSelects, function( key, value ) {
                selectIds.push(parseInt(value.Id));
            });
            asynTask({type:'post',url:controllers["employee"]["logout"],
                jsonData:
                    {
                        UserIds:selectIds
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#user-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        });

    });
    $("#query-btn").click(function () {
        select = $("#org-select").val();
        if(select!="0")
            orgId=select;
        else orgId=null;
        $('#user-table').bootstrapTable('refresh');
    });
    $("#assign-btn").click(function () {
        var selects=$('.select2').val();
        var rd={Id:parseInt($("#user-id").val()),RoleIds:[]};
        $.each( selects, function( key, value ) {
            rd.RoleIds.push(parseInt(value));
        });
        asynTask({type:'post',url:controllers["employee"]["assign-role"],
            jsonData:rd,
            successCallback:function (response) {
                if(response.Code==200)
                {
                    toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                    $('#user-table').bootstrapTable('refresh');
                }
                else {
                    toastr.error(response.Data, '错误信息', {timeOut: 3000});
                }
            }
        });
    });


    $("#edit-user-form").validate({
        rules: {
            'edit-login-name': {
                required: true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            },
            'edit-login-pwd': {
                required: true,
                normalizer: function(value) {
                    return $.trim(value);
                }
            }
        },
        submitHandler:function (form) {

            asynTask({type:'post',url:controllers['employee']["update-employee"],
                jsonData:
                    {
                        Id:parseInt($('#edit-user-id').val()),
                        LoginName:$('#edit-login-name').val(),
                        LoginPwd:$('#edit-login-pwd').val(),
                        UserName:userRow.UserName,
                        UserCode:userRow.UserCode,
                        Sex:userRow.Sex,
                        Telephone:userRow.Telephone,
                        Email:userRow.Email,
                        Address:userRow.Address,
                        Img:userRow.Img
                    },
                successCallback:function (response) {
                    if(response.Code==200)
                    {
                        toastr.success("操作成功！", '系统信息', {timeOut: 3000});
                        $('#user-table').bootstrapTable('refresh');
                    }
                    else {
                        toastr.error(response.Data, '错误信息', {timeOut: 3000});
                    }
                }
            });
        }
    });

    $("#sync-btn").click(function () {
        asynTask({
            type: 'post',
            url: controllers["sys-config"]["update-config"],
            jsonData: { KName: "用户信息同步", KVal: "1" },
            successCallback: function (response) {
                if (response.Code == 200)
                    toastr.success("操作成功！", '系统信息', { timeOut: 3000 });
                else
                    toastr.success(response.Data, '系统信息', { timeOut: 3000 });
            }
        });
    });
});