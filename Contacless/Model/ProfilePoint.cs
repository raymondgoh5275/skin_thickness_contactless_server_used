using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Contacless.Model
{
    public class ProfilePoint
    {
        public int id { get; set; }     // AutoNumber	Unique Record Identifier
        public int Profile_row_id { get; set; } // int	Database foreign key to profile record

        public decimal X { get; set; }		    // float	 
        public decimal Y { get; set; }		    // float

        public decimal Max { get; set; }		    // float Maximum Tolerance
        public decimal Nom { get; set; }		    // float Nominal Tolerance
        public decimal Min { get; set; }		    // float Minimum Tolerance

        public int? TL_Number { get; set; }      // The Traffic Light number for this row

    }
}
