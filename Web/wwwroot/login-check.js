var loginCheck = function () {
    var wmsuser = $.cookie('wms-user');
    if(wmsuser==undefined||wmsuser==null||wmsuser==""||wmsuser=="null"){
        var parent = window.parent;
        if(parent==undefined||parent==null||parent=="")
        {
            window.location.href = 'login.html';
        }
        else
        {
            parent.location.href="login.html";
        }
    }
};
