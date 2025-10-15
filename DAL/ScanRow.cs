using System.Data.SqlClient;
using DAL.Properties;
using System.Data;
using System.Collections.Generic;
using System;

namespace DAL
{
    public class ScanRow
    {
        public int id { get; private set; }			// DB Auto number
        public int Scan_id { get; set; }			// Database foreign key to profile record

        public BladeForm Form { get; set; }               // 0=inside - 180=outside

        public float Y { get; set; }				// section 420	Y cord
        public float DeltaX { get; set; }           // Amount to shift X for row to match tolerances

        public ScanPoint[] Points { get; set; }

        public ScanRow()
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

                SqlCommand cmd = new SqlCommand("scan_row_create", conn);

                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.Add("@scan_id", SqlDbType.Int);
                cmd.Parameters["@scan_id"].Value = this.Scan_id;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = this.Form;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = this.Y;

                cmd.Parameters.Add("@DeltaX", SqlDbType.Int);
                cmd.Parameters["@DeltaX"].Value = this.DeltaX;

                cmd.Parameters.Add("@OutVar", SqlDbType.Int);
                cmd.Parameters["@OutVar"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.id = (int)cmd.Parameters["@OutVar"].Value;

                for (int i = 0; i < this.Points.Length; i++)
                {
                    this.Points[i].ScanRow_id = this.id;
                    this.Points[i].Save();
                }
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

                SqlCommand cmd = new SqlCommand("scan_row_update", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                cmd.Parameters.Add("@scan_id", SqlDbType.Int);
                cmd.Parameters["@scan_id"].Value = this.Scan_id;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = this.Form;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = this.Y;

                cmd.Parameters.Add("@DeltaX", SqlDbType.Int);
                cmd.Parameters["@DeltaX"].Value = this.DeltaX;

                cmd.ExecuteNonQuery();

                for (int i = 0; i < this.Points.Length; i++)
                {
                    this.Points[i].ScanRow_id = this.id;
                    this.Points[i].Save();
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        internal static ScanRow[] LoadFrom_ScanID(BladeForm bladeForm, int ScanID)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;
            List<ScanRow> ReturnData = new List<ScanRow>();

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_row_select_from_scan_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@scan_id", SqlDbType.Int);
                cmd.Parameters["@scan_id"].Value = ScanID;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)bladeForm;

                cmd.CommandTimeout = 300;
                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ScanRow newScanRow = new ScanRow();

                    newScanRow.id = Convert.ToInt32(rdr["id"]);
                    newScanRow.Scan_id = Convert.ToInt32(rdr["scan_id"]);
                    newScanRow.Form = (BladeForm)Enum.ToObject(typeof(BladeForm), Convert.ToInt32(rdr["form"]));
                    newScanRow.Y = (float)Convert.ToDouble(rdr["y"]);
                    newScanRow.DeltaX = (float)Convert.ToDouble(rdr["DeltaX"]);

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

            foreach (ScanRow row in ReturnData)
            {
                row.Points = ScanPoint.LoadFrom_ScanRowID(row.id);
            }

            return ReturnData.ToArray();
        }
    }
}
