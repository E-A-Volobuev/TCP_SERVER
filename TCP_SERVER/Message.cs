using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_SERVER
{
    [Table(Name = "Messages")]
    public class Message
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(Name = "FromUser")]
        public string FromUser { get; set; }
        [Column(Name = "ToUser")]
        public string ToUser { get; set; }
        [Column(Name = "TimeDate")]
        public DateTime TimeDate  { get; set; }
        [Column(Name = "Text")]
        public string Text { get; set; }
    }
}
