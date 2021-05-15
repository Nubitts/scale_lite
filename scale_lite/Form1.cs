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

        List<strucdata.transporter> lTransporter = new List<strucdata.transporter>();

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

                lTransporter = procedure.ConvertToList<strucdata.transporter>(procedure.Predata(1, "id_transp, transportista, tipo_transp", "transpt", "", sConexion));

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

            if (e.FocusedRowHandle < 0) { return; }

            try
            {
                if (this.gridView1.FocusedColumn.FieldName == "ticket")
                {

                    string sOpcion = toolStripComboBox1.Text.Trim().ToUpper();

                    switch (sOpcion)
                    {
                        case "PETROLEO":

                            string sCampop = "ticket, pesob, CONCAT_WS(' ',FECPEN,HORENT)as entrada, transportista, fletero";

                            string sTickp = this.gridView1.GetRowCellValue(e.FocusedRowHandle, gridView1.FocusedColumn).ToString();

                            label37.Text = sTickp;

                            var lTickp = procedure.ConvertToList<strucdata.headertickpt>(procedure.Predata(1, sCampop, "btkt_pet", "zafra = " + izafra.ToString() + " and ticket = " + sTickp, sConexion));

                            label40.Text = lTickp[0].pesob.ToString();

                            textBox10.Text = string.Empty;
                            textBox9.Text = string.Empty;
                            label42.Text = string.Empty;

                            break;
                        case "AZUCAR":

                            string sCampoz = "ticket, pesot, CONCAT_WS(' ',FECPEN,HORENT)as entrada, transportista, fletero";

                            string sTickz = this.gridView1.GetRowCellValue(e.FocusedRowHandle, gridView1.FocusedColumn).ToString();

                            label29.Text = sTickz;

                            var lTickz = procedure.ConvertToList<strucdata.headertickaz>(procedure.Predata(1, sCampoz, "btkt_az", "zafra = " + izafra.ToString() + " and ticket = " + sTickz, sConexion));

                            label23.Text = lTickz[0].pesot.ToString();

                            textBox3.Text = string.Empty;
                            textBox4.Text = string.Empty;
                            label24.Text = string.Empty;

                            break;
                        case "CAÑA":
                            lTicko.Clear();

                            if (gridView1.RowCount > 0)
                            {
                                string sCampos = "ticket, fecpen, horent,nom_grupo,(select nombre from fleteros where num_fle = b.NUMTRA and seltipo = 'FLET' ) as fletero, pesob";

                                textEdit5.Text = this.gridView1.GetRowCellValue(e.FocusedRowHandle, gridView1.FocusedColumn).ToString();

                                lTicko = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, sCampos, "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0 and ticket = " + textEdit5.Text, sConexion));

                                textEdit7.Text = lTicko[0].pesob.ToString();
                                textEdit10.Text = lTicko[0].pesob.ToString();
                            }
                            break;
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
            gridControl1.DataSource = null;
            gridView1.Columns.Clear();

            lTransporter.Clear();

            lTransporter = procedure.ConvertToList<strucdata.transporter>(procedure.Predata(1, "id_transp, transportista, tipo_transp", "transpt", "", sConexion));

            switch (iOpcion)
            {
                case 2:

                    toolStripComboBox1.Text = "PETROLEO";

                    textBox5.Text = string.Empty; textBox6.Text = string.Empty; textBox7.Text = string.Empty; textBox8.Text = string.Empty;
                    label40.Text = string.Empty; label42.Text = string.Empty;label37.Text = string.Empty;

                    comboBoxEdit4.Properties.Items.Clear();
                    comboBoxEdit5.Properties.Items.Clear();
                    comboBoxEdit6.Properties.Items.Clear();

                    var ifpget = procedure.ConvertToList<strucdata.lastfcane>(procedure.Predata(1, "max(ticket) as ufol", "btkt_pet", "zafra = " + izafra.ToString(), sConexion));

                    int iFpet = ifpget[0].ufol + 1;

                    textBox5.Text = iFpet.ToString();

                    ComboBoxItemCollection coll4 = comboBoxEdit4.Properties.Items;
                    ComboBoxItemCollection coll5 = comboBoxEdit5.Properties.Items;
                    ComboBoxItemCollection coll6 = comboBoxEdit6.Properties.Items;

                    var lCarrier1 = procedure.ConvertToList<strucdata.carriers>(procedure.Predata(1, "transportista", "transpt", "tipo_transp = 'PETROLEO'", sConexion));

                    foreach (var Itm in lCarrier1)
                    {
                        coll4.Add(Itm.transportista);
                    }

                    var lProcede = procedure.ConvertToList<strucdata.procedencias>(procedure.Predata(1, "procedencia", "btkt_pet", "zafra = " + izafra.ToString() + " and procedencia is not null group by procedencia order by procedencia", sConexion));

                    foreach (var Itm in lProcede)
                    {
                        coll5.Add(Itm.procedencia);
                    }

                    var lForw1 = procedure.ConvertToList<strucdata.forwarders>(procedure.Predata(1, "nombre", "fleteros", "zafra = " + izafra.ToString() + " and nombre is not null group by nombre order by nombre", sConexion));

                    foreach (var Itm in lForw1)
                    {
                        coll6.Add(Itm.nombre);
                    }

                    tabControl1.SelectedTab = tabPage5;
                    tabControl2.SelectedTab = tabPage6;

                    gridControl1.DataSource = Headertpet();


                    break;
                case 0:

                    toolStripComboBox1.Text = "CAÑA";

                    gridControl1.DataSource = Headert();
                    CleanControls();
                    break;
                case 1:

                    label23.Text = string.Empty;
                    label24.Text = string.Empty;
                    label29.Text = string.Empty;

                    comboBoxEdit1.Properties.Items.Clear();
                    comboBoxEdit2.Properties.Items.Clear();
                    comboBoxEdit3.Properties.Items.Clear();

                    comboBoxEdit2.Text = string.Empty;
                    textEdit11.Text = string.Empty;
                    comboBoxEdit3.Text = string.Empty;
                    textBox2.Text = string.Empty;


                    ComboBoxItemCollection coll1 = comboBoxEdit1.Properties.Items;
                    ComboBoxItemCollection coll2 = comboBoxEdit2.Properties.Items;
                    ComboBoxItemCollection coll3 = comboBoxEdit3.Properties.Items;

                    toolStripComboBox1.Text = "AZUCAR";

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

                    gridControl1.DataSource = Headertaz();

                    break;

            }
        }

        private BindingList<strucdata.headertickaz> Headertaz()
        {
            BindingList<strucdata.headertickaz> lResult = new BindingList<strucdata.headertickaz>();

            string sCampos = "ticket, pesot, CONCAT_WS(' ',FECPEN,HORENT) as entrada, transportista, fletero";

            var lheadert = procedure.ConvertToList<strucdata.headertickaz>(procedure.Predata(1, sCampos, "btkt_az", "zafra = " + izafra.ToString() + "  and status = 'PATIO' and pesot>0 order by ticket desc", sConexion));

            foreach (var Itm in lheadert)
            {
                lResult.Add(new strucdata.headertickaz
                {
                    ticket = Itm.ticket,
                    pesot = Itm.pesot,
                    entrada = Itm.entrada,
                    transportista = Itm.transportista,
                    fletero = Itm.fletero
                });
            }

            return lResult;
        }

        private BindingList<strucdata.headertickpt> Headertpet()
        {
            BindingList<strucdata.headertickpt> lResult = new BindingList<strucdata.headertickpt>();

            string sCampos = "ticket, pesob, CONCAT_WS(' ',FECPEN,HORENT) as entro, transportista, fletero";

            var lheadert = procedure.ConvertToList<strucdata.headertickpt>(procedure.Predata(1, sCampos, "btkt_pet", "zafra = " + izafra.ToString() + "  and status = 'PATIO' order by ticket desc", sConexion));

            foreach (var Itm in lheadert)
            {
                lResult.Add(new strucdata.headertickpt
                {
                    ticket = Itm.ticket,
                    pesob = Itm.pesob,
                    entro = Itm.entro,
                    transportista = Itm.transportista,
                    fletero = Itm.fletero
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
                    PrepareData(0);
                    break;
                case "AZUCAR":
                    tabControl1.SelectedTab = tabPage2;
                    tabControl2.SelectedTab = tabPage4;
                    PrepareData(1);
                    break;
                case "PETROLEO":
                    PrepareData(2);
                    tabControl1.SelectedTab = tabPage5;
                    tabControl2.SelectedTab = tabPage6;
                    break;
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            TabPage taTabSel = tabControl1.SelectedTab;

            toolStripComboBox1.Text = taTabSel.Text.ToUpper().Trim();

            switch (taTabSel.Text.ToUpper().Trim())
            {
                case "CAÑA":
                    PrepareData(0);
                    break;
                case "AZUCAR":                    
                    PrepareData(1);
                    break;
                case "PETROLEO":
                    PrepareData(2);
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

        private void simpleButton7_Click(object sender, EventArgs e)
        {

            if (comboBoxEdit2.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener un chofer..."); return; }
            if (textEdit11.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener placas de la unidad..."); return; }
            if (comboBoxEdit3.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener transportista..."); return; }
            if (textBox2.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener un cliente..."); return; }


            if (XtraMessageBox.Show("Procede a dar entrada a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            string sInserta = string.Empty;

            var lIdtr = lTransporter.Where(x => x.transportista == comboBoxEdit3.Text && x.tipo_transp == "AZUCAR").ToList();

            if (lIdtr.Count() == 0) { XtraMessageBox.Show("El transportista no es valido..."); return; }


            int iTara = 0;

            if (int.TryParse(textBox2.Text, out iTara))
            {
                int iIdtr = lIdtr[0].id_transp;
                string sFen = DateTime.Now.ToString("yyyy-MM-dd");
                string sHen = DateTime.Now.ToString("HH:mm");

                string sCampos = "zafra,ticket,id_transp,transportista,fletero,placas,fecpen,horent,pesot,material,status,ent_usuario";

                sInserta = izafra.ToString().Trim()+","+textBox1.Text.Trim()+","+iIdtr.ToString()+",'"+comboBoxEdit3.Text.Trim()+"','"+comboBoxEdit2.Text+"','"+textEdit11.Text+"',";
                sInserta = sInserta + "'" + sFen + "', '" + sHen + "', " + iTara.ToString() + ",2,'PATIO', '" + sUserC + "'";

                string sArmado = procedure.stringexe(3, sCampos, "btkt_az", sInserta);

                procedure.Executecmm(sArmado, sConexion);

                PrepareData(1);

            }
            else
            {
                XtraMessageBox.Show("El peso tara debe ser numerico...");
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

            int iPesob = 0;

            if (textBox3.Text.Trim().Length == 0) { return; }

            if (int.TryParse(textBox3.Text, out iPesob))
            {
                label24.Text = (Convert.ToInt32(textBox3.Text) - Convert.ToInt32(label23.Text)).ToString();
            }
            else
            {
                XtraMessageBox.Show("El peso tara debe ser numerico...");
            }


        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {

            if (textBox3.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso bruto..."); return; }

            if (textBox4.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener unidades bultos..."); return; }

            if (label24.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso neto..."); return; }

            if (comboBoxEdit1.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener un cliente..."); return; }

            if (XtraMessageBox.Show("Procede a dar salida a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes) { return; }

            int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));

            string sFes = DateTime.Now.ToString("yyyy-MM-dd");
            string sHes = DateTime.Now.ToString("HH:mm");

            string sNofecha = DateTime.Now.ToString("yyMMdd");

            if (iHora < 6) { sNofecha = DateTime.Now.AddDays(-1).ToString("yyMMdd"); }

            string sActualiza = "pesob = " + textBox3.Text + ", peson = " + label24.Text + ", bultos = " + textBox4.Text + ", fecpes = '" + sFes + "', horsal = '" + sHes + "', status = 'OK'" ;

            sActualiza = sActualiza + ", sal_usuario = '" + sUserC + "', nofecha = " + sNofecha + ", cliente = '" + comboBoxEdit1.Text + "'";

            string sArmado = procedure.stringexe(2, sActualiza, "btkt_az", " ticket = " + label29.Text + " and zafra = " + izafra);

            procedure.Executecmm(sArmado, sConexion);

            PrepareData(1);

            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            label24.Text = string.Empty;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBoxEdit6.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener fletero..."); return; }
            if (textBox7.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener placas de la unidad..."); return; }
            if (textBox8.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso definido..."); return; }


            if (XtraMessageBox.Show("Procede a dar entrada a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes) { return; }

            string sInserta = string.Empty;

            int iBruto = 0;

            if (int.TryParse(textBox8.Text, out iBruto))
            {


                string sFen = DateTime.Now.ToString("yyyy-MM-dd");
                string sHen = DateTime.Now.ToString("HH:mm");


                string sCampos = "zafra,ticket,fletero,placas,fecpen,horent,pesob,material,status,usuario, procedencia";

                sInserta = izafra.ToString().Trim() + "," + textBox5.Text.Trim() + ", '" + textBox7.Text + "','" + textBox7.Text + "',";
                sInserta = sInserta + "'" + sFen + "', '" + sHen + "', " + iBruto.ToString() + ",5,'PATIO', '" + sUserC + "', '" + comboBoxEdit5.Text + "'";

                string sArmado = procedure.stringexe(3, sCampos, "btkt_pet", sInserta);

                procedure.Executecmm(sArmado, sConexion);

                PrepareData(2);


            }
            else
            {
                XtraMessageBox.Show("El peso bruto debe ser numerico...");
            }

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            int iPesot = 0;

            if (textBox10.Text.Trim().Length == 0) { return; }

            if (int.TryParse(textBox10.Text, out iPesot))
            {
                label42.Text = (Convert.ToInt32(label40.Text) - Convert.ToInt32(textBox10.Text)).ToString();
            }
            else
            {
                XtraMessageBox.Show("El peso tara debe ser numerico...");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (comboBoxEdit4.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener un transportista..."); return; }
            if (comboBoxEdit5.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener procedencia..."); return; }
            if (textBox6.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener folio de remision..."); return; }


            if (textBox10.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso tara..."); return; }

            if (textBox9.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener unidades litros..."); return; }

            if (label42.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso neto..."); return; }

            if (XtraMessageBox.Show("Procede a dar salida a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes) { return; }

            var lIdtr = lTransporter.Where(x => x.transportista == comboBoxEdit4.Text && x.tipo_transp == "PETROLEO").ToList();

            if (lIdtr.Count() == 0) { XtraMessageBox.Show("El transportista no es valido..."); return; }

            int iIdtr = lIdtr[0].id_transp;

            int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));

            string sFes = DateTime.Now.ToString("yyyy-MM-dd");
            string sHes = DateTime.Now.ToString("HH:mm");

            string sNofecha = DateTime.Now.ToString("yyMMdd");

            if (iHora < 6) { sNofecha = DateTime.Now.AddDays(-1).ToString("yyMMdd"); }

            string sActualiza = "pesot = " + textBox10.Text + ", peson = " + label42.Text + ", litros = " + textBox9.Text + ", fecpes = '" + sFes + "', horsal = '" + sHes + "', status = 'OK'";

            sActualiza = sActualiza + ", nofecha = " + sNofecha + ", id_transp = " + iIdtr.ToString() + ", transportista = '" + comboBoxEdit4.Text + "', procedencia = '" + comboBoxEdit5.Text + "', remision = '" +  textBox6.Text + "'";

            string sArmado = procedure.stringexe(2, sActualiza, "btkt_pet", " ticket = " + label37.Text + " and zafra = " + izafra);

            procedure.Executecmm(sArmado, sConexion);

            PrepareData(2);

            textBox10.Text = string.Empty;
            textBox9.Text = string.Empty;
            label42.Text = string.Empty;
            label37.Text = string.Empty;

            comboBoxEdit4.Text = string.Empty;
            comboBoxEdit5.Text = string.Empty;
            textBox6.Text = string.Empty;

        }
    }
}