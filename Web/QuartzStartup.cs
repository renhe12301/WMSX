﻿
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
                            .WithCronSchedule("0/5 * * * * ?")
                            .Build();
            var jobDetail = JobBuilder.Create<Web.Jobs.DashboardJob>()
                            .WithIdentity("job", "group")
                            .Build();
            await _scheduler.ScheduleJob(jobDetail, trigger);
            _logger.LogInformation("Quarzt.net 启动成功...");
        }
        public void Stop()
        {
            if (_scheduler == null)
            {
                return;
            }

            if (_scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
                _scheduler = null;
            else
            {
            }
            _logger.LogCritical("Schedule job upload as application stopped");
        }
    }
}