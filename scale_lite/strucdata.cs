using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;

namespace scale_lite
{
    class strucdata
    {

        public DataTable Predata(int iOption, string sCampos, string sTabla, string sCondicion, string sConecta)
        {
            DataTable dtTabla = new DataTable();

            switch (iOption)
            {
                case 1:
                    dtTabla = ObtainRecords(stringexe(iOption,sCampos,sTabla,sCondicion), sConecta);
                    break;
            }

            return dtTabla;
        }
        public string stringexe(int iOption, string sCampos, string sTabla, string sCondicion )
        {
            string sQuery ="";

            string sCondiciona = (sCondicion.Trim().Length > 0) ? " where " + sCondicion : "";

            switch (iOption)
            {
                case 1:
                    sQuery = "select " + sCampos + " from " + sTabla + sCondiciona;
                    break;
                case 2:
                    sQuery = "update " + sTabla + " set " + sCampos + " " + sCondiciona;
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

        public int Executecmm(string sQuery, string sConecta)
        {
            int iFlag = 0;

            MySqlConnection mysql_cnn = new MySqlConnection(sConecta);

            try
            {
                mysql_cnn.Open();

                MySqlCommand cmComando = new MySqlCommand(sQuery, mysql_cnn);

                cmComando.ExecuteNonQuery();

                mysql_cnn.Close();

            }
            catch (Exception e)
            {
                XtraMessageBox.Show(e + " Exception caught. ", "Aviso", MessageBoxButtons.OK);
                iFlag = 1;
            }

            return iFlag;
        }

        #region Clases_pub
        public class users
        {
            public string id { get; set; }
            public string user { get; set; }
            public string password { get; set; }
            public string nombre_full { get; set; }
        }

        public class ticketfree
        {
            public int ticket { get; set; }
            public string nombre_p { get; set; }
            public string ordcte { get; set; }
            public string nom_grupo { get; set; }
            public string tabla { get; set; }
            public string ciclo { get; set; }
        }

        public class forwarder
        {
            public string num_fle { get; set; }
            public string nombre { get; set; }
        }

        public class lifting
        {
            public string num_fle { get; set; }
            public string nombre { get; set; }
        }

        public class dhour
        {
            public int hourd { get; set; }
            public int hourt { get; set; }
        }

        public class headertick
        {
            [Display(Name = "TIcket")]
            public int ticket { get; set; }

            [Display(Name = "Fletero")]
            public string numtra { get; set; }

            [Display(Name = "F. Entr")]
            public string fecpen { get; set; }

            [Display(Name = "H. Entr")]
            public string horent { get; set; }

            [Display(Name = "Grupo")]
            public string nom_grupo { get; set; }

            [Display(Name = "Peso Bruto")]
            public int pesob { get; set; }

        }

        public class dayzafra
        {
            public int diazafra { get; set; }
        }

        #endregion

    }
}
