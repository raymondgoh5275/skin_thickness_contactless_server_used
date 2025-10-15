// -----------------------------------------------------------------------
// <copyright file="File_ExtensionMethods.cs" company="">
// Copyright (c) 2012 ATS Ltd.
// </copyright>
// -----------------------------------------------------------------------

namespace ATS_Global.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class FileHelper
    {
        public static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private static bool MoveFile(string src, string Dest)
        {
            DateTime startTime = DateTime.Now;

            if (File.Exists(src))
                while (FileHelper.IsFileLocked(src))
                {
                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);

                    if (span.TotalSeconds > 5)
                        break;
                }
            else
                return false;

            try
            {
                File.Move(src, Dest);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CopyFile(string src, string Dest)
        {
            return FileHelper.CopyFile(src, Dest, false);
        }

        public static bool CopyFile(string src, string Dest, bool overWrite)
        {
            DateTime startTime = DateTime.Now;

            if (File.Exists(src))
                while (FileHelper.IsFileLocked(src))
                {
                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);

                    if (span.TotalSeconds > 5)
                        return false;
                }
            else
                return false;

            try
            {
                File.Copy(src, Dest, overWrite);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
