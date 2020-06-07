const baseUrl = 'http://localhost:5001/api';
const controllers =
    {
            'organization':
                {
                        'get-organizations': baseUrl + "/organization/get-organizations"
                },
            'warehouse':
                {
                         'get-warehouses': baseUrl + '/warehouse/get-warehouses',
                         'warehouse-asset-chart':baseUrl+'/warehouse/warehouse-asset-chart',
                         'warehouse-material-chart':baseUrl+'/warehouse/warehouse-material-chart',
                         'warehouse-tray-chart':baseUrl+'/warehouse/warehouse-tray-chart',
                         'warehouse-entry-out-record-chart':baseUrl+'/warehouse/warehouse-entry-out-record-chart'
                },
            'reservoir-area':
                {
                        'get-areas': baseUrl + '/reservoir-area/get-areas',
                        'assign-location': baseUrl + '/reservoir-area/assign-location',
                        'area-asset-chart':baseUrl+'/reservoir-area/area-asset-chart',
                        'area-material-chart':baseUrl+'/reservoir-area/area-material-chart',
                        'area-tray-chart':baseUrl+'/reservoir-area/area-warehouse-tray-chart',
                        'area-entry-out-record-chart':baseUrl+'/reservoir-area/area-entry-out-record-chart'
                },
             'supplier':
              {
                  'get-suppliers': baseUrl + '/supplier/get-suppliers',
                  'get-supplier-sites': baseUrl + '/supplier/get-supplier-sites'
              },
            'sys-role':
                {
                    'get-roles':baseUrl+'/sys-role/get-roles',
                    'add-role':baseUrl+'/sys-role/add-role',
                    'update-role':baseUrl+'/sys-role/update-role',
                    'logout':baseUrl+'/sys-role/logout',
                    'enable':baseUrl+'/sys-role/enable',
                    'assign-menu':baseUrl+'/sys-role/assign-menu'
                },
            'ou':
                {
                    'get-ous':baseUrl+'/ou/get-ous',
                    'get-ou-trees': baseUrl + '/ou/get-outrees'
                },
            'sys-menu':
                {
                    'get-menu-trees':baseUrl+'/sys-menu/get-menu-trees',
                    'get-menus':baseUrl+'/sys-menu/get-menus'
                },
            'employee':
                {
                    'get-employees':baseUrl+'/employee/get-employees',
                    'get-roles':baseUrl+'/employee/get-roles',
                    'logout':baseUrl+'/employee/logout',
                    'enable':baseUrl+'/employee/enable',
                    'assign-role':baseUrl+'/employee/assign-role',
                    'login':baseUrl+'/employee/login'
                },
              'material-type':
                  {
                      'get-material-type-tree':baseUrl+'/material-type/get-material-type-trees',
                      'get-material-types':baseUrl+'/material-type/get-material-types',
                      'material-type-chart':baseUrl+'/material-type/material-type-chart'
                  },
              'material-dic':
                  {
                      'get-material-dics':baseUrl+'/material-dic/get-material-dics'
                  },
              'location':
                  {
                      'get-locations':baseUrl+'/location/get-locations',
                      'add-location':baseUrl+'/location/add-location',
                      'update-location':baseUrl+'/location/update-location',
                      'build-location':baseUrl+'/location/build-location',
                      'disable':baseUrl+'/location/disable',
                      'enable':baseUrl+'/location/enable',
                      'clear':baseUrl+'/location/clear',
                      'get-max-floor-item-col':baseUrl+'/location/get-max-floor-item-col'
                  },
               'warehouse-material':
                   {
                       'get-materials':baseUrl+"/warehouse-material/get-materials"
                   },
                'warehouse-tray':
                    {
                        'get-trays':baseUrl+"/warehouse-tray/get-trays"
                    },
                'in-out-task':
                  {
                      'get-in-out-tasks':baseUrl+"/in-out-task/get-in-out-tasks"
                  },
                 'phy-warehouse':
                     {
                         'get-phy-warehouses':baseUrl+"/phy-warehouse/get-phy-warehouses",
                         'get-phy-warehouse-trees':baseUrl+"/phy-warehouse/get-phy-warehouse-trees"
                     },
                'order':
                    {
                        'get-orders':baseUrl+"/order/get-orders",
                        'get-order-rows':baseUrl+"/order/get-order-rows",
                        'get-tkorder-materials':baseUrl+"/order/get-tkorder-materials",
                        'create-out-order':baseUrl+"/order/create-out-order",
                        'close-order':baseUrl+"/order/close-order",
                        'close-order-row':baseUrl+"/order/close-order-row"
                    },
                'sub-order':
                    {
                        'get-orders':baseUrl+"/sub-order/get-orders",
                        'get-order-rows':baseUrl+"/sub-order/get-order-rows"
                    },
                  'log-record':
                      {
                          'get-log-records':baseUrl+"/log-record/get-log-records"
                      },
                    'sys-config':
                      {
                          'update-config': baseUrl + "/sys-config/update-config"
                      },
                    'ebsproject':
                      {
                          'get-projects': baseUrl + "/ebsproject/get-projects"
                      },
                    'ebstask':
                      {
                          'get-tasks': baseUrl + "/ebstask/get-tasks"
                      }



    };
