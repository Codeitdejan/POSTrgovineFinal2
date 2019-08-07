using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPOS.Eracun.Entities
{
    public class FakturaStavka
    {
        public string Naziv { get; set; }
        public decimal Cijena { get; set; }
        public decimal Kolicina { get; set; }
        public decimal Pdv { get; set; }
        public decimal IznosBezPdv { get; set; }
        public decimal IznosSaPdv { get; set; }
    }
}
