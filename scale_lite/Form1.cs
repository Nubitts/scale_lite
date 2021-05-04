using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace scale_lite
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public string sServer;
        public string sUser;
        public string sPassword;
        public string sDB;
        public string sPort;
        public string sConexion;
        public bool bLifeconecta = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Valuesconfig();
        }

        #region stores_proc
        private void Valuesconfig()
        {
            toolStripStatusLabel1.Text = string.Empty;

            sServer = ConfigurationManager.AppSettings.Get("server");
            sUser = ConfigurationManager.AppSettings.Get("user");
            sPassword = ConfigurationManager.AppSettings.Get("password");
            sDB = ConfigurationManager.AppSettings.Get("dba");
            sPort = ConfigurationManager.AppSettings.Get("port");

            sConexion = "Server=" + sServer + ";Port=" + sPort + ";Database=" + sDB + ";Uid=" + sUser + ";password= " + sPassword + ";";

            bLifeconecta = testconnect(sConexion);

            if (bLifeconecta)
            {
                toolStripStatusLabel1.Text = "Conexion con " + sServer;
                toolStripTextBox1.Enabled = true;toolStripTextBox2.Enabled = true;
            }
            else
            {
                XtraMessageBox.Show("No hay conexion con base de datos...");
            }


        }

        public bool testconnect(string sConecta)
        {
            MySqlConnection conexion = new MySqlConnection(sConecta);

            try
            {
                conexion.Open();
                conexion.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }

        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Trim().Length ==0) { XtraMessageBox.Show("Debe anotar el usuario a conectar..."); }
            if (toolStripTextBox2.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el password del usuario a conectar..."); }

            if (toolStripTextBox1.Text.Trim().ToUpper() == "USUARIO") { XtraMessageBox.Show("Debe anotar el usuario a conectar..."); }
            if (toolStripTextBox2.Text.Trim().ToUpper() == "PASSWORD") { XtraMessageBox.Show("Debe anotar el password del usuario a conectar..."); }



        }
    }
}
