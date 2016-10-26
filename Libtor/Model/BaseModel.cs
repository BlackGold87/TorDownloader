using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libtor.Model
{
    public abstract class BaseModel
    {
        public int ID { get; set; }
        public DateTime UTC_CreationDate { get; set; }
        public DateTime UTC_LastModify => DateTime.UtcNow;
    }
}
