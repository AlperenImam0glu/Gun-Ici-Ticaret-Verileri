using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

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
            var Client = new HttpClient();
            Client.BaseAddress = new Uri("https://seffaflik.epias.com.tr/transparency/");
            HttpResponseMessage response = await Client.GetAsync("service/market/intra-day-trade-history?endDate=2022-01-26&startDate=2022-01-26");

            string veriler = await response.Content.ReadAsStringAsync();

            ///string ifadenin basindaki ve sonudaki fazlalık olan degerleri atıyoruz
            veriler = veriler.Remove(0, 83);
            veriler = veriler.Remove(veriler.Length-181);
            ///string ifadenin basindaki ve sonudaki fazlalık olan degerleri atıyoruz
            

            List<JsonResult> jsonResults = JsonConvert.DeserializeObject<List<JsonResult>>(veriler);
            List<ResultTable> resultTables = new List<ResultTable>();
            List<JsonResult> jsonResultsShort = new List<JsonResult>();
            int sayac = 0;
            string tarihler="";
            string conractDegeri = "";

            int adad = jsonResults.Count;

            for (int i = 0; i < jsonResults.Count; i++)
            {
                conractDegeri = jsonResults[i].conract;
                jsonResultsShort.Add(jsonResults[i]);
                for (int j = 1; j < jsonResults.Count; j++)
                {
                    if(conractDegeri == jsonResults[j].conract)
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

            foreach (var item in jsonResultsShort)
            {
                if (!item.conract.Contains("PB"))
                {
                    if ( resultTables.Count != 0)
                    {
                        item.date = item.date.AddSeconds(-1 * item.date.Second);
                        resultTables[resultTables.Count - 1].date = resultTables[resultTables.Count - 1].date.AddSeconds(-1 * resultTables[resultTables.Count - 1].date.Second);


                        if ( item.date == resultTables[resultTables.Count - 1].date && item.conract== resultTables[resultTables.Count - 1].conract)
                        {
                            tarihler += "tarihleri ve conractlar eşit = "+item.date+"- "+ resultTables[resultTables.Count - 1].date +"\n";
                            var tutar = ((item.price * item.quantity) / 10);
                            var miktar = (item.quantity / 10);

                            resultTables[resultTables.Count - 1].Fiyat += 0;
                            resultTables[resultTables.Count - 1].Miktar += miktar;
                            resultTables[resultTables.Count - 1].Tutar += tutar;
                        }
                        else
                        {
                            tarihler += "" + item.date + "- " + resultTables[resultTables.Count - 1].date +" "+item.conract+" "+ resultTables[resultTables.Count - 1].conract + " " +"\n";
                            ResultTable degerler = new ResultTable();

                            var tutar = ((item.price * item.quantity) / 10);
                            var miktar = (item.quantity / 10);

                            degerler.conract = item.conract;
                            degerler.date = item.date;
                            degerler.Fiyat = 0;
                            degerler.Miktar = miktar;
                            degerler.Tutar = tutar;

                            resultTables.Add(degerler);
                        }
                    }
                    else
                    {
                        item.date = item.date.AddSeconds(-1 * item.date.Second);
                        sayac += 1;
                        ResultTable degerler = new ResultTable();

                        var tutar = ((item.price * item.quantity) / 10);
                        var miktar = (item.quantity / 10);

                        degerler.conract = item.conract;
                        degerler.date = item.date;
                        degerler.Fiyat = 0;
                        degerler.Miktar = miktar;
                        degerler.Tutar = tutar;

                        resultTables.Add(degerler);
                    }
                }
             
               
            }
            string tamam = "";
            foreach (var item in jsonResultsShort)
            {
                if (item.id == 444121195)
                {
                    tamam = "lsitede var";
                }
            }
            richTextBox1.Text ="" + adad+"\n"+jsonResultsShort.Count;
            //dataGridView1.DataSource = resultTables;
            
            dataGridView1.DataSource = resultTables;
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
            public double Tutar  { get; set; }
            public double Fiyat  { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

