using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using RSSNews.Services.BGTasks;
using System.Configuration;

namespace RSSNews.Services
{
    public class JobScheduler
    {
        public static void Start(string sourcesXMLPath)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            CheckSourcesStart(scheduler, sourcesXMLPath);
            CleanUpNewsStart(scheduler);
            AcquireNewsStart(scheduler);
        }

        private static void CheckSourcesStart(IScheduler scheduler, string XMLPath)
        {
            IJobDetail job = JobBuilder.Create<CheckSources>().UsingJobData("XMLPath", XMLPath).Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(Int32.Parse(ConfigurationManager.AppSettings["CheckSourcesInterval"]))
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public static void CleanUpNewsStart(IScheduler scheduler)
        {
            IJobDetail job = JobBuilder.Create<CleanUpNews>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(Int32.Parse(ConfigurationManager.AppSettings["CleanUpInterval"]))
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public static void AcquireNewsStart(IScheduler scheduler)
        {
            IJobDetail job = JobBuilder.Create<AcquireNews>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(Int32.Parse(ConfigurationManager.AppSettings["AcquireNewsInterval"]))
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

    }
}