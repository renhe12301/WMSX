using System;
using System.Collections.Generic;
using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Logging;
using ApplicationCore.Misc;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Web.Interfaces;
using Web.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System.ServiceModel;
using Quartz.Spi;
using SoapCore;
using Web.Jobs;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz.Impl;
using Quartz;
using Web;
using Web.Hubs;
using Web.WebServices.Interfaces;
using Web.WebServices.Services;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<BaseContext>(c =>
              c.UseMySql(Configuration.GetConnectionString("WMSConnection")).EnableSensitiveDataLogging());
            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<BaseContext>(c =>
              c.UseMySql(Configuration.GetConnectionString("WMSConnection")));
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost:5001");
            }));
            //DI 注入
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IEmployeeService), typeof(EmployeeService));
            services.AddScoped(typeof(IInOutTaskService), typeof(InOutTaskService));
            services.AddScoped(typeof(ILocationService), typeof(LocationService));
            services.AddScoped(typeof(IMaterialDicService), typeof(MaterialDicService));
            services.AddScoped(typeof(IOrganizationService), typeof(OrganizationService));
            services.AddScoped(typeof(IReservoirAreaService), typeof(ReservoirAreaService));
            services.AddScoped(typeof(IRoleService), typeof(RoleService));
            services.AddScoped(typeof(ISupplierService), typeof(SupplierService));
            services.AddScoped(typeof(ISysConfigService), typeof(SysConfigService));
            services.AddScoped(typeof(IMaterialTypeService), typeof(MaterialTypeService));
            services.AddScoped(typeof(IWarehouseService), typeof(WarehouseService));
            services.AddScoped(typeof(IMaterialTypeService), typeof(MaterialTypeService));
            services.AddScoped(typeof(ISysMenuService), typeof(SysMenuService));
            services.AddScoped(typeof(IEBSProjectService), typeof(EBSProjectServicecs));
            services.AddScoped(typeof(IEBSTaskService), typeof(EBSTaskServicecs));
            services.AddScoped(typeof(ILogRecordService), typeof(LogRecordService));
            services.AddScoped(typeof(ISysConfigService), typeof(SysConfigService));
            services.AddScoped(typeof(ISubOrderService), typeof(SubOrderService));

            services.AddScoped(typeof(IOrderViewModelService), typeof(OrderViewModelService));
            services.AddScoped(typeof(IOrderRowViewModelService), typeof(OrderRowViewModelService));
            services.AddScoped(typeof(IEmployeeViewModelService), typeof(EmployeeViewModelService));
            services.AddScoped(typeof(IInOutTaskViewModelService), typeof(InOutTaskViewModelService));
            services.AddScoped(typeof(ILocationViewModelService), typeof(LocationViewModelService));
            services.AddScoped(typeof(IMaterialDicViewModelService), typeof(MaterialDicViewModelService));
            services.AddScoped(typeof(IMaterialTypeViewModelService), typeof(MaterialTypeViewModelService));
            services.AddScoped(typeof(IOrganizationViewModelService), typeof(OrganizationViewModelService));
            services.AddScoped(typeof(IReservoirAreaViewModelService), typeof(ReservoirAreaViewModelService));
            services.AddScoped(typeof(ISupplierViewModelService), typeof(SupplierViewModelService));
            services.AddScoped(typeof(ISysRoleViewModelService), typeof(SysRoleViewModelService));
            services.AddScoped(typeof(IWarehouseMaterialViewModelService), typeof(WarehouseMaterialViewModelService));
            services.AddScoped(typeof(IWarehouseTrayViewModelService), typeof(WarehouseTrayViewModelService));
            services.AddScoped(typeof(IWarehouseViewModelService), typeof(WarehouseViewModelService));
            services.AddScoped(typeof(IOUViewModelService), typeof(OUViewModelService));
            services.AddScoped(typeof(ISysMenuViewModelService), typeof(SysMenuViewModelService));
            services.AddScoped(typeof(IPhyWarehouseViewModelService), typeof(PhyWarehouseViewModelService));
            services.AddScoped(typeof(IEBSProjectViewModelService), typeof(EBSProjectViewModelService));
            services.AddScoped(typeof(IEBSTaskViewModelService), typeof(EBSTaskViewModelService));
            services.AddScoped(typeof(ILogRecordViewModelService), typeof(LogRecordViewModelService));
            services.AddScoped(typeof(ISysConfigViewModelService), typeof(SysConfigViewModelService));
            services.AddScoped(typeof(ISubOrderViewModelService), typeof(SubOrderViewModelService));

            //定时器工厂
            services.AddSingleton<QuartzStartup>();
            services.AddTransient<Web.Jobs.DashboardJob>();     
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<IJobFactory, IOCJobFactory>();

            //soap 服务
            //services.TryAddScoped<WebServices.Interfaces.IOrderSOAPService, WebServices.Services.OrderSOAPService>();
            //services.AddScoped(typeof(WebServices.Interfaces.IOrderSOAPService), typeof(WebServices.Services.OrderSOAPService));
         
            EnginContext.initialize(new GeneralEngine(services.BuildServiceProvider()));

            services.Configure<AppSettings>(Configuration);

            //开启路由地址转换功能
            services.AddRouting(options =>
            {
                options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });
            
            services.AddSoapCore();
            services.TryAddSingleton<OrderSOAPService>();

            services.AddMvc(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(
                         new SlugifyParameterTransformer()));

            });

            //开始试图到控制器映射功能
            services.AddControllersWithViews();
            services.AddControllers();

            services.AddHttpContextAccessor();

            //开启自动生成Web API 页面功能
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WMS API",
                    Description = "郝胜(上海)智能-仓库管理系统Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "任贺",
                        Email = "512354417@qq.com"
                    }

                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //开启显示所有注册服务功能
            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);
                config.Path = "/allservices";
            });
            services.AddSignalR();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseShowAllServicesMiddleware();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseAuthorization();

            app.UseCors();
            
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                MaxBufferPoolSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                OpenTimeout = new TimeSpan(0, 10, 0),
                CloseTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                ReceiveTimeout = new TimeSpan(0, 10, 0)
            };
            
            // WSDL 服务类
            app.UseEndpoints(endpoints => {
                endpoints.UseSoapEndpoint<OrderSOAPService>("/WebService/OrderService.wsdl", new BasicHttpBinding());
            });
            
            //路由url格式转换，统一转换成小写
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller:slugify=Home}/{action:slugify=Index}/{id?}");
                endpoints.MapControllerRoute("areas", "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}");
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS API V1");
            });
            
            app.UseSignalR((routes) =>
            {
                routes.MapHub<DashboardHub>("/hubs/dashboard");
            });
          
            var quartz = app.ApplicationServices.GetRequiredService<QuartzStartup>();
            quartz.Start().Wait();

        }
    }
}
