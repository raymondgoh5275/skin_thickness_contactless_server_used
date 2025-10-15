using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Reflection;
using WindowsBackgroundService.Properties;

namespace WindowsBackgroundService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new SkinThicknessEvaluationService() };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception Ex)
            {
                StreamWriter LogFile = new StreamWriter(Path.Combine(Settings.Default.ErrorsPath, string.Format("{0}_MainApp_CrashReport.txt", Assembly.GetExecutingAssembly().FullName)), true);
                LogFile.WriteLine((DateTime.Now).ToString());
                LogFile.WriteLine("Message:-{0}", Ex.Message.ToString());
                LogFile.WriteLine("Data:-{0}", Ex.Data.ToString());
                LogFile.WriteLine("Source:-{0}", Ex.Source.ToString());
                LogFile.WriteLine("Stack Trace:-{0}{1}", Environment.NewLine, Ex.StackTrace.ToString());
                LogFile.Flush();
                LogFile.Close();
            }
        }
    }
}
