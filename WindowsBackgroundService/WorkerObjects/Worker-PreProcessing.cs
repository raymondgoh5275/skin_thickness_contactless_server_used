using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Xml;
using WindowsBackgroundService.Properties;
using BLL;
using System.Collections.Generic;
using System.Reflection;

namespace WindowsBackgroundService.WorkerObjects
{
    public class Worker_PreProcessing : Worker, I_Worker
    {
        FileSystemWatcher watcher;
        Queue<string> FilesToProcess = new Queue<string>();

        public Worker_PreProcessing(Guid id)
            : base(id)
        {
            string[] tmpList = Directory.GetFiles(Settings.Default.MESfilePath);

            lock (FilesToProcess)
            {
                foreach (string FilePath in tmpList)
                {
                    // add the path to the new file.
                    FilesToProcess.Enqueue(FilePath);
                }
            }
        }

        public override void ExecuteTask()
        {
            watcher = new FileSystemWatcher(Settings.Default.MESfilePath);
            watcher.Filter = "*.xml";
            watcher.Created += new FileSystemEventHandler(watcher_Created);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Execute until told to stop
            while (ServiceStarted)
            {

                try
                {
                    // while there are Files in the Process Que
                    while ((FilesToProcess.Count > 0) && (ServiceStarted))
                    {
                        string FullPath = string.Empty;
                        lock (FilesToProcess)
                        {
                            FullPath = FilesToProcess.Dequeue();
                        }


                        BizLogic BizObj = new BizLogic();
                        BizObj.ProcessScanResults(FullPath);
                        File.Delete(FullPath);

                        StreamWriter LogFile = new StreamWriter(Path.Combine(Settings.Default.LogsPath, string.Format("{0}_Proccess_Log.txt", DateTime.Now.ToString("yyyy_MMM"))), true);
                        LogFile.WriteLine(string.Format("[{0}] - Processed {1}", (DateTime.Now).ToString(), Path.GetFileName(FullPath)));
                        LogFile.Flush();
                        LogFile.Close();
                    }

                    // yield 
                    if (ServiceStarted)
                        Thread.Sleep(new TimeSpan(0, 0, 15));
                }
                catch (Exception Ex)
                {
                    StreamWriter CrashLogFile = new StreamWriter(Path.Combine(Settings.Default.ErrorsPath, string.Format("{0}_Proccess_CrashReport.txt", Assembly.GetExecutingAssembly().FullName)), true);
                    CrashLogFile.WriteLine((DateTime.Now).ToString());
                    CrashLogFile.WriteLine("Message:-{0}", Ex.Message.ToString());
                    CrashLogFile.WriteLine("Data:-{0}", Ex.Data.ToString());
                    CrashLogFile.WriteLine("Source:-{0}", Ex.Source.ToString());
                    CrashLogFile.WriteLine("Stack Trace:-{0}{1}", Environment.NewLine, Ex.StackTrace.ToString());
                    CrashLogFile.Flush();
                    CrashLogFile.Close();
                }
            }

            Thread.CurrentThread.Abort();
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (FilesToProcess)
            {
                // add the path to the new file.
                FilesToProcess.Enqueue(e.FullPath);
            }
        }
    }
}
