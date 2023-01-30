  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfwltest
{
   public static class WeightData
    {
        public static event EventHandler NetoChanged;
        public static event EventHandler TaraChanged;
        public static event EventHandler BrutoChanged;
        public static event EventHandler ProizvodChanged;
        public static event EventHandler AmbalazaChanged;
        public static event EventHandler KolicinaChanged;
        public static event EventHandler VremeChanged;
        public static event EventHandler IdChanged;

        private static string _neto;
        private static string _tara;
        private static string _bruto;
        private static string _proizvod;
        private static string _ambalaza;
        private static string _kolicina;
        private static string _id;
        private static string _vreme;



        public static string Neto
        {
            get { return _neto; }
            set
            {
                if (value != _neto)
                {
                    _neto = value;

                    if (NetoChanged != null)
                        NetoChanged(null, EventArgs.Empty);
                }
            }
        }
        public static string Tara
        {
            get { return _tara; }
            set
            {
                if (value != _tara)
                {
                    _tara = value;

                    if (TaraChanged != null)
                        TaraChanged(null, EventArgs.Empty);
                }
            }
        }
        public static string Bruto
        {
            get { return _bruto; }
            set
            {
                if (value != _bruto)
                {
                    _bruto = value;

                    if (BrutoChanged != null)
                        BrutoChanged(null, EventArgs.Empty);
                }
            }
        }
        public static string Proizvod
        {
            get { return _proizvod; }
            set
            {
                if (value != _proizvod)
                {
                    _proizvod = value;

                    if (ProizvodChanged != null)
                        ProizvodChanged(null, EventArgs.Empty);
                }
            }
        }

        public static string Ambalaza
        {
            get { return _ambalaza; }
            set
            {
                if (value != _ambalaza)
                {
                    _proizvod = value;

                    if (AmbalazaChanged != null)
                        AmbalazaChanged(null, EventArgs.Empty);
                }
            }
        }

        //ovde je izracunavao malinarske gajbice 400g
        public static string Kolicina
        {
            get { return _kolicina; }
            set 
            {
                if (value != _kolicina)
                {
                    _kolicina = value;
                    double tezina = 0.4 * double.Parse(_kolicina);
                    double netoTezina = double.Parse(Bruto) - tezina;
                    Neto = netoTezina.ToString();
                    if (KolicinaChanged != null)
                        KolicinaChanged(null, EventArgs.Empty);
                }
            }
        }

        public static string Vreme
        {
            get { return _vreme; }
            set
            {
                if (value != _vreme)
                {
                    _vreme = value;

                    if (VremeChanged != null)
                        VremeChanged(null, EventArgs.Empty);
                }
            }
        }

        public static string Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;

                    if (IdChanged != null)
                        IdChanged(null, EventArgs.Empty);
                }
            }
        }



    }
}
