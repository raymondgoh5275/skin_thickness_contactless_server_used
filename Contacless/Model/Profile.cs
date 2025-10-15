using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Contacless.Model
{
    public class Profile
    {
        public int id { get; set; }			//	Unique Record Identifier
        public string Name { get; set; }			//	Name of the Profile
        public string Pro_Filename { get; set; }	//	Pointer to the original file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85735 trent xwb v1.2.pro
        public string Omin_Filename { get; set; }	//	Pointer to the csv file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85737 omin v1.2.csv
        public string Onom_Filename { get; set; }	//	Pointer to the csv file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85736 onom v1.2.csv
        public string Omax_Filename { get; set; }	//	Pointer to the csv file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85738 omax v1.2.csv
        public string Imin_Filename { get; set; }	//	Pointer to the csv file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85740 imin v1.2.csv
        public string Inom_Filename { get; set; }	//	Pointer to the csv file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85739 inom v1.2.csv
        public string Imax_Filename { get; set; }	//	Pointer to the csv file location Z:\skin thickness\Trent XWB\profile 01k85735 v1.2\01k85741 imax v1.2.csv

        public ProfileRow[] InsideForm { get; set; }
        public ProfileRow[] OutsideForm { get; set; }
    
    }
}
