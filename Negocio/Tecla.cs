using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.IO;
using System.Reflection;
using System.Media;
using Negocio.Properties;
using Negocio.Utils;

namespace Negocio.Model
{
    public class Tecla
    {
        public Dimensiones Dimensiones { get; set; }

        protected Nota _nota;

        public Nota Nota { get { return _nota; } set { _nota = value; } }

        public Model3DGroup TeclaModel3D { get; set; }

        protected MediaPlayer MPlayer { get; set; }

        protected Uri UriSound { get; set; }

        protected bool IsPlaying { get; set; }

        protected Tecla(Nota nota){
            Nota = nota;
            MPlayer = new MediaPlayer();
            String fileName = nota.ToString() + ".wav";
            UriSound = new Uri(@"Sonidos\" + fileName, UriKind.Relative);
        }

        public bool IntersectsWith(Rect3D rect3D) {
            bool intersects = false;
            foreach (Model3D media3D in TeclaModel3D.Children)
            {
                if (media3D.Bounds.IntersectsWith(rect3D))
                {
                    intersects = true;
                }
            }
            return intersects;
        }

        public bool UpdatePosition(Point3D posicionTecla)
        {
            bool emiteSonido = false;
            TranslateTransform3D translateTransform3DIzq = new TranslateTransform3D();
            translateTransform3DIzq.OffsetX = posicionTecla.X;
            translateTransform3DIzq.OffsetY = posicionTecla.Y;
            translateTransform3DIzq.OffsetZ = posicionTecla.Z;
            TeclaModel3D.Transform = translateTransform3DIzq;
            if (!IsPlaying)
            {
                if (puedeEmitirSonido(posicionTecla.Y))
                {
                    Sonar();
                    emiteSonido = true;
                    IsPlaying = true;
                }
                else
                {
                    IsPlaying = false;
                }
            }
            else {
                if (!puedeEmitirSonido(posicionTecla.Y))
                {
                    IsPlaying = false;
                }
            }
            return emiteSonido;
        }

        private bool puedeEmitirSonido(double despY) {
            return (Math.Abs(despY) >= TeclaModel3D.Bounds.SizeY / 4);
        }

        public void Sonar() {
            if (MPlayer.Clock != null)
            {
                if (MPlayer.Clock.CurrentState == ClockState.Filling
                    || MPlayer.Clock.CurrentState == ClockState.Stopped
                    )
                {
                    MPlayer.Clock.Controller.Begin();
                }
                else if (MPlayer.Clock.CurrentState == ClockState.Active)
                {
                    MPlayer.Clock.Controller.Stop();
                    MPlayer.Clock.Controller.Begin();
                }
            }
            else
            {
                MediaClock mediaClock = new MediaTimeline(UriSound).CreateClock(true) as MediaClock;
                MPlayer.Clock = mediaClock;
            }
        }
    }

    public class TeclaBlanca3D : Tecla 
    {
        public TeclaBlanca3D(Nota nota) : base(nota)
        {
        }
    }

    public class TeclaNegra3D : Tecla
    {
        public TeclaNegra3D(Nota nota) : base(nota)
        {
        }
    }
}