using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Web;
using PokerCardPlay.Core;

namespace Common
{
    public class Excel_Act
    {

        public string GetJson()
        {
            string result = string.Empty;
            var data = ExecleDs("rzmd.xlsx", "");
            //name	num	address	tel
            List<RZMD> list = new List<RZMD>();
            foreach (DataRow row in data.Tables[0].Rows)
            {
                string name = row["name"].ToString();
                string num = row["num"].ToString();
                string address = row["address"].ToString();
                string tel = row["tel"].ToString();
                RZMD rz = new RZMD { name = name, address = address, num = num, tel = tel };
                list.Add(rz);
            }
            RZMD_DATA rz_data = new RZMD_DATA { RZMDS = list };
            result = JSONSerilizer.ToJSON(rz_data);
            return result;
        }


        public DataSet ExecleDs(string filenameurl, string table)
        {
            string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + filenameurl + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();//打开连接
            //建立数据连接
            try
            {
                OleDbDataAdapter odda = new OleDbDataAdapter("select * from [Sheet1$]", conn);
                DataSet ds = new DataSet();
                odda.Fill(ds, "[Sheet1$]");

                odda.Dispose();
                conn.Close();
                conn.Dispose();
                return ds;
            }
            catch (Exception e)
            {
                conn.Close();
                return null;
            }
        }

    }
    public class RZMD_DATA
    {
        public List<RZMD> RZMDS { get; set; }
    }

    [Serializable]
    public class RZMD
    {
        public string name { get; set; }

        public string num { get; set; }

        public string address { get; set; }

        public string tel { get; set; }
    }
}
