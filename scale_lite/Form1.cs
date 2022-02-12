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
using System.IO.Ports;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Text;

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

        public string scaleentry;
        public string scaleout;
        public string scalercom1;
        public string scalercom2;
        public string sReadScale1;
        public string sReadScale2;
        public string sPrinterdev;
        public string sApigetasig;

        public int counter = 0;

        public bool boApi = true;

        public bool boFlrfid = false;
        public string sSerialrfid = string.Empty;

        public SerialPort port1 = new SerialPort("COM3", 2400, Parity.None, 7, StopBits.One);
        public SerialPort port2 = new SerialPort("COM4", 2400, Parity.Even, 7, StopBits.One);

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

        List<strucdata.assigndata> lasignacion = new List<strucdata.assigndata>();

        List<strucdata.databurni> lticketburn = new List<strucdata.databurni>();

        List<strucdata.tpunishment> lTablep = new List<strucdata.tpunishment>();

        List<strucdata.datazafra> lDatazafra = new List<strucdata.datazafra>();

        List<strucdata.qdrivers> lDroth = new List<strucdata.qdrivers>();

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
            toolStripLabel6.Text = string.Empty;

            sServer = ConfigurationManager.AppSettings.Get("server");
            sUser = ConfigurationManager.AppSettings.Get("user");
            sPassword = ConfigurationManager.AppSettings.Get("password");
            sDB = ConfigurationManager.AppSettings.Get("dba");
            sPort = ConfigurationManager.AppSettings.Get("port");
            izafra = Convert.ToInt32(ConfigurationManager.AppSettings.Get("zafra"));
            dzafra = ConfigurationManager.AppSettings.Get("ddiazafra");
            fzafra = Convert.ToInt32(ConfigurationManager.AppSettings.Get("fdiazafra"));

            scaleentry = ConfigurationManager.AppSettings.Get("scaleentry");
            scaleout = ConfigurationManager.AppSettings.Get("scaleout");
            scalercom1 = ConfigurationManager.AppSettings.Get("readlinecom1");
            scalercom2 = ConfigurationManager.AppSettings.Get("readlinecom2");
            sPrinterdev = ConfigurationManager.AppSettings.Get("printer");
            sApigetasig = ConfigurationManager.AppSettings.Get("apiasign");

            sConexion = "Server=" + sServer + ";Port=" + sPort + ";Database=" + sDB + ";Uid=" + sUser + ";password= " + sPassword + ";";

            bLifeconecta = testconnect(sConexion);

            //validate_files();

            CleanControls();

            //procedure.MakeStructure(sConexl);

            toolStripStatusLabel3.Text = "Lectura API";

            if (bLifeconecta)
            {
                toolStripStatusLabel1.Text = "Conexion con " + sServer;

                toolStripTextBox1.Enabled = true; toolStripTextBox2.Enabled = true;

                ObtainSerialrfid();

                lUsers = procedure.ConvertToList<strucdata.users>(procedure.Predata(1, "id,user,password,nombre_full", "usuarios", "", sConexion));

                lForward = procedure.ConvertToList<strucdata.forwarder>(procedure.Predata(1, "num_fle, nombre", "fleteros", "selTipo = 'FLET'", sConexion));

                llifting = procedure.ConvertToList<strucdata.lifting>(procedure.Predata(1, "num_fle, nombre", "fleteros", "selTipo in('ALZD','COSCH')", sConexion));

                lTransporter = procedure.ConvertToList<strucdata.transporter>(procedure.Predata(1, "id_transp, transportista, tipo_transp", "transpt", "", sConexion));

                lasignacion = procedure.ConvertToList<strucdata.assigndata>(procedure.Predata(1, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", "", sConexion));

                lticketburn = procedure.ConvertToList<strucdata.databurni>(procedure.Predata(1, "ticket,tpocan,fecque,horque,typeburn", "databurn", "", sConexion));

                lTablep = procedure.ConvertToList<strucdata.tpunishment>(procedure.Predata(1, "typecane,typebourn,at_hour,to_hour,percent_punish,subject_analisis", "table_punish", "", sConexion));

                lDatazafra = procedure.ConvertToList<strucdata.datazafra>(procedure.Predata(1, "zafra,zafra_", "zafra", "actual = 1", sConexion));

                lDroth = procedure.ConvertToList<strucdata.qdrivers>(procedure.Predata(1, "id_transp,plates,driver,tarew", "qdrivers", "", sConexion));

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

            if (Directory.Exists(@"c:\logs") == false)
            {
                Directory.CreateDirectory(@"c:\logs");
            }

            if (!File.Exists(@"c:\data_scale\scaleinca.db"))
            {
               SQLiteConnection.CreateFile(@"c:\data_scale\scaleinca.db");               
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
            textEdit1.Text = string.Empty; textEdit2.Text = string.Empty; textEdit3.Text = string.Empty; textEdit4.Text = string.Empty;textEdit8.Text = string.Empty;
            label2.Text = string.Empty; label6.Text = string.Empty; label7.Text = string.Empty;textEdit9.Text = string.Empty;textEdit10.Text = string.Empty;

            Obtainassigment();
            Obtaindataburn();

            textEdit1.Focus();

        }

        private BindingList<strucdata.headertick1> Headert()
        {
            BindingList<strucdata.headertick1> lResult = new BindingList<strucdata.headertick1>();

            string sCampos = "ticket,numtra, fecpen, horent,nom_grupo, pesob, castigo, descto";

            var lheadert = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, sCampos, "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0", sConexion));

            foreach(var Itm in lheadert)
            {
                lResult.Add(new strucdata.headertick1
                {
                    ticket = Itm.ticket,
                    fecpen = Convert.ToDateTime( Itm.fecpen).ToString("yyyy-MM-dd"),
                    horent = Itm.horent,
                    nom_grupo = Itm.nom_grupo,
                    numtra = Itm.numtra,
                    pesob = Itm.pesob,
                    //castigo = Itm.castigo,
                    //descto = Itm.descto
                   
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
                                string sCampos = "ticket, fecpen, horent,nom_grupo,(select nombre from fleteros where num_fle = b.NUMTRA and seltipo = 'FLET' ) as fletero, pesob, castigo,descto";

                                textEdit5.Text = this.gridView1.GetRowCellValue(e.FocusedRowHandle, gridView1.FocusedColumn).ToString();

                                lTicko = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, sCampos, "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0 and ticket = " + textEdit5.Text, sConexion));

                                textEdit7.Text = lTicko[0].pesob.ToString();
                                textEdit10.Text = lTicko[0].pesob.ToString();
                                textEdit9.Text = lTicko[0].castigo.ToString();
                                textEdit8.Text = lTicko[0].descto.ToString();
                            }
                            break;
                        default:
                            string sOpcions = toolStripComboBox1.Text;
                            string sTable = string.Empty;

                            string sFcampo = "pesot";

                            switch (label50.Text.Trim().ToUpper())
                            {
                                case "MIEL":
                                    sTable = "btkt_miel";
                                    break;
                                case "BIOMASA":
                                    sTable = "btkt_bm";
                                    sFcampo = "pesob";
                                    break;
                                case "CACHAZA":
                                    sTable = "btkt_chz";
                                    break;
                                case "PROD QUIMICOS":
                                    sTable = "btkt_pkim";
                                    break;
                                case "OTROS":
                                    sTable = "btkt_otr";
                                    break;
                            }

                            string sCampozz = "ticket, " + sFcampo + " as peso, CONCAT_WS(' ',FECPEN,HORENT)as entrada, transportista, fletero";

                            string sTickzz = this.gridView1.GetRowCellValue(e.FocusedRowHandle, gridView1.FocusedColumn).ToString();

                            label51.Text = sTickzz;

                            var lTickzz = procedure.ConvertToList<strucdata.headerother>(procedure.Predata(1, sCampozz, sTable, "zafra = " + izafra.ToString() + " and ticket = " + sTickzz, sConexion));

                            label57.Text = lTickzz[0].peso.ToString();

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

            try //boque try con todas las operaciones
            {

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

                    int iTd = 0; int iTc = 0; int iTpar = 0;

                    if (doDesc > 0)
                    {

                        double doTDesc = (doDesc / 100);
                        iTd = Convert.ToInt32(Math.Round((iPesoN * doTDesc), 0));
                        iTpar = iPesoN - iTd;
                        label13.Text = iTd.ToString();
                    }
                    else
                    {
                        iTpar = iPesoN;
                        label13.Text = string.Empty;
                    }

                    if (doCast > 0)
                    {
                        double doTcast = (doCast / 100);
                        iTc = Convert.ToInt32(Math.Round((iTpar * doTcast), 0));
                        iTpar = iTpar - iTc;
                        label14.Text = iTc.ToString();
                    }
                    else
                    {
                        label14.Text = string.Empty;
                    }

                    textEdit7.Text = iPesoN.ToString();
                    textEdit10.Text = (iTpar).ToString();

                }

            }
            catch (Exception ex) //bloque catch para captura de error
            {
               // string error = ex.Message; //acción para manejar el error
            }

        }

        private void Totals1()
        {
            int iTara;

            try //boque try con todas las operaciones
            {

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

                    int iTd = 0; int iTc = 0; int iTpar = 0;

                    if (doDesc > 0)
                    {

                        double doTDesc = (doDesc / 100);
                        iTd = Convert.ToInt32(Math.Round((iPesoN * doTDesc), 0));
                        iTpar = iPesoN - iTd;
                        label13.Text = iTd.ToString();
                    }
                    else
                    {
                        iTpar = iPesoN;
                        label13.Text = string.Empty;
                    }

                    if (doCast > 0)
                    {
                        double doTcast = (doCast / 100);
                        iTc = Convert.ToInt32(Math.Round((iTpar * doTcast), 0));
                        label14.Text = iTc.ToString();
                    }
                    else
                    {
                        label14.Text = string.Empty;
                    }

                    textEdit7.Text = iPesoN.ToString();
                    textEdit10.Text = (iTpar).ToString();

                }

            }
            catch (Exception ex) //bloque catch para captura de error
            {
                // string error = ex.Message; //acción para manejar el error
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
                case 3:

                    textBox12.Text = string.Empty; comboBoxEdit8.Text = string.Empty; textEdit12.Text = string.Empty; comboBoxEdit7.Text = string.Empty;
                    textBox14.Text = string.Empty; textBox14.Text = string.Empty; comboBoxEdit9.Text = string.Empty; label57.Text = string.Empty; label51.Text = string.Empty; label55.Text = string.Empty;

                    comboBoxEdit7.Properties.Items.Clear();
                    comboBoxEdit8.Properties.Items.Clear();
                    comboBoxEdit9.Properties.Items.Clear();

                    string sTabla = string.Empty;
                    string sFieldcb = string.Empty;
                    string sCondicion = string.Empty;
                    int iFlOtros = 0;

                    string sTipo = toolStripComboBox1.Text.Trim().ToUpper();

                    switch (sTipo)
                    {
                        case "MIEL":
                            sTabla = "btkt_miel";
                            label58.Visible = false; comboBoxEdit9.Visible = false;
                            break;
                        case "BIOMASA":
                            sTabla = "btkt_bm";
                            label58.Visible = true; comboBoxEdit9.Visible = true; iFlOtros = 1; sFieldcb = "biomasa";
                            break;
                        case "CACHAZA":
                            sTabla = "btkt_chz";
                            label58.Visible = false; comboBoxEdit9.Visible = false;
                            break;
                        case "PROD QUIMICOS":
                            sTabla = "btkt_pkim";
                            sCondicion = "";
                            label58.Visible = true; comboBoxEdit9.Visible = true; iFlOtros = 1; sFieldcb = "producto";
                            break;
                        case "OTROS":
                            sTabla = "btkt_otr";
                            label58.Visible = true; comboBoxEdit9.Visible = true; iFlOtros = 1; sFieldcb = "producto";
                            break;
                    }

                    var ifpget1 = procedure.ConvertToList<strucdata.lastfcane>(procedure.Predata(1, "max(ticket) + 1 as ufol", sTabla, "zafra = " + izafra.ToString() + sCondicion, sConexion));

                    int iFpet1 = ifpget1[0].ufol + 1;

                    textBox12.Text = iFpet1.ToString();

                    ComboBoxItemCollection coll8 = comboBoxEdit8.Properties.Items;
                    ComboBoxItemCollection coll7 = comboBoxEdit7.Properties.Items;
                    ComboBoxItemCollection coll9 = comboBoxEdit9.Properties.Items;

                    string sTipf = (sTipo.Trim().Length < 12) ? sTipo : sTipo.Substring(0, 12);

                    var lCarrier2 = procedure.ConvertToList<strucdata.carriers>(procedure.Predata(1, "transportista", "transpt", "tipo_transp = '" +  sTipf + "'", sConexion));

                    foreach (var Itm in lCarrier2)
                    {
                        coll7.Add(Itm.transportista);
                    }

                    var lForw2 = procedure.ConvertToList<strucdata.forwarders>(procedure.Predata(1, "nombre", "fleteros", "zafra = " + izafra.ToString() + " and nombre is not null group by nombre order by nombre", sConexion));

                    foreach (var Itm in lForw2)
                    {
                        coll8.Add(Itm.nombre);
                    }

                    if ( iFlOtros ==1)
                    {
                        var lOthers = procedure.ConvertToList<strucdata.others>(procedure.Predata(1,sFieldcb  + " as producto ", sTabla, sFieldcb + " is not null group by " + sFieldcb + " order by " + sFieldcb, sConexion));

                        foreach(var Itm in lOthers)
                        {
                            coll9.Add(Itm.producto);
                        }

                    }

                    gridControl1.DataSource = HeaderOther(sTabla);


                    break;
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

                    var lCarrier = procedure.ConvertToList<strucdata.carriers>(procedure.Predata(1, "transportista", "transpt", "tipo_transp = 'PETROLEO'", sConexion));

                    foreach (var Itm in lCarrier)
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

                    var lCarrierx = procedure.ConvertToList<strucdata.carriers>(procedure.Predata(1, "transportista", "transpt", "tipo_transp = 'AZUCAR'", sConexion));

                    foreach (var Itm in lCarrierx)
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

        private BindingList<strucdata.headerother> HeaderOther(string sTabla)
        {
            BindingList<strucdata.headerother> lResult = new BindingList<strucdata.headerother>();

            string sCampos = "ticket, ifnull(pesob,pesot) as peso,CONCAT_WS(' ',FECPEN,HORENT) as entro,transportista,fletero";

            var lheadert = procedure.ConvertToList<strucdata.headerother>(procedure.Predata(1, sCampos, sTabla, "zafra = " + izafra.ToString() + " and horsal is null order by ticket desc", sConexion));

            foreach (var Itm in lheadert)
            {
                lResult.Add(new strucdata.headerother
                {
                    ticket = Itm.ticket,
                    peso = Itm.peso,
                    entro = Itm.entro,
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

        private string connectBasc(int iPuerto)
        {

            string sPeso = string.Empty;

            try
            {

                switch(iPuerto)
                {
                    case 3:
                        if (!this.port1.IsOpen)
                        {
                            this.port1.Open();
                            sPeso = "Vuelva a leer";
                        }
                        else
                        {
                            sPeso = "Vuelva a leer";
                        }
                        break;
                    case 4:
                        if (!this.port2.IsOpen)
                        {
                            this.port2.Open();
                            sPeso = "Vuelva a leer";
                        }
                        else
                        {
                            sPeso = "Vuelva a leer";
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                sPeso = "no logra leer";

            }

            return sPeso;
        }

        private string readscales(int iBascula)
        {

            string sPeso = string.Empty;

            try
            {           
                switch (iBascula)
                {
                    case 3:
                        if (this.port1.IsOpen)
                        {
                            string str2 = this.port1.ReadExisting();

                            toolStripLabel6.Text = str2;

                            string sResult = "";
                            if (str2.Length > 0)
                            {

                                int iPos1 = str2.IndexOf("+");
                                int iPos2 = str2.IndexOf("k");

                                string sTempo = str2.Substring((iPos1+1), iPos2-1);

                                toolStripLabel6.Text = sTempo;

                                sResult = sTempo.Substring(0,8);

                                string result = string.Concat(sResult.Where(c => Char.IsDigit(c)));

                                sResult = result;

                                int myInt;
                                bool isNumerical = int.TryParse(sResult, out myInt);

                                sPeso = (isNumerical) ? CleanInput(sResult).Substring(0, 7) : "Vuelva a leer " + sTempo ;
                            }
                            else
                            {
                                sPeso = connectBasc(3);
                            }

                            this.port1.Close();
                        }
                        else
                        {
                            sPeso = connectBasc(3);
                        }
                        break;
                    case 4:
                        if (this.port2.IsOpen)
                        {
                            string str2 = this.port2.ReadExisting();

                           toolStripLabel6.Text = str2 ;

                            if (str2.Length > 0)
                            {

                                int iPos1 = str2.IndexOf(" ");

                                string sTempo = str2.Substring(iPos1+1, 11);

                                toolStripLabel6.Text = sTempo;

                                string sResult;

                                string result = string.Concat(sTempo.Where(c => Char.IsDigit(c)));

                                sResult = result;

                                int myInt;
                                bool isNumerical = int.TryParse(sResult, out myInt);

                                sPeso = (isNumerical) ? CleanInput(sResult).Substring(0, 5) : "Vuelva a leer " + sTempo; 
                            }
                            else
                            {
                                sPeso = connectBasc(4);
                            }

                            this.port2.Close();

                        }
                        else
                        {
                            sPeso = connectBasc(4);
                        }
                        break;
                }

            }
            catch (Exception erorre)
            {
                
            }

            return sPeso;
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        private void evaltkforwarder(string sString)
        {
            string sEvalua = sString;

            Obtaindataburn();

            Obtainassigment();

            var lForwinbatey = procedure.ConvertToList<strucdata.forwinbatey>(procedure.Predata(1, "numtra", "b_ticket", "status = 'BATEY'", sConexion));

            lasignacion = procedure.ConvertToList<strucdata.assigndata>(procedure.Predata(1, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", "", sConexion));

            lTablep = procedure.ConvertToList<strucdata.tpunishment>(procedure.Predata(1, "typecane,typebourn,at_hour,to_hour,percent_punish,subject_analisis", "table_punish", "", sConexion));

            if (sEvalua.Substring(0, 1).ToUpper() == "Z")
            {
                int iZafral = Convert.ToInt32(textEdit2.Text.Substring(1, 4));

                if (iZafral != izafra)
                {
                    XtraMessageBox.Show("No corresponde a la zafra actual...");
                    textEdit2.Text = string.Empty;
                }
                else
                {
                    int iFletero = Convert.ToInt32(textEdit2.Text.Substring(6, textEdit2.Text.Length - 6));

                    var lFrw = lForward.Where(x => Convert.ToInt32(x.num_fle) == iFletero).ToList();

                    var lfrwb = lForwinbatey.Where(x => x.numtra == iFletero).ToList();

                    if (lFrw.Count() > 0 && lfrwb.Count() == 0 )
                    {
                        var lAsig = lasignacion.Where(x => x.fleter == iFletero).ToList();

                        textEdit2.Text = iFletero.ToString();

                        label6.Text = lFrw[0].nombre;

                        if (lAsig.Count() > 0)
                        {
                            fulldataticket(lAsig[0].ticket);
                        }

                        textEdit1.Focus();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encuentra dato de fletero o Se encuentra en BATEY con ticket asignado ...");
                    }


                }
            }
            else
            {
                var lFrw = lForward.Where(x => Convert.ToInt32(x.num_fle) == Convert.ToInt32(sString)).ToList();

                var lfrwb = lForwinbatey.Where(x => x.numtra == Convert.ToInt32(sString)).ToList();

                if (lFrw.Count() > 0 && lfrwb.Count() == 0)
                {
                    label6.Text = lFrw[0].nombre;
                }
                else
                {
                    XtraMessageBox.Show("No se encuentra dato de fletero o Se encuentra en BATEY con ticket asignado ...");
                }
            }
        }

        private void evaltklifting(string sString)
        {
            string sEvalua = sString;

            if (sEvalua.Substring(0, 1).ToUpper() == "Z")
            {
                int iZafral = Convert.ToInt32(textEdit4.Text.Substring(1, 4));

                if (iZafral != izafra)
                {
                    XtraMessageBox.Show("No corresponde a la zafra actual...");
                    textEdit4.Text = string.Empty;
                }
                else
                {
                    int iFletero = Convert.ToInt32(textEdit4.Text.Substring(6, textEdit4.Text.Length - 6));

                    var lFrw = llifting.Where(x => Convert.ToInt32(x.num_fle) == iFletero).ToList();

                    if (lFrw.Count() > 0 )
                    {
                        textEdit4.Text = iFletero.ToString();

                        label7.Text = lFrw[0].nombre;

                        textEdit3.Focus();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encuentra dato de alzadora, reportar a Informatica/Credito...");
                    }


                }
            }
        }

        private void evalticket(string sTicket)
        {

            
            int iTicketFr = 0;

            int iPos = sTicket.ToUpper().IndexOf("T")+1;

            string sRTicket = sTicket.Substring(iPos, sTicket.Length - iPos);

            Obtaindataburn();

            lTablep = procedure.ConvertToList<strucdata.tpunishment>(procedure.Predata(1, "typecane,typebourn,at_hour,to_hour,percent_punish,subject_analisis", "table_punish", "", sConexion));


            if (sTicket.Substring(0, 1).ToUpper() == "Z")
            {

                int iZafral = Convert.ToInt32(sTicket.Substring(1, 4));

                if (iZafral != izafra)
                {
                    XtraMessageBox.Show("No corresponde a la zafra actual...");
                    textEdit1.Text = string.Empty;
                }
                else
                {

                    var ltburn = lticketburn.Where(x => x.ticket == Convert.ToInt32(sRTicket)).ToList();

                    if (ltburn.Count() > 0)
                    {
                        bool boResp = Punishment(Convert.ToInt32(sRTicket), ltburn.ToList());
                    }

                    List<strucdata.ticketfree> lTicketf = new List<strucdata.ticketfree>();

                    lTicketf = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob = 0 or pesob is null)", sConexion));

                    iTicketFr = Convert.ToInt32(sRTicket);

                    var lfTicket = lTicketf.Where(x => x.ticket == iTicketFr).ToList();

                    if (lfTicket.Count() > 0)
                    {
                            label2.Text = "Productor: " + lfTicket[0].nombre_p + " Orden: " + lfTicket[0].ordcte + " Grupo : " + lfTicket[0].nom_grupo + " Tabla: " + lfTicket[0].tabla + " Ciclo: " + lfTicket[0].ciclo;
                            textEdit1.Text = sRTicket;

                            radioButton3.Focus();
                    }
                    else
                    {
                        List<strucdata.ticketfree> lTicketE = new List<strucdata.ticketfree>();

                        lTicketE = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo, fecpes,horent", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob >0)", sConexion));

                        iTicketFr = Convert.ToInt32(sRTicket);

                        var lTicketEE = lTicketE.Where(x => x.ticket == iTicketFr).ToList();

                        if (lTicketEE.Count() > 0)
                        {
                            XtraMessageBox.Show("Ya fue utilizado "+ Convert.ToDateTime( lTicketEE[0].fecpes).ToString("dd/MM/yyyy") + " hora " + lTicketEE[0].horent);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encuentra ticket!!!...");
                        }

                        CleanControls();
                    }


                }

            }
            else
            {
                if (int.TryParse(sRTicket, out iTicketFr))
                {
                    List<strucdata.ticketfree> lTicketf = new List<strucdata.ticketfree>();

                    lTicketf = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo, fecpes,horent", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob >0)", sConexion));


                    var lfTicket = lTicketf.Where(x => x.ticket == iTicketFr).ToList();

                    if (lfTicket.Count() > 0)
                    {
                        label2.Text = "Productor: " + lfTicket[0].nombre_p + " Orden: " + lfTicket[0].ordcte + " Grupo : " + lfTicket[0].nom_grupo + " Tabla: " + lfTicket[0].tabla + " Ciclo: " + lfTicket[0].ciclo;
                    }
                    else
                    {
                        List<strucdata.ticketfree> lTicketE = new List<strucdata.ticketfree>();

                        lTicketE = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo, fecpes,horent", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob >0)", sConexion));

                        iTicketFr = Convert.ToInt32(textEdit1.Text.Trim());

                        var lTicketEE = lTicketE.Where(x => x.ticket == iTicketFr).ToList();

                        if (lTicketEE.Count() > 0)
                        {
                            XtraMessageBox.Show("Ya fue utilizado " + Convert.ToDateTime(lTicketEE[0].fecpes).ToString("dd/MM/yyyy") + " hora " + lTicketEE[0].horent);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encuentra ticket!!!...");
                        }

                        CleanControls();
                    }

                }
                else
                {
                    XtraMessageBox.Show("No es un valor valido de un ticket...");
                }
            }

        }

        private void print_ticket(int iTicket)
        {
            try
            {
                 printticket.CrearTicket ticket = new printticket.CrearTicket();

                string sCampos = "ticket,ordcte,codigo,nombre_p,grupo,nom_grupo,tipocanes,tabla,ciclo,fletero,fecpen,horent,pesob,peson,fecpes,HORSAL as horsal,pesotara,descto,castigo,totaldescuento,alzadora,pesol,totalcastigo";

                var lticketprint = procedure.ConvertToList<strucdata.printtick>(procedure.Predata(1, sCampos, "vb_ticket", "ticket = " + iTicket.ToString(), sConexion));


                ticket.TextoIzquierda(" ");
                ticket.TextoCentro("INGENIO EL CARMEN SA DE CV");
                ticket.TextoCentro("ZAFRA 21-22");
                ticket.TextoIzquierda(" ");
                // ticket.TextoExtremos("FECHA : " + txtFecha.Text, "HORA : " + txtHora.Text);
                ticket.TextoIzquierda("O. CORTE = " + lticketprint[0].ordcte);
                ticket.TextoIzquierda("TICKET   = " + iTicket.ToString());
                ticket.TextoIzquierda("CLAVE    = " + lticketprint[0].codigo);
                ticket.TextoIzquierda(lticketprint[0].nombre_p);
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda("GRUPO    = " + lticketprint[0].grupo);
                ticket.TextoIzquierda(lticketprint[0].nom_grupo);

                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda("TIPO     = " + lticketprint[0].tipocanes);
                ticket.TextoIzquierda("TABLA    = " + lticketprint[0].tabla);
                ticket.TextoIzquierda("CICLO    = " + lticketprint[0].ciclo);
                ticket.TextoIzquierda("FLETERO  = " + lticketprint[0].fletero);
                ticket.TextoIzquierda("ALZ/COSH = " + lticketprint[0].alzadora);
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda("FECHA Y HORA ENTRADA");
                ticket.TextoIzquierda(Convert.ToDateTime(lticketprint[0].FECPEN).ToString("dd/MM/yyyy") +  " " + lticketprint[0].horent);
                ticket.TextoIzquierda(lticketprint[0].pesob.ToString("##,###") + " KG. BRUTO");
                ticket.TextoIzquierda("FECHA Y HORA SALIDA");
                ticket.TextoIzquierda(Convert.ToDateTime( lticketprint[0].fecpes).ToString("dd/MM/yyyy") + " " + lticketprint[0].horsal);
                ticket.TextoIzquierda(lticketprint[0].pesotara.ToString("##,###") + " KG. TARA");

                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(lticketprint[0].pesob.ToString("##,###") + " KG. BRUTO");
                ticket.TextoIzquierda(lticketprint[0].pesotara.ToString("##,###") + " KG. TARA");
                ticket.TextoIzquierda("--------------------");
                ticket.TextoIzquierda(lticketprint[0].peson.ToString("##,###") + " KG. NETO");
                ticket.TextoIzquierda(lticketprint[0].totaldescuento.ToString("##,###") + " kgs. " + lticketprint[0].descto.ToString("0#") + "% DESCTO");
                if (lticketprint[0].castigo >1)
                {
                   ticket.TextoIzquierda(lticketprint[0].totalcastigo.ToString("##,###") + " kgs. " + lticketprint[0].castigo.ToString("0#") + "% CASTIGO");
                }
                
                ticket.TextoIzquierda(lticketprint[0].pesol.ToString("##,###") + " Peso liq");

                //ticket.EncabezadoVenta();
                ticket.lineasGuio();
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                //foreach (DataGridViewRow fila in dataGridView1.Rows)
                //{
                //    ticket.AgregaArticulo(fila.Cells[1].Value.ToString(), int.Parse(fila.Cells[0].Value.ToString()), decimal.Parse(fila.Cells[3].Value.ToString()));
                //}
                //ticket.lineasIgual();
                //ticket.AgregarTotales("          TOTAL COMPRADO : $ ", decimal.Parse(txtCompra.Text));
                //ticket.AgregarTotales("          TOTAL VENDIDO  : $ ", decimal.Parse(txtVenta.Text));
                //ticket.TextoIzquierda(" ");
                //ticket.AgregarTotales("          GANANCIA       : $ ", decimal.Parse(txtResultado.Text));
                //ticket.TextoIzquierda(" ");
                //ticket.TextoIzquierda(" ");
                //ticket.TextoIzquierda(" ");
                //ticket.TextoIzquierda(" ");
                //ticket.TextoIzquierda(" ");
                //ticket.TextoIzquierda(" ");
                //ticket.CortaTicket();

                auditstrip(ticket.Contenidoticket());

                ticket.ImprimirTicket(sPrinterdev);
            }
            catch (Exception eeee) { }

        }

        private void Obtainassigment()
        {
            apilayer Contenedor = new apilayer();

            if (boApi == true)
            {
                string sArmado = procedure.stringexe(4, "", "assigndata", "");

                procedure.Executecmm(sArmado, sConexion);

                string sResulta = Contenedor.ObtaingGet(sApigetasig, "getassign", "", "");

                if (sResulta.Trim().Length > 0)
                {
                    if (!sResulta.Contains("html"))
                    {

                        var vDetails = JObject.Parse(sResulta);

                        var vRegistros = vDetails["registros"];

                        string sRegistros = string.Empty;
                        string sQuery = string.Empty;

                        if (vRegistros is object || vRegistros.Count() > 0)
                        {
                            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<strucdata.Root1>(sResulta);

                            strucdata.Root1 Content3 = (strucdata.Root1)Container;

                            List<strucdata.assigndata> lAsigFr = model.registros.ToList();

                            List<strucdata.assigndata> lAsigHr = procedure.ConvertToList<strucdata.assigndata>(procedure.Predata(1, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", "", sConexion));

                            if (lAsigHr.Count() == 0)
                            {
                                foreach (var item in lAsigFr)
                                {
                                    sRegistros = item.orden + ", " + item.ticket + ", " + item.zona + ", " + item.fleter + ", '" + item.fullnamefleter + "', " + item.lifting + ", '" + item.fullnamelifting + "', " + item.harvest + ", '" + item.fullnameharvest + "'";

                                    sArmado = procedure.stringexe(3, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", sRegistros);

                                    procedure.Executecmm(sArmado, sConexion);

                                    lAsigHr.Add(new strucdata.assigndata { orden = item.orden, ticket = item.ticket, zona = item.zona, fleter = item.fleter, fullnamefleter = item.fullnamefleter, lifting = item.lifting, fullnamelifting = item.fullnamelifting, harvest = item.harvest, fullnameharvest = item.fullnameharvest });

                                }
                            }

                            if (lAsigFr.Count() > lAsigHr.Count())
                            {
                                var lResulta = lAsigFr.Where(x => !lAsigHr.Any(y => x.ticket == y.ticket)).ToList();

                                foreach (var item in lResulta)
                                {
                                    sRegistros = item.orden + ", " + item.ticket + ", " + item.zona + ", " + item.fleter + ", '" + item.fullnamefleter + "', " + item.lifting + ", '" + item.fullnamelifting + "', " + item.harvest + ", '" + item.fullnameharvest + "'";

                                    sArmado = procedure.stringexe(3, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", sRegistros);

                                    procedure.Executecmm(sArmado, sConexion);

                                };

                            }

                        }

                    }
                }

            }

        }

        private void Obtaindataburn()
        {
            apilayer Contenedor = new apilayer();

            if (boApi == true)
            {
                string sArmado = procedure.stringexe(4, "", "databurn", "");

                procedure.Executecmm(sArmado, sConexion);

                string sResulta = Contenedor.ObtaingGet(sApigetasig, "getdataburn", "", "");

                if (sResulta.Trim().Length > 0)
                {
                    if (!sResulta.Contains("html"))
                    {

                        var vDetails = JObject.Parse(sResulta);

                        var vRegistros = vDetails["registros"];

                        string sRegistros = string.Empty;
                        string sQuery = string.Empty;

                        if (vRegistros is object || vRegistros.Count() > 0)
                        {
                            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<strucdata.Root2>(sResulta);

                            strucdata.Root2 Content3 = (strucdata.Root2)Container;

                            List<strucdata.databurni> lAsigFr = model.registros.ToList();

                            List<strucdata.databurni> lAsigHr = procedure.ConvertToList<strucdata.databurni>(procedure.Predata(1, "ticket,tpocan,fecque,horque,typeburn", "databurn", "", sConexion));

                            if (lAsigHr.Count() == 0)
                            {
                                foreach (var item in lAsigFr)
                                {
                                    sRegistros = item.ticket + ", '" + item.tpocan + "', '" + item.fecque + "', '" + item.horque + "', '" + item.typeburn + "'";

                                    sArmado = procedure.stringexe(3, "ticket,tpocan,fecque,horque,typeburn", "databurn", sRegistros);

                                    procedure.Executecmm(sArmado, sConexion);

                                    lAsigHr.Add(new strucdata.databurni { ticket = item.ticket, tpocan = item.tpocan, fecque = item.fecque, horque = item.horque, typeburn = item.typeburn });

                                }
                            }

                            if (lAsigFr.Count() > lAsigHr.Count())
                            {
                                var lResulta = lAsigFr.Where(x => !lAsigHr.Any(y => x.ticket == y.ticket)).ToList();

                                foreach (var item in lResulta)
                                {
                                    sRegistros = item.ticket + ", '" + item.tpocan + "', '" + item.fecque + "', '" + item.horque + "', '" + item.typeburn + "'";

                                    sArmado = procedure.stringexe(3, "ticket,tpocan,fecque,horque,typeburn", "databurn", sRegistros);

                                    procedure.Executecmm(sArmado, sConexion);

                                };

                            }

                        }

                    }
                }

            }

        }

        private void ObtainSerialrfid()
        {
            apilayer Contenedor = new apilayer();

            if (boApi == true)
            {

                string sArmado = string.Empty;

                string sResulta = Contenedor.ObtaingGet(sApigetasig, "getrfid", "", "");

                if (sResulta.Trim().Length > 0)
                {
                    if (!sResulta.Contains("html"))
                    {

                        var vDetails = JObject.Parse(sResulta);

                        var vRegistros = vDetails["registros"];

                        string sRegistros = string.Empty;
                        string sQuery = string.Empty;

                        if (vRegistros.Count() > 0 || vRegistros.Count() > 0)
                        {
                            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<strucdata.Root3>(sResulta);

                            strucdata.Root3 Content3 = (strucdata.Root3)Container;

                            List<strucdata.forwarder> lrfidfo = model.registros.ToList();

                            List<strucdata.forwarder> lrfidfl = procedure.ConvertToList<strucdata.forwarder>(procedure.Predata(1, "num_fle, nombre, serial_rfid", "fleteros", "seltipo = 'FLET' and serial_rfid is not null", sConexion));

                            if (lrfidfo.Count() == lrfidfl.Count())
                            {
                                var lResulta = lrfidfo.Where(x => !lrfidfl.Any(y => x.serial_rfid == y.serial_rfid)).ToList();

                                foreach (var item in lResulta)
                                {
                                    sRegistros = "serial_rfid = '" + item.serial_rfid + "'";

                                    sArmado = procedure.stringexe(2,sRegistros, "fleteros", "num_fle = " + item.num_fle);

                                    procedure.Executecmm(sArmado, sConexion);

                                };

                            }

                        }

                    }
                }

            }

        }

        private void fulldataticket(int iTicket)
        {

            var lAsig = lasignacion.Where(x => x.ticket ==iTicket).ToList();

            var lTicket = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo", "b_ticket", "zafra = " + izafra.ToString() + " and (IFNULL(pesob,0) = 0 and IFNULL(peson,0) = 0) and ticket = " + lAsig[0].ticket , sConexion));


                if (lTicket.Count() > 0)
                {

                    var ltburn = lticketburn.Where(x => x.ticket == lTicket[0].ticket);

                    if (ltburn.Count() > 0)
                    {
                       bool boResp =  Punishment(lAsig[0].ticket, ltburn.ToList());
                    }

                    textEdit1.Text = iTicket.ToString();
                    label2.Text = "Productor: " + lTicket[0].nombre_p + " Orden: " + lTicket[0].ordcte + " Grupo : " + lTicket[0].nom_grupo + " Tabla: " + lTicket[0].tabla + " Ciclo: " + lTicket[0].ciclo;
                    label6.Text = lAsig[0].fullnamefleter;
                    textEdit4.Text = lAsig[0].lifting.ToString();
                    label7.Text = lAsig[0].fullnamelifting;

                    simpleButton9.Focus();
                }
                else
                {
                    List<strucdata.ticketfree> lTicketE = new List<strucdata.ticketfree>();

                    lTicketE = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo, fecpes,horent", "b_ticket", "zafra = " + izafra.ToString() + " and (IFNULL(pesob,0) >0 and IFNULL(peson,0) = 0) and ticket = " + lAsig[0].ticket, sConexion));

                    if (lTicketE.Count() > 0)
                    {
                        XtraMessageBox.Show("El Fletero tiene Ticket " + lTicketE[0].ticket + " en Batey " + Convert.ToDateTime(lTicketE[0].fecpes).ToString("dd/MM/yyyy") + " hora " + lTicketE[0].horent);
                        CleanControls();
                    }
                    else
                    {
                        textEdit1.Focus();
                    }

                    
                }

        }

        public void auditstrip(string sContent)
        {
            string fileName = @"C:\logs\t" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            FileStream stream = null;
            try
            {
                // Create a FileStream with mode CreateNew  
                stream = new FileStream(fileName, FileMode.OpenOrCreate);
                // Create a StreamWriter from FileStream  
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(sContent);
                    //writer.WriteLine("C# Corner Authors");
                    //writer.WriteLine("==================");
                    //writer.WriteLine("Monica Rathbun");
                    //writer.WriteLine("Vidya Agarwal");
                    //writer.WriteLine("Mahesh Chand");
                    //writer.WriteLine("Vijay Anand");
                    //writer.WriteLine("Jignesh Trivedi");
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

        }

        private Boolean Punishment(int iTicket, List<strucdata.databurni> daTicketmark)
        {
            bool boResp = false;

            switch(daTicketmark[0].tpocan)
            {
                case "Q":
                    radioButton3.Checked = true;
                    break;
                case "C":
                    radioButton4.Checked = true;
                    break;
            }

            string sV = "yyyy/MM/dd";

            DateTime sDburn = Convert.ToDateTime(daTicketmark[0].fecque);

            DateTime dtDateburn = DateTime.Parse(sDburn.ToString("yyyy/MM/dd") + " " + daTicketmark[0].horque);

            int iDifhours = ((int)DateTime.Now.Subtract(dtDateburn).TotalHours);

            var lTabpsiniend = lTablep.Where(x => x.typecane == daTicketmark[0].tpocan && x.typebourn == daTicketmark[0].typeburn && x.at_hour == 0).ToList();

            if (lTabpsiniend.Count() > 0)
            {
                int hoursmin = lTabpsiniend.Min(x => x.to_hour);
                int hoursmax = lTabpsiniend.Max(x => x.to_hour);

                var lfticketbr = daTicketmark.Where(x => x.ticket == iTicket).ToList();

                string sArmadopr = procedure.stringexe(2, "numavi = 1, fecque = '" + dtDateburn.ToString("yyyy/MM/dd") + "', horque = '" + dtDateburn.ToString("HH:mm") + "'", "b_ticket", "ticket = " + iTicket + " and zafra = " + izafra);

                procedure.Executecmm(sArmadopr, sConexion);


                var lTabps1 = lTablep.Where(x => x.typecane == daTicketmark[0].tpocan && x.typebourn == daTicketmark[0].typeburn && (iDifhours >= x.at_hour && iDifhours <= x.to_hour)).ToList();

                if (lTabps1.Count() > 0)
                {
                    int iPdesc = lTabps1[0].percent_punish;

                    string sArmado = procedure.stringexe(2, "exceedtimebourn = 0,/* castigo = " + iPdesc + ",*/ percentpunish = " + iPdesc + ", diffhoursbourn = " + iDifhours, "b_ticket", "ticket = " + iTicket + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);

                }
                else
                {
                    var lResul1 = lTabpsiniend.Where(x => iDifhours <= x.to_hour ).ToList();
                    if (lResul1.Count > 0)
                    {
                        int iPdesc = lResul1[0].percent_punish;

                        string sArmado = procedure.stringexe(2, "exceedtimebourn = 0, /* castigo = " + iPdesc + ",*/ percentpunish = " + iPdesc + ", diffhoursbourn = " + iDifhours, "b_ticket", "ticket = " + iTicket + " and zafra = " + izafra);

                        procedure.Executecmm(sArmado, sConexion);

                    }
                    else
                    {
                        XtraMessageBox.Show("Este ticket rebasa el tiempo maximo de horas definido en Orden de Quema....");

                        string sArmado = procedure.stringexe(2, "exceedtimebourn = 1, percentpunish = 20, /* castigo = 20 ,*/ diffhoursbourn = " + iDifhours, "b_ticket", "ticket = " + iTicket + " and zafra = " + izafra);

                        procedure.Executecmm(sArmado, sConexion);

                    }
                }

            }


            return boResp;
        }

        private void print_ticket_o(int iTicket, string sTipo)
        {

            string sCampos = string.Empty;
            string sTabla = string.Empty;

            switch (sTipo.Trim().ToUpper())
            {
                case "AZUCAR":
                    sTabla = "btkt_az";
                    sCampos = "ticket, bultos,transportista as transporte,fletero as chofer,placas,cliente,pesot,pesob,peson,fecpes as fecha,horsal as hora";
                    break;
                case "PETROLEO":
                    sTabla = "btkt_pet";
                    sCampos = "ticket,transportista as transporte,fletero as chofer,placas,procedencia,remision,litros,pesob,pesot,peson,fecpes as fecha,horsal as hora,densidad";
                    break;
                default:

                    switch (sTipo.Trim().ToUpper())
                    {
                        case "MIEL":
                            sTabla = "btkt_miel";
                            break;
                        case "BIOMASA":
                            sTabla = "btkt_bm";
                            break;
                        case "CACHAZA":
                            sTabla = "btkt_chz";
                            break;
                        case "PROD QUIMICOS":
                            sTabla = "btkt_pkim";
                            break;
                        case "OTROS":
                            sTabla = "btkt_otr";
                            break;
                    }

                    sCampos = "ticket,transportista as transporte,fletero as chofer,placas,cliente,pesot,pesob,peson,fecpes as fecha,horsal as hora" + (sTipo.Trim().ToUpper() == "OTROS" || sTipo.Trim().ToUpper() == "PROD QUIMICOS" ? ", producto" :"" ) ;
                    break;
            }

            var lticketprint = procedure.ConvertToList<strucdata.ticketot>(procedure.Predata(1, sCampos, sTabla, "ticket = " + iTicket.ToString() + " and zafra = (select zafra from zafra where actual = 1)", sConexion));

            try
            {
                printticket.CrearTicket ticket = new printticket.CrearTicket();

                ticket.TextoIzquierda(" ");
                ticket.TextoCentro("INGENIO EL CARMEN SA DE CV");
                ticket.TextoCentro("ZAFRA " + lDatazafra[0].zafra_);
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoCentro(sTipo.Trim().ToUpper());
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda("TICKET   = " + iTicket.ToString());

                switch (sTipo.Trim().ToUpper())
                {
                    case "AZUCAR":
                        ticket.TextoIzquierda("BULTOS   = " + lticketprint[0].bultos.ToString());
                        break;
                    case "OTROS": 
                    case "PROD QUIMICOS":
                        ticket.TextoIzquierda("PRODUCTO = " + lticketprint[0].producto.ToString());
                        break;
                }

                ticket.TextoIzquierda("TRANSPORTISTA");
                ticket.TextoIzquierda(lticketprint[0].transporte);
                ticket.TextoIzquierda("OPERADOR");
                ticket.TextoIzquierda(lticketprint[0].chofer);
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda("PLACAS    = " + lticketprint[0].placas);

                switch (sTipo.Trim().ToUpper())
                {
                    case "AZUCAR":
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("CLIENTE");
                        ticket.TextoIzquierda(lticketprint[0].cliente);
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("TARA  = " + lticketprint[0].pesot);
                        ticket.TextoIzquierda("BRUTO = " + lticketprint[0].pesob);
                        ticket.TextoIzquierda("NETO  = " + lticketprint[0].peson);
                        break;
                    case "PETROLEO":
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("PROCEDENCIA");
                        ticket.TextoIzquierda(lticketprint[0].procedencia);
                        ticket.TextoIzquierda("REMISION  = " + lticketprint[0].remision);
                        ticket.TextoIzquierda("LITROS    = " + lticketprint[0].litros);
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("BRUTO = " + lticketprint[0].pesob);
                        ticket.TextoIzquierda("TARA  = " + lticketprint[0].pesot);
                        ticket.TextoIzquierda("NETO  = " + lticketprint[0].peson);
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("DENSIDAD = " + lticketprint[0].densidad);
                        break;
                    case "BIOMASA":
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("BRUTO = " + lticketprint[0].pesob);
                        ticket.TextoIzquierda("TARA  = " + lticketprint[0].pesot);
                        ticket.TextoIzquierda("NETO  = " + lticketprint[0].peson);
                        break;
                    default:
                        ticket.TextoIzquierda(" ");
                        ticket.TextoIzquierda("TARA  = " + lticketprint[0].pesot);
                        ticket.TextoIzquierda("BRUTO = " + lticketprint[0].pesob);
                        ticket.TextoIzquierda("NETO  = " + lticketprint[0].peson);
                        break;
                }

                ticket.TextoIzquierda(" ");
                ticket.TextoExtremos("FECHA : " + lticketprint[0].fecha, "HORA : " + lticketprint[0].hora);



                //ticket.EncabezadoVenta();
                ticket.lineasGuio();
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");

                auditstrip(ticket.Contenidoticket());

                ticket.ImprimirTicket(sPrinterdev);
            }
            catch (Exception eeee) { }

        }

        private string readscaleauto()
        {
            string sPuerto = (string)toolStripComboBox2.SelectedItem;

            int iPuerto = Convert.ToInt32(sPuerto.Substring(3, 1));

            string sRespuesta = string.Empty;

            if (iPuerto >0)
            {
                sRespuesta = readscales(iPuerto);
            }
            else
            {
                sRespuesta = "Determine puerto de bascula....";
            }

            return sRespuesta;
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
                textEdit6.Enabled = true;
                simpleButton10.Enabled = true;
            }
            else
            {
                toolStripTextBox1.Text = string.Empty; toolStripTextBox2.Text = string.Empty;
                XtraMessageBox.Show("Usuario no valido para acceder...");
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

            simpleButton13.Visible = false;

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
                default:
                    tabControl1.SelectedTab = tabPage7;
                    tabControl2.SelectedTab = tabPage8;
                    label50.Text = sOpcion;

                    if (sOpcion == "CACHAZA" || sOpcion == "OTROS")
                    {
                        simpleButton13.Visible = true;
                    }

                    if (sOpcion == "BIOMASA")
                    {
                        label46.Text = "Bruto"; label54.Text = "Bruto"; label59.Text = "Tara";label45.Text = "Proveedor";
                    }

                    if (sOpcion == "PROD QUIMICOS")
                    {
                        label46.Text = "Bruto"; label54.Text = "Bruto"; label59.Text = "Tara"; label45.Text = "Proveedor";
                    }

                    else
                    {
                        label46.Text = "Tara"; label54.Text = "Tara"; label59.Text = "Bruto";label45.Text = "Transportista";
                    }

                    PrepareData(3);
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

            if (Convert.ToInt32( textEdit6.Text.Trim()) < 1) { XtraMessageBox.Show("Debe anotar peso tara...");return; }

            if (int.TryParse(textEdit6.Text, out iTara))
            {
                if (XtraMessageBox.Show("Procede a dar salida a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    string sActualiza = string.Empty;

                    Obtaindataburn();

                    var lTicketS = procedure.ConvertToList<strucdata.tickettmp>(procedure.Predata(1, "ticket,pesob", "b_ticket", "zafra = " + izafra.ToString() + " and (IFNULL(pesob,0) >0 and IFNULL(peson,0) = 0) and ticket = " + textEdit5.Text, sConexion));

                    if (lTicketS.Count() >0)
                    {
                        if (lTicketS[0].pesob == Convert.ToDouble( textEdit7.Text))
                        {
                            XtraMessageBox.Show("Existe error en Peso Bruto y Peso Neto favor de clarificar Peso Tara para continuar...");
                            return;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Existe un fallo en el ticket favor de avisar a Informatica...");
                        return;
                    }

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
                    sActualiza = sActualiza + ", status = 'OK', diazafra = " + DiaZa.ToString()+ ", hr_code = " + iHorP + ", fechakk = '" + dFechakk + "', sal_usuario = '" + sUserC + "', exittime=now()";


                    string sArmado = procedure.stringexe(2, sActualiza, "b_ticket", " ticket = " + textEdit5.Text + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);

                    if (boFlrfid == true)
                    {
                        sArmado = procedure.stringexe(2, "out_ = now(), State_ = 2", "rfid_hist", " serial = '" + sSerialrfid + "'");

                        procedure.Executecmm(sArmado, sConexion);

                        boFlrfid = false;
                    }


                    if (XtraMessageBox.Show("Imprime Ticket?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        print_ticket(Convert.ToInt32(textEdit5.Text)); ;
                    }

                    textEdit6.Text = string.Empty; textEdit8.Text = string.Empty; label13.Text = string.Empty; label14.Text = string.Empty;
                    gridControl1.DataSource = Headert();

                    CleanControls();


                }

            }
            else
            {
                XtraMessageBox.Show("Debe anotar peso tara...");
            }
            Obtainassigment();

        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {


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

            print_ticket_o(Convert.ToInt32(label29.Text), "AZUCAR");

            PrepareData(1);

            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            label24.Text = string.Empty;

            

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox8.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso definido..."); return; }


            if (XtraMessageBox.Show("Procede a dar entrada a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes) { return; }

            string sInserta = string.Empty;

            int iBruto = 0;

            if (int.TryParse(textBox8.Text, out iBruto))
            {


                string sFen = DateTime.Now.ToString("yyyy-MM-dd");
                string sHen = DateTime.Now.ToString("HH:mm");


                string sCampos = "zafra,ticket,fecpen,horent,pesob,material,status,usuario";

                sInserta = izafra.ToString().Trim() + "," + textBox5.Text.Trim() + ",";
                sInserta = sInserta + "'" + sFen + "', '" + sHen + "', " + iBruto.ToString() + ",5,'PATIO', '" + sUserC + "'";

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

            if (comboBoxEdit6.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener Conductor..."); return; }
            if (textBox7.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener placas de la unidad..."); return; }

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

            sActualiza = sActualiza + ", fletero = '" + comboBoxEdit6.Text + "', placas = '" + textBox7.Text + "'";

            string sArmado = procedure.stringexe(2, sActualiza, "btkt_pet", " ticket = " + label37.Text + " and zafra = " + izafra);

            print_ticket_o(Convert.ToInt32(label37.Text), "PETROLEO");

            try
            {
                procedure.Executecmm(sArmado, sConexion);
            }
            catch (Exception erorre)
            {
                Console.WriteLine("{0} Avisar a Informatica.", erorre);
            }

            PrepareData(2);

            textBox10.Text = string.Empty;
            textBox9.Text = string.Empty;
            label42.Text = string.Empty;
            label37.Text = string.Empty;

            comboBoxEdit4.Text = string.Empty;
            comboBoxEdit5.Text = string.Empty;
            textBox6.Text = string.Empty;

        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
                textEdit3.Text = sRespuesta;
            }
            else
            {
                
                XtraMessageBox.Show(sRespuesta);
            }
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
                textEdit6.Text = sRespuesta;
            }
            else
            {
               
                XtraMessageBox.Show(sRespuesta);
            }

        }

        private void simpleButton3_Click_1(object sender, EventArgs e)
        {
            int iPeso = 0;

            if (textEdit3.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar peso inicial..."); textEdit3.Text = string.Empty; return; }

            if (int.TryParse(textEdit3.Text, out iPeso))
            {

                if (label2.Text.Trim().Length == 0) { XtraMessageBox.Show("No ha focalizado ningun ticket..."); return; }

                if (label6.Text.Trim().Length == 0) { XtraMessageBox.Show("No ha focalizado ningun fletero..."); return; }


                if (XtraMessageBox.Show("Procede a guardar datos capturados?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {

                    Obtaindataburn();

                    string sCondicion = string.Empty;
                    int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));
                    string sTipoc = (radioButton3.Checked==true) ? "Q'" : "C'";

                    if (sTipoc.Trim().Length == 0)
                    {
                        XtraMessageBox.Show("Defina el tipo de caña antes de proseguir...");
                        return;
                    }

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
                    sCondicion = sCondicion + ", tpocan = '" + sTipoc;
                    sCondicion = sCondicion + ", numavi = case when fecque is null then 2000 else 1 end, material = 1, peson = 0, pesot = 0, pesol = 0, pesob = " + textEdit3.Text;
                    sCondicion = sCondicion + ", fecpen = '" + DateTime.Now.ToString("yyyy-MM-dd") + "', horent = '" + DateTime.Now.ToString("HH:mm") + "'";
                    sCondicion = sCondicion + ", nofecha =" + sNofecha;
                    sCondicion = sCondicion + ",  fecque = ifnull(fecque,'" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "'),horque = ifnull(horque,'18:00')";
                    sCondicion = sCondicion + ", status = 'BATEY', diazafra = 0, ent_usuario = '" + sUserC + "', entrytime = now()";

                    string sArmado = procedure.stringexe(2, sCondicion, "b_ticket", " ticket = " + textEdit1.Text + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);

                    if (boFlrfid == true)
                    {
                        sArmado = procedure.stringexe(2, "Entry = now(), State_ = 1", "rfid_hist", " serial = '" + sSerialrfid + "'");

                        procedure.Executecmm(sArmado, sConexion);

                        boFlrfid = false;
                    }

                    gridControl1.DataSource = Headert();

                    CleanControls();

                }

            }
            else
            {
                XtraMessageBox.Show("No es un valor valido Peso Inicial...");
            }
            Obtainassigment();
        }

        private void textEdit2_EditValueChanged_1(object sender, EventArgs e)
        {

            
        }

        private void simpleButton2_Click_1(object sender, EventArgs e)
        {
            int iFletero = 0;

            if (textEdit2.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el codigo de fletero a conectar..."); textEdit2.Text = string.Empty; return; }

            if (int.TryParse(textEdit2.Text, out iFletero))
            {

                Obtaindataburn();

                Obtainassigment();

                lasignacion = procedure.ConvertToList<strucdata.assigndata>(procedure.Predata(1, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", "", sConexion));

                lTablep = procedure.ConvertToList<strucdata.tpunishment>(procedure.Predata(1, "typecane,typebourn,at_hour,to_hour,percent_punish,subject_analisis", "table_punish", "", sConexion));

                var lAsig = lasignacion.Where(x => x.fleter == Convert.ToInt32(textEdit2.Text)).ToList();

                if (lAsig.Count() > 0)
                {
                    fulldataticket(lAsig[0].ticket);

                    if (label2.Text.Trim().Length == 0)
                    {
                        evaltkforwarder(textEdit2.Text);
                    }
                }
                else
                {
                    var lFrw = lForward.Where(x => Convert.ToInt32(x.num_fle) == iFletero).ToList();

                    if (lFrw.Count() > 0)
                    {
                        label6.Text = lFrw[0].nombre;
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encuentra el dato de fletero, reportar a Informatica/Credito...");
                    }
                    
                }
            }
            else
            {
                evaltkforwarder(textEdit2.Text);

            }
        }

        private void textEdit2_EditValueChanging(object sender, ChangingEventArgs e)
        {

        }

        private void textEdit2_KeyDown(object sender, KeyEventArgs e)
        {
          
        }

        private void textEdit2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Enter)
            {
                evaltkforwarder(textEdit2.Text);
            }
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
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
                    Obtaindataburn();

                    lTablep = procedure.ConvertToList<strucdata.tpunishment>(procedure.Predata(1, "typecane,typebourn,at_hour,to_hour,percent_punish,subject_analisis", "table_punish", "", sConexion));

                    var ltburn = lticketburn.Where(x => x.ticket == iTicketFr).ToList();

                    if (ltburn.Count() > 0)
                    {
                        bool boResp = Punishment(iTicketFr, ltburn.ToList());
                    }

                    label2.Text = "Productor: " + lfTicket[0].nombre_p + " Orden: " + lfTicket[0].ordcte + " Grupo : " + lfTicket[0].nom_grupo + " Tabla: " + lfTicket[0].tabla + " Ciclo: " + lfTicket[0].ciclo;
                }
                else
                {
                    List<strucdata.ticketfree> lTicketE = new List<strucdata.ticketfree>();

                    lTicketE = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo, fecpes,horent", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob >0)", sConexion));

                    iTicketFr = Convert.ToInt32(textEdit1.Text.Trim());

                    var lTicketEE = lTicketE.Where(x => x.ticket == iTicketFr).ToList();

                    if (lTicketEE.Count() > 0)
                    {
                        XtraMessageBox.Show("Ya fue utilizado " + Convert.ToDateTime(lTicketEE[0].fecpes).ToString("dd/MM/yyyy") + " hora " + lTicketEE[0].horent);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encuentra ticket!!!...");
                    }

                    CleanControls();
                }

            }
            else
            {

                if (textEdit1.Text.Substring(0, 1).ToUpper() == "Z")
                {

                    int iZafral = Convert.ToInt32(textEdit1.Text.Substring(1, 4));

                    if (iZafral != izafra)
                    {
                        XtraMessageBox.Show("No corresponde a la zafra actual...");
                        textEdit1.Text = string.Empty;
                    }
                    else
                    {
                        int iPos = textEdit1.Text.ToUpper().IndexOf("T") + 1;

                        string sRTicket = textEdit1.Text.Substring(iPos, textEdit1.Text.Length - iPos);

                        List<strucdata.ticketfree> lTicketf1 = new List<strucdata.ticketfree>();

                        lTicketf1 = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob = 0 or pesob is null)", sConexion));

                        iTicketFr = Convert.ToInt32(sRTicket);

                        var lfTicket = lTicketf.Where(x => x.ticket == iTicketFr).ToList();

                        if (lfTicket.Count() > 0)
                        {
                            var ltburn = lticketburn.Where(x => x.ticket == iTicketFr);

                            if (ltburn.Count() > 0)
                            {
                               bool boResp = Punishment(iTicketFr, ltburn.ToList());
                            }

                            label2.Text = "Productor: " + lfTicket[0].nombre_p + " Orden: " + lfTicket[0].ordcte + " Grupo : " + lfTicket[0].nom_grupo + " Tabla: " + lfTicket[0].tabla + " Ciclo: " + lfTicket[0].ciclo;
                            textEdit1.Text = sRTicket;

                            radioButton3.Focus();
                        }
                        else
                        {
                            List<strucdata.ticketfree> lTicketE = new List<strucdata.ticketfree>();

                            lTicketE = procedure.ConvertToList<strucdata.ticketfree>(procedure.Predata(1, "ticket,nombre_p,ordcte, nom_grupo, tabla, ciclo, fecpes,horent", "b_ticket", "zafra = " + izafra.ToString() + " and (pesob >0)", sConexion));

                            iTicketFr = Convert.ToInt32(sRTicket);

                            var lTicketEE = lTicketE.Where(x => x.ticket == iTicketFr).ToList();

                            if (lTicketEE.Count() > 0)
                            {
                                XtraMessageBox.Show("Ya fue utilizado " + Convert.ToDateTime(lTicketEE[0].fecpes).ToString("dd/MM/yyyy") + " hora " + lTicketEE[0].horent);
                            }
                            else
                            {
                                XtraMessageBox.Show("No se encuentra ticket!!!...");
                            }

                            CleanControls();
                        }


                    }

                }
            }
        }

        private void textEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                evalticket(textEdit1.Text);
            }
        }

        private void simpleButton5_Click_1(object sender, EventArgs e)
        {
            CleanControls();
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void simpleButton4_Click_1(object sender, EventArgs e)
        {
            int iAlza = 0;

            if (textEdit4.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar el codigo de Alzadora a conectar..."); textEdit4.Text = string.Empty; return; }

            if (int.TryParse(textEdit4.Text, out iAlza))
            {

                var lAlzad = llifting.Where(x => Convert.ToInt32(x.num_fle) == iAlza).ToList();

                if (lAlzad.Count() > 0)
                {
                    label7.Text = (iAlza > 0) ? lAlzad[0].nombre : "Sin Alzadora";
                }
                else
                {
                    XtraMessageBox.Show("No se encuentra dato de alzadora, reportar a Informatica/Credito...");
                }

            }
            else
            {
                XtraMessageBox.Show("No es un valor valido identidad Alzadora...");
            }
        }

        private void textEdit4_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void textEdit4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                evaltklifting(textEdit4.Text);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            print_ticket_o(1,"AZUCAR");
        }

        private void simpleButton7_Click_1(object sender, EventArgs e)
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

                sInserta = izafra.ToString().Trim() + "," + textBox1.Text.Trim() + "," + iIdtr.ToString() + ",'" + comboBoxEdit3.Text.Trim() + "','" + comboBoxEdit2.Text + "','" + textEdit11.Text + "',";
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

        private void simpleButton11_Click(object sender, EventArgs e)
        {
            if (comboBoxEdit8.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener un chofer..."); return; }
            if (textEdit12.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener placas de la unidad..."); return; }
            if (comboBoxEdit7.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener transportista..."); return; }
            if (textBox11.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso ..."); return; }


            if (XtraMessageBox.Show("Procede a dar entrada a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            string sInserta = string.Empty;

            String sTipotr = (label50.Text.Trim().Length >= 12) ? label50.Text.Trim().Substring(0, 12): label50.Text;

            var lIdtr = lTransporter.Where(x => x.transportista == comboBoxEdit7.Text && x.tipo_transp == sTipotr).ToList();

            if (lIdtr.Count() == 0) {

                if (XtraMessageBox.Show("No se encuentra el Transportista, se agrega al Catalogo?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    string sFields = "id_transp,transportista,tipo_transp";

                    string sTypeTag = string.Empty;

                    if (label50.Text.Trim().Length >= 12)
                    {
                        sTypeTag = label50.Text.Substring(0, 12);
                    }
                    else
                    {
                        sTypeTag = label50.Text;
                    }

                    string sValues = "maxtranspt(), '" + comboBoxEdit7.Text + "', '" + sTypeTag + "'" ;

                    string sArmy = procedure.stringexe(3, sFields, "transpt", sValues);

                    procedure.Executecmm(sArmy, sConexion);

                    lTransporter.Clear();

                    lTransporter = procedure.ConvertToList<strucdata.transporter>(procedure.Predata(1, "id_transp, transportista, tipo_transp", "transpt", "", sConexion));

                    lIdtr = lTransporter.Where(x => x.transportista == comboBoxEdit7.Text && x.tipo_transp == label50.Text).ToList();

                }
                   
            }

            var lDatot = lDroth.Where(x => x.plates == textEdit12.Text).ToList();

            if (lDatot.Count() == 0 && label50.Text != "PROD QUIMICOS" )
            {
                if (XtraMessageBox.Show("Conservar la tara y el transportista para la proxima pesada?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sCamposr = "id_transp,plates,driver,tarew";

                    var lTransptt = lTransporter.Where(x => x.transportista == comboBoxEdit7.Text).ToList();

                    string sValores = lTransptt[0].id_transp + ", '" + textEdit12.Text + "','" + comboBoxEdit8.Text + "'," + textBox11.Text;

                    string sExetrns = procedure.stringexe(3, sCamposr, "qdrivers", sValores);

                    procedure.Executecmm(sExetrns, sConexion);

                    lDroth.Clear();

                    lDroth = procedure.ConvertToList<strucdata.qdrivers>(procedure.Predata(1, "id_transp,plates,driver,tarew", "qdrivers", "", sConexion));

                }
            }
            else
            {

                if (label50.Text != "PROD QUIMICOS")
                {
                    string sActua = "tarew = " + textBox11.Text;

                    string sExetrns = procedure.stringexe(2, sActua, "qdrivers", "plates = '" + textEdit12.Text + "'");

                    procedure.Executecmm(sExetrns, sConexion);

                    lDroth.Clear();

                    lDroth = procedure.ConvertToList<strucdata.qdrivers>(procedure.Predata(1, "id_transp,plates,driver,tarew", "qdrivers", "", sConexion));

                }

            }

            int iTara = 0;

            if (int.TryParse(textBox11.Text, out iTara))
            {
                string sTabdest = string.Empty;
                string sMaterial = string.Empty;
                string sFPeso = "pesot";

                switch (label50.Text.Trim().ToUpper())
                {
                    case "MIEL":
                        sTabdest = "btkt_miel";
                        sMaterial = "7";
                        break;
                    case "BIOMASA":
                        sTabdest = "btkt_bm";
                        sMaterial = "6";
                        sFPeso = "pesob";
                        break;
                    case "CACHAZA":
                        sTabdest = "btkt_chz";
                        sMaterial = "7";
                        break;
                    case "PROD QUIMICOS":
                        sTabdest = "btkt_pkim";
                        sMaterial = "8";
                        break;
                    case "OTROS":
                        sTabdest = "btkt_otr";
                        sMaterial = "12";
                        break;
                }

                int iIdtr = lIdtr[0].id_transp;
                string sFen = DateTime.Now.ToString("yyyy-MM-dd");
                string sHen = DateTime.Now.ToString("HH:mm");

                string sCampos = "zafra,ticket,id_transp,transportista,fletero,placas,fecpen,horent,"+ sFPeso.Trim() + ",material,status,ent_usuario";

                sInserta = izafra.ToString().Trim() + "," + textBox12.Text.Trim() + "," + iIdtr.ToString() + ",'" + comboBoxEdit7.Text.Trim() + "','" + comboBoxEdit8.Text + "','" + textEdit12.Text + "',";
                sInserta = sInserta + "'" + sFen + "', '" + sHen + "', " + iTara.ToString() + "," + sMaterial + ",'PATIO', '" + sUserC + "'";

                string sArmado = procedure.stringexe(3, sCampos, sTabdest, sInserta);

                procedure.Executecmm(sArmado, sConexion);

                PrepareData(3);

            }
            else
            {
                XtraMessageBox.Show("El peso tara debe ser numerico...");
            }
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            int iTara;

            if (textBox14.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar peso tara...");  return; }

            if (Convert.ToInt32(textBox14.Text.Trim()) < 1) { XtraMessageBox.Show("Debe anotar peso tara..."); return; }

            if (label50.Text.Trim().ToUpper() == "OTROS" || label50.Text.Trim().ToUpper() == "PROD QUIMICOS")
            {
                if (comboBoxEdit9.Text.Trim().Length == 0) { XtraMessageBox.Show("Defina producto de salida..."); return; }
            }

            if (int.TryParse(textBox14.Text, out iTara))
            {
                if (XtraMessageBox.Show("Procede a dar salida a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    string sActualiza = string.Empty;
                    string sTabdest = string.Empty;
                    string sFpeso = "pesot";



                    switch(label50.Text.Trim().ToUpper())
                    {
                        case "MIEL":
                            sTabdest = "btkt_miel";
                            break;
                        case "BIOMASA":
                            sTabdest = "btkt_bm";
                            break;
                        case "CACHAZA":
                            sTabdest = "btkt_chz";
                            break;
                        case "PROD QUIMICOS":
                            sTabdest = "btkt_otr";
                            break;
                        case "OTROS":
                            sTabdest = "btkt_otr";
                            break;
                    }

                    if (label50.Text == "BIOMASA")
                    {
                        sFpeso = "pesob";
                    }


                    var lTicketS = procedure.ConvertToList<strucdata.tickettmp>(procedure.Predata(1, "ticket,"+ sFpeso , sTabdest, "zafra = " + izafra.ToString() + " and (IFNULL(" +sFpeso+ ",0) >0 and IFNULL(peson,0) = 0) and ticket = " + label51.Text, sConexion));

                    if (lTicketS.Count() > 0)
                    {
                        if (lTicketS[0].pesot == Convert.ToDouble(textBox14.Text))
                        {
                            XtraMessageBox.Show("Existe error en Peso Bruto y Peso Neto favor de clarificar Peso...");
                            return;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Existe un fallo en el ticket favor de avisar a Informatica...");
                        return;
                    }

                    int iHora = Convert.ToInt32(DateTime.Now.ToString("HH"));

                    int iHorP = lhorlab.Where(x => x.hourd == iHora).ToList()[0].hourt;

                    int iHour = Convert.ToInt32(DateTime.Now.ToString("HH"));

                    TimeSpan tDifer = DateTime.Now - Convert.ToDateTime(dzafra);

                    string sNofecha = DateTime.Now.ToString("yyMMdd");

                    if (iHora < 6) { sNofecha = DateTime.Now.AddDays(-1).ToString("yyMMdd"); }

                    var dFechakk = (iHora < 6) ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                    int DiaZa = fzafra + tDifer.Days;

                    if (iHora < 6) { DiaZa = fzafra + (tDifer.Days - 1); }

                    if (label50.Text == "BIOMASA")
                    {
                        sFpeso = "pesot";
                    }
                    else
                    {
                        sFpeso = "pesob";
                    }

                    sActualiza = sFpeso + " = " + textBox14.Text; 
                    sActualiza = sActualiza + ", peson = " + label55.Text;
                    sActualiza = sActualiza + ", nofecha =" + sNofecha;

                    if (label50.Text.Trim().ToUpper() == "OTROS" || label50.Text.Trim().ToUpper() == "PROD QUIMICOS")
                    {
                        sActualiza = sActualiza + ", producto = '" + comboBoxEdit9.Text + "'" ;
                    }

                    if (label50.Text.Trim().ToUpper() == "BIOMASA")
                    {
                        sActualiza = sActualiza + ", biomasa = '" + comboBoxEdit9.Text + "'";
                    }


                    sActualiza = sActualiza + ", fecpes = '" + DateTime.Now.ToString("yyyy-MM-dd") + "', horsal = '" + DateTime.Now.ToString("HH:mm") + "', hora = " + DateTime.Now.ToString("HH");
                    sActualiza = sActualiza + ", status = 'OK', diazafra = " + DiaZa.ToString() + ", hr_code = " + iHorP + ", sal_usuario = '" + sUserC + "'";


                    string sArmado = procedure.stringexe(2, sActualiza, sTabdest, " ticket = " + label51.Text + " and zafra = " + izafra);

                    procedure.Executecmm(sArmado, sConexion);


                    if (XtraMessageBox.Show("Imprime Ticket?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        print_ticket_o(Convert.ToInt32(label51.Text),label50.Text); ;
                    }

                    label51.Text = string.Empty;label57.Text = string.Empty;textBox14.Text = string.Empty;label55.Text = string.Empty;comboBoxEdit9.Text = string.Empty;
                    gridControl1.DataSource = HeaderOther(sTabdest);

                    CleanControls();

                }

            }
            else
            {
                XtraMessageBox.Show("Debe anotar peso tara...");
            }
            Obtainassigment();
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            int iPesob = 0;

            if (textBox14.Text.Trim().Length == 0) { return; }

            if (int.TryParse(textBox14.Text, out iPesob))
            {
                if (label50.Text == "BIOMASA")
                {
                    label55.Text = (Convert.ToInt32(label57.Text) -  Convert.ToInt32(textBox14.Text)).ToString();
                }
                else
                {
                    label55.Text = (Convert.ToInt32(textBox14.Text) - Convert.ToInt32(label57.Text)).ToString();
                }
                
            }
            else
            {
                XtraMessageBox.Show("El peso debe ser numerico...");
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox8.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe contener peso definido..."); return; }


            if (XtraMessageBox.Show("Procede a dar entrada a la unidad?", "Confirme", MessageBoxButtons.YesNo) != DialogResult.Yes) { return; }

            string sInserta = string.Empty;

            int iBruto = 0;

            if (int.TryParse(textBox8.Text, out iBruto))
            {


                string sFen = DateTime.Now.ToString("yyyy-MM-dd");
                string sHen = DateTime.Now.ToString("HH:mm");


                string sCampos = "zafra,ticket,fecpen,horent,pesob,material,status,usuario";

                sInserta = izafra.ToString().Trim() + "," + textBox5.Text.Trim() + ",";
                sInserta = sInserta + "'" + sFen + "', '" + sHen + "', " + iBruto.ToString() + ",5,'PATIO', '" + sUserC + "'";

                string sArmado = procedure.stringexe(3, sCampos, "btkt_pet", sInserta);

                procedure.Executecmm(sArmado, sConexion);

                PrepareData(2);


            }
            else
            {
                XtraMessageBox.Show("El peso bruto debe ser numerico...");
            }

        }

        private void simpleButton13_Click(object sender, EventArgs e)
        {
            if (comboBoxEdit7.Text.Trim().Length == 0) { XtraMessageBox.Show("Debe anotar codigo de transportista..."); return; }

            int iTransp = 0;

            if (int.TryParse(comboBoxEdit7.Text, out iTransp))
            {
                var lDataTr = lDroth.Where(x => x.id_transp == iTransp).ToList();

                if (lDataTr.Count() == 0) { XtraMessageBox.Show("No existe el transportista, anote la descripcion en el mismo campo..."); simpleButton13.Visible = false;  return; }

                textEdit12.Text = lDataTr[0].plates;
                comboBoxEdit8.Text = lDataTr[0].driver;
                textBox11.Text = lDataTr[0].tarew.ToString();
                comboBoxEdit7.Text = lTransporter.Where(x => x.id_transp == iTransp).ToList()[0].transportista;
            }


        }

        private void simpleButton14_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
                textBox2.Text = sRespuesta;
            }
            else
            {
                
                XtraMessageBox.Show(sRespuesta);
            }

        }

        private void simpleButton15_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
                textBox8.Text = sRespuesta;
            }
            else
            {
                
                XtraMessageBox.Show(sRespuesta);
            }

        }

        private void simpleButton16_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {                
                textBox11.Text = sRespuesta;
            }
            else
            {
                XtraMessageBox.Show(sRespuesta);
            }

        }

        private void simpleButton17_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
                
                textBox3.Text = sRespuesta;
            }
            else
            {
                XtraMessageBox.Show(sRespuesta);
            }
        }

        private void simpleButton18_Click(object sender, EventArgs e)
        {
            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
                textBox10.Text = sRespuesta;
            }
            else
            {
                
                XtraMessageBox.Show(sRespuesta);
            }

        }

        private void simpleButton19_Click(object sender, EventArgs e)
        {

            string sRespuesta = readscaleauto();
            int iLectura = 0;

            if (int.TryParse(sRespuesta, out iLectura))
            {
               
                textBox14.Text = sRespuesta;
            }
            else
            {
                XtraMessageBox.Show(sRespuesta);
            }

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (boApi == true) { boApi = false; toolStripStatusLabel3.Text = "No lectura API";  } else { boApi = true;toolStripStatusLabel3.Text = "Lectura API"; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var lForwrfid = procedure.ConvertToList<strucdata.rfidhistory>(procedure.Predata(1, "num_fle,nombre,Serial,Entry,State_,Out_", "vDepenrfid", "Entry is null and out_ is null", sConexion));

            if (lForwrfid.Count() > 0)
            {
                foreach (var itm in lForwrfid)
                {
                    comboBox1.Items.Add(itm.num_fle + " " + itm.nombre);
                }
            }

            var lForwrfido = procedure.ConvertToList<strucdata.rfidhistory>(procedure.Predata(1, "num_fle,nombre,Serial,Entry,State_,Out_", "vDepenrfid", "Entry is not null and out_ is null", sConexion));

            if (lForwrfido.Count() > 0)
            {
                foreach (var itm in lForwrfido)
                {
                    comboBox2.Items.Add(itm.num_fle + " " + itm.nombre);
                }
            }
            toolStripStatusLabel4.Text = "Lectura RFID";
        }

        private void simpleButton20_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                string sConten = strucdata.stringBetween(comboBox1.Text, comboBox1.Text.Substring(0,1) ," ");
                int iValue = 0;

                Obtaindataburn();

                Obtainassigment();

                var lTassign = procedure.ConvertToList<strucdata.assigndata>(procedure.Predata(1, "orden,ticket,zona,fleter,fullnamefleter,lifting,fullnamelifting,harvest,fullnameharvest", "vassigndata", "", sConexion));

                var lDprfid = procedure.ConvertToList<strucdata.forwarder>(procedure.Predata(1, "num_fle,nombre,Serial", "vdepenrfid", "", sConexion));

                if (int.TryParse(sConten, out iValue))
                {
                    var lTicketa = lTassign.Where(x => x.fleter == iValue).ToList();

                    var lDpn = lDprfid.Where(x => x.num_fle == sConten).ToList();

                    if (lTicketa.Count() > 0)
                    {
                        boFlrfid = true;

                        sSerialrfid = lDpn[0].serial_rfid;
                       
                        evalticket(lTicketa[0].ticket.ToString());
                    }
                    else
                    {
                        XtraMessageBox.Show("No tiene ticket asignado...");
                    }

                }

            }
        }

        private void simpleButton21_Click(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count > 0)
            {
                string sConten = strucdata.stringBetween(comboBox2.Text, comboBox1.Text.Substring(0, 1), " ");
                int iValue = 0;

                if (int.TryParse(sConten, out iValue))
                {

                    var lDprfid = procedure.ConvertToList<strucdata.forwarder>(procedure.Predata(1, "num_fle,nombre,Serial", "vdepenrfid", "", sConexion));

                    lTicko = procedure.ConvertToList<strucdata.headertick>(procedure.Predata(1, "ticket,pesob,castigo,descto", "b_ticket as b", "zafra = " + izafra.ToString() + " and peson = 0 and pesob > 0 and numtra = " + iValue , sConexion));

                    if(lTicko.Count() > 0)
                    {

                        boFlrfid = true;

                        var lDpn = lDprfid.Where(x => x.num_fle == sConten).ToList();

                        sSerialrfid = lDpn[0].serial_rfid;

                        textEdit5.Text = lTicko[0].ticket.ToString();
                        textEdit7.Text = lTicko[0].pesob.ToString();
                        textEdit10.Text = lTicko[0].pesob.ToString();
                        textEdit9.Text = lTicko[0].castigo.ToString();
                        textEdit8.Text = lTicko[0].descto.ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("El fletero no tiene ningun ticket asignado...");
                    }

                }


            }



        }
    }
}