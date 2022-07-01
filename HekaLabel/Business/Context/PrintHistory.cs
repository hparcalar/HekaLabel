using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaLabel.Business.Context
{
    public class PrintHistory
    {
        public int Id { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public DateTime PrintDate { get; set; }
    }
}
