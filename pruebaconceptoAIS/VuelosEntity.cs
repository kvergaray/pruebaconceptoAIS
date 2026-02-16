using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaconceptoAIS
{
    public class VueloEntity
    {
        public string cod_vuelo { get; set; }
        public string tip_ope { get; set; }
        public string tip_trafico { get; set; }
        public string dsc_estado { get; set; }
        public string num_puerta { get; set; }
        public DateTime fch_hra_prog { get; set; }
        public DateTime fch_hra_ult { get; set; }


        public string VueloEstado { get; set; } // vuelo_estado
        public string TipMq { get; set; } // tip_mq
    }

}
