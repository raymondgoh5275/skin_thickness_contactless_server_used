
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System;
namespace Contacless.Model
{
    public class ScanPoint
    {
        public int id { get; set; }		// AutoNumber Unique Record Identifier
        public int ScanRow_id { get; set; }		// int Database foreign key to scan record
        public decimal X { get; set; }			// float
        public decimal Y { get; set; }			// float

        public decimal Thickness { get; set; }	// float Thickness

        public int? TrafficLight { get; set; }	// annullable int traffic light position

    }
}
