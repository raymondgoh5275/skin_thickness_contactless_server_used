using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DAL.Properties;

namespace DAL
{
    public class ProfilePoint
    {
        public int id { get; private set; }     // AutoNumber	Unique Record Identifier
        public int Profile_row_id { get; set; } // int	Database foreign key to profile record

        public float X { get; set; }		    // float	 
        public float Y { get; set; }		    // float

        public float Max { get; set; }		    // float Maximum Tolerance
        public float Nom { get; set; }		    // float Nominal Tolerance
        public float Min { get; set; }		    // float Minimum Tolerance

        public int? TL_Number { get; set; }      // The Traffic Light number for this row

        public ProfilePoint()
        {
            id = -1;
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

                SqlCommand cmd = new SqlCommand("profile_point_create", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_row_id", SqlDbType.Int);
                cmd.Parameters["@profile_row_id"].Value = this.Profile_row_id;

                cmd.Parameters.Add("@x", SqlDbType.Float);
                cmd.Parameters["@X"].Value = this.X;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@Y"].Value = this.Y;

                cmd.Parameters.Add("@min", SqlDbType.Float);
                cmd.Parameters["@min"].Value = this.Min;

                cmd.Parameters.Add("@max", SqlDbType.Float);
                cmd.Parameters["@max"].Value = this.Max;

                cmd.Parameters.Add("@nom", SqlDbType.Float);
                cmd.Parameters["@nom"].Value = this.Nom;

                cmd.Parameters.Add("@tl_num", SqlDbType.Int);
                if (this.TL_Number == null)
                {
                    cmd.Parameters["@tl_num"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@tl_num"].Value = this.TL_Number;
                }

                cmd.Parameters.Add("@OutVar", SqlDbType.Int);
                cmd.Parameters["@OutVar"].Direction = ParameterDirection.Output;

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

        private void Update()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_point_update", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                cmd.Parameters.Add("@profile_row_id", SqlDbType.Int);
                cmd.Parameters["@profile_row_id"].Value = this.Profile_row_id;

                cmd.Parameters.Add("@x", SqlDbType.Float);
                cmd.Parameters["@X"].Value = this.X;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@Y"].Value = this.Y;

                cmd.Parameters.Add("@min", SqlDbType.Float);
                cmd.Parameters["@min"].Value = this.Min;

                cmd.Parameters.Add("@max", SqlDbType.Float);
                cmd.Parameters["@max"].Value = this.Max;

                cmd.Parameters.Add("@nom", SqlDbType.Float);
                cmd.Parameters["@nom"].Value = this.Nom;

                cmd.Parameters.Add("@tl_num", SqlDbType.Float);
                if (this.TL_Number == null)
                {
                    cmd.Parameters["@tl_num"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@tl_num"].Value = this.TL_Number;
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

        internal static ProfilePoint[] LoadInsideFrom_Profile_Row_ID(int id)
        {
            return ProfilePoint.LoadFrom_Profile_Row_ID(0, id);
        }

        internal static ProfilePoint[] LoadOutsideFrom_Profile_Row_ID(int id)
        {
            return ProfilePoint.LoadFrom_Profile_Row_ID(180, id);
        }

        internal static ProfilePoint[] LoadFrom_Profile_Row_ID(int Form, int RowId)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;
            List<ProfilePoint> ReturnData = new List<ProfilePoint>();

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_point_select_from_profile_row_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_row_id", SqlDbType.Int);
                cmd.Parameters["@profile_row_id"].Value = RowId;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ProfilePoint newProfilePoint = new ProfilePoint();

                    newProfilePoint.id = Convert.ToInt32(rdr["id"]);
                    newProfilePoint.Profile_row_id = Convert.ToInt32(rdr["profile_row_id"]);
                    newProfilePoint.X = (float)Convert.ToDouble(rdr["x"]);
                    newProfilePoint.Y = (float)Convert.ToDouble(rdr["y"]);
                    newProfilePoint.Min = (float)Convert.ToDouble(rdr["min"]);
                    newProfilePoint.Max = (float)Convert.ToDouble(rdr["max"]);
                    newProfilePoint.Nom = (float)Convert.ToDouble(rdr["nom"]);

                    if (rdr["tl"] == DBNull.Value)
                        newProfilePoint.TL_Number = null;
                    else
                        newProfilePoint.TL_Number = Convert.ToInt32(rdr["tl"]);

                    ReturnData.Add(newProfilePoint);
                }
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }

            return ReturnData.ToArray();
        }
    }
}
