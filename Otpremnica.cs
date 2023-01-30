using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfwltest
{
    public class Otpremnica
    {
        
        private String komitent;
        private String prevoznik;
        private String robuIzdao;
        private String robuPrimio;
        private String kultura;
        public List<Merenja> merenjas;
        public Otpremnica()
        {

        }

        public Otpremnica(String komitent, String prevoznik, String robuIzdao, String robuPrimio,String kultura)
        {
            
            this.Komitent = komitent;
            this.Prevoznik = prevoznik;
            this.RobuIzdao = robuIzdao;
            this.RobuPrimio = robuPrimio;
            this.Kultura = kultura;
        }
        public string Komitent { get => komitent; set => komitent = value; }
        public string Prevoznik { get => prevoznik; set => prevoznik = value; }
        public string RobuIzdao { get => robuIzdao; set => robuIzdao = value; }
        public string RobuPrimio { get => robuPrimio; set => robuPrimio = value; }
        public string Kultura { get => kultura; set => kultura = value; }
    }

}

