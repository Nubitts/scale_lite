using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Data.SQLite;
using DevExpress.XtraEditors.Controls;

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
        public string sUserC;
        public int iPesoB;
        public string dzafra;
        public int fzafra;

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

        List<strucdata.headertick> lTicko = new List<strucdata.headertick>();

        strucdata procedure = new strucdata();

        public string sConexl = @"Data Source=c:\\data_scale\\scaleinca.db;Version=3;Compress=True;";

        public Form1()
        {
            InitializeComponent();

            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.FocusedRowChanged += GridView1_FocusedRowChanged;
           
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
            label13.Text = string.Empty;
            label14.Text = string.Empty;
            textEdit3.Text = string.Empty;

            sServer = ConfigurationManager.AppSettings.Get("server");
            sUser = ConfigurationManager.AppSettings.Get("user");
            sPassword = ConfigurationManager.AppSettings.Get("password");
            sDB = ConfigurationManager.AppSettings.Get("dba");
            sPort = ConfigurationManager.AppSettings.Get("port");
            izafra = Convert.ToInt32(ConfigurationManager.AppSettings.Get("zafra"));
            dzafra = ConfigurationManager.AppSettings.Get("ddiazafra");
            fzafra = Convert.ToInt32(ConfigurationManager.AppSettings.Get("fdiazafra"));

            sConexion = "Server=" + sServer + ";Port=" + sPort + ";Database=" + sDB + ";Uid=" + sUser + ";password= " + sPassword + ";";



        bLifeconecta = testconnect(sConexion);

            validate_files();

            procedure.MakeStructure(sConexl);

            if (bLifeconecta)
            {
                toolStripStatusLabel1.Text = "Conexion con " + sServer;
                toolStripTextBox1.Enabled = true; toolStripTextBox2.Enabled = true;
                lUsers = procedure.ConvertToList<strucdata.users>(procedure.Predata(1, "id,user,password,nombre_full", "usuarios", "", sConexion));

                lForward = procedure.ConvertToList<strucdata.forwarder>(procedure.Predata(1, "num_fle, nombre", "fleteros", "selTipo = 'FLET'", sConexion));

                llifting = procedure.ConvertToList<strucdata.lifting>(procedure.Predata(1, "num_fle, nombre", "fleteros", "selTipo = 'ALZD'", sConexion));

                gridControl1.DataSource = Headert();

            }
            else
            {
                XtraMessageBox.Show("No hay conexion con base de datos...");
            }


        }

        private void validate_files()
        {

            if (Directory.Exists(@"c:\data_scale") == false)
            {
                Directory.CreateDirectory(@"c:\data_scale");
            }

            if (!File.Exists(@"c:\data_scale\scaleinca.db"))
            {
               SQLiteConnection.CreateFile(@"c:\data_scale\scaleinca.db");           }
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

        private BindingList<strucdata.headertick> Headert()
        {
            BindingList<strucdata.headertick> lResult = new BindingList<strucdata.headertick>();

            string sCampos = "ticket,numtra, fecpen, horent,nom_grupo, pesob";

            var lheadert = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, sCampos, "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0", sConexion));

            foreach(var Itm in lheadert)
            {
                lResult.Add(new strucdata.headertick
                {
                    ticket = Itm.ticket,
                    fecpen = Convert.ToDateTime( Itm.fecpen).ToString("yyyy-MM-dd"),
                    horent = Itm.horent,
                    nom_grupo = Itm.nom_grupo,
                    numtra = Itm.numtra,
                    pesob = Itm.pesob
                });
            }

            return lResult;
        } 

        private void GridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            try
            {
                if (this.gridView1.FocusedColumn.FieldName == "ticket")
                {
                    lTicko.Clear();

                    if (gridView1.RowCount > 0)
                    {
                        string sCampos = "ticket, fecpen, horent,nom_grupo,(select nombre from fleteros where num_fle = b.NUMTRA and seltipo = 'FLET' ) as fletero, pesob";

                        textEdit5.Text = this.gridView1.GetRowCellValue(e.FocusedRowHandle, gridView1.FocusedColumn).ToString();

                        lTicko = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, sCampos, "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0 and ticket = " + textEdit5.Text, sConexion));

                        textEdit7.Text = lTicko[0].pesob.ToString();
                        textEdit10.Text = lTicko[0].pesob.ToString();
                    }

                }
            }
            catch 
            {
            }
        }

        private void Totals()
        {
            int iTara;

            if (textEdit7.Text.Trim().Length == 0)
            {
                textEdit7.Text = lTicko[0].pesob.ToString();
                textEdit10.Text = lTicko[0].pesob.ToString();
            }

            if (int.TryParse(textEdit6.Text, out iTara))
            {

                int iPesob = lTicko[0].pesob;

                double doDesc = (textEdit8.Text.Trim().Length > 0) ? Convert.ToInt32(textEdit8.Text) : 0;

                double doCast = (textEdit9.Text.Trim().Length > 0) ? Convert.ToInt32(textEdit9.Text) : 0;

                int iPesoN = iPesob - Convert.ToInt32(textEdit6.Text);

                int iTd = 0; int iTc = 0;

                if (doDesc > 0)
                {

                    double doTDesc = (doDesc / 100);
                    iTd =  Convert.ToInt32( Math.Round( (iPesoN * doTDesc),0)) ;
                    label13.Text = iTd.ToString();
                }
                else
                {
                    label13.Text = string.Empty;
                }

                if (doCast > 0)
                {
                    double doTcast =  (doCast / 100);
                    iTc = Convert.ToInt32(Math.Round((iPesoN * doTcast), 0));
                    label14.Text = iTc.ToString();
                }
                else
                {
                    label14.Text = string.Empty;
                }

                textEdit7.Text = iPesoN.ToString();
                textEdit10.Text = (iPesoN - (iTd + iTc)).ToString();

            }

        }

        private void PrepareData(int iOpcion)
        {

            comboBoxEdit1.Properties.Items.Clear();
            comboBoxEdit2.Properties.Items.Clear();
            comboBoxEdit3.Properties.Items.Clear();

            ComboBoxItemCollection coll1 = comboBoxEdit1.Properties.Items;
            ComboBoxItemCollection coll2 = comboBoxEdit2.Properties.Items;
            ComboBoxItemCollection coll3 = comboBoxEdit3.Properties.Items;

            switch (iOpcion)
            {
                case 1:

                    var ifolioget = procedure.ConvertToList<strucdata.lastfcane>(procedure.Predata(1, "max(ticket) as ufol", "btkt_az", "zafra = " + izafra.ToString() , sConexion));

                    int iFolio = ifolioget[0].ufol + 1;

                    textBox1.Text = iFolio.ToString();

                    var lClients = procedure.ConvertToList<strucdata.clients>(procedure.Predata(1, "cliente", "btkt_az", "zafra = " + izafra.ToString() + " and cliente is not null group by cliente order by cliente", sConexion));

                    foreach(var Itm in lClients)
                    {
                        coll1.Add(Itm.cliente);
                    }

                    var lForw = procedure.ConvertToList<strucdata.forwarders>(procedure.Predata(1, "nombre", "fleteros", "zafra = " + izafra.ToString() + " and nombre is not null group by nombre order by nombre", sConexion));

                    foreach (var Itm in lForw)
                    {
                        coll2.Add(Itm.nombre);
                    }

                    var lCarrier = procedure.ConvertToList<strucdata.carriers>(procedure.Predata(1, "transportista", "transpt", "tipo_transp = 'AZUCAR'", sConexion));

                    foreach (var Itm in lCarrier)
                    {
                        coll3.Add(Itm.transportista);
                    }

                    tabControl1.SelectedTab = tabPage2;
                    tabControl2.SelectedTab = tabPage4;

                    break;

            }
        }

        private BindingList<strucdata.headertick> Headertaz()
        {
            BindingList<strucdata.headertick> lResult = new BindingList<strucdata.headertick>();

            string sCampos = "ticket,numtra, fecpen, horent,nom_grupo, pesob";

            var lheadert = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, sCampos, "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0", sConexion));

            foreach (var Itm in lheadert)
            {
                lResult.Add(new strucdata.headertick
                {
                    ticket = Itm.ticket,
                    fecpen = Convert.ToDateTime(Itm.fecpen).ToString("yyyy-MM-dd"),
                    horent = Itm.horent,
                    nom_grupo = Itm.nom_grupo,
                    numtra = Itm.numtra,
                    pesob = Itm.pesob
                });
            }

            return lResult;
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
                gridControl1.Enabled = true;
                tabControl2.Enabled = true;
                sUserC = lUserv[0].user;
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

            lTicketf = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob = 0 or pesob is null)", sConexion));

            if (int.TryParse(textEdit1.Text, out iTicketFr))
            {

                var lfTicket = lTicketf.Where(x => x.ticket == iTicketFr).ToList();

                if (lfTicket.Count() > 0)
                {
                    label2.Text = "Productor: " + lfTicket[0].nombre_p + " Orden: " + lfTicket[0].ordcte + " Grupo : " + lfTicket[0].nom_grupo + " Tabla: " + lfTicket[0].tabla + " Ciclo: " + lfTicket[0].ciclo;
                }
                else
                {
                    XtraMessageBox.Show("No se encuentra ticket...");
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


                if (XtraMessageBox.Show("Procede a guardar datos capturados?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    string sCondicion = string.Empty;
                    int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));
                    string sTipoc = (radioButton3.Checked) ? "Q'" : "C'";

                    int iHorP = lhorlab.Where(x => x.hourd == iHora).ToList()[0].hourt;

                    string sNofecha = DateTime.Now.ToString("yyMMdd");

                    if (iHora < 6)
                    {
                        sNofecha = DateTime.Now.AddDays(-1).ToString("yyMMdd");
                    }

                    string sNumtra = (textEdit2.Text.Trim().Length > 0) ? textEdit2.Text : "0";
                    string sNumalz = (textEdit4.Text.Trim().Length > 0) ? textEdit4.Text : "0";

                    sCondicion = "numtra = " + sNumtra;
                    sCondicion = sCondicion + ", numalz = " + sNumalz;
                    sCondicion = sCondicion + ", tipque = 'P'";
                    sCondicion = sCondicion + ", tpocan = '" + sTipoc ;
                    sCondicion = sCondicion + ", numavi = 20000, material = 1, peson = 0, pesot = 0, pesol = 0, pesob = " + textEdit3.Text;
                    sCondicion = sCondicion + ", fecpen = '" + DateTime.Now.ToString("yyyy-MM-dd") + "', horent = '" + DateTime.Now.ToString("HH:mm") +"'";
                    sCondicion = sCondicion + ", nofecha =" + sNofecha;
                    sCondicion = sCondicion + ", fecque = '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "', horque = '18:00'";
                    sCondicion = sCondicion + ", status = 'BATEY', diazafra = 0, ent_usuario = '" + sUserC + "'";

                    string sArmado = procedure.stringexe(2, sCondicion , "b_ticket", " ticket = " + textEdit1.Text + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);

                    gridControl1.DataSource = Headert();

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

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void textEdit6_EditValueChanged(object sender, EventArgs e)
        {

            Totals();
        }

        private void textEdit8_EditValueChanged(object sender, EventArgs e)
        {
            Totals();

        }

        private void textEdit9_EditValueChanged(object sender, EventArgs e)
        {
            Totals();
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sOpcion = toolStripComboBox1.SelectedItem.ToString().ToUpper();

            CleanControls();

            switch(sOpcion)
            {
                case "CAÑA":
                    tabPage1.Select();
                    tabControl1.SelectedTab = tabPage1;
                    tabControl2.SelectedTab = tabPage3;
                    break;
                case "AZUCAR":
                    tabControl1.SelectedTab = tabPage2;
                    tabControl2.SelectedTab = tabPage4;
                    PrepareData(1);
                    break;
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            TabPage taTabSel = tabControl1.SelectedTab;

            switch (taTabSel.Text.ToUpper().Trim())
            {
                case "AZUCAR":
                    PrepareData(1);
                    break;
            }
        }

        private void textEdit6_EditValueChanged_1(object sender, EventArgs e)
        {
            Totals();
        }

        private void textEdit8_EditValueChanged_1(object sender, EventArgs e)
        {
            Totals();
        }

        private void textEdit9_EditValueChanged_1(object sender, EventArgs e)
        {
            Totals();
        }

        private void simpleButton6_Click_1(object sender, EventArgs e)
        {
            int iTara;

            if (textEdit6.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar peso tara..."); textEdit3.Text = string.Empty; return; }

            if (int.TryParse(textEdit6.Text, out iTara))
            {
                if (XtraMessageBox.Show("Procede a dar salida a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    string sActualiza = string.Empty;

                    int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));

                    int iHorP = lhorlab.Where(x => x.hourd == iHora).ToList()[0].hourt;

                    int iHour = Convert.ToInt32(DateTime.Now.ToString("HH"));

                    TimeSpan tDifer = DateTime.Now - Convert.ToDateTime(dzafra);

                    string sNofecha = DateTime.Now.ToString("yyMMdd");

                    if (iHora < 6) { sNofecha = DateTime.Now.AddDays(-1).ToString("yyMMdd"); }

                    var dFechakk = (iHora < 6) ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                    int DiaZa = fzafra + tDifer.Days;

                    if (iHora < 6) { DiaZa = fzafra + (tDifer.Days-1); }

                    string sDescto = (textEdit8.Text.Trim().Length > 0) ? textEdit8.Text : "0";
                    string sCast = (textEdit9.Text.Trim().Length > 0) ? textEdit9.Text : "0";
                    string sTDescto = (label13.Text.Trim().Length > 0) ? label13.Text : "0";
                    string sTCast = (label14.Text.Trim().Length > 0) ? label14.Text : "0";

                    sActualiza = "pesot = " + textEdit6.Text;
                    sActualiza = sActualiza + ", peson = " + textEdit7.Text;
                    sActualiza = sActualiza + ", pesol = " + textEdit10.Text;
                    sActualiza = sActualiza + ", descto = " + sDescto;
                    sActualiza = sActualiza + ", castigo = " + sCast;
                    sActualiza = sActualiza + ", totaldescuento = " + sTDescto;
                    sActualiza = sActualiza + ", totalcastigo = " + sTCast;
                    sActualiza = sActualiza + ", nofecha =" + sNofecha;
                    sActualiza = sActualiza + ", fecpes = '" + DateTime.Now.ToString("yyyy-MM-dd") + "', horsal = '" + DateTime.Now.ToString("HH:mm") + "', hora = " + DateTime.Now.ToString("HH");
                     sActualiza = sActualiza + ", status = 'OK', diazafra = " + DiaZa.ToString()+ ", hr_code = " + iHorP + ", fechakk = '" + dFechakk + "', sal_usuario = '" + sUserC + "'";


                    string sArmado = procedure.stringexe(2, sActualiza, "b_ticket", " ticket = " + textEdit5.Text + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);

                    textEdit6.Text = string.Empty; textEdit8.Text = string.Empty; label13.Text = string.Empty; label14.Text = string.Empty;
                    gridControl1.DataSource = Headert();


                }

            }
            else
            {
                XtraMessageBox.Show("Debe anotar peso tara...");
            }

        }
    }
}