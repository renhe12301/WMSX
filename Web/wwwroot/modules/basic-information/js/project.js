function projectQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit
    }
}

function taskQueryParams(params) {
    return {
        pageIndex: params.offset,
        itemsPage: params.limit,
        projectId:0
    }
}

var projectId=null;

var projectNameClick=function(id)
{
    projectId=id;
    $('#task-table').bootstrapTable('refresh');
};

$(function () {
    $('#project-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            var projectName=$("#project-name").val();
            if (projectName!="")
                rd.projectName = projectName;
            asynTask({
                type: 'get',
                url: controllers.ebsproject["get-projects"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#project-table').bootstrapTable('load', response.Data);
                    $('#project-table').bootstrapTable('hideLoading');
                }
            });
        },
        height:300,
        queryParams:'projectQueryParams',
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
                    title: '项目编码',
                    field: 'ProjectCode',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        var e = '<a  href="#"  onclick="projectNameClick(' + row.Id + ')">' + row.ProjectCode+'</a> ';
                        return e;
                    }
                },
                {
                    title: '项目名称',
                    field: 'ProjectName',
                    valign: 'middle',
                    align: 'center',
                    formatter : function(value, row, index) {
                        var e = '<a  href="#"  onclick="projectNameClick(' + row.Id + ')">' + row.ProjectName+'</a> ';
                        return e;
                    }
                },
                {
                    title: '项目全名',
                    field: 'ProjectFullName',
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
                    title: '公司部门',
                    field: 'OrgName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '类型编码',
                    field: 'TypeCode',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '状态编码',
                    field: 'StatusCode',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                    title: '项目类别',
                    field: 'ProjectCategory',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                    title: '项目经理',
                    field: 'EmployeeName',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                    title: '创建时间',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                    title: '结束时间',
                    field: 'EndTime',
                    valign: 'middle',
                    align: 'center'
                }

            ]
    });


    $('#task-table').bootstrapTable({
        ajax:function(request)
        {
            var rd=request.data;
            if(projectId)
                rd.projectId = projectId;
            asynTask({
                type: 'get',
                url: controllers.ebstask["get-tasks"],
                jsonData: rd,
                successCallback:function(response)
                {
                    $('#task-table').bootstrapTable('load', response.Data);
                    $('#task-table').bootstrapTable('hideLoading');
                }
            });
        },
        height:300,
        queryParams:'taskQueryParams',
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
                    title: '任务编码',
                    field: 'TaskCode',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '任务名称',
                    field: 'TaskName',
                    valign: 'middle',
                    align: 'center'
                },
                {
                    title: '任务内容',
                    field: 'Summary',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                title: '任务等级',
                field: 'TaskLevel',
                valign: 'middle',
                align: 'center'
                }
                ,
                {
                    title: '创建时间',
                    field: 'CreateTime',
                    valign: 'middle',
                    align: 'center'
                }
                ,
                {
                    title: '结束时间',
                    field: 'EndTime',
                    valign: 'middle',
                    align: 'center'
                }
            ]
    });

    $("#query-btn").click(function () {
        $('#project-table').bootstrapTable('refresh');
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