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

            ///string ifadenin basindaki ve sonnudaki fazlalık olan degerleri atıyoruz
            veriler = veriler.Remove(0, 83);
            veriler = veriler.Remove(veriler.Length-181);
            ///string ifadenin basindaki ve sonnudaki fazlalık olan degerleri atıyoruz
            

            List<JsonResult> jsonResults = JsonConvert.DeserializeObject<List<JsonResult>>(veriler);
            richTextBox1.Text =veriler;
            dataGridView1.DataSource = jsonResults;

        }

        public class JsonResult
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public string conract { get; set; }
            public double price { get; set; }
            public int quantity { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}


//private async void button1_Click(object sender, EventArgs e)
//{
//    var Client = new HttpClient();
//    Client.BaseAddress = new Uri("https://seffaflik.epias.com.tr/transparency/");
//    HttpResponseMessage response = await Client.GetAsync("service/market/intra-day-trade-history?endDate=2022-01-26&startDate=2022-01-26");
//    string Result = await response.Content.ReadAsStringAsync();
//    // List<dynamic> GelenVeri = await response.Content.ReadAsByteArrayAsync();
//    richTextBox1.Text = Result;
//}