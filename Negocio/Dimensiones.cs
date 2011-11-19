using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PianoWPFClient.Model
{
    public class Dimensiones
    {
        public double Ancho { get; set; }

        public double Alto { get; set; }

        public double Profundidad { get; set; }

        public Dimensiones(double ancho, double alto, double profundidad)
        {
            this.Ancho = ancho;
            this.Alto = alto;
            this.Profundidad = profundidad;
        }

        public Dimensiones()
        {
        }
    }
}
