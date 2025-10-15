using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Contacless.Model
{
    public class Scan
    {
        public int id { get;  set; }       // Unique Record Identifier
        public string FilePath { get; set; }        // Full path to Scan .DAT file
        public string SerialNo { get; set; }        // Blade Id
        public string Batch { get; set; }           // Batch number for this scan.
        public DateTime Created { get; set; }       // Date of scan
        public string Resource { get; set; }        // Tank asset number
        public int TankNumber { get; set; }         // Tank number
        public string Inspector { get; set; }       // inspector user-id but nobody logs in!
        public bool Result { get; set; }            // PASS/FAIL

        public DateTime? DeleteDate { get; set; }    // NULL if the rows are still in the cache

        public ScanRow[] InsideForm { get; set; }
        public ScanRow[] OutsideForm { get; set; }

        public int Profile_id { get; set; }

    }
}
