using System;
using System.Diagnostics;

namespace WindowsBackgroundService.WorkerObjects
{
    public abstract class Worker : I_Worker
    {
        public bool ServiceStarted { get; set; }

        //protected EventLog _serviceEventLog;
        protected Guid _id;

        protected double _interval = 3600;
        protected DateTime _lastRunTime = DateTime.UtcNow;

        //public Worker(Guid id, EventLog ReportingService)
        public Worker(Guid id)
        {
            //_serviceEventLog = ReportingService;
            _id = id;
        }

        public abstract void ExecuteTask();

    }
}
