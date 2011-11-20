using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Negocio.Model
{

    [DataContract]
    public enum TipoNota { DO, RE, MI, FA, SOL, LA, SI };

    [DataContract]
    public class Nota
    {

        [DataMember]
        public TipoNota TipoNota { get; set; }

        [DataMember]
        public bool Sostenido { get; set; }

        [DataMember]
        public int Octava { get; set; }

       
        public Nota(TipoNota tipoNota, bool sostenido, int octava)
        {
            TipoNota = tipoNota;
            Sostenido = sostenido;
            Octava = octava;
        }

        public override string ToString()
        {
            return new StringBuilder(TipoNota.ToString())
                    .Append(((Sostenido) ? "_sostenido_" : ""))
                    .Append(Octava)
                    .ToString();
        }

    }

}


