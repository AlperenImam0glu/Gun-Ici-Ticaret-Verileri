using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace smartpuls_Deneme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

 
        private async void button1_Click(object sender, EventArgs e)
        {

            string veriler;

            veriler = await VerileriGetir();
            veriler = VeriDuzenle(veriler);

            List<JsonResult> jsonResults = DataToJson(veriler);
            List<ResultTable> resultTables = new List<ResultTable>();

            resultTables = TabloVerileriniDuzenle(jsonResults);

            label1.Text = "";
            label1.Text += "Veriler conract parametrelerine göre"+
                " gruplanmış ve dakikalara göre toplanmıştır.\n"
                +tarihAl()+" Tarihli veriler getirilmiştir.";
            label1.Visible = true;

            dataGridView1.DataSource = resultTables;

        
        }

        private async Task<string> VerileriGetir()
        {
            var Client = new HttpClient();
            Client.BaseAddress = new Uri("https://seffaflik.epias.com.tr/transparency/");

            string tarih;
            tarih = tarihAl();

            HttpResponseMessage response = await Client.GetAsync("service/market/intra-day-trade-history?endDate="+tarih+"&startDate="+tarih);

            string veriler = await response.Content.ReadAsStringAsync();

            return veriler;
        }


        private  string tarihAl()
        {
            string tarih;

            DateTime dateTime = DateTime.Now;

            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;

            tarih = "" + year;
            if (month<10){
                tarih += "-0" + month;
            }
            else{
                tarih += "-" + month;
            }
            if (day < 10){
                tarih += "-0" + day;
            }
            else{
                tarih += "-" + day;
            }

            return tarih;
        }

        private List<JsonResult> DataToJson(string veriler)
        {

            List<JsonResult> jsonResults = JsonConvert.DeserializeObject<List<JsonResult>>(veriler);
            List<JsonResult> jsonResultsShort = new List<JsonResult>();
          
            string conractDegeri = "";

            for (int i = 0; i < jsonResults.Count; i++)
            {
                conractDegeri = jsonResults[i].conract;
                jsonResultsShort.Add(jsonResults[i]);
                for (int j = 1; j < jsonResults.Count; j++)
                {
                    if (conractDegeri == jsonResults[j].conract)
                    {
                        jsonResultsShort.Add(jsonResults[j]);
                        jsonResults.RemoveAt(j);
                        j = 0;
                    }
                }
                jsonResults.RemoveAt(i);
                i = 0;
                if (jsonResults.Count == 0)
                {
                    break;
                }
            }

            return jsonResultsShort;
        }

        private string VeriDuzenle(string veri)
        {
            int i = 0;
            for (i = 0; i < veri.Length; i++)
            {
                if (veri[i] =='[')
                {
                    break;
                }
            }
            veri = veri.Remove(0, i);
            Boolean x =false;
            for (i = veri.Length-1; i > 0; i--)
            {
                if (veri[i] == '[')
                {
                    x = true;
                }
                if (veri[i] == ']' && x == true)
                {
                    break;
                }

            }
            i += 1;
            veri = veri.Remove(veri.Length - (veri.Length-i));
            return veri;
        }

        private List<ResultTable> TabloVerileriniDuzenle(List<JsonResult> jsonResultsShort)
        {
            Boolean tekrar= false;
            List<ResultTable> resultTables = new List<ResultTable>();
            foreach (var item in jsonResultsShort)
            {
                if (!item.conract.Contains("PB"))
                {
                    if (resultTables.Count != 0)
                    {
                        item.date = item.date.AddSeconds(-1 * item.date.Second);
                        resultTables[resultTables.Count - 1].date = resultTables[resultTables.Count - 1].date.AddSeconds(-1 * resultTables[resultTables.Count - 1].date.Second);


                        if (item.date == resultTables[resultTables.Count - 1].date && item.conract == resultTables[resultTables.Count - 1].conract)
                        {
                            tekrar = true;
                            var tutar = ((item.price * item.quantity) / 10);
                            var miktar = (item.quantity / 10);

                            resultTables[resultTables.Count - 1].Fiyat += 0;
                            resultTables[resultTables.Count - 1].Miktar += miktar;
                            resultTables[resultTables.Count - 1].Tutar += tutar;
                        }
                        else
                        {
                            if (tekrar == true)
                            {
                                resultTables[resultTables.Count - 1].Fiyat += resultTables[resultTables.Count - 1].Tutar / resultTables[resultTables.Count - 1].Miktar;
                            }
                            tekrar = false;
                            ResultTable degerler = new ResultTable();

                            var tutar = ((item.price * item.quantity) / 10);
                            var miktar = (item.quantity / 10);

                            degerler.conract = item.conract;
                            degerler.date = item.date;
                            degerler.Fiyat = tutar/miktar;
                            degerler.Miktar = miktar;
                            degerler.Tutar = tutar;

                            resultTables.Add(degerler);
                        }
                    }
                    else
                    {
                        item.date = item.date.AddSeconds(-1 * item.date.Second);
                        ResultTable degerler = new ResultTable();

                        var tutar = ((item.price * item.quantity) / 10);
                        var miktar = (item.quantity / 10);

                        degerler.conract = item.conract;
                        degerler.date = item.date;
                        degerler.Fiyat = tutar / miktar;
                        degerler.Miktar = miktar;
                        degerler.Tutar = tutar;

                        resultTables.Add(degerler);
                    }
                }

            }
            return resultTables;
        }

        public class JsonResult
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public string conract { get; set; }
            public double price { get; set; }
            public double quantity { get; set; }
        }

        public class ResultTable
        {
            public string conract { get; set; }
            public DateTime date { get; set; }
            public double Miktar { get; set; }
            public double Tutar { get; set; }
            public double Fiyat { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

