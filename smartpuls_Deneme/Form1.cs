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

            foreach (var item in jsonResults)
            {

                ResultTable degerler = new ResultTable();

                var tutar = ((item.price * item.quantity) / 10);
                var miktar = (item.quantity / 10);
              
                degerler.date = item.date;
                degerler.Fiyat = tutar / miktar;
                degerler.Miktar = miktar;
                degerler.Tutar = tutar;

                resultTables.Add(degerler);
            }

            richTextBox1.Text =veriler;
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
