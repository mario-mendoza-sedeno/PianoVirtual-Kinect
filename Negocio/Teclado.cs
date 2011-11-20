using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Negocio.Model;
using Negocio.Utils;

namespace Negocio
{
    public class Teclado
    {
        public Dictionary<string, Tecla> Teclas { get; set; }

        public Dimensiones DimTeclaBlanca { get; set; }

        public Dimensiones DimTeclaNegra { get; set; }

        private TeclaFactory TeclaFactory { get; set; }

        private int Octavas { get; set; }

        public Teclado(int octavas)
        {
            Octavas = octavas;
            Teclas = new Dictionary<string, Tecla>();
            this.DimTeclaBlanca = new Dimensiones(1.0, 0.7, 6.0);
            this.DimTeclaNegra = new Dimensiones(0.6, 0.85, 4.0);
            TeclaFactory = new TeclaFactory(DimTeclaBlanca, DimTeclaNegra);
            
            //Agregar las octavas al teclado
            // DO, DO#, RE, RE#, MI, FA, FA#, SOL, SOL#, LA, LA#, SI
            for (int i = 1; i <= Octavas; i++) {
                AddNewTecla(TipoNota.DO, false, i);
                AddNewTecla(TipoNota.DO, true, i);
                AddNewTecla(TipoNota.RE, false, i);
                AddNewTecla(TipoNota.RE, true, i);
                AddNewTecla(TipoNota.MI, false, i);
                AddNewTecla(TipoNota.FA, false, i);
                AddNewTecla(TipoNota.FA, true, i);
                AddNewTecla(TipoNota.SOL, false, i);
                AddNewTecla(TipoNota.SOL, true, i);
                AddNewTecla(TipoNota.LA, false, i);
                AddNewTecla(TipoNota.LA, true, i);
                AddNewTecla(TipoNota.SI, false, i);
            }
        }

        private void AddNewTecla(TipoNota tipo, bool sostenido, int octava) {
            Tecla tecla = TeclaFactory.newTecla(tipo, sostenido, octava);
            Teclas.Add(tecla.Nota.ToString(), tecla);
        }

        public void Draw3D(Viewport3D viewport) {
            TecladoDrawer3D.Draw(this, viewport, DimTeclaBlanca, DimTeclaNegra);
        }
    }
        
}
