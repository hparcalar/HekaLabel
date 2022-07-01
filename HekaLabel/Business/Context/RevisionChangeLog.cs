using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaLabel.Business.Context
{
    public class RevisionChangeLog
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string ModelNo { get; set; }
        public string RevisionNo { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
