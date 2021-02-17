using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_SERVER
{
    [Table(Name = "Zadachi")]
    public class Zadacha
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(Name = "Otdel")]
        public string Otdel { get; set; }
        [Column(Name = "Stadia")]
        public string Stadia { get; set; }
        [Column(Name = "Shifr")]
        public string Shifr { get; set; }
        [Column(Name = "Obj")]
        public string Obj { get; set; }//объект
        [Column(Name = "Prioritet")]
        public string Prioritet { get; set; }
        [Column(Name = "Srok")]
        public DateTime Srok { get; set; }
        [Column(Name = "Avtor")]
        public string Avtor { get; set; }
        [Column(Name = "Ispolnitel")]
        public string Ispolniteli { get; set; }
        [Column(Name = "Comment")]
        public string Comment { get; set; }
        [Column(Name = "Gotovnost")]
        public bool Gotovnost { get; set; }
        [Column(Name = "Prinyato")]
        public bool Prinyato { get; set; }
    }
}
