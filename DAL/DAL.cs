using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DAL.Properties;
using System.Data;

namespace DAL
{
    public class DAL
    {
        public static string strConn = Settings.Default.DB_Connection;
//System.Configuration.ConfigurationManager.AppSettings["DB_Connection"];
//Settings.Default.DB_Connection;

        public string TestConnection()
        {
            using (SqlConnection myConnection = new SqlConnection(DAL.strConn))
            {
                try
                {
                    //
                    // Open the SqlConnection.
                    //
                    myConnection.Open();
                    myConnection.Close();                    
                }
                catch (SqlException SQLex)
                {
                    return SQLex.Message;
                }
            }

            return "Working.";
        }

        public static SPC_Data SPC_Thickness(int profileId, BladeForm Form, float intX, float intY, string SampleSize, DateTime DateStart, DateTime DateStop) 
        {
            SPC_Data returnValue = new SPC_Data();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("SPC_Thickness", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = profileId;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)Form;

                cmd.Parameters.Add("@x", SqlDbType.Float);
                cmd.Parameters["@x"].Value = (float)intX;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = (float)intY;

                cmd.Parameters.Add("@sample_size", SqlDbType.VarChar, 10);
                cmd.Parameters["@sample_size"].Value = SampleSize.ToString();

                cmd.Parameters.Add("@date_start", SqlDbType.DateTime);
                cmd.Parameters["@date_start"].Value = DateStart;

                cmd.Parameters.Add("@date_stop", SqlDbType.DateTime);
                cmd.Parameters["@date_stop"].Value = DateStop;

                cmd.CommandTimeout = 600;
                // execute the command
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    returnValue.SetTolerances((double)rdr["MaxT"], (double)rdr["MinT"], (double)rdr["NomT"]);
                    returnValue.AddValue(rdr["name"].ToString(), (double)rdr["thickness"]);
                    // iterate through results
                    while (rdr.Read())
                    {
                        returnValue.AddValue(rdr["name"].ToString(), (double)rdr["thickness"]);
                    }
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

            return returnValue;
        }

        public static SPC_Data SPC_Delta(int profileId, BladeForm Form, float intY, string SampleSize, DateTime DateStart, DateTime DateStop) 
        {
            SPC_Data returnValue = new SPC_Data();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("SPC_Delta", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = profileId;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)Form;

                cmd.Parameters.Add("@y", SqlDbType.Float);
                cmd.Parameters["@y"].Value = (float)intY;

                cmd.Parameters.Add("@sample_size", SqlDbType.VarChar, 10);
                cmd.Parameters["@sample_size"].Value = SampleSize.ToString();

                cmd.Parameters.Add("@date_start", SqlDbType.DateTime);
                cmd.Parameters["@date_start"].Value = DateStart;

                cmd.Parameters.Add("@date_stop", SqlDbType.DateTime);
                cmd.Parameters["@date_stop"].Value = DateStop;

                cmd.CommandTimeout = 600;
                // execute the command
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    returnValue.SetTolerances((double)rdr["MaxT"], (double)rdr["MinT"], (double)rdr["NomT"]);
                    returnValue.AddValue(rdr["name"].ToString(), (double.Parse(rdr["DeltaX"].ToString()) / 0.5));
                    // iterate through results
                    while (rdr.Read())
                    {
                        returnValue.AddValue(rdr["name"].ToString(), (double.Parse(rdr["DeltaX"].ToString()) / 0.5));
                    }
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

            return returnValue;
        }

        public static void HouseKeeping()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("HouseKeeping", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = 60000;

                // execute the command
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
    }
}
