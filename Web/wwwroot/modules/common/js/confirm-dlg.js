var confirmShow;
$(function () {
var dlg='<div class="modal fade" id="option-confirm-dlg" style="display: none;" aria-hidden="true">'+
        '<div class="modal-dialog">'+
        '<div class="modal-content bg-warning">'+
        '<div class="modal-header">'+
        '<h4 class="modal-title">系统提示</h4>'+
        '<button type="button" class="close" data-dismiss="modal" aria-label="Close">'+
        '<span aria-hidden="true">×</span></button>'+
        '</div>'+
        '<div class="modal-body">'+
          '<p>确认要继续此操作吗？</p>'+
        '</div>'+
       '<div class="modal-footer justify-content-between">'+
        '<button type="button" class="btn btn-outline-dark" data-dismiss="modal">关闭</button>'+
        '<button type="button" id="option-confirm-btn" class="btn btn-outline-dark">是的,继续操作！</button>'+
        '</div>'+
        '</div>'+
        '</div>'+
        '</div>';
$("body").append(dlg);
    confirmShow=function(option)
    {
        $('#option-confirm-dlg').modal('show');
        $("#option-confirm-btn").unbind("click");
        $("#option-confirm-btn").click(function () {
            option();
            $('#option-confirm-dlg').modal('hide');
        });
    };
});