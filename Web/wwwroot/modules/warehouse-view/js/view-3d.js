var phyId=null;
var types=null;

$(function () {
    asynTask({
        type:'get',
        url:controllers["phy-warehouse"]["get-phy-warehouses"],
        successCallback:function(response)
        {
            if(response.Code==200)
            {
                var data=[];
                $.each(response.Data, function(key, val) {
                    data.push({id:val.Id,text:val.PhyName});
                });
                $(".select2").select2({
                    data: data,
                    theme: 'bootstrap4'
                });
                $(".select2").change(function () {
                    phyId=parseInt($("#phy-select").val());
                    $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);
                });
                if(data.length>0)
                {
                    phyId=parseInt(data[0].id);
                    $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);
                }
            }
        }
    });
});