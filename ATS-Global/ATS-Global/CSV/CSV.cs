// -----------------------------------------------------------------------
// <copyright file="CSV.cs" company="">
// Copyright (c) 2012 ATS Ltd.
// </copyright>
// -----------------------------------------------------------------------

namespace ATS_Global.CSV
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    static public class CsvHelper
    {
        static public List<string[]> Load(string path)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                int MaxCols = 0;
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');

                        if (MaxCols < row.Length)
                            MaxCols = row.Length;

                        parsedData.Add(row);
                    }
                }

                for (int i = 0; i < parsedData.Count; i++)
                {
                    string[] row = parsedData[i];

                    if (row.Length < MaxCols)
                    {
                        Array.Resize<string>(ref row, MaxCols);

                        parsedData[i] = row;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                throw new Exception(string.Format("{0}, CSV File {1} not found.", DateTime.Now, e.FileName));
            }

            return parsedData;
        }

        public static void Save(string path, List<string[]> CSVdata)
        {
            using (StreamWriter writeFile = new StreamWriter(path))
            {
                foreach (string[] line in CSVdata)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (i > 0)
                            writeFile.Write(",");

                        writeFile.Write(line[i]);
                    }
                    writeFile.WriteLine("");
                }
            }
        }
    }
}
