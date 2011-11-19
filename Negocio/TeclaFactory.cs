using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PianoWPFClient.Model
{
    public class TeclaFactory
    {
        private Dimensiones dimTeclaBlanca;

        private Dimensiones dimTeclaNegra;

        private TeclaFactory() { }

        public TeclaFactory(Dimensiones dimTeclaBlanca, Dimensiones dimTeclaNegra)
        {
            if (dimTeclaBlanca.Ancho <= dimTeclaNegra.Ancho
                || dimTeclaBlanca.Profundidad <= dimTeclaNegra.Profundidad) {
                    throw new System.ArgumentException("El Ancho y Profundidad de la tecla negra deben ser menores a los de la tecla blanca", "dimTeclaNegra");
            }
            if (dimTeclaBlanca.Alto >= dimTeclaNegra.Alto) {
                throw new System.ArgumentException("El Alto de la tecla negra deben ser mayor al de la tecla blanca", "dimTeclaNegra");
            }
            this.dimTeclaBlanca = dimTeclaBlanca;
            this.dimTeclaNegra = dimTeclaNegra;
        }

        public Tecla newTecla(TipoNota tipoNota, bool sostenido, int escala)
        {
            Tecla newTecla = null;
            if (sostenido)
            {
                newTecla = new TeclaNegra3D(new Nota(tipoNota, sostenido, escala));
                newTecla.Dimensiones = dimTeclaNegra;
            }
            else {
                newTecla = new TeclaBlanca3D(new Nota(tipoNota, sostenido, escala)); ;
                newTecla.Dimensiones = dimTeclaBlanca;
            }
            return newTecla;
        }
    }
}
