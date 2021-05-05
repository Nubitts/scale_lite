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
        public int izafra;

        List<strucdata.users> lUsers = new List<strucdata.users>();

        List<strucdata.ticketfree> lTicketsf = new List<strucdata.ticketfree>();

        List<strucdata.forwarder> lForward = new List<strucdata.forwarder>();

        List<strucdata.lifting> llifting = new List<strucdata.lifting>();

        List<strucdata.dhour> lhorlab = new List<strucdata.dhour>
        {
            new strucdata.dhour { hourd = 6, hourt =  1},
            new strucdata.dhour { hourd = 7, hourt =  2},
            new strucdata.dhour { hourd = 8, hourt =  3},
            new strucdata.dhour { hourd = 9, hourt =  4},
            new strucdata.dhour { hourd = 10, hourt =  5},
            new strucdata.dhour { hourd = 11, hourt =  6},
            new strucdata.dhour { hourd = 12, hourt =  7},
            new strucdata.dhour { hourd = 13, hourt =  8},
            new strucdata.dhour { hourd = 14, hourt =  9},
            new strucdata.dhour { hourd = 15, hourt =  10},
            new strucdata.dhour { hourd = 16, hourt =  11},
            new strucdata.dhour { hourd = 17, hourt =  12},
            new strucdata.dhour { hourd = 18, hourt =  13},
            new strucdata.dhour { hourd = 19, hourt =  14},
            new strucdata.dhour { hourd = 20, hourt =  15},
            new strucdata.dhour { hourd = 21, hourt =  16},
            new strucdata.dhour { hourd = 22, hourt =  17},
            new strucdata.dhour { hourd = 23, hourt =  18},
            new strucdata.dhour { hourd = 0, hourt =  19},
            new strucdata.dhour { hourd = 1, hourt =  20},
            new strucdata.dhour { hourd = 2, hourt =  21},
            new strucdata.dhour { hourd = 3, hourt =  22},
            new strucdata.dhour { hourd = 4, hourt =  23},
            new strucdata.dhour { hourd = 5, hourt =  24}
        };

        strucdata procedure = new strucdata();
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
            toolStripStatusLabel2.Text = string.Empty;
            label2.Text = string.Empty;
            label6.Text = string.Empty;
            label7.Text = string.Empty;
            textEdit3.Text = string.Empty;

            sServer = ConfigurationManager.AppSettings.Get("server");
            sUser = ConfigurationManager.AppSettings.Get("user");
            sPassword = ConfigurationManager.AppSettings.Get("password");
            sDB = ConfigurationManager.AppSettings.Get("dba");
            sPort = ConfigurationManager.AppSettings.Get("port");
            izafra = Convert.ToInt32(ConfigurationManager.AppSettings.Get("zafra"));

            sConexion = "Server=" + sServer + ";Port=" + sPort + ";Database=" + sDB + ";Uid=" + sUser + ";password= " + sPassword + ";";

            bLifeconecta = testconnect(sConexion);

            if (bLifeconecta)
            {
                toolStripStatusLabel1.Text = "Conexion con " + sServer;
                toolStripTextBox1.Enabled = true; toolStripTextBox2.Enabled = true;
                lUsers = procedure.ConvertToList<strucdata.users>(procedure.Predata(1, "id,user,password,nombre_full", "usuarios", "", sConexion));

                lForward = procedure.ConvertToList<strucdata.forwarder>(procedure.Predata(1, "num_fle, nombre", "fleteros", "selTipo = 'FLET'", sConexion));

                llifting = procedure.ConvertToList<strucdata.lifting>(procedure.Predata(1, "num_fle, nombre", "fleteros", "selTipo = 'ALZD'", sConexion));

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

        private void CleanControls()
        {
            textEdit1.Text = string.Empty; textEdit2.Text = string.Empty; textEdit3.Text = string.Empty; textEdit4.Text = string.Empty;
            label2.Text = string.Empty; label6.Text = string.Empty; label7.Text = string.Empty;
            textEdit1.Focus();

        }

        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el usuario a conectar..."); toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty; return; }
            if (toolStripTextBox2.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el password del usuario a conectar..."); toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty; return; }

            if (toolStripTextBox1.Text.Trim().ToUpper() == "USUARIO") { XtraMessageBox.Show("Debe anotar el usuario a conectar..."); toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty; return; }
            if (toolStripTextBox2.Text.Trim().ToUpper() == "PASSWORD") { XtraMessageBox.Show("Debe anotar el password del usuario a conectar..."); toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty; return; }

            var lUserv = lUsers.Where(x => x.user.Trim().ToUpper() == toolStripTextBox1.Text.Trim().ToUpper() && x.password.Trim().ToUpper() == toolStripTextBox2.Text.Trim().ToUpper()).ToList();

            if (lUserv.Count() > 0)
            {
                toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty; toolStripTextBox1.Enabled = false; toolStripTextBox2.Enabled = false;
                toolStripComboBox1.Enabled = true;
                toolStripStatusLabel2.Text = lUserv[0].nombre_full;
                tabControl1.Enabled = true;
            }
            else
            {
                toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty;
                XtraMessageBox.Show("Usuario no valido para acceder...");
            }



        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            int iTicketFr = 0;

            if (textEdit1.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el ticket a conectar..."); textEdit1.Text = string.Empty; return; }

            List<strucdata.ticketfree> lTicketf = new List<strucdata.ticketfree>();

            lTicketf = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo", "b_ticket", "zafra = " + izafra.ToString() + " and peson is null", sConexion));

            if (int.TryParse(textEdit1.Text, out iTicketFr))
            {

                var lfTicket = lTicketf.Where(x => x.ticket == iTicketFr).ToList();

                if (lfTicket.Count() > 0)
                {
                    label2.Text = "Productor: " + lfTicket[0].nombre_p + " Orden: " + lfTicket[0].ordcte + " Grupo : " + lfTicket[0].nom_grupo + " Tabla: " + lfTicket[0].tabla + " Ciclo: " + lfTicket[0].ciclo;
                }

            }
            else
            {
                XtraMessageBox.Show("No es un valor valido de un ticket...");
            }

        }


        private void simpleButton2_Click(object sender, EventArgs e)
        {

            int iFletero = 0;

            if (textEdit2.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el codigo de fletero a conectar..."); textEdit2.Text = string.Empty; return; }

            if (int.TryParse(textEdit2.Text, out iFletero))
            {

                var lFrw = lForward.Where(x => Convert.ToInt32(x.num_fle) == iFletero).ToList();

                label6.Text = lFrw[0].nombre;

            }
            else
            {
                XtraMessageBox.Show("No es un valor valido identidad fletero...");
            }

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            int iAlza = 0;

            if (textEdit4.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el codigo de Alzadora a conectar..."); textEdit4.Text = string.Empty; return; }

            if (int.TryParse(textEdit4.Text, out iAlza))
            {

                var lAlzad = llifting.Where(x => Convert.ToInt32(x.num_fle) == iAlza).ToList();

                label7.Text = (iAlza > 0) ? lAlzad[0].nombre : "Sin Alzadora"; 

            }
            else
            {
                XtraMessageBox.Show("No es un valor valido identidad Alzadora...");
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            int iPeso = 0;

            if (textEdit3.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar peso inicial..."); textEdit3.Text = string.Empty; return; }

            if (int.TryParse(textEdit3.Text, out iPeso))
            {

                if (label2.Text.Trim().Length == 0) { XtraMessageBox.Show("No ha focalizado ningun ticket..."); return; }

                if (label6.Text.Trim().Length == 0) { XtraMessageBox.Show("No ha focalizado ningun fletero..."); return; }

                if (label7.Text.Trim().Length == 0) { XtraMessageBox.Show("No ha focalizado ningun alzadora..."); return; }

                if (XtraMessageBox.Show("Procede a guardar datos capturados?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    string sCondicion = string.Empty;
                    int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));
                    string sTipoq = (radioButton1.Checked) ? "P'" : "A'";
                    string sTipoc = (radioButton3.Checked) ? "Q'" : "C'";

                    int iHorP = lhorlab.Where(x => x.hourd == iHora).ToList()[0].hourt;

                    string sNofecha = DateTime.Now.ToString("yyMMdd");

                    if (iHora < 6)
                    {
                        sNofecha = DateTime.Now.AddDays(-1).ToString("yyMMdd");
                    }

                    sCondicion = "numtra = " + textEdit2.Text;
                    sCondicion = sCondicion + ", numalz = " + textEdit4.Text;
                    sCondicion = sCondicion + ", tipque = '" + sTipoq ;
                    sCondicion = sCondicion + ", tpocan = '" + sTipoc;
                    sCondicion = sCondicion + ", numtra = " + textEdit2.Text;
                    sCondicion = sCondicion + ", numalz = " + textEdit4.Text;
                    sCondicion = sCondicion + ", numavi = 20000, material = 1";
                    sCondicion = sCondicion + ", fecpen = '" + DateTime.Now.ToString("yyyy-MM-dd") + "', horent = '" + DateTime.Now.ToString("HH:mm") +"', hora = " + DateTime.Now.ToString("HH");
                    sCondicion = sCondicion + ", fecque = '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "', horque = '18:00', hr_code = " + iHorP ;
                    sCondicion = sCondicion + ", nofecha =" +  sNofecha;

                    string sArmado = procedure.stringexe(2, sCondicion , "b_ticket", " ticket = " + textEdit1.Text + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);

                    XtraMessageBox.Show("Se ingreso entrada...");

                    CleanControls();

                }

            }
            else
            {
                XtraMessageBox.Show("No es un valor valido Peso Inicial...");
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            CleanControls();
        }
    }
}