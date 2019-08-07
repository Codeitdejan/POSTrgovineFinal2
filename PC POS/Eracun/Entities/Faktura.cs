using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPOS.Eracun.Entities
{
    public class Faktura
    {
        public int BrojFakture { get; set; }
        public int IdPartner { get; set; }
        public int IdFakturirati { get; set; }
        public string Model { get; set; }
        public string ZiroRacun { get; set; }
        public int GodinaFakture { get; set; }
        public string Napomena { get; set; }
        public DateTime Datum { get; set; }
        public List<FakturaStavka> Stavke { get; set; }
    }
}
