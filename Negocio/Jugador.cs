using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PianoWPFClient.Model;
using System.Windows.Media.Media3D;
using PianoWPFClient.Utils;

namespace PianoWPFClient
{
    public class Jugador
    {
        public Dimensiones DimMano { get; set; }

        public Model3DGroup ManoIzquierda { get; set; }

        public Model3DGroup ManoDerecha { get; set; }

        public Jugador(Dimensiones dimensiones) 
        {
            this.DimMano = dimensiones;
        }

        public void Draw3D(Viewport3D viewport)
        {
            JugadorDrawer3D.Draw(this, viewport, DimMano);
        }

        public void UpdatePositionManoIzquierda(Point3D posicionManoIzq) {
            TranslateTransform3D translateTransform3DIzq = new TranslateTransform3D();
            translateTransform3DIzq.OffsetX = posicionManoIzq.X;
            translateTransform3DIzq.OffsetY = posicionManoIzq.Y;
            translateTransform3DIzq.OffsetZ = posicionManoIzq.Z;
            ManoIzquierda.Transform = translateTransform3DIzq;
        }

        public void UpdatePositionManoDerecha(Point3D posicionManoDer)
        {
            TranslateTransform3D translateTransform3DDer = new TranslateTransform3D();
            translateTransform3DDer.OffsetX = posicionManoDer.X;
            translateTransform3DDer.OffsetY = posicionManoDer.Y;
            translateTransform3DDer.OffsetZ = posicionManoDer.Z;
            ManoDerecha.Transform = translateTransform3DDer;
        }

    }
}
