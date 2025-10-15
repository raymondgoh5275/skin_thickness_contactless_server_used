using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System;

namespace Contacless.Model
{
    public class ScanRow
    {
        public int id { get; set; }			// DB Auto number
        public int Scan_id { get; set; }			// Database foreign key to profile record

        public BladeForm Form { get; set; }               // 0=inside - 180=outside

        public decimal Y { get; set; }				// section 420	Y cord
        public decimal DeltaX { get; set; }           // Amount to shift X for row to match tolerances

        public ScanPoint[] Points { get; set; }

    }
}
