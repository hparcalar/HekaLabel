using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Extensions;
using System.Data;
using System.Collections.Generic;

namespace HekaLabel.Design
{
    public partial class HekaReport : DevExpress.XtraReports.UI.XtraReport
    {
        public HekaReport()
        {
            InitializeComponent();
        }

        public static void RaporYazdir<T>(string raporDosyaAdi, T veriNesnesi)
        {
            string tasarimYolu = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\" + raporDosyaAdi + "";

            if (System.IO.File.Exists(tasarimYolu))
            {
                HekaReport rpr = new HekaReport();

                List<T> veriKaynagi = new List<T>();
                veriKaynagi.Add(veriNesnesi);

                rpr.Yazdir<T>(tasarimYolu, veriKaynagi);

                rpr.Dispose();
            }
        }

        public void Dizayn(string RaporAdi, DataTable dt)
        {
            HekaReport rpr = new HekaReport() { DataSource = dt };
            if (!string.IsNullOrEmpty(RaporAdi))
                rpr.LoadLayout(RaporAdi);
            ReportDesignTool tool = new ReportDesignTool(rpr);
            tool.ShowDesigner();
            rpr.PaperName = RaporAdi;
            rpr.DisplayName = RaporAdi;
            rpr.Name = RaporAdi;
            rpr.FromDisplayName(RaporAdi);
        }

        public void Dizayn(string RaporAdi, DataSet ds)
        {
            HekaReport rpr = new HekaReport() { DataSource = ds };
            rpr.LoadLayout(RaporAdi);
            ReportDesignTool tool = new ReportDesignTool(rpr);
            tool.ShowDesigner();
            rpr.PaperName = RaporAdi;
            rpr.DisplayName = RaporAdi;
            rpr.Name = RaporAdi;
            rpr.FromDisplayName(RaporAdi);
        }

        public void Dizayn<T>(string RaporAdi, IList<T> ds)
        {
            HekaReport rpr = new HekaReport() { DataSource = ds };
            if (!string.IsNullOrEmpty(RaporAdi))
                rpr.LoadLayout(RaporAdi);
            ReportDesignTool tool = new ReportDesignTool(rpr);
            tool.ShowDesigner();
            rpr.PaperName = RaporAdi;
            rpr.DisplayName = RaporAdi;
            rpr.Name = RaporAdi;
            rpr.FromDisplayName(RaporAdi);
        }

        //public void Onizleme(string RaporAdi, DataTable dt)
        //{
        //    HekaReport rpr = new HekaReport();
        //    rpr.DataSource = dt;
        //    rpr.LoadLayout(RaporAdi);
        //    rpr.Show.ShowPreview();

        //}

        //public void Onizleme<T>(string RaporAdi, IList<T> datasource)
        //{
        //    HekaReport rpr = new HekaReport();
        //    rpr.DataSource = datasource;
        //    rpr.LoadLayout(RaporAdi);
        //    rpr.ShowPreview();
        //}

        public void Yazdir<T>(string RaporAdi, IList<T> dt, string printerName = "")
        {
            HekaReport rpr = new HekaReport();
            rpr.DataSource = dt;
            rpr.LoadLayout(RaporAdi);
            rpr.CreateDocument();

            if (!string.IsNullOrEmpty(printerName))
                rpr.Print(printerName);
            else
                rpr.Print();
        }

        //public void Onizleme(string RaporAdi, DataSet ds)
        //{
        //    Report.Rapor rpr = new Report.Rapor();
        //    rpr.DataSource = ds;
        //    rpr.LoadLayout(RaporAdi);
        //    rpr.ShowPreview();

        //}

        public System.IO.Stream PdfGetir<T>(string raporAdi, IList<T> datasource)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            HekaReport rpr = new HekaReport();
            rpr.DataSource = datasource;
            rpr.LoadLayout(raporAdi);
            rpr.ExportToPdf(mStream);

            return mStream;
        }
    }
}