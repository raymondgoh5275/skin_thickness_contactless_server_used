using System;
using System.Data;
using System.Data.SqlClient;
using DAL.Properties;
using System.Collections.Generic;

namespace DAL
{
    public class Profile
    {
        public int id { get; private set; }			//	Unique Record Identifier
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

        public Profile()
        {
            this.id = -1;
        }

        public Profile(int p)
        {
            if (p > 0)
            {
                this.LoadThis(p);

                this.InsideForm = ProfileRow.LoadFrom_ProfileID(this.id, BladeForm.Inside);
                this.OutsideForm = ProfileRow.LoadFrom_ProfileID(this.id, BladeForm.Outside);
            }
        }

        public Profile(int p, BladeForm form)
        {
            if (p > 0)
            {
                this.LoadThis(p);

                if (form == BladeForm.Inside)
                {
                    this.InsideForm = ProfileRow.LoadFrom_ProfileID(this.id, BladeForm.Inside);
                }
                else
                {
                    this.OutsideForm = ProfileRow.LoadFrom_ProfileID(this.id, BladeForm.Outside);
                }
            }
        }

        private void LoadThis(int id)
        {
            this.id = id;
            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_select_from_id", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    this.id = Convert.ToInt32(rdr["id"]);
                    this.Name = rdr["name"].ToString();
                    this.Pro_Filename = rdr["pro_filename"].ToString();
                    this.Omin_Filename = rdr["omin_filename"].ToString();
                    this.Omax_Filename = rdr["omax_filename"].ToString();
                    this.Onom_Filename = rdr["onom_filename"].ToString();
                    this.Imin_Filename = rdr["imin_filename"].ToString();
                    this.Imax_Filename = rdr["imax_filename"].ToString();
                    this.Inom_Filename = rdr["inom_filename"].ToString();
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

                SqlCommand cmd = new SqlCommand("profile_create", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmd.Parameters["@name"].Value = this.Name;

                cmd.Parameters.Add("@pro_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@pro_filename"].Value = this.Pro_Filename;

                cmd.Parameters.Add("@omin_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@omin_filename"].Value = this.Omin_Filename;

                cmd.Parameters.Add("@omax_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@omax_filename"].Value = this.Omax_Filename;

                cmd.Parameters.Add("@onom_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@onom_filename"].Value = this.Onom_Filename;

                cmd.Parameters.Add("@imin_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@imin_filename"].Value = this.Imin_Filename;

                cmd.Parameters.Add("@imax_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@imax_filename"].Value = this.Imax_Filename;

                cmd.Parameters.Add("@inom_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@inom_filename"].Value = this.Inom_Filename;

                cmd.Parameters.Add("@OutVar", SqlDbType.Int);
                cmd.Parameters["@OutVar"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.id = (int)cmd.Parameters["@OutVar"].Value;

                for (int i = 0; i < this.InsideForm.Length; i++)
                {
                    this.InsideForm[i].Profile_id = this.id;
                    this.InsideForm[i].Save();
                }
                for (int i = 0; i < this.OutsideForm.Length; i++)
                {
                    this.OutsideForm[i].Profile_id = this.id;
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

        private void Update()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_update", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = this.id;

                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmd.Parameters["@name"].Value = this.Name;

                cmd.Parameters.Add("@pro_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@pro_filename"].Value = this.Pro_Filename;

                cmd.Parameters.Add("@omin_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@omin_filename"].Value = this.Omin_Filename;

                cmd.Parameters.Add("@omax_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@omax_filename"].Value = this.Omax_Filename;

                cmd.Parameters.Add("@onom_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@onom_filename"].Value = this.Onom_Filename;

                cmd.Parameters.Add("@imin_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@imin_filename"].Value = this.Imin_Filename;

                cmd.Parameters.Add("@imax_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@imax_filename"].Value = this.Imax_Filename;

                cmd.Parameters.Add("@inom_filename", SqlDbType.VarChar, 255);
                cmd.Parameters["@inom_filename"].Value = this.Inom_Filename;

                cmd.ExecuteNonQuery();

                for (int i = 0; i < this.InsideForm.Length; i++)
                {
                    if (this.InsideForm[i].Profile_id > -1)
                        this.InsideForm[i].Profile_id = this.id;

                    this.InsideForm[i].Save();
                }

                for (int i = 0; i < this.OutsideForm.Length; i++)
                {
                    if (this.OutsideForm[i].Profile_id > -1)
                        this.OutsideForm[i].Profile_id = this.id;

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

        public void LoadFromName()
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_select_from_name", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmd.Parameters["@name"].Value = this.Name;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    this.id = Convert.ToInt32(rdr["id"]);
                    this.Name = rdr["name"].ToString();
                    this.Pro_Filename = rdr["pro_filename"].ToString();
                    this.Omin_Filename = rdr["omin_filename"].ToString();
                    this.Omax_Filename = rdr["omax_filename"].ToString();
                    this.Onom_Filename = rdr["onom_filename"].ToString();
                    this.Imin_Filename = rdr["imin_filename"].ToString();
                    this.Imax_Filename = rdr["imax_filename"].ToString();
                    this.Inom_Filename = rdr["inom_filename"].ToString();
                }

                this.InsideForm = ProfileRow.LoadFrom_ProfileID(this.id, BladeForm.Inside);
                this.OutsideForm = ProfileRow.LoadFrom_ProfileID(this.id, BladeForm.Outside);
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
        }

        public static System.Collections.Generic.Dictionary<string, string> Get_ProfileList()
        {
            Dictionary<string, string> ReturnValue = new Dictionary<string, string>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_select_list", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ReturnValue.Add(rdr["name"].ToString(), rdr["id"].ToString());
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

        public static System.Collections.Generic.Dictionary<string, string> Get_ProfileList(string BladeList)
        {
            Dictionary<string, string> ReturnValue = new Dictionary<string, string>();

            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(DAL.strConn);
                conn.Open();

                SqlCommand cmd = new SqlCommand("profile_select_list_from_blades", conn);

                cmd.Parameters.Add("@blade_SNs", SqlDbType.VarChar, 1024);
                cmd.Parameters["@blade_SNs"].Value = BladeList;

                cmd.CommandType = CommandType.StoredProcedure;

                // execute the command
                rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console
                while (rdr.Read())
                {
                    ReturnValue.Add(rdr["name"].ToString(), rdr["id"].ToString());
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
    }
}
