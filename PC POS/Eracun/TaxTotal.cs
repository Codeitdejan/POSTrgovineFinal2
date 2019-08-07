using PCPOS.Eracun.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPOS.Eracun
{
    public class TaxTotal
    {
        public TaxCategory Category { get; set; }
        public TaxScheme Scheme { get; set; }

        public struct TaxCategory
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public decimal Percent { get; set; }
        }

        public struct TaxScheme
        {
            public string Name { get; set; }
            public TaxTypeCode TaxTypeCode { get; set; }
        }
    }
}
