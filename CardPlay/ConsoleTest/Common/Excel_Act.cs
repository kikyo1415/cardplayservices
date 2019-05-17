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
using ConsoleTest;
using PokerCardPlay.Core;

namespace Common
{
    public class Excel_Act
    {
        public string GetJson()
        {
            string result = string.Empty;
            var data = ExecleDs("rzmd.xlsx");
            //name	num	address	tel
            List<RZMD> list = new List<RZMD>();
            int a = 1;
            foreach (DataRow row in data.Tables[0].Rows)
            {
                string name = row["name"].ToString();
                string num = row["num"].ToString();
                string address = row["address"].ToString();
                string tel = row["tel"].ToString();
                string path = string.Format("image{0:D3}.jpg", a++);
                RZMD rz = new RZMD { name = name, address = address, num = num, tel = tel, path = path };
                list.Add(rz);
            }
            RZMD_DATA rz_data = new RZMD_DATA { RZMDS = list };
            result = JSONSerilizer.ToJSON(rz_data);
            return result;
        }


        public DataSet ExecleDs(string SourceFileName)
        {
            //DataTable sourceTable = new DataTable();
            try
            {
                DataSet dataSet = new DataSet();
                string connStr = String.Concat("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= ", SourceFileName, ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'");
                if (Path.GetExtension(SourceFileName).ToLower() == ".xlsx")
                    connStr = String.Concat("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", SourceFileName, ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'");

                using (var conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });//获取excel工作表信息
                    //加载Excel中的数据
                    for (int i = 0; i < dbSchema.Rows.Count; i++)
                    {
                        var sheetName = dbSchema.Rows[i]["TABLE_NAME"].ToString().Trim();
                        if (IsTable(sheetName))
                        {
                            //SheetName为excel中表的名字
                            var sql = String.Concat("select * from [", sheetName, "]");
                            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
                                da.Fill(dataSet, sheetName);
                        }
                    }

                    // 移除不符合标准的表
                    for (int i = 0; i < dataSet.Tables.Count; i++)
                    {
                        if (dataSet.Tables[i].Columns.Count < 2)
                        {
                            dataSet.Tables.Remove(dataSet.Tables[i].TableName);
                            i--;
                        }
                    }
                }

                //sourceTable = dataSet.Tables[0];
                return dataSet;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private bool IsTable(string tableName)
        {
            bool blIsTable = false;
            if (tableName != String.Empty)
            {
                int nIndex = tableName.LastIndexOf("$");
                if (nIndex != -1 && tableName.Length - 1 == nIndex)
                {
                    blIsTable = true;
                }
            }
            return blIsTable;
        }
    }
}
