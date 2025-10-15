using System;
using System.Data.SqlClient;
using System.Data;
using DAL.Properties;
using System.Collections.Generic;
using System.Globalization;

namespace DAL
{
    public class Scan
    {
        public int id { get; protected set; }       // Unique Record Identifier
        public string FilePath { get; set; }        // Full path to Scan .DAT file
        public string SerialNo { get; set; }        // Blade Id
        public string Batch { get; set; }           // Batch number for this scan.
        public DateTime Created { get; set; }       // Date of scan
        public string Resource { get; set; }        // Tank asset number
        public int TankNumber { get; set; }         // Tank number
        public string Inspector { get; set; }       // inspector user-id but nobody logs in!
        public bool Result { get; set; }            // PASS/FAIL

        public DateTime? DeleteDate { get; set; }    // NULL if the rows are still in the cache

        public ScanRow[] InsideForm { get; set; }
        public ScanRow[] OutsideForm { get; set; }

        public Profile ProfileInfo { get; set; }
        public bool IsCsv { get; set; }
        public Scan()
        {
            this.id = -1;
        }

        public Scan(int id)
        {
            int ProfileID = this.LoadThis(id);

            this.ProfileInfo = new Profile(ProfileID);

            if (this.DeleteDate == null)
            {
                this.InsideForm = ScanRow.LoadFrom_ScanID(BladeForm.Inside, this.id);
                this.OutsideForm = ScanRow.LoadFrom_ScanID(BladeForm.Outside, this.id);
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public Scan(int id, BladeForm form)
        {
            int ProfileID = this.LoadThis(id);

            this.ProfileInfo = new Profile(ProfileID);

            if (this.DeleteDate == null)
            {
                if (form == BladeForm.Inside)
                {
                    this.InsideForm = ScanRow.LoadFrom_ScanID(BladeForm.Inside, this.id);
                }
                else
                {
                    this.OutsideForm = ScanRow.LoadFrom_ScanID(BladeForm.Outside, this.id);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        private int LoadThis(int id)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;

            int ProfileID = -1;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_select_from_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = id;

                cmd.CommandTimeout = 600;
                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    this.id = int.Parse(rdr["id"].ToString());
                    this.FilePath = rdr["filepath"].ToString();
                    this.SerialNo = rdr["SerialNo"].ToString();
                    this.Batch = rdr["Batch"].ToString();
                    this.Created = DateTime.Parse(rdr["created"].ToString());
                    this.Resource = rdr["Resource"].ToString();
                    this.TankNumber = int.Parse(rdr["tank_number"].ToString());
                    this.Inspector = rdr["inspector"].ToString();
                    if (string.Equals(rdr["result"].ToString(), "PASS", StringComparison.OrdinalIgnoreCase))
                    {
                        this.Result = true;
                    }
                    else
                    {
                        this.Result = false;
                    }
                    if (rdr["delete_date"] == DBNull.Value)
                    {
                        this.DeleteDate = null;
                    }
                    else
                    {
                        this.DeleteDate = DateTime.Parse(rdr["delete_date"].ToString());
                    }

                    ProfileID = int.Parse(rdr["profile_id"].ToString());
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
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

            return ProfileID;
        }

        public void Save()
        {
            if (IsCsv) { return; }
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
                // TODO : Ensure the Profile has been Saved.

                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_create", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@filepath", SqlDbType.VarChar, 255);
                cmd.Parameters["@filepath"].Value = this.FilePath;

                cmd.Parameters.Add("@SerialNo", SqlDbType.VarChar, 128);
                cmd.Parameters["@SerialNo"].Value = this.SerialNo;

                cmd.Parameters.Add("@Batch", SqlDbType.VarChar, 128);
                cmd.Parameters["@Batch"].Value = this.Batch;

                cmd.Parameters.Add("@created", SqlDbType.DateTime);
                cmd.Parameters["@created"].Value = this.Created;

                cmd.Parameters.Add("@Resource", SqlDbType.VarChar, 128);
                cmd.Parameters["@Resource"].Value = this.Resource;

                cmd.Parameters.Add("@tank_number", SqlDbType.Int);
                cmd.Parameters["@tank_number"].Value = this.TankNumber;

                cmd.Parameters.Add("@inspector", SqlDbType.VarChar, 255);
                cmd.Parameters["@inspector"].Value = this.Inspector;

                cmd.Parameters.Add("@result", SqlDbType.VarChar, 5);
                cmd.Parameters["@result"].Value = (this.Result) ? "PASS" : "FAIL";
                
                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = this.ProfileInfo.id;

                cmd.Parameters.Add("@OutVar", SqlDbType.Int);
                cmd.Parameters["@OutVar"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.id = (int)cmd.Parameters["@OutVar"].Value;

                for (int i = 0; i < this.InsideForm.Length; i++)
                {
                    this.InsideForm[i].Scan_id = this.id;
                    this.InsideForm[i].Save();
                }
                for (int i = 0; i < this.OutsideForm.Length; i++)
                {
                    this.OutsideForm[i].Scan_id = this.id;
                    this.OutsideForm[i].Save();
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

                SqlCommand cmd = new SqlCommand("scan_update", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                cmd.Parameters.Add("@filepath", SqlDbType.VarChar, 255);
                cmd.Parameters["@filepath"].Value = this.FilePath;

                cmd.Parameters.Add("@SerialNo", SqlDbType.VarChar, 128);
                cmd.Parameters["@SerialNo"].Value = this.SerialNo;

                cmd.Parameters.Add("@Batch", SqlDbType.VarChar, 128);
                cmd.Parameters["@Batch"].Value = this.Batch;

                cmd.Parameters.Add("@created", SqlDbType.DateTime);
                cmd.Parameters["@created"].Value = this.Created;

                cmd.Parameters.Add("@Resource", SqlDbType.Int);
                cmd.Parameters["@Resource"].Value = this.Resource;

                cmd.Parameters.Add("@tank_number", SqlDbType.Int);
                cmd.Parameters["@tank_number"].Value = this.TankNumber;

                cmd.Parameters.Add("@inspector", SqlDbType.VarChar, 255);
                cmd.Parameters["@inspector"].Value = this.Inspector;

                cmd.Parameters.Add("@result", SqlDbType.VarChar, 5);
                cmd.Parameters["@result"].Value = (this.Result) ? "PASS" : "FAIL";

                cmd.Parameters.Add("@delete_date", SqlDbType.DateTime);
                cmd.Parameters["@delete_date"].Value = this.DeleteDate;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = this.ProfileInfo.id;

                cmd.ExecuteNonQuery();

                for (int i = 0; i < this.InsideForm.Length; i++)
                {
                    this.InsideForm[i].Scan_id = this.id;
                    this.InsideForm[i].Save();
                }
                for (int i = 0; i < this.OutsideForm.Length; i++)
                {
                    this.OutsideForm[i].Scan_id = this.id;
                    this.OutsideForm[i].Save();
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

        public static Dictionary<string, string> GetList_From_ProfileID(int id)
        {
            Dictionary<string, string> ReturnValue = new Dictionary<string, string>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_select_from_profile_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = id;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    string Lable = string.Format("{0}, {1}({2})", rdr["SerialNo"].ToString(), rdr["created"].ToString(), rdr["result"].ToString());
                    ReturnValue.Add(rdr["id"].ToString(), Lable);
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

            return ReturnValue;
        }

        public static Dictionary<string, string> GetList_From_ProfileID(int id, string BladeList)
        {
            Dictionary<string, string> ReturnValue = new Dictionary<string, string>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_select_from_profile_id_and_Blades", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.Add("@blade_SNs", SqlDbType.VarChar, 1024);
                cmd.Parameters["@blade_SNs"].Value = BladeList;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = id;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    string Lable = string.Format("{0}, {1}({2})", rdr["SerialNo"].ToString(), rdr["created"].ToString(), rdr["result"].ToString());
                    ReturnValue.Add(rdr["id"].ToString(), Lable);
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

            return ReturnValue;
        }

        public static List<string> GetRows_From_ProfileID(int profileId, BladeForm form)
        {
            List<string> ReturnValue = new List<string>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_rows_from_profile_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = profileId;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)form;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ReturnValue.Add(rdr["y"].ToString());
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

            return ReturnValue;
        }

        public static List<string> GetCols_From_ProfileID(int profileId, BladeForm form, int intRow)
        {
            List<string> ReturnValue = new List<string>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_cols_from_profile_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@profile_id", SqlDbType.Int);
                cmd.Parameters["@profile_id"].Value = profileId;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)form;

                cmd.Parameters.Add("@row", SqlDbType.Int);
                cmd.Parameters["@row"].Value = (int)intRow;
                                
                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ReturnValue.Add(rdr["x"].ToString());
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

            return ReturnValue;
        }

        public static List<pointData> ThicknessStatistics(int[] IDlist, BladeForm form)
        {
            List<pointData> ReturnStream = new List<pointData>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            // Added to move down to .NET 3.5
            string[] tmpIDlist = new string[IDlist.Length];
            for (int i = 0; i < IDlist.Length; i++)
            {
                tmpIDlist[i] = IDlist[i].ToString();
            }

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_thickness_statistics", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@blade_ids", SqlDbType.VarChar, 1024);
                cmd.Parameters["@blade_ids"].Value = string.Join(",", tmpIDlist); // IDlist);

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)form;

                cmd.CommandTimeout = 600;
                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results
                while (rdr.Read())
                {
                    pointData temp = new pointData();
                    temp.X = float.Parse(rdr["X"].ToString());
                    temp.Y = float.Parse(rdr["Y"].ToString());
                    temp.Average = float.Parse(rdr["Average"].ToString());
                    if (rdr["StdDev"] == DBNull.Value)
                        temp.STDev = 65535;
                    else
                        temp.STDev = float.Parse(rdr["StdDev"].ToString());
                    temp.Max = float.Parse(rdr["Max"].ToString());
                    temp.Min = float.Parse(rdr["Min"].ToString());
                    temp.MinT = float.Parse(rdr["MinT"].ToString());
                    temp.NomT = float.Parse(rdr["NomT"].ToString());
                    temp.MaxT = float.Parse(rdr["MaxT"].ToString());

                    ReturnStream.Add(temp);
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

            return ReturnStream;
        }

        public static List<pointData> Thickness(int bladeid, BladeForm form)
        {
            List<pointData> ReturnStream = new List<pointData>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("scan_thickness", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@blade_id", SqlDbType.Int);
                cmd.Parameters["@blade_id"].Value = bladeid;

                cmd.Parameters.Add("@form", SqlDbType.Int);
                cmd.Parameters["@form"].Value = (int)form;

                cmd.CommandTimeout = 600;
                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results
                while (rdr.Read())
                {
                    pointData temp = new pointData();
                    temp.X = float.Parse(rdr["X"].ToString());
                    temp.Y = float.Parse(rdr["Y"].ToString());
                    temp.Average = float.Parse(rdr["Thickness"].ToString());
                    temp.MinT = float.Parse(rdr["MinT"].ToString());
                    temp.NomT = float.Parse(rdr["NomT"].ToString());
                    temp.MaxT = float.Parse(rdr["MaxT"].ToString());

                    ReturnStream.Add(temp);
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

            return ReturnStream;
        }
    }
}
