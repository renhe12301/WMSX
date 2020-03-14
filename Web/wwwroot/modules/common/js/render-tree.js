var renderTree=function (paramConfig) {
    jsonData={};
    if(paramConfig.rootId)
        jsonData.rootId=paramConfig.rootId;
    if(paramConfig.depthTag)
        jsonData.depthTag=paramConfig.depthTag;
    asynTask(
        {
            type:'get',
            url:paramConfig.url,
            jsonData:jsonData,
            successCallback:function (response) {
                if(response.Code==200) {
                    var rdata = response.Data;
                    if(paramConfig.successCallback)
                        paramConfig.successCallback(rdata);
                    var treeData = {
                        text: rdata.Name,
                        icon: 'fas fa-folder',
                        data:{
                            id: rdata.Id,
                            text:rdata.Name,
                            type: rdata.Type,
                            parentId: rdata.ParentId
                        },
                        state : { opened : true},
                        children: []
                    };
                    var formaterData = function (current, tree) {
                        $.each(current.Children, function (i, v) {
                            var icon='fas fa-university';
                            var icon2='fas fa-th-large';
                            var icon3='fas fa-warehouse';
                            var icon4='fas fa-user';
                            var icon5='fas fa-folder';
                            var icon6='fas fa-user-circle';
                            var icon7="fas fa-leaf";
                            var icon8="fas fa-users";
                            if(v.Type=="area")
                                icon=icon2;
                            else if(v.Type=="warehouse")
                                icon=icon3;
                            else if(v.Type=="user")
                                icon=icon4;
                            else if(v.Type=="dir")
                                icon=icon5;
                            else if(v.Type=="role")
                                icon=icon6;
                            else if(v.Type=="leaf")
                                icon=icon7;
                            else if(v.Type=="ou")
                                icon=icon8;
                            var treeConfig={
                                text: v.Name,
                                icon:icon,
                                data:{
                                    id: v.Id,
                                    text:v.Name,
                                    type: v.Type,
                                    parentId: v.ParentId
                                },
                                state : { opened : true },
                                children: []
                            };
                            if(paramConfig.showCheckbox)
                            {
                                treeConfig.state.selected=paramConfig.defaultSelecteds.findIndex((f)=>f==v.Id)>-1;
                                tree.children.push(treeConfig);
                            }
                            else
                            {
                                tree.children.push(treeConfig);
                            }

                            formaterData(v, tree.children[i]);
                        })
                    };
                    formaterData(rdata, treeData);
                    if(paramConfig.showCheckbox)
                    {
                        $('#' + paramConfig.renderTarget).jstree("destroy");
                        $('#' + paramConfig.renderTarget).jstree({
                            'core': {
                                'data': treeData
                            },
                            "checkbox" : {
                                "keep_selected_style" : false
                            },
                            "plugins" : [ "checkbox" ]
                        });

                    }
                    else {
                        $('#' + paramConfig.renderTarget).jstree("destroy");
                        $('#' + paramConfig.renderTarget).jstree({
                            'core': {
                                'data': treeData
                            }
                        });
                    }
                    if (paramConfig.selectNodeCall) {
                        if(!paramConfig.showCheckbox) {
                            $('#' + paramConfig.renderTarget).on("changed.jstree", function (e, node) {
                                paramConfig.selectNodeCall(node.node, node.node.data);
                            });
                        }
                    }
                    if(!paramConfig.showRoot)
                    {
                        $('#' + paramConfig.renderTarget).on("loaded.jstree", function(e, data) {
                            $('#' + paramConfig.renderTarget+" > ul > li > i.jstree-icon").remove();
                            $('#' + paramConfig.renderTarget+" > ul > li > a.jstree-anchor").remove();
                        });
                    }
                }

            }
        });

};