using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DAL.Properties;

namespace DAL
{
    public enum BladeForm
    {
        Outside = 0,
        Inside = 180,
    }

    public class ProfileRow
    {
        public int id { get; private set; }			// DB Auto-number
        public int Profile_id { get; set; }			// Database foreign key to profile record

        public BladeForm Form { get; set; }               // 0=inside - 180=outside

        public float Y { get; set; }				// section 420	Y cord
        public float le { get; set; }				// axis to le[Haldenby, Ben] Where we expect to see the pocket LE (e.g. 10mm inboard from start of scan) 	-122.873	Distance from centre to leading edge
        public float te { get; set; }				// axis to te[Haldenby, Ben] Where we expect to see the pocket TE (e.g. 10mm inboard from end of scan) 	154.1988	Distance from centre to trailing edge
        public float ScanStart { get; set; }		// axis to le +15[Haldenby, Ben]  This is LE start of scan 	-132.873	Distance from centre to leading edge Plus 15.
        public float ScanStop { get; set; }			// axis to te +5[Haldenby, Ben] This is TE end of scan 	164.1988	Distance from centre to trailing edge Plus 5. 
        public float TolerancesStart { get; set; }	// start to le[Haldenby, Ben] Gap from leading edge pocket to start of tolerances 	5	Unknown
        public float TolerancesStop { get; set; }	// remainder at te[Haldenby, Ben] LE+TE compared to  	0.0096	Unknown

        public ProfilePoint[] Points { get; set; }

        public ProfileRow()
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

        private void Insert()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_row_create", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = this.Profile_id;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = this.Form;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = this.Y;

                cmd.Parameters.Add("@le", SqlDbType.Float);
                cmd.Parameters["@le"].Value = this.le;

                cmd.Parameters.Add("@te", SqlDbType.Float);
                cmd.Parameters["@te"].Value = this.te;

                cmd.Parameters.Add("@scan_start", SqlDbType.Float);
                cmd.Parameters["@scan_start"].Value = this.ScanStart;

                cmd.Parameters.Add("@scan_stop", SqlDbType.Float);
                cmd.Parameters["@scan_stop"].Value = this.ScanStop;

                cmd.Parameters.Add("@tolerances_start", SqlDbType.Float);
                cmd.Parameters["@tolerances_start"].Value = this.TolerancesStart;

                cmd.Parameters.Add("@tolerances_stop", SqlDbType.Float);
                cmd.Parameters["@tolerances_stop"].Value = this.TolerancesStop;

                cmd.Parameters.Add("@OutVar", SqlDbType.Int);
                cmd.Parameters["@OutVar"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.id = (int)cmd.Parameters["@OutVar"].Value;

                for (int i = 0; i < this.Points.Length; i++)
                {
                    this.Points[i].Profile_row_id = this.id;
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

        private void Update()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_row_update", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = this.Profile_id;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = this.Form;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = this.Y;

                cmd.Parameters.Add("@le", SqlDbType.Float);
                cmd.Parameters["@le"].Value = this.le;

                cmd.Parameters.Add("@te", SqlDbType.Float);
                cmd.Parameters["@te"].Value = this.te;

                cmd.Parameters.Add("@scan_start", SqlDbType.Float);
                cmd.Parameters["@scan_start"].Value = this.ScanStart;

                cmd.Parameters.Add("@scan_stop", SqlDbType.Float);
                cmd.Parameters["@scan_stop"].Value = this.ScanStop;

                cmd.Parameters.Add("@tolerances_start", SqlDbType.Float);
                cmd.Parameters["@tolerances_start"].Value = this.TolerancesStart;

                cmd.Parameters.Add("@tolerances_stop", SqlDbType.Float);
                cmd.Parameters["@tolerances_stop"].Value = this.TolerancesStop;

                cmd.ExecuteNonQuery();

                for (int i = 0; i < this.Points.Length; i++)
                {
                    this.Points[i].Profile_row_id = this.id;
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

        internal static ProfileRow[] LoadFrom_ProfileID(int Profile_id, BladeForm Form)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;
            List<ProfileRow> ReturnData = new List<ProfileRow>();

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_row_select_from_profile_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = Profile_id;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)Form;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ProfileRow newProfileRow = new ProfileRow();

                    newProfileRow.id = Convert.ToInt32(rdr["id"]);
                    newProfileRow.Profile_id = Convert.ToInt32(rdr["profile_id"]);
                    newProfileRow.Y = (float)Convert.ToDouble(rdr["y"]);
                    newProfileRow.le = (float)Convert.ToDouble(rdr["le"]);
                    newProfileRow.te = (float)Convert.ToDouble(rdr["te"]);
                    newProfileRow.ScanStart = (float)Convert.ToDouble(rdr["scan_start"]);
                    newProfileRow.ScanStop = (float)Convert.ToDouble(rdr["scan_stop"]);
                    newProfileRow.TolerancesStart = (float)Convert.ToDouble(rdr["tolerances_start"]);
                    newProfileRow.TolerancesStop = (float)Convert.ToDouble(rdr["tolerances_stop"]);

                    newProfileRow.Points = ProfilePoint.LoadInsideFrom_Profile_Row_ID(newProfileRow.id);

                    ReturnData.Add(newProfileRow);
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

        internal static ProfileRow[] LoadFrom_ProfileID_TL(int Profile_id, BladeForm Form)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;
            List<ProfileRow> ReturnData = new List<ProfileRow>();

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_row_select_from_profile_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = Profile_id;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)Form;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ProfileRow newProfileRow = new ProfileRow();

                    newProfileRow.id = Convert.ToInt32(rdr["id"]);
                    newProfileRow.Profile_id = Convert.ToInt32(rdr["profile_id"]);
                    newProfileRow.Y = (float)Convert.ToDouble(rdr["y"]);
                    newProfileRow.le = (float)Convert.ToDouble(rdr["le"]);
                    newProfileRow.te = (float)Convert.ToDouble(rdr["te"]);
                    newProfileRow.ScanStart = (float)Convert.ToDouble(rdr["scan_start"]);
                    newProfileRow.ScanStop = (float)Convert.ToDouble(rdr["scan_stop"]);
                    newProfileRow.TolerancesStart = (float)Convert.ToDouble(rdr["tolerances_start"]);
                    newProfileRow.TolerancesStop = (float)Convert.ToDouble(rdr["tolerances_stop"]);

                    newProfileRow.Points = ProfilePoint.LoadInsideFrom_Profile_Row_ID(newProfileRow.id);

                    ReturnData.Add(newProfileRow);
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
