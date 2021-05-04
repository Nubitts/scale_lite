using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace scale_lite
{
    class strucdata
    {
        public string stringexe(int iOption, string sCampos, string sTabla, string sCondicion )
        {
            string sQuery ="";

            switch (iOption)
            {
                case 1:
                    sQuery = "select " + sCampos + " from " + sTabla +  " where cbxBascula = 1";
                    break;
            }

            return sQuery;
        }

        public List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }

        public DataTable ObtainRecords(string sQuery, string sConecta)
        {
            DataTable dtTable = new DataTable();

            MySqlConnection mysql_conn = new MySqlConnection(sConecta);
            mysql_conn.Open();

            MySqlCommand selectCommand = new MySqlCommand(sQuery, mysql_conn);
            MySqlDataReader dataReader = selectCommand.ExecuteReader();
            DataSet ds = new DataSet();
            dtTable.Load(dataReader);

            mysql_conn.Close();

            return dtTable;
        }


        #region Clases_pub
        public class users
        {
            public string id { get; set; }
            public string user { get; set; }
            public string password { get; set; }
            public string nombre_full { get; set; }
        }

        #endregion

    }
}
