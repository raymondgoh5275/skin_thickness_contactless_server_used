using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Contacless.Model
{
    public enum BladeForm
    {
        Outside = 0,
        Inside = 180,
    }

    public class ProfileRow
    {
        public int id { get; set; }			// DB Auto-number
        public int Profile_id { get; set; }			// Database foreign key to profile record

        public BladeForm Form { get; set; }               // 0=inside - 180=outside

        public decimal Y { get; set; }				// section 420	Y cord
        public decimal le { get; set; }				// axis to le[Haldenby, Ben] Where we expect to see the pocket LE (e.g. 10mm inboard from start of scan) 	-122.873	Distance from centre to leading edge
        public decimal te { get; set; }				// axis to te[Haldenby, Ben] Where we expect to see the pocket TE (e.g. 10mm inboard from end of scan) 	154.1988	Distance from centre to trailing edge
        public decimal ScanStart { get; set; }		// axis to le +15[Haldenby, Ben]  This is LE start of scan 	-132.873	Distance from centre to leading edge Plus 15.
        public decimal ScanStop { get; set; }			// axis to te +5[Haldenby, Ben] This is TE end of scan 	164.1988	Distance from centre to trailing edge Plus 5. 
        public decimal TolerancesStart { get; set; }	// start to le[Haldenby, Ben] Gap from leading edge pocket to start of tolerances 	5	Unknown
        public decimal TolerancesStop { get; set; }	// remainder at te[Haldenby, Ben] LE+TE compared to  	0.0096	Unknown

        public ProfilePoint[] Points { get; set; }

    
    }
}
