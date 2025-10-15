using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using WindowsBackgroundService.WorkerObjects;
using System.IO;
using System.Reflection;
using WindowsBackgroundService.Properties;

namespace WindowsBackgroundService
{
    public partial class SkinThicknessEvaluationService : ServiceBase
    {
        const string EventLog_Source = "SkinThicknessService";
        const string EventLog_Log = "SkinThicknessLog";

        // array of worker threads
        List<Thread> workerThreads;

        // the objects that do the actual work
        List<I_Worker> arrWorkers;

        public SkinThicknessEvaluationService()
        {
            InitializeComponent();

            // Create the source, if it does not already exist.
            if (!EventLog.SourceExists(EventLog_Source))
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                //Execute this sample a second time to use the new source.
                EventLog.CreateEventSource(EventLog_Source, EventLog_Log);

                // The source is created.  Exit the application to allow it to be registered.
                return;
            }

            // Create an EventLog instance and assign its source.
            eventLog1.Source = EventLog_Source;
            eventLog1.Log = EventLog_Log;

            // Write an entry to the log.        
            eventLog1.WriteEntry("Writing to event log on " + eventLog1.MachineName);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                eventLog1.WriteEntry("Skin Thickness Service onStart");

                arrWorkers = new List<I_Worker>();
                workerThreads = new List<Thread>();

                // Add Jobs
                arrWorkers.Add(new Worker_PreProcessing(Guid.NewGuid()));
                arrWorkers.Add(new Worker_HouseKeeping(Guid.NewGuid()));

                for (int i = 0; i < arrWorkers.Count; i++)
                {
                    // set properties on the object
                    arrWorkers[i].ServiceStarted = true;

                    // create a thread and attach to the object
                    workerThreads.Add(new Thread(new ThreadStart(arrWorkers[i].ExecuteTask)));
                }

                // start the threads
                for (int i = 0; i < workerThreads.Count; i++)
                {
                    workerThreads[i].Start();
                }
            }
            catch (Exception Ex)
            {
                StreamWriter LogFile = new StreamWriter(Path.Combine(Settings.Default.ErrorsPath, string.Format("{0}_OnStart_CrashReport.txt", Assembly.GetExecutingAssembly().FullName)), true);
                LogFile.WriteLine((DateTime.Now).ToString());
                LogFile.WriteLine("Message:-{0}", Ex.Message.ToString());
                LogFile.WriteLine("Data:-{0}", Ex.Data.ToString());
                LogFile.WriteLine("Source:-{0}", Ex.Source.ToString());
                LogFile.WriteLine("Stack Trace:-{0}{1}", Environment.NewLine, Ex.StackTrace.ToString());
                LogFile.Flush();
                LogFile.Close();
                throw Ex;
            }
        }

        protected override void OnStop()
        {
            try
            {
                eventLog1.WriteEntry("Skin Thickness Service OnStop");

                // Tell all workers they need to stop.
                for (int i = 0; i < workerThreads.Count; i++)
                {
                    // set flag to stop worker thread
                    arrWorkers[i].ServiceStarted = false;
                }
                // Wait for all worker threads to finish.
                for (int i = 0; i < workerThreads.Count; i++)
                {
                    // give it a little time to finish any pending work
                    workerThreads[i].Join(new TimeSpan(0, 2, 0));
                }
            }
            catch (Exception Ex)
            {
                StreamWriter LogFile = new StreamWriter(Path.Combine(Settings.Default.ErrorsPath, string.Format("{0}_OnStop_CrashReport.txt", Assembly.GetExecutingAssembly().FullName)), true);
                LogFile.WriteLine((DateTime.Now).ToString());
                LogFile.WriteLine("Message:-{0}", Ex.Message.ToString());
                LogFile.WriteLine("Data:-{0}", Ex.Data.ToString());
                LogFile.WriteLine("Source:-{0}", Ex.Source.ToString());
                LogFile.WriteLine("Stack Trace:-{0}{1}", Environment.NewLine, Ex.StackTrace.ToString());
                LogFile.Flush();
                LogFile.Close();
                throw Ex;
            }
        }
    }
}
