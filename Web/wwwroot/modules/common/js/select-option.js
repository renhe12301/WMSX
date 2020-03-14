var removeOptions=function (id,excludeVals) {
    $("#"+id+" >option").each(function(){
        var id = $(this).attr("value");
        var isDel=true;
        $.each(excludeVals, function( key, value ) {
            if(value==id)
                isDel=false;
        });
        if(isDel){
            this.remove();
        }
    });
};