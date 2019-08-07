using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPOS.Eracun.Enums
{
    public enum QuantityCode
    {
        [Description("")]
        Null,
        [Description("H87")]
        Piece,
        [Description("KGM")]
        Kilogram,
        [Description("KMT")]
        Kilometre,
        [Description("GRM")]
        Gram,
        [Description("MTR")]
        Metre,
        [Description("LTR")]
        Litre,
        [Description("TNE")]
        Tonne,
        [Description("MTK")]
        SquareMetre,
        [Description("MTQ")]
        CubicMetre,
        [Description("MIN")]
        Minute,
        [Description("HUR")]
        Hour,
        [Description("DAY")]
        Day,
        [Description("MON")]
        Month,
        [Description("ANN")]
        Year
    }
}
