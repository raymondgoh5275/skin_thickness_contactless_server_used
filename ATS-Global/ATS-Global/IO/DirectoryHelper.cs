// -----------------------------------------------------------------------
// <copyright file="DirectoryHelper.cs" company="">
// Copyright (c) 2012 ATS Ltd.
// </copyright>
// -----------------------------------------------------------------------

namespace ATS_Global.IO
{
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DirectoryHelper
    {
        public static void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }
}
