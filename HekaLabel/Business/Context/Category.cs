using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace HekaLabel.Business.Context
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public string ShiftCode { get; set; }
        public bool PrintDate { get; set; }
        public bool PrintShift { get; set; }
        public bool PrintBarcode { get; set; }
        public int SerialNo { get; set; }
        public int PrintCount { get; set; }
        public int CopyCount { get; set; }

        public string RevisionNo { get; set; }
        public string DeviceNo { get; set; }
        public string FirmNo { get; set; }

        public string SpecialCode { get; set; }
        public string LastPrinterName { get; set; }

        public int? XModelNo { get; set; }
        public int? YModelNo { get; set; }

        public int? XModelName { get; set; }
        public int? YModelName { get; set; }
        
        public int? XShiftCode { get; set; }
        public int? YShiftCode { get; set; }

        public int? XDate { get; set; }
        public int? YDate { get; set; }

        public int? XBarcode { get; set; }
        public int? YBarcode { get; set; }
        public int? BarcodeSize { get; set; }

    }
}
