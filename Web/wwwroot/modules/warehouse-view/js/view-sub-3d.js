var href = window.location.href;

var phyId=parseInt(href.split('?')[1].split('=')[1]);
$(function () {

    $('#sidebar3').overlayScrollbars({ });

    asynTask({
        type: 'get',
        url: controllers.statistical["py-warehouse-chart"],
        jsonData: { pyId: phyId },
        successCallback: function (response) {
            if (response.Code == 200) {
                $("#nor-cnt").text(response.Data.norLocCnt);
                $("#task-cnt").text(response.Data.taskLocCnt);
                $("#dis-cnt").text(response.Data.disLocCnt);
                $("#empty-tray-cnt").text(response.Data.emptyTrayLocCnt);
                $("#material-loc-cnt").text(response.Data.materialLocCnt);
                $("#ou-cnt").text(response.Data.ouCnt);
                $("#supplier-cnt").text(response.Data.supplierCnt);
                $("#supplier-site-cnt").text(response.Data.supplierSiteCnt);
                $("#org-cnt").text(response.Data.warehouseCnt);
                $("#area-cnt").text(response.Data.areaCnt);
                $("#empty-cnt-cnt").text(response.Data.emptyLocCnt);
            }
        }
    });
    

    var stats = initStats();
    var scene, camera, renderer, controls,raycaster;
    var mouse = new THREE.Vector2();
    var matArrayA=[];//内墙
    var matArrayB = [];//外墙
    var rackMat, rackMat2, cargoMat,emptyCargoMat;
    var floorW=2600,floorH=1400;
    var cargoTagFont;

    function initMat() {
        rackMat = new THREE.MeshLambertMaterial();
        rackMat2 = new THREE.MeshPhongMaterial({color:0x1C86EE});
        cargoMat = new THREE.MeshLambertMaterial();
        emptyCargoMat = new THREE.MeshLambertMaterial();
        new THREE.TextureLoader().load( "../../lib/treejs/my/rack.png", function( map ) {
            rackMat.map = map;
            rackMat.needsUpdate = true;
        } );

        new THREE.TextureLoader().load( "../../lib/treejs/textures/crate.gif", function( map ) {
            cargoMat.map = map;
            cargoMat.needsUpdate = true;
        } );
        new THREE.TextureLoader().load( "../../lib/treejs/textures/brick_diffuse.jpg", function( map ) {
            emptyCargoMat.map = map;
            emptyCargoMat.needsUpdate = true;
        } );
    }

    // 初始化场景
    function initScene() {
        scene = new THREE.Scene();
        scene.fog = new THREE.Fog(scene.background, 3000, 5000);
        
        raycaster = new THREE.Raycaster();
        raycaster.linePrecision = 30 / 2;
    }

    // 初始化相机
    function initCamera() {
        camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 10000);
        camera.position.set(0, 800, 1300);
        camera.lookAt(new THREE.Vector3(0, 0, 0));
    }

    // 初始化灯光
    function initLight() {
        var directionalLight = new THREE.DirectionalLight( 0xffffff, 0.3 );
        directionalLight.color.setHSL( 0.1, 1, 0.95 );
        directionalLight.position.set( 0, 200, 0).normalize();
        scene.add( directionalLight );

        var ambient = new THREE.AmbientLight( 0xffffff, 1 );
        ambient.position.set(0,0,0);
        scene.add( ambient );
    }

    // 初始化性能插件
    function initStats() {
        var stats = new Stats();

        stats.domElement.style.position = 'absolute';
        stats.domElement.style.left = '0px';
        stats.domElement.style.top = '0px';
        document.body.appendChild(stats.domElement);
        return stats;
    }

    // 初始化渲染器
    function initRenderer() {
        renderer = new THREE.WebGLRenderer({antialias: true});
        renderer.setSize(window.innerWidth, window.innerHeight);
        renderer.setClearColor(0x353a40,1.0);
        document.body.appendChild(renderer.domElement);

    }

    //创建地板
    function createFloor(){
        var loader = new THREE.TextureLoader();
        loader.load("../../lib/treejs/my/floor.jpg",function(texture){
            texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
            texture.repeat.set( 10, 10 );
            var floorGeometry = new THREE.BoxGeometry(floorW, floorH, 1);
            var floorMaterial = new THREE.MeshBasicMaterial( { map: texture, side: THREE.DoubleSide } );
            var floor = new THREE.Mesh(floorGeometry, floorMaterial);
            floor.position.y = -0.5;
            floor.rotation.x = Math.PI / 2;
            floor.name = "地面";
            scene.add(floor);
        });
    }

    //创建墙
    function createCubeWall(width, height, depth, angle, material, x, y, z, name){
        var cubeGeometry = new THREE.BoxGeometry(width, height, depth );
        var cube = new THREE.Mesh( cubeGeometry, material );
        cube.position.x = x;
        cube.position.y = y;
        cube.position.z = z;
        cube.rotation.y += angle*Math.PI;  //-逆时针旋转,+顺时针
        cube.name = name;
        scene.add(cube);
    }

    //创建门_左侧
    function createDoor_left(width, height, depth, angle, x, y, z, name){
        var loader = new THREE.TextureLoader();
        loader.load("../../lib/treejs/my/door_left.png",function(texture){
            var doorgeometry = new THREE.BoxGeometry(width, height, depth);
            doorgeometry.translate(50, 0, 0);
            var doormaterial = new THREE.MeshBasicMaterial({map:texture,color:0xffffff});
            doormaterial.opacity = 1.0;
            doormaterial.transparent = true;
            var door = new THREE.Mesh( doorgeometry,doormaterial);
            door.position.set(x, y, z);
            door.rotation.y += angle*Math.PI;  //-逆时针旋转,+顺时针
            door.name = name;
            scene.add(door);
        });
    }

    //创建门_右侧
    function createDoor_right(width, height, depth, angle, x, y, z, name){
        var loader = new THREE.TextureLoader();
        loader.load("../../lib/treejs/my/door_right.png",function(texture){
            var doorgeometry = new THREE.BoxGeometry(width, height, depth);
            doorgeometry.translate(-50, 0, 0);
            var doormaterial = new THREE.MeshBasicMaterial({map:texture,color:0xffffff});
            doormaterial.opacity = 1.0;
            doormaterial.transparent = true;
            var door = new THREE.Mesh( doorgeometry,doormaterial);
            door.position.set(x, y, z);
            door.rotation.y += angle*Math.PI;  //-逆时针旋转,+顺时针
            door.name = name;
            scene.add(door);
        });
    }

    //创建窗户
    function createWindow(width, height, depth, angle, x, y, z, name){
        var loader = new THREE.TextureLoader();
        loader.load("../../lib/treejs/my/window.png",function(texture){
            var windowgeometry = new THREE.BoxGeometry(width, height, depth);
            var windowmaterial = new THREE.MeshBasicMaterial({map:texture,color:0xffffff});
            windowmaterial.opacity = 1.0;
            windowmaterial.transparent = true;
            var window = new THREE.Mesh( windowgeometry,windowmaterial);
            window.position.set(x, y, z);
            window.rotation.y += angle*Math.PI;  //-逆时针旋转,+顺时针
            window.name = name;
            scene.add(window);
        });
    }

    //返回墙对象
    function returnWallObject(width, height, depth, angle, material, x, y, z, name){
        var cubeGeometry = new THREE.BoxGeometry(width, height, depth);
        var cube = new THREE.Mesh( cubeGeometry, material );
        cube.position.x = x;
        cube.position.y = y;
        cube.position.z = z;
        cube.rotation.y += angle*Math.PI;
        cube.name = name;
        return cube;
    }

    //墙上挖门，通过两个几何体生成BSP对象
    function createResultBsp(bsp,objects_cube){
        var material = new THREE.MeshPhongMaterial({color:0x9cb2d1,specular:0x9cb2d1,shininess:30,transparent:true,opacity:1});
        var BSP = new ThreeBSP(bsp);
        for(var i = 0; i < objects_cube.length; i++){
            var less_bsp = new ThreeBSP(objects_cube[i]);
            BSP = BSP.subtract(less_bsp);
        }
        var result = BSP.toMesh(material);
        result.material.flatshading = THREE.FlatShading;
        result.geometry.computeFaceNormals();  //重新计算几何体侧面法向量
        result.geometry.computeVertexNormals();
        result.material.needsUpdate = true;  //更新纹理
        result.geometry.buffersNeedUpdate = true;
        result.geometry.uvsNeedUpdate = true;
        scene.add(result);
    }

    //创建墙纹理
    function createWallMaterail(){
        matArrayA.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //前  0xafc0ca :灰色
        matArrayA.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //后
        matArrayA.push(new THREE.MeshPhongMaterial({color: 0xd6e4ec}));  //上  0xd6e4ec： 偏白色
        matArrayA.push(new THREE.MeshPhongMaterial({color: 0xd6e4ec}));  //下
        matArrayA.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //左    0xafc0ca :灰色
        matArrayA.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //右

        matArrayB.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //前  0xafc0ca :灰色
        matArrayB.push(new THREE.MeshPhongMaterial({color: 0x9cb2d1}));  //后  0x9cb2d1：淡紫
        matArrayB.push(new THREE.MeshPhongMaterial({color: 0xd6e4ec}));  //上  0xd6e4ec： 偏白色
        matArrayB.push(new THREE.MeshPhongMaterial({color: 0xd6e4ec}));  //下
        matArrayB.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //左   0xafc0ca :灰色
        matArrayB.push(new THREE.MeshPhongMaterial({color: 0xafc0ca}));  //右
    }

    // 初始化模型
    function initContent() {
        createFloor();
        createWallMaterail();
        createCubeWall(10, 200, 1400, 0, matArrayB, -1295, 100, 0, "墙面");
        createCubeWall(10, 200, 1400, 1, matArrayB, 1295, 100, 0, "墙面");
        createCubeWall(10, 200, 2600, 1.5, matArrayB, 0, 100, -700, "墙面");

        //创建挖了门的墙
        var wall = returnWallObject(2600, 200, 10, 0, matArrayB, 0, 100, 700, "墙面");
        var door_cube1 = returnWallObject(200, 180, 10, 0, matArrayB, -600, 90, 700, "前门1");
        var door_cube2 = returnWallObject(200, 180, 10, 0, matArrayB, 600, 90, 700, "前门2");
        var window_cube1 = returnWallObject(100, 100, 10, 0, matArrayB, -900, 90, 700, "窗户1");
        var window_cube2 = returnWallObject(100, 100, 10, 0, matArrayB, 900, 90, 700, "窗户2");
        var window_cube3 = returnWallObject(100, 100, 10, 0, matArrayB, -200, 90, 700, "窗户3");
        var window_cube4 = returnWallObject(100, 100, 10, 0, matArrayB, 200, 90, 700, "窗户4");
        var objects_cube = [];
        objects_cube.push(door_cube1);
        objects_cube.push(door_cube2);
        objects_cube.push(window_cube1);
        objects_cube.push(window_cube2);
        objects_cube.push(window_cube3);
        objects_cube.push(window_cube4);
        createResultBsp(wall, objects_cube);
        //为墙面安装门
        createDoor_left(100, 180, 2, 0, -700, 90, 700, "左门1");
        createDoor_right(100, 180, 2, 0, -500, 90, 700, "右门1");
        createDoor_left(100, 180, 2, 0, 500, 90, 700, "左门2");
        createDoor_right(100, 180, 2, 0, 700, 90, 700, "右门2");
        //为墙面安装窗户
        createWindow(100, 100, 2, 0, -900, 90, 700, "窗户");
        createWindow(100, 100, 2, 0, 900, 90, 700, "窗户");
        createWindow(100, 100, 2, 0, -200, 90, 700, "窗户");
        createWindow(100, 100, 2, 0, 200, 90, 700, "窗户");
    }

    // 初始化轨迹球控件
    function initControls() {
        controls = new THREE.OrbitControls( camera, renderer.domElement );
        controls.enableDamping = true;
        controls.dampingFactor = 0.5;
        // 视角最小距离
        controls.minDistance = 100;
        // 视角最远距离
        controls.maxDistance = 5000;
        // 最大角度
        controls.maxPolarAngle = Math.PI/2.2;
    }

    function addRacks(x,y,z,plane_x,plane_y,plane_z,holder_x,holder_y,holder_z,index,num,tag) {
        var plane = new THREE.BoxGeometry(plane_x, plane_y, plane_z);
        var holder = new THREE.BoxGeometry(holder_x, holder_y, holder_z);
        var cargo = new THREE.BoxGeometry(30, 30, 30);
        cargo.Tag=tag;
        
        var emptyCargo = new THREE.BoxGeometry(30, 5, 30);
        emptyCargo.Tag=tag;

        var offset_x = x + index * plane_x;
        var obj = new THREE.Mesh(plane, rackMat);
        obj.position.set(offset_x, y, z);
        scene.add(obj);

        if (index % 2 == 0) {
            var rack1 = new THREE.Mesh(holder, rackMat2, 0);
            var rack2 = new THREE.Mesh(holder, rackMat2, 0);
            rack1.position.set(offset_x - plane_x / 2 + holder_x / 2, y - holder_y / 2, z + plane_z / 2 - 3);
            rack2.position.set(offset_x - plane_x / 2 + holder_x / 2, y - holder_y / 2, z + plane_z / 2 - plane_z + 3);
            scene.add(rack1);
            scene.add(rack2);
        }
        if (index + 1 >= num) {
            var rack1 = new THREE.Mesh(holder, rackMat2, 0);
            var rack2 = new THREE.Mesh(holder, rackMat2, 0);
            rack1.position.set(x + num * plane_x - plane_x / 2 + holder_x / 2, y - holder_y / 2, z + plane_z / 2 - 3);
            rack2.position.set(x + num * plane_x - plane_x / 2 + holder_x / 2, y - holder_y / 2, z + plane_z / 2 - plane_z + 3);
            scene.add(rack1);
            scene.add(rack2);
        }
        var cargoMesh = new THREE.Mesh(cargo, cargoMat);
        cargoMesh.visible = false;
        cargoMesh.position.set(offset_x, y + 17, z);

        var emptyCargoMesh = new THREE.Mesh(emptyCargo, emptyCargoMat);
        emptyCargoMesh.visible = false;
        emptyCargoMesh.position.set(offset_x, y+5, z);
        
        scene.add(cargoMesh);
        scene.add(emptyCargoMesh);
        
        var geometryCube = cube(30);
        var lineSegments = new THREE.LineSegments(geometryCube, new THREE.LineDashedMaterial({ color: '#FCFCFC', dashSize: 3, gapSize: 1 }));
        lineSegments.computeLineDistances();
        lineSegments.translateOnAxis(new THREE.Vector3(0, 0, 1), z);
        lineSegments.translateOnAxis(new THREE.Vector3(1, 0, 0), offset_x);
        lineSegments.translateOnAxis(new THREE.Vector3(0, 1, 0), y + 17);
        lineSegments.visible = false;
        scene.add(lineSegments);


        if (tag.InStock == "有货")
            cargoMesh.visible = true;
        if (tag.InStock == "无货")
            lineSegments.visible = true;
        if (tag.InStock == "空托盘")
            emptyCargoMesh.visible = true;
           
       
    }
     
    function buildRacks() {
        asynTask({type:'get',url:controllers["location"]["get-max-floor-item-col"],
            jsonData: {phyId:phyId},
            successCallback:function (response) {
                if(response.Code==200)
                {
                    var rowRanCols=response.Data;
                    asynTask({type:'get',url:controllers["location"]["get-locations"],
                        jsonData: {phyId:phyId},
                        successCallback:function (response2) {
                            if(response2.Code==200)
                            {
                                var row=rowRanCols[0];
                                for (var i=1;i<=row;i++)
                                {
                                    for (var j=1;j<=rowRanCols[1];j++) {
                                        for (var col = 1; col <= rowRanCols[2]; col++) {
                                            var l = response2.Data.find(x => x.Row == i && x.Rank == j && x.Col == col);
                                            addRacks(-1200, 35 * i, -600 + j * 120, 45 * 5, 3, 35, 2, 40, 2, col, rowRanCols[2], l);
                                        }
                                    }
                                }
                                //buildCargoTag(rowRanCols,response2.Data);
                                loadingClose();
                            }
                            else {
                                toastr.error(response2.Data, '错误信息', {timeOut: 3000});
                            }
                        }
                    });
                }
                else {
                    toastr.error(response.Data, '错误信息', {timeOut: 3000});
                }
            }
        });
    }

    function buildCargoTag(rls,locations) {
        new THREE.FontLoader().load('../../lib/treejs/libs/FZYaoTi_Regular.json',function(font){
            var row=rls[0];
            for (var i=1;i<=row;i++)
            {
                for (var j=1;j<=rls[1];j++) {
                    for (var col=1;col<=rls[2];col++) {
                        var l = locations.find(x => x.Row == i && x.Rank == j && x.Col == col);
                        var text = new THREE.TextGeometry(l.SysCode, {
                            font: font,
                            size: 1,
                            height: 0.01
                        });
                        text.computeBoundingBox();
                        var m = new THREE.MeshStandardMaterial({color: "#FF0000"});
                        var cagoTagMesh = new THREE.Mesh(text, m);
                        var offset_x = -1300 + col * 45 * 5;
                        cagoTagMesh.position.set(offset_x, 35 * i, (-600 + j * 120) + 20);
                        scene.add(cagoTagMesh);
                    }
                }
            }
            loadingClose();
        });

    }

    function update() {
        stats.update();
        controls.update();
    }
    
    function initEvent() {
        document.addEventListener('resize', onWindowResize, false);
        document.addEventListener('mousemove', function (event) {
            event.preventDefault();
            mouse.x = (event.layerX / window.innerWidth) * 2 - 1;
            mouse.y = -(event.layerY / window.innerHeight) * 2 + 1;
        }, false);
        document.addEventListener('mouseup', function (event) {
            event.preventDefault();
            mouse.x = (event.layerX / document.innerWidth) * 2 - 1;
            mouse.y = -(event.layerY / document.innerHeight) * 2 + 1;
            var intersects = raycaster.intersectObjects(scene.children, true);
            var selectV;
            $.each(intersects, function (index, val) {
                if (val.object)
                    if (val.object.geometry)
                        if (val.object.geometry.Tag) selectV = val.object.geometry.Tag;
            });
            if (selectV) {
                $('#location-detail-dlg').modal('show');
                $("#location-code").text(selectV.UserCode);
                $("#tray-code").text("");
                $("#material-code").val("");
                $("#material-name").val("");
                $("#material-count").val("");
                $("#area-name").val("");
                $("#warehouse-name").val("");
                $("#ou-name").val("");
                $("#material-spec").val("");
                asynTask({
                    type: 'get',
                    url: controllers["warehouse-material"]["get-materials"],
                    jsonData: { locationId: selectV.Id },
                    successCallback: function (response) {
                        if (response.Code == 200) {
                            if (response.Data.length > 0) {
                                var data = response.Data[0];
                                $("#location-code").text(data.LocationCode);
                                $("#tray-code").text(data.TrayCode);
                                $("#material-code").val(data.Code);
                                $("#material-name").val(data.MaterialName);
                                $("#material-count").val(data.MaterialCount);
                                $("#area-name").val(data.ReservoirAreaName);
                                $("#warehouse-name").val(data.WarehouseName);
                                $("#ou-name").val(data.OUName);
                                $("#material-spec").val(data.Spec);
                            }

                        }
                    }
                });
            }
        }, false);

    }


    function init() {
        loadingShow();
        initMat();
        initScene();
        initCamera();
        initRenderer();
        initContent();
        initLight();
        initControls();
        buildRacks();
        initEvent();
    }

    function onWindowResize() {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    function animate() {
        requestAnimationFrame(animate);
        camera.updateMatrixWorld();
        raycaster.setFromCamera(mouse, camera);
        renderer.render(scene, camera);
        update();
    }

    init();
    animate();



    function cube(size) {

        var h = size * 0.5;

        var geometry = new THREE.BufferGeometry();
        var position = [];

        position.push(
            - h, - h, - h,
            - h, h, - h,

            - h, h, - h,
            h, h, - h,

            h, h, - h,
            h, - h, - h,

            h, - h, - h,
            - h, - h, - h,

            - h, - h, h,
            - h, h, h,

            - h, h, h,
            h, h, h,

            h, h, h,
            h, - h, h,

            h, - h, h,
            - h, - h, h,

            - h, - h, - h,
            - h, - h, h,

            - h, h, - h,
            - h, h, h,

            h, h, - h,
            h, h, h,

            h, - h, - h,
            h, - h, h
        );

        geometry.setAttribute('position', new THREE.Float32BufferAttribute(position, 3));

        return geometry;

    }
});