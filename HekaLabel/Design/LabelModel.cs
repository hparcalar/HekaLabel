using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaLabel.Design
{
    public class LabelModel
    {
        public string ModelNo { get; set; }
        public string Revision { get; set; }
        public string Barcode { get; set; }
        public string SerialNo { get; set; }
        public string TestDevice { get; set; }
        public string ProductionDate { get; set; }
        public string PlantCountryCode { get; set; }
        public string FirmCode { get; set; }
        public string ProductionTime { get; set; }

        public string Barcode2 { get; set; }
        public string SerialNo2 { get; set; }
        public string ProductionDate2 { get; set; }
    }
}
