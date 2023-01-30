using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfwltest
{
    public class Merenja
    {
        public string Neto { get; set; }
        public string Bruto { get; set; }
        public string Tara { get; set; }
        public string Proizvod { get; set; }
        public string Ambalaza { get; set; }
        public string Kolicina { get; set; }
        public string Vreme { get; set; }


        public Merenja() { }
        public Merenja(string neto, string bruto, string tara,string proizvod,string ambalaza,string kolicina, string vreme)
        {
            this.Neto = neto;
            this.Bruto = bruto;
            this.Tara = tara;
            this.Proizvod = proizvod;
            this.Ambalaza = ambalaza;
            this.Kolicina = kolicina;
            this.Vreme = vreme;

        }
    }
}
