
using System.Data.SqlClient;
using DAL.Properties;
using System.Data;
using System.Collections.Generic;
using System;
namespace DAL
{
    public class ScanPoint
    {
        public int id { get; private set; }		// AutoNumber Unique Record Identifier
        public int ScanRow_id { get; set; }		// int Database foreign key to scan record
        public float XP { get; set; }
        public float X { get; set; }			// float
        public float Y { get; set; }			// float

        public float Thickness { get; set; }	// float Thickness

        public int? TrafficLight { get; set; }	// annullable int traffic light position

        public ScanPoint()
        {
            this.id = -1;
        }

        public void Save()
        {
            if (this.id == -1)
                this.Insert();
            else
                this.Update();
        }

        internal void Insert()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_point_create", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@scan_row_id", SqlDbType.Int);
                cmd.Parameters["@scan_row_id"].Value = this.ScanRow_id;

                cmd.Parameters.Add("@x", SqlDbType.Float);
                cmd.Parameters["@x"].Value = this.X;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = this.Y;

                cmd.Parameters.Add("@thickness", SqlDbType.Float);
                cmd.Parameters["@thickness"].Value = this.Thickness;

                cmd.Parameters.Add("@OutVar", SqlDbType.Int);
                cmd.Parameters["@OutVar"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@tl_num", SqlDbType.Int);
                if (this.TrafficLight == null)
                {
                    cmd.Parameters["@tl_num"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@tl_num"].Value = this.TrafficLight;
                }

                cmd.ExecuteNonQuery();

                this.id = (int)cmd.Parameters["@OutVar"].Value;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        internal void Update()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_point_update", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                cmd.Parameters.Add("@scan_row_id", SqlDbType.Int);
                cmd.Parameters["@scan_row_id"].Value = this.ScanRow_id;

                cmd.Parameters.Add("@x", SqlDbType.Float);
                cmd.Parameters["@x"].Value = this.X;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = this.Y;

                cmd.Parameters.Add("@thickness", SqlDbType.Float);
                cmd.Parameters["@thickness"].Value = this.Thickness;

                cmd.Parameters.Add("@tl_num", SqlDbType.Int);
                if (this.TrafficLight == null)
                {
                    cmd.Parameters["@tl_num"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@tl_num"].Value = this.TrafficLight;
                }
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        internal static ScanPoint[] LoadFrom_ScanRowID(int ScanRowID)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;
            List<ScanPoint> ReturnData = new List<ScanPoint>();

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_point_select_from_scan_row_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@scan_row_id", SqlDbType.Int);
                cmd.Parameters["@scan_row_id"].Value = ScanRowID;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ScanPoint newScanRow = new ScanPoint();

                    newScanRow.id = Convert.ToInt32(rdr["id"]);
		            newScanRow.ScanRow_id = Convert.ToInt32(rdr["scan_row_id"]);
		            newScanRow.X = (float)Convert.ToDouble(rdr["x"]);
                    newScanRow.Y = (float)Convert.ToDouble(rdr["y"]);
                    newScanRow.Thickness = (float)Convert.ToDouble(rdr["thickness"]);

                    if (rdr["tl"] == DBNull.Value)
                        newScanRow.TrafficLight = null;
                    else
                        newScanRow.TrafficLight = Convert.ToInt32(rdr["tl"]);

                    ReturnData.Add(newScanRow);
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (rdr != null)
                {
                    rdr.Close();
                }
            }

            return ReturnData.ToArray();
        }
    }
}
