var asynTask = function (asynParam) {
    var paramConfig={
        type: asynParam.type,
        url: asynParam.url,
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (asynParam.successCallback)
                asynParam.successCallback(response);
        },
        error:function (response) {
            toastr.error("访问服务出现故障！", '错误信息', {timeOut: 3000});
        }
    };
    if(asynParam.type=="post")
        paramConfig.data = JSON.stringify(asynParam.jsonData);
    else if(asynParam.type=="get")
    {
        var len=0;
        $.each(asynParam.jsonData, function(key, val) {
            len++;
        });
        if(len>0)
            paramConfig.url+='?';
        var index=0;
        $.each(asynParam.jsonData, function(key, val) {
            paramConfig.url+=key;
            paramConfig.url+='=';
            paramConfig.url+=val;
            if(index<len-1)
                paramConfig.url+='&';
            index++;
        });

    }
    return $.ajax(paramConfig);
};