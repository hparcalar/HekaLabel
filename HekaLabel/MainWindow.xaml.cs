using HekaLabel.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HekaLabel.Business.Context;
using HekaLabel.Design;
using System.IO;
using HekaLabel.Runners;
using System.Timers;

namespace HekaLabel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var configuration = new Configuration();
                configuration.TargetDatabase = new DbConnectionInfo("LabelContext");

                var migrator = new DbMigrator(configuration);
                migrator.Update();
            }
            catch (Exception)
            {

            }
        }

        private Category _editingCategory;
        private SensorListener _sensorListener = new SensorListener();
        private Timer _tmrTriggerStatusDisabler = new Timer();

        private void BindCategoryList()
        {
            using (LabelContext db = new LabelContext())
            {
                var catList = db.Category.OrderBy(d => d.ModelNo).ToArray();
                lstCategory.ItemsSource = null;
                lstCategory.ItemsSource = catList;

                lstPrintCategory.ItemsSource = null;
                lstPrintCategory.ItemsSource = catList;
            }
        }

        private void BindPrinters()
        {
            try
            {
                var printers = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
                cmbPrinters.ItemsSource = printers;
            }
            catch (Exception)
            {

            }
        }

        private void BindCategoryDetail()
        {
            if (_editingCategory == null)
            {
                txtCategoryFirmNo.Text = "";
                txtModelNo.Text = "";
                txtCategoryDeviceNo.Text = "MT";
                txtCategoryRevisionNo.Text = "";
                txtCategorySpecialCode.Text = "";
            }
            else
            {
                txtCategoryFirmNo.Text = !string.IsNullOrEmpty(_editingCategory.FirmNo) ? _editingCategory.FirmNo : "";
                txtModelNo.Text = _editingCategory.ModelNo;
                txtCategoryDeviceNo.Text = _editingCategory.DeviceNo;
                txtCategoryRevisionNo.Text = _editingCategory.RevisionNo;
                txtCategorySpecialCode.Text = _editingCategory.SpecialCode;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCategory();
        }

        private void SaveCategory()
        {
            string errMsg = "";
            if (string.IsNullOrEmpty(txtModelNo.Text) || txtModelNo.Text.Length != 10)
                errMsg = "Model no 10 haneli olarak girilmelidir.";
            else if (string.IsNullOrEmpty(txtCategoryDeviceNo.Text) || txtCategoryDeviceNo.Text.Length != 2)
                errMsg = "Cihaz no 2 haneli olarak girilmelidir.";
            else if (string.IsNullOrEmpty(txtCategoryRevisionNo.Text) || txtCategoryRevisionNo.Text.Length != 2)
                errMsg = "Revizyon no 2 haneli olarak girilmelidir.";
            else if (string.IsNullOrEmpty(txtCategoryFirmNo.Text) || txtCategoryFirmNo.Text.Length != 2)
                errMsg = "Firma no 2 haneli olarak girilmelidir.";
            else if (txtCategorySpecialCode.Text.Length > 0 && txtCategorySpecialCode.Text.Length < 10)
                errMsg = "Özel alan 10 haneli olarak girilmelidir.";

            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg, "Uyarı");
                return;
            }

            using (LabelContext db = new LabelContext())
            {
                bool checkRevision = false;

                if (_editingCategory == null)
                {
                    if (db.Category.Any(d => d.ModelNo == txtModelNo.Text))
                    {
                        MessageBox.Show("Aynı model numarası zaten girilmiş.", "Uyarı");
                        return;
                    }

                    _editingCategory = new Category();
                    db.Category.Add(_editingCategory);
                }
                else
                {
                    _editingCategory = db.Category.FirstOrDefault(d => d.Id == _editingCategory.Id);
                    checkRevision = true;
                }

                if (_editingCategory != null)
                {
                    if (checkRevision)
                    {
                        if (_editingCategory.RevisionNo != txtCategoryRevisionNo.Text)
                        {
                            RevisionChangeLog changeLog = new RevisionChangeLog
                            {
                                CategoryId = _editingCategory.Id,
                                ModelNo = txtModelNo.Text,
                                RevisionNo = _editingCategory.RevisionNo + "->" + txtCategoryRevisionNo.Text,
                                ChangeDate = DateTime.Now,
                            };
                            db.RevisionChangeLog.Add(changeLog);
                        }
                    }

                    _editingCategory.ModelNo = txtModelNo.Text;
                    _editingCategory.RevisionNo = txtCategoryRevisionNo.Text;
                    _editingCategory.DeviceNo = txtCategoryDeviceNo.Text;
                    _editingCategory.FirmNo = txtCategoryFirmNo.Text;
                    _editingCategory.SpecialCode = txtCategorySpecialCode.Text;

                    db.SaveChanges();
                }

                string standartRaporTanimi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_1.repx";
                string raporDosyaAdi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + _editingCategory.Id.ToString() + ".repx";
                if (!File.Exists(raporDosyaAdi))
                    File.Copy(standartRaporTanimi, raporDosyaAdi, true);

                string standartRaporTanimi2 = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_1_2.repx";
                string raporDosyaAdi2 = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + _editingCategory.Id.ToString() + "_2.repx";
                if (!File.Exists(raporDosyaAdi2))
                    File.Copy(standartRaporTanimi2, raporDosyaAdi2, true);
            }

            BindCategoryList();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_editingCategory != null && _editingCategory.Id > 0)
            {
                if (MessageBox.Show("Bu şablonu silmek istediğinizden emin misiniz?", "Uyarı", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (LabelContext db = new LabelContext())
                    {
                        var dbCat = db.Category.FirstOrDefault(d => d.Id == _editingCategory.Id);
                        if (dbCat != null)
                        {
                            db.Category.Remove(dbCat);
                            db.SaveChanges();
                            _editingCategory = null;
                        }
                    }

                    BindCategoryList();
                    BindCategoryDetail();
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            lstCategory.SelectedIndex = -1;
            _editingCategory = null;
            BindCategoryDetail();
            txtModelNo.Focus();
        }

        private void lstCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstCategory.SelectedValue != null)
            {
                using (LabelContext db = new LabelContext())
                {
                    int currentId = Convert.ToInt32(lstCategory.SelectedValue);
                    _editingCategory = db.Category.FirstOrDefault(d => d.Id == currentId);
                    BindCategoryDetail();
                }
            }
        }

        private void lstPrintCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPrintCategory.SelectedValue != null)
            {
                using (LabelContext db = new LabelContext())
                {
                    int currentId = Convert.ToInt32(lstPrintCategory.SelectedValue);
                    var printingCategory = db.Category.FirstOrDefault(d => d.Id == currentId);

                    txtPrintModelNo.Text = printingCategory.ModelNo;
                    txtPrintFirmNo.Text = printingCategory.FirmNo;
                    txtPrintRevision.Text = printingCategory.RevisionNo;
                    txtPrintTestDevice.Text = printingCategory.DeviceNo;
                    cmbPrinters.Text = printingCategory.LastPrinterName;
                    txtPrintingSpecialCode.Text = printingCategory.SpecialCode;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindCategoryList();
            BindPrinters();
            BindTriggerPath();

            _tmrTriggerStatusDisabler.Elapsed += _tmrTriggerStatusDisabler_Elapsed;
            _tmrTriggerStatusDisabler.Interval = 1200;

            _sensorListener.OnPrintOrderArrived += _sensorListener_OnPrintOrderArrived;
            _sensorListener.Run();
        }

        private void _sensorListener_OnPrintOrderArrived()
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                btnTriggerStatus.Background = Brushes.Green;
                PrintLabel();

                
                _tmrTriggerStatusDisabler.Enabled = true;
                _tmrTriggerStatusDisabler.Start();
            });
        }

        private void _tmrTriggerStatusDisabler_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                btnTriggerStatus.Background = Brushes.White;
                _tmrTriggerStatusDisabler.Stop();
            });
        }

        private void BindTriggerPath()
        {
            try
            {
                using (LabelContext db = new LabelContext())
                {
                    var dbStg = db.AppSetting.FirstOrDefault(d => d.AppKey == "SensorTriggerPath");
                    if (dbStg != null)
                    {
                        txtSensorTriggerFilePath.Text = dbStg.AppVal;
                        _sensorListener.TriggerPath = dbStg.AppVal;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void PrintLabel()
        {
            if (lstPrintCategory.SelectedValue != null)
            {
                int testVal = 0;
                if (
                    string.IsNullOrEmpty(txtPrintSerialCount.Text) ||
                    Int32.TryParse(txtPrintSerialCount.Text, out testVal) == false)
                {
                    MessageBox.Show("Yazdırılacak seri adedi bilgisi girilmelidir", "Uyarı");
                    return;
                }

                if (txtPrintingSpecialCode.Text.Length > 0 && txtPrintingSpecialCode.Text.Length < 10)
                {
                    MessageBox.Show("Özel alan 10 haneli olarak girilmelidir.", "Uyarı");
                    return;
                }

                //if ((testVal > 1 && cmbLabelType.SelectedIndex != 1) || (testVal > 2 && cmbLabelType.SelectedIndex == 1) || testVal < 0)
                //{
                //    MessageBox.Show("Maksimum 100 adet etiket yazdırılabilir.");
                //    return;
                //}

                using (LabelContext db = new LabelContext())
                {
                    int currentId = Convert.ToInt32(lstPrintCategory.SelectedValue);
                    var printingCategory = db.Category.FirstOrDefault(d => d.Id == currentId);

                    if (printingCategory != null)
                    {
                        int startSerialNo = printingCategory.SerialNo;
                        int serialCount = Convert.ToInt32(txtPrintSerialCount.Text);

                        string raporDosyaAdi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + printingCategory.Id.ToString() + ".repx";
                        if (cmbLabelType.SelectedIndex == 1)
                            raporDosyaAdi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + printingCategory.Id.ToString() + "_2.repx";

                        HekaReport rpr = new HekaReport();

                        int currentSerialNo = 0;

                        try
                        {
                            var dtStartStr = string.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 00:00:00";
                            var dtEndStr = string.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 23:59:59";

                            var dtStart = DateTime.ParseExact(dtStartStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                            var dtEnd = DateTime.ParseExact(dtEndStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.GetCultureInfo("tr"));

                            var historyCount = db.PrintHistory.Where(d => d.CategoryId == printingCategory.Id
                                && d.PrintDate >= dtStart && d.PrintDate <= dtEnd).Count();
                            currentSerialNo = historyCount;

                            currentSerialNo++;

                            if (currentSerialNo == 0)
                                currentSerialNo++;
                        }
                        catch (Exception ex)
                        {

                        }

                        if (currentSerialNo == 0)
                            currentSerialNo++;

                        // tek satırda çift etiket çıkan tasarımda toplam sayıyı 2 ye böl
                        if (cmbLabelType.SelectedIndex == 1)
                        {
                            serialCount = serialCount / 2;
                        }

                        for (int i = 0; i < serialCount; i++)
                        {
                            if (cmbLabelType.SelectedIndex == 1)
                            {
                                rpr.Yazdir<LabelModel>(raporDosyaAdi, new List<LabelModel>() {
                                    new LabelModel
                                    {
                                        ModelNo = printingCategory.ModelNo,
                                        ProductionDate = string.Format("{0:ddMMyy HHmm}", DateTime.Now),
                                        Revision = txtPrintRevision.Text,
                                        SerialNo = string.Format("{0:0000}", currentSerialNo),
                                        TestDevice = printingCategory.DeviceNo,
                                        Barcode = printingCategory.ModelNo.PadLeft(10, '0') +printingCategory.RevisionNo + printingCategory.FirmNo
                                            + string.Format("{0:ddMMyy}", DateTime.Now) + string.Format("{0:HHmm}", DateTime.Now)
                                            + (
                                                string.Format("{0:0000}", currentSerialNo)
                                              ) + printingCategory.DeviceNo
                                            + txtPrintingSpecialCode.Text,
                                        SerialNo2 = string.Format("{0:0000}", currentSerialNo + 1),
                                        ProductionDate2 = string.Format("{0:ddMMyy HHmm}", DateTime.Now),
                                        Barcode2 = printingCategory.ModelNo.PadLeft(10, '0') +printingCategory.RevisionNo + printingCategory.FirmNo
                                            + string.Format("{0:ddMMyy}", DateTime.Now) + string.Format("{0:HHmm}", DateTime.Now)
                                            + (
                                                string.Format("{0:0000}", currentSerialNo + 1)
                                              ) + printingCategory.DeviceNo
                                            + txtPrintingSpecialCode.Text,
                                    },
                                }, cmbPrinters.Text);

                                var newHistory = new Business.Context.PrintHistory
                                {
                                    CategoryId = printingCategory.Id,
                                    PrintDate = DateTime.Now,
                                };
                                var newHistory2 = new Business.Context.PrintHistory
                                {
                                    CategoryId = printingCategory.Id,
                                    PrintDate = DateTime.Now,
                                };
                                db.PrintHistory.Add(newHistory);
                                db.PrintHistory.Add(newHistory2);

                                currentSerialNo += 2;
                            }
                            else
                            {
                                rpr.Yazdir<LabelModel>(raporDosyaAdi, new List<LabelModel>() {
                                    new LabelModel
                                    {
                                        ModelNo = printingCategory.ModelNo,
                                        ProductionDate = string.Format("{0:ddMMyy HHmm}", DateTime.Now),
                                        Revision = txtPrintRevision.Text,
                                        SerialNo = string.Format("{0:0000}", currentSerialNo),
                                        TestDevice = printingCategory.DeviceNo,
                                        Barcode = printingCategory.ModelNo.PadLeft(10, '0') +printingCategory.RevisionNo + printingCategory.FirmNo
                                            + string.Format("{0:ddMMyy}", DateTime.Now) + string.Format("{0:HHmm}", DateTime.Now)
                                            + (
                                                string.Format("{0:0000}", currentSerialNo)
                                              ) + printingCategory.DeviceNo
                                            + txtPrintingSpecialCode.Text
                                    },
                                }, cmbPrinters.Text);

                                var newHistory = new Business.Context.PrintHistory
                                {
                                    CategoryId = printingCategory.Id,
                                    PrintDate = DateTime.Now,
                                };
                                db.PrintHistory.Add(newHistory);

                                currentSerialNo++;
                            }
                        }

                        printingCategory.LastPrinterName = cmbPrinters.Text;
                        db.SaveChanges();
                    }
                }
            }

        }
        private void btnDesign_Click(object sender, RoutedEventArgs e)
        {
            if (_editingCategory.Id <= 0)
            {
                SaveCategory();
            }

            string standartRaporTanimi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_1.repx";
            string raporDosyaAdi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + _editingCategory.Id.ToString() + ".repx";
            if (!File.Exists(raporDosyaAdi))
                File.Copy(standartRaporTanimi, raporDosyaAdi);

            HekaReport rpr = new HekaReport();
            rpr.Dizayn<LabelModel>(raporDosyaAdi, new List<LabelModel>() { new LabelModel { } });
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintLabel();
        }

        private void txtPrintFirmNo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnExportChangeLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Revision_Change_Log"; // Default file name
                dlg.DefaultExt = ".csv"; // Default file extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;

                    using (LabelContext db = new LabelContext())
                    {
                        var rawData = db.RevisionChangeLog.OrderBy(d => d.ChangeDate)
                            .ToList();
                        //.Select(d => new
                        //{
                        //    Tarih = string.Format("{0:dd.MM.yyyy HH:mm}", d.ChangeDate),
                        //    ModelNo = d.ModelNo,
                        //    RevizyonNo = d.RevisionNo,
                        //}).ToArray();

                        StreamWriter sw = new StreamWriter(filename);

                        foreach (var item in rawData)
                        {
                            sw.WriteLine(string.Format("{0:dd.MM.yyyy HH:mm}", item.ChangeDate) + ";" + item.ModelNo + ";" + item.RevisionNo);
                        }

                        sw.Flush();
                        sw.Close();

                        MessageBox.Show("Veriler dosyaya transfer edildi.", "Bilgi");
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnDesign2_Click(object sender, RoutedEventArgs e)
        {
            if (_editingCategory.Id <= 0)
            {
                SaveCategory();
            }

            string standartRaporTanimi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_1.repx";
            string raporDosyaAdi = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + _editingCategory.Id.ToString() + "_2.repx";
            if (!File.Exists(raporDosyaAdi))
                File.Copy(standartRaporTanimi, raporDosyaAdi);

            HekaReport rpr = new HekaReport();
            rpr.Dizayn<LabelModel>(raporDosyaAdi, new List<LabelModel>() { new LabelModel { } });
        }

        private void btnCopyDesignForAllModels_Click(object sender, RoutedEventArgs e)
        {
            if (_editingCategory != null && _editingCategory.Id > 0)
            {
                string design1 = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_"+ _editingCategory.Id +".repx";
                string design2 = System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_"+ _editingCategory.Id +"_2.repx";

                using (LabelContext db = new LabelContext())
                {
                    var otherModels = db.Category.Where(d => d.Id != _editingCategory.Id).ToArray();
                    foreach (var item in otherModels)
                    {
                        if (File.Exists(design1))
                            File.Copy(design1, System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + item.Id + ".repx", true);

                        if (File.Exists(design2))
                            File.Copy(design2, System.AppDomain.CurrentDomain.BaseDirectory + "Design\\Label_" + item.Id + "_2.repx", true);
                    }
                }

                MessageBox.Show("Bu tasarım tüm modeller için başarıyla uygulandı.", "Bilgilendirme");
            }
        }

        private void btnSelectSensorTriggerFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
                Nullable<bool> result = openFileDlg.ShowDialog();
                if (result == true)
                {
                    txtSensorTriggerFilePath.Text = openFileDlg.FileName;

                    using (LabelContext db = new LabelContext())
                    {
                        var dbStg = db.AppSetting.FirstOrDefault(d => d.AppKey == "SensorTriggerPath");
                        if (dbStg == null)
                        {
                            dbStg = new AppSetting
                            {
                                AppKey = "SensorTriggerPath",
                                AppVal = "",
                            };
                            db.AppSetting.Add(dbStg);
                        }

                        dbStg.AppVal = txtSensorTriggerFilePath.Text;
                        _sensorListener.TriggerPath = txtSensorTriggerFilePath.Text;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _sensorListener.Stop();
                _sensorListener.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}
