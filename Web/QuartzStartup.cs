
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IScheduler = Quartz.IScheduler;

namespace Web
{
    public class QuartzStartup
    {
       
         private readonly ILogger<QuartzStartup> _logger;
         private readonly  ISchedulerFactory _schedulerFactory;  
         private readonly IJobFactory _iocJobfactory;
         private  IScheduler _scheduler;
        public QuartzStartup(IJobFactory iocJobfactory, ILogger<QuartzStartup> logger, ISchedulerFactory schedulerFactory)
        {
            this._logger = logger;
            this._schedulerFactory = schedulerFactory;
            this._iocJobfactory = iocJobfactory;
        }
        public async Task Start()                                           
        {      
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = this._iocJobfactory;
            await _scheduler.Start();
            
            var trigger = TriggerBuilder.Create()
                .WithCronSchedule("0/10 * * * * ?")
                .Build();
            var jobDetail = JobBuilder.Create<Web.Jobs.DashboardJob>()
                .WithIdentity("job", "group")
                .Build();
            
            var trigger2 = TriggerBuilder.Create()
                .WithCronSchedule("0/5 * * * * ?")
                .Build();
            var jobDetail2 = JobBuilder.Create<Web.Jobs.RuKuJob>()
                .WithIdentity("job2", "group2")
                .Build();
            
            var trigger3 = TriggerBuilder.Create()
                .WithCronSchedule("0/5 * * * * ?")
                .Build();
            var jobDetail3 = JobBuilder.Create<Web.Jobs.ChuKuKJob>()
                .WithIdentity("job3", "group3")
                .Build();
            
            var trigger4 = TriggerBuilder.Create()
                .WithCronSchedule("0/5 * * * * ?")
                .Build();
            var jobDetail4 = JobBuilder.Create<Web.Jobs.OrderStatusSyncJob>()
                .WithIdentity("job4", "group4")
                .Build();
            
            var trigger5 = TriggerBuilder.Create()
                .WithCronSchedule("0/5 * * * * ?")
                .Build();
            var jobDetail5 = JobBuilder.Create<Web.Jobs.SendWcsTaskJob>()
                .WithIdentity("job5", "group5")
                .Build();
            
            var trigger6 = TriggerBuilder.Create()
                .WithCronSchedule("0/5 * * * * ?")
                .Build();
            var jobDetail6 = JobBuilder.Create<Web.Jobs.StockSyncJob>()
                .WithIdentity("job6", "group6")
                .Build();
            

            await _scheduler.ScheduleJob(jobDetail,trigger);
            await _scheduler.ScheduleJob(jobDetail2,trigger2);
            await _scheduler.ScheduleJob(jobDetail3,trigger3);
            await _scheduler.ScheduleJob(jobDetail4,trigger4);
            await _scheduler.ScheduleJob(jobDetail5,trigger5);
            await _scheduler.ScheduleJob(jobDetail6,trigger6);

            _logger.LogInformation("Quarzt.net 启动成功...");
        }
    }
}
