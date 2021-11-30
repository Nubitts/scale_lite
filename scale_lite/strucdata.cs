using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;

namespace scale_lite
{
    class strucdata
    {

        public void MakeStructure(string sConecta)
        {

            try
            {
                List<string> lsSQL = new List<string>();

                // Usuario
                lsSQL.Add("CREATE TABLE IF NOT EXISTS parameters (parameter TEXT NULL, value1 TEXT NULL, value2 TEXT NULL);");
                // Informes

                SQLiteConnection Con;
                SQLiteCommand Cmmd;

                Con = new SQLiteConnection(sConecta);

                Con.Open();

                foreach (string Value in lsSQL)
                {

                    Cmmd = new SQLiteCommand(Value, Con);

                    Cmmd.ExecuteNonQuery();
                }

                Con.Close();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " " + e.Source);
            }

        }


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

            string sCondiciona = (sCondicion.Trim().Length > 0) ? ((iOption == 3) ? "": " where ") + sCondicion : "";

            switch (iOption)
            {
                case 1:
                    sQuery = "select " + sCampos + " from " + sTabla + sCondiciona;
                    break;
                case 2:
                    sQuery = "update " + sTabla + " set " + sCampos + " " + sCondiciona;
                    break;
                case 3:
                    sQuery = "insert " + sTabla + " (" + sCampos + ") values (" + sCondiciona + ")";
                    break;
                case 4:
                    sQuery = "delete from " + sTabla + sCondiciona;
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
            public string fecpes { get; set; }
            public string horent { get; set; }
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

        public class lastfcane
        {
            public int ufol { get; set; }
        }

        public class clients
        {
            public string cliente { get; set; }
        }

        public class forwarders
        {
            public string nombre { get; set; }
        }

        public class carriers
        {
            public string transportista { get; set; }
        }

        public class headertickaz
        {
            [Display(Name = "TIcket")]
            public int ticket { get; set; }

            [Display(Name = "Peso Tara")]
            public int pesot { get; set; }

            [Display(Name = "Entro")]
            public string entrada { get; set; }

            [Display(Name = "Transportista")]
            public string transportista { get; set; }

            [Display(Name = "Fletero")]
            public string fletero { get; set; }

        }

        public class transporter
        {
            public int id_transp { get; set; }
            public string transportista { get; set; }
            public string tipo_transp { get; set; }
        }

        public class headertickpt
        {
            [Display(Name = "TIcket")]
            public int ticket { get; set; }

            [Display(Name = "Peso Bruto")]
            public int pesob { get; set; }

            [Display(Name = "Entro")]
            public string entro { get; set; }

            [Display(Name = "Transportista")]
            public string transportista { get; set; }

            [Display(Name = "Fletero")]
            public string fletero { get; set; }

        }

        public class procedencias
        {
            public string procedencia { get; set; }
        }

        public class printtick
        {
            public int ticket { get; set; }
            public int ordcte { get; set; }
            public int codigo { get; set; }
            public string nombre_p { get; set; }
            public int grupo { get; set; }
            public string nom_grupo { get; set; }
            public string tipocanes { get; set; }
            public int tabla { get; set; }
            public string ciclo { get; set; }
            public int fletero { get; set; }
            public string FECPEN { get; set; }
            public string horent { get; set; }
            public double pesob { get; set; }
            public double peson { get; set; }
            public double pesotara { get; set; }
            public double descto { get; set; }
            public double castigo { get; set; }
            public string fecpes { get; set; }
            public string horsal { get; set; }
            public double totaldescuento { get; set; }
            public int alzadora { get; set; }
            public double pesol { get; set; }
            public double totalcastigo { get; set; }
        }

        public class assigndata
        {
            public int orden {get;set;}
	        public int ticket { get; set; }
	        public int zona { get; set; }
	        public int fleter { get; set; }
	        public string fullnamefleter { get; set; }
	        public int lifting { get; set; }
	        public string fullnamelifting { get; set; }
	        public int harvest { get; set; }
	        public string fullnameharvest { get; set; }
        }

        public class Root1
        {
            public bool error { get; set; }
            public string message { get; set; }
            public List<assigndata> registros { get; set; }
        }

        public class databurni
        {
            public int ticket { get; set; }
            public string tpocan { get; set; }
            public string fecque { get; set; }
            public string horque { get; set; }
            public string typeburn { get; set; }
        }

        public class Root2
        {
            public bool error { get; set; }
            public string message { get; set; }
            public List<databurni> registros { get; set; }
        }

        public class tickettmp
        {
            public int ticket { get; set; }
            public double pesob { get; set; }
            public string fecque { get; set; }
            public string horque { get; set; }
        }

        public class tpunishment
        {
            public string  typecane { get; set; }
            public string typebourn { get; set; }
            public int at_hour { get; set; }
            public int to_hour { get; set; }
            public int percent_punish { get; set; }
            public int subject_analisis { get; set; }
        }

        #endregion

    }
}
