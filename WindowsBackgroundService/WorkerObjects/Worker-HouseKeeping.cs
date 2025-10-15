using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BLL;

namespace WindowsBackgroundService.WorkerObjects
{
    class Worker_HouseKeeping : Worker, I_Worker
    {

        public Worker_HouseKeeping(Guid id) : base(id) { }

        public override void ExecuteTask()
        {
            // Execute until told to stop
            while (ServiceStarted)
            {
                // ignore the time, just compare the date
                if (_lastRunTime.Date < DateTime.Now.Date)
                {
                    BizLogic.HouseKeeping();
                    
                    _lastRunTime = DateTime.Now;
                }

                // yield 
                if (ServiceStarted)
                    Thread.Sleep(new TimeSpan(0, 1, 0));
            }

            Thread.CurrentThread.Abort();
        }
    }
}
