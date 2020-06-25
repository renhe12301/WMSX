const baseUrl = 'http://localhost:8089/api';
const controllers =
    {
            'organization':
                {
                        'get-organizations': baseUrl + "/organization/get-organizations"
                },
            'warehouse':
                {
                         'get-warehouses': baseUrl + '/warehouse/get-warehouses'
                },
            'reservoir-area':
                {
                        'get-areas': baseUrl + '/reservoir-area/get-areas',
                        'assign-location': baseUrl + '/reservoir-area/assign-location'
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
                      'get-material-types':baseUrl+'/material-type/get-material-types'
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
                      'get-in-out-tasks':baseUrl+"/in-out-task/get-in-out-tasks",
                      'send-wcs':baseUrl+"/in-out-task/send-wcs"
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
                      },
                      'statistical': 
                          {
                              'material-chart': baseUrl + "/statistical/material-chart",
                              'in-record-chart': baseUrl + "/statistical/in-record-chart",
                              'out-record-chart': baseUrl + "/statistical/out-record-chart",
                              'in-order-chart': baseUrl + "/statistical/in-order-chart",
                              'out-order-chart': baseUrl + "/statistical/out-order-chart",
                              'in-sub-order-chart': baseUrl + "/statistical/in-sub-order-chart",
                              'out-sub-order-chart': baseUrl + "/statistical/out-sub-order-chart",
                              'material-sheet': baseUrl + "/statistical/material-sheet",
                              'in-record-sheet': baseUrl + "/statistical/in-record-sheet",
                              'out-record-sheet': baseUrl + "/statistical/out-record-sheet",
                              'in-order-sheet': baseUrl + "/statistical/in-order-sheet",
                              'out-order-sheet': baseUrl + "/statistical/out-order-sheet",
                              'in-sub-order-sheet': baseUrl + "/statistical/in-sub-order-sheet",
                              'out-sub-order-sheet': baseUrl + "/statistical/out-sub-order-sheet",
                              'py-warehouse-chart': baseUrl + "/statistical/py-warehouse-chart"
                          }



    };
