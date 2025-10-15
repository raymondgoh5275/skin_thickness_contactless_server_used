using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using WindowsBackgroundService.WorkerObjects;

namespace BackgroundServiceTest
{
    class Program
    {
        // array of worker threads
        static List<Thread> workerThreads;

        // the objects that do the actual work
        static List<I_Worker> arrWorkers;

        static void Main(string[] args)
        {
            OnStart(args);
            while (string.IsNullOrEmpty(Console.ReadKey(true).KeyChar.ToString()))
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
            }
            OnStop();
        }

        static void OnStart(string[] args)
        {
            arrWorkers = new List<I_Worker>();
            workerThreads = new List<Thread>();

            // Add Jobs
//            arrWorkers.Add(new Worker_PreProcessing(Guid.NewGuid(), new EventLog()));
            arrWorkers.Add(new Worker_PreProcessing(Guid.NewGuid()));
            
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

        static void OnStop()
        {
            for (int i = 0; i < workerThreads.Count; i++)
            {
                // set flag to stop worker thread
                arrWorkers[i].ServiceStarted = false;

                // give it a little time to finish any pending work
                workerThreads[i].Join(new TimeSpan(0, 2, 0));
            }
        }

    }
}
