var phyId=1;
var types=null;

$(function () {
    parentHeight = parent.document.getElementById("contentFrame").height - 100;
    $(".tab-content border border-top-0").css("height", parentHeight);
    $("#contentFrame").attr("height", parentHeight);
    // asynTask({
    //     type:'get',
    //     url:controllers["phy-warehouse"]["get-phy-warehouses"],
    //     successCallback:function(response)
    //     {
    //         if(response.Code==200)
    //         {
    //             var data=[];
    //             $.each(response.Data, function(key, val) {
    //                 data.push({id:val.Id,text:val.PhyName});
    //             });
    //             $(".select2").select2({
    //                 data: data,
    //                 theme: 'bootstrap4'
    //             });
    //             $(".select2").change(function () {
    //                 phyId=parseInt($("#phy-select").val());
    //                 $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);
    //             });
    //             if(data.length>0)
    //             {
    //                 phyId=parseInt(data[0].id);
    //                 $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);
    //             }
    //         }
    //     }
    // });

    $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);


    $("#ljx-tab").click(function () {
        phyId = 1;
        $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);
    });
    $("#gh-tab").click(function () {
        phyId = 2;
        $("#contentFrame").attr("src","view-sub-3d.html?phyId="+phyId);
    });
});