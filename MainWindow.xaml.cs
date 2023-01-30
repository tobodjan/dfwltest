using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
//using System.Data.SQLite;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using iTextSharp.text.pdf.draw;
using System.Data.SQLite;
using System.Data;


namespace dfwltest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SQLiteConnection connection;
        public MainWindow()
        {
            SetBindingLists();
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yy";
            Thread.CurrentThread.CurrentCulture = ci;
            InitializeComponent();
            this.Title = "Tobing Sistem";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = this;
            SetPropertyWeightData();
            //otvaranje konekcije sa bazom i inicijalizacija
            bool exist = File.Exists("C:\\temp\\MyDatabase.sqlite");
            if (!exist)
            {
                SQLiteConnection.CreateFile("C:\\temp\\MyDatabase.sqlite");
            }

            if (connection == null)
            {
                ctreateConnection();
            }

            string sql = "create table merenje(neto varchar(25), bruto varchar(25), tara varchar(25),proizvod varchar(25),ambalaza varchar(25),kolicina varchar(25),  vreme varchar(25), id INTEGER NOT NULL primary key)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);

            SqlCommandExecute(sql);



            string comPort = ConfigurationManager.AppSettings.Get("COM");
            string baudPort = ConfigurationManager.AppSettings.Get("BAUD");
            int baudRate = Convert.ToInt32(baudPort);


            _serialPort = new SerialPort(comPort, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;

            try
            {
                _serialPort.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("COM port nije podesen!", "Greska", MessageBoxButton.OK);
            }
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);




        }

        //Za bazu
        private static void SqlCommandExecute(string commandText)
        {
            SQLiteCommand command = new SQLiteCommand(commandText, connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("------------EXCEPTION:" + e.Message);
            }
        }

        private void ctreateConnection()
        {
            try
            {
                connection = new SQLiteConnection("Data Source=C:\\temp\\MyDatabase.sqlite;Version=3");
                connection.Open();
            }
            catch (Exception e)
            {
                string path = @"C:\temp\logs.txt";
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(e.Message);
                    }
                }
            }
        }

        //dovde 


        public static SerialPort _serialPort;
        public static BindingList<Merenja> listaMerenja { get; set; }
        public static BindingList<Otpremnica> listaOtpremnica { get; set; }
        public static BindingList<Otpremnica> bindingListOtpremnica { get; set; }
        public Otpremnica selectedOtpremnica { get; set; }
        private void Dugme1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aplikacija se zatvara");
            this.Close();
        }
        private void loadConfigFromFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"c:\temp\tools.xml");

        }
        public void SetPropertyWeightData()
        {
            WeightData.Neto = "0";
            WeightData.Tara = "0";
            WeightData.Bruto = "0";
        }


        //float gajba = 0;
        //gajba = float.Parse(Kultura_TextBox_Copy2.Text);

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int bytesToRead = sp.BytesToRead;
            Thread.Sleep(1000);
            string indata = sp.ReadExisting();
            Console.WriteLine("-------ispis-----------" + indata);

            string[] splitDataMessage1 = indata.Split('W');
            int indexN = splitDataMessage1[splitDataMessage1.Length - 2].IndexOf('N');
            int indexB = splitDataMessage1[splitDataMessage1.Length - 2].IndexOf('g');
            string valuee = splitDataMessage1[splitDataMessage1.Length - 2].Substring(indexN + 1, indexB);
            WeightData.Bruto = valuee;



            //float tar = 0;
            //WeightData.Tara = tar.ToString() ;
            //float net = float.Parse(WeightData.Neto.Split('k')[0]);
            //float rez = net - tar;
            //WeightData.Bruto = rez.ToString();

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "Merenje":
                    break;

                case "Otpremnice":
                    //loadPdfNames();
                    break;
                case "Podesavanja":
                    ScanComPorts();
                    break;
                case "Otprema":
                    break;

                default:
                    return;
            }
        }
        private void loadPdfNames()
        {

            listaOtpremnica.Clear();
            string path = ConfigurationManager.AppSettings.Get("PdfPath");
            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
            {
                Otpremnica o = new Otpremnica();
                o.Komitent = fileName;
                listaOtpremnica.Add(o);
            }


        }

        public void ScanComPorts()
        {
            //ComPortComboBox.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                //Console.WriteLine(port);
                if (ComPortComboBox.Items.IndexOf(port) == -1)
                {
                    ComPortComboBox.Items.Add(port);
                }
            }

        }
        private void ComPortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //podesiti com port za komunikaciju
            _serialPort.Close();
            ComboBox cmb = (ComboBox)sender;
            string selectedValue = (string)cmb.SelectedValue;
            if (selectedValue == null)
            {
                MessageBox.Show("Uredjaj nije konektovan na taj port: ", "Greska", MessageBoxButton.OK);
            }
            else
            {
                _serialPort.PortName = selectedValue;
            }

            try
            {
                _serialPort.Open();
            }
            catch (Exception ee)
            {
                cmb.SelectedIndex = -1;
                MessageBox.Show("Uredjaj nije konektovan na taj port: ", "Greska", MessageBoxButton.OK);
            }

        }

        private void SetBindingLists()
        {



            if (bindingListOtpremnica == null)
            {
                bindingListOtpremnica = new BindingList<Otpremnica>();
            }

            if (listaOtpremnica == null)
            {
                listaOtpremnica = new BindingList<Otpremnica>();
            }
        }
        //private void PodCom_Click(object sender, RoutedEventArgs e)
        //   {
        //     ScanComPorts();
        //}
        private void Button_Click_EndDelivery(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                string destPdf = "";
                try
                {
                    destPdf = createDocument();
                    //     MainWindow.sendMailWithPdf(destPdf);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Pdf problem: " + exception.Message, "Greska", MessageBoxButton.OK);

                }
                //Send mail with pdf

                //Otpremnica o = new Otpremnica(komitentTextBox.Text, prevoznikTextBox.Text, robuIzdaoTextBox.Text, robuPrimioTextBox.Text, Kultura_TextBox.Text);




                WeightData.Neto = "0";

                //komitentTextBox.Clear();
                //prevoznikTextBox.Clear();
                //robuIzdaoTextBox.Clear();
                //robuPrimioTextBox.Clear();
                //Kultura_TextBox.Clear();
                //TaraTextBox.Clear();
                //BrutoTextBox.Clear();

            }
        }
        public static void InsertMerenjeInDB(Merenja m)
        {
            string sqlinsert = "insert into merenje(neto, bruto, tara, proizvod, ambalaza, kolicina, vreme,  values " +
                    "('" + m.Neto + "','" + m.Bruto + "','" + m.Tara + "','" + m.Proizvod + "','" + m.Ambalaza + "','" + m.Kolicina + "','" + m.Vreme + "' )";
            SQLiteCommand command = new SQLiteCommand(sqlinsert, connection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("------------EXCEPTION:" + e.Message);
            }
        }
        public async void CreateWcfComunication()
        {


            while (true)
            {

                int size = _serialPort.BytesToRead;

                if (size != 0)
                {
                    string message = _serialPort.ReadLine();


                    Console.WriteLine("-------ispis-------" + message);

                    // string[] splitDataMessage1 = message.Split(',');


                    //  WeightData.Neto = splitDataMessage1[0];

                    WeightData.Neto = message;
                    Merenja merenja = new Merenja(WeightData.Neto, WeightData.Bruto, WeightData.Tara, WeightData.Proizvod,WeightData.Ambalaza,WeightData.Kolicina, WeightData.Vreme);



                }

            }

        }
        private string izbor;
        private string createDocument()
        {
            float netoSum = 0;

            string netoispisporuka = merenjeTextBox.ToString();
            string[] splitDataNetoIspis = netoispisporuka.Split(':');
            string neto_ispis = splitDataNetoIspis[1];

            //string taraispisporuka = TaraTextBox.ToString();
            //string[] splitDataTaraIspis = taraispisporuka.Split(':');
            //string tara_ispis = splitDataTaraIspis[1];

            string brutoispisporuka = BrutoTextBox.ToString();
            string[] splitDataBrutoIspis = brutoispisporuka.Split(':');
            string bruto_ispis = splitDataBrutoIspis[1];

            String pdfName = "\\" + izbor;//"\\odvaga"; //+ DateTime.Now + ".pdf";
            String pdfName1 = DateTime.Now + ".pdf";
            pdfName1 = pdfName1.Replace(':', '-');
            pdfName1 = pdfName1.Replace('\\', '-');
            pdfName1 = pdfName1.Replace('/', '-');
            string pdfPath = ConfigurationManager.AppSettings.Get("PdfPath"); ;
            string fullPath = pdfPath + "\\merenje-" + pdfName1;
            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter.GetInstance(document, new FileStream(fullPath, System.IO.FileMode.OpenOrCreate));
            document.Open();

            iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("FORTIS DOO, Beograd Knjeginje Zorke 38: PIB:104779749: TR;", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Objekat: Makoviste 3,: Telefon:064-2682584; ", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Otkupno mesto: Makovište 3 - Redovan otkup", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            Chunk linebreak = new Chunk(new LineSeparator(3f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1));
            document.Add(linebreak);

            p = new iTextSharp.text.Paragraph("Otkupni list-priznanica broj: " + IDTextBox.Text, new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_LEFT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Proizvodjac: " + komitentTextBox.Text, new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Za otkupljeni poljoprivredni proizvod", new Font(Font.FontFamily.HELVETICA, 10));
            p.Alignment = Element.ALIGN_LEFT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Adresa: " + prevoznikTextBox.Text, new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Datum prometa: " + DateTime.Now, new Font(Font.FontFamily.HELVETICA, 10));
            p.Alignment = Element.ALIGN_LEFT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("JMBG/BRRPG: " + licnaTextBox.Text, new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Tekuci racun: " + registracijaTextBox.Text, new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            PdfPTable table = new PdfPTable(5);

            //table.AddCell("Merenje:");
            Font font = new Font(Font.FontFamily.HELVETICA, 15, 1, BaseColor.BLACK);

            PdfPCell cell = new PdfPCell(new Phrase("", font));
            //table.AddCell(cell);


            //table.AddCell("-");
            //cell = new PdfPCell(new Phrase("-", font));
            //table.AddCell(cell);

            p = new iTextSharp.text.Paragraph("Merenje:", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_LEFT;
            document.Add(p);

            table.AddCell("Naziv prizvoda");
            table.AddCell("Cena");
            table.AddCell("Bruto");
            table.AddCell("Neto");
            table.AddCell("Za isplatu");
            cell = new PdfPCell(new Phrase(Kultura_TextBox.Text, font));
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(Kultura_TextBox_Copy.Text + " rsd/kg", font));
            //table.AddCell(cell);

            cell = new PdfPCell(new Phrase(merenjeTextBox.Text + " kg", font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(BrutoTextBox.Text + " kg", font));
            table.AddCell(cell);

            //double zaIsplatu = double.Parse(BrutoTextBox.Text) * double.Parse(Kultura_TextBox_Copy.Text);

            //cell = new PdfPCell(new Phrase(zaIsplatu.ToString() + " rsd", font));
            table.AddCell(cell);

            document.Add(table);
            ////string slikapath = "C:\\temp\\slike\\marbo.png";
            ////iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagePath);

            ////document.Add(img);
            p = new iTextSharp.text.Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            p = new iTextSharp.text.Paragraph("Ambalaza:", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_LEFT;
            document.Add(p);

            PdfPTable table2 = new PdfPTable(2);

            table2.AddCell("Izdato");
            table2.AddCell("Primljeno");
            //cell = new PdfPCell(new Phrase(Kultura_TextBox_Copy1.Text, font));
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase(Kultura_TextBox_Copy2.Text, font));
            table2.AddCell(cell);

            document.Add(table2);

            p = new iTextSharp.text.Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            p = new iTextSharp.text.Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_CENTER;
            document.Add(p);

            //p = new iTextSharp.text.Paragraph("UPISI", new Font(Font.FontFamily.HELVETICA, 12));
            //p.Alignment = Element.ALIGN_CENTER;
            //document.Add(p);

            Chunk linebreak2 = new Chunk(new LineSeparator(3f, 30f, BaseColor.BLACK, Element.ALIGN_RIGHT, -1));
            document.Add(linebreak2);
            p = new iTextSharp.text.Paragraph("Otkupio", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            p = new iTextSharp.text.Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            Chunk linebreak3 = new Chunk(new LineSeparator(3f, 30f, BaseColor.BLACK, Element.ALIGN_RIGHT, -1));
            document.Add(linebreak3);
            p = new iTextSharp.text.Paragraph("Predao", new Font(Font.FontFamily.HELVETICA, 12));
            p.Alignment = Element.ALIGN_RIGHT;
            document.Add(p);

            document.Close();
            return fullPath;
        }

        private void Dugme2_Click(object sender, RoutedEventArgs e)
        {

            string pdfPath = ConfigurationManager.AppSettings.Get("PdfPath");
            System.Diagnostics.Process.Start(pdfPath);
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string s = selectedOtpremnica.Komitent;
            System.Diagnostics.Process.Start(selectedOtpremnica.Komitent);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            loadPdfNames();
        }

        //private void ButtonCSV_Click(object sender, RoutedEventArgs e)
        //{
        //    String vreme = "otprema"+DateTime.Now;
        //    vreme = vreme.Replace(':', '-');
        //    vreme = vreme.Replace('\\', '-');
        //    vreme = vreme.Replace('/', '-');

        //    string netoispisporuka = merenjeTextBox.ToString();
        //    string[] splitDataNetoIspis = netoispisporuka.Split(':');
        //    string neto_ispis = splitDataNetoIspis[1];

        //    string taraispisporuka = TaraTextBox.ToString();
        //    string[] splitDataTaraIspis = taraispisporuka.Split(':');
        //    string tara_ispis = splitDataTaraIspis[1];

        //    string brutoispisporuka = BrutoTextBox.ToString();
        //    string[] splitDataBrutoIspis = brutoispisporuka.Split(':');
        //    string bruto_ispis = splitDataBrutoIspis[1];
        //    // pass in true to cause writes to be appended
        //    StreamWriter sw = new StreamWriter("C:\\Users\\Toma\\Desktop\\otpremnice\\"+vreme+".csv", true);
        //    string a = komitentTextBox.Text;
        //    string b = prevoznikTextBox.Text;
        //    string c = robuIzdaoTextBox.Text;
        //    string d = robuPrimioTextBox.Text;
        //    string f = Kultura_TextBox.Text;
        //    // use writeline so we get a newline character written
        //    sw.WriteLine("{0}, {1}, {2}, {3}, {4}", a, b, c,d,f);
        //    sw.WriteLine("Masa:{0},Tara:{1},Bruto:{2}",neto_ispis,tara_ispis,bruto_ispis);
        //    // Ensure data is written to disk
        //    sw.Close();


        //}

        private void dugme_stampa_Click(object sender, RoutedEventArgs e)
        {

            string Folder = ConfigurationManager.AppSettings.Get("PdfPath");
            var files = new DirectoryInfo(Folder).GetFiles("*.*");
            string latestfile = "";

            DateTime lastupdated = DateTime.MinValue;
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > lastupdated)
                {
                    lastupdated = file.LastWriteTime;
                    latestfile = file.Name;
                }
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "print",
                FileName = Folder + "\\" + latestfile//put the correct path here
            };
            p.Start();

        }

        private void stampaotpremnice_Click(object sender, RoutedEventArgs e)
        {
            string s = selectedOtpremnica.Komitent;
            System.Diagnostics.Process.Start(selectedOtpremnica.Komitent);
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "print",
                FileName = s //put the correct path here
            };
            p.Start();
        }

        private void Prvomerenje_Click(object sender, RoutedEventArgs e)
        {
            //generate pdf
            //prit pdf
            //save in db or binding list
            if (IDTextBox.Text != "" && komitentTextBox.Text != "" &&
                prevoznikTextBox.Text != "" && licnaTextBox.Text != "" &&
                registracijaTextBox.Text != "" && merenjeTextBox.Text != "" &&
                BrutoTextBox.Text != "" && Kultura_TextBox.Text != "")
            {
                string destPdf = "";
                try
                {
                    destPdf = createDocument();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Pdf problem: " + exception.Message, "Greska", MessageBoxButton.OK);

                }
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    Verb = "",
                    FileName = destPdf
                };
                try
                {
                    p.Start();
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                }
                p.WaitForExit(41000);
                if (p.HasExited == false)
                {
                    p.Kill();
                }
                p.EnableRaisingEvents = true;
                p.Close();
            }



        }

        private void Drugomerenje_Click(object sender, RoutedEventArgs e)
        {
            bool potvrda = false;
        }


        private string amb;
        private double ambalaza_tez;


        private void amblistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string temp_amb = amblistbox.SelectedItem.ToString();
            string[] splitDataMessage1 = temp_amb.Split(':');

            amb = splitDataMessage1[1];

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_Ucitaj_Istoriju_Merenja(object sender, RoutedEventArgs e)
        {


            //izmesteno dole u try
            //string sql = "select * from merenje";
            //SQLiteCommand command = new SQLiteCommand(sql, connection);

            try
            {

                string sql = "select * from merenje";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("merenje");
                dataAdp.Fill(dt);
                dataGridMerenja.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);

                connection.Close();

            }
            catch (Exception)
            {
                Console.WriteLine("------------EXCEPTION:");
            }


        }
    }
}
