using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Negocio.Model;
using System.Timers;
using System.Windows.Threading;

namespace Negocio
{
    public class JuegoPiano
    {
        public Dictionary<int,Jugador> Jugadores {get; set;}

        public Teclado Teclado { get; set; }

        public Viewport3D Viewport3D { get; set; }

        public PianoWSDuplexClient pianoWSDuplexClient;

        public JuegoPiano(Viewport3D viewport3D) {
            Jugadores = new Dictionary<int,Jugador>();
            //Se inicializa el teclado con el número de octavas
            Teclado = new Teclado(2);
            Teclado.Draw3D(viewport3D);
            this.Viewport3D = viewport3D;
            //init WS
            pianoWSDuplexClient = new PianoWSDuplexClient();
            pianoWSDuplexClient.SetEjecutarNotaAction(delegate(string nota) {
                Tecla tecla = Teclado.Teclas[nota];
                tecla.UpdatePosition(new Point3D(0, - tecla.Dimensiones.Alto, 0));
                tecla.Sonar();

                //Timer para regresar la tecla a su lugar original
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler((Action<object, EventArgs>)delegate(object sender, EventArgs e)
                {
                    tecla.UpdatePosition(new Point3D(0, 0, 0));
                    (sender as DispatcherTimer).Stop();
                });
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            });
        }

        public void IniciarSesion() {
            pianoWSDuplexClient.IniciarSesionAsync();
        }

        public void FinalizarSesion() {
            pianoWSDuplexClient.FinalizarSesionAsync();
        }

        public void addJugador(int id) {
            if (!Jugadores.ContainsKey(id) && Jugadores.Count != 1) {
                Jugador jugador = new Jugador(new Dimensiones(0.3, 0.10, 0.6));
                jugador.Draw3D(Viewport3D);
                Point3D posicionInicial = new Point3D(0.0, 0.4, 2.0);
                jugador.UpdatePositionManoDerecha(posicionInicial);
                jugador.UpdatePositionManoIzquierda(posicionInicial);
                Jugadores.Add(id, jugador);
            }
        }

        public void removeJugador(int id) {
            if (Jugadores.ContainsKey(id)) {
                Jugadores.Remove(id);
            }
        }

        public void UpdatePosition(int id, Point3D posicionManoIzq, Point3D posicionManoDer){

            if (posicionManoIzq.Y < 0)
            {
                posicionManoIzq.Y = 0;
                Jugadores[id].UpdatePositionManoIzquierda(posicionManoIzq);
            }
            if (posicionManoDer.Y < 0)
            {
                posicionManoDer.Y = 0;
                Jugadores[id].UpdatePositionManoDerecha(posicionManoDer);
            }
            //actualizar posicion jugador
            Jugadores[id].UpdatePositionManoIzquierda(posicionManoIzq);
            Jugadores[id].UpdatePositionManoDerecha(posicionManoDer);
            Rect3D boundsManoIzq = Jugadores[id].ManoIzquierda.Bounds;
            Rect3D boundsManoDer = Jugadores[id].ManoDerecha.Bounds;
            foreach (KeyValuePair<string, Tecla> pair in Teclado.Teclas)
            {
                Tecla tecla = pair.Value;
                bool intersectsWithManoIzq = tecla.IntersectsWith(boundsManoIzq);
                bool intersectsWithManoDer = tecla.IntersectsWith(boundsManoDer);

                if (intersectsWithManoIzq || intersectsWithManoDer)
                {
                    if (intersectsWithManoIzq)
                    {
                        Point3D newPosition = new Point3D();
                        newPosition.Y = boundsManoIzq.Y - tecla.Dimensiones.Alto;
                        if (tecla.UpdatePosition(newPosition))
                        {
                            pianoWSDuplexClient.PublicarNota(tecla.Nota.ToString());
                        }
                    }
                    if (intersectsWithManoDer)
                    {
                        Point3D newPosition = new Point3D();
                        newPosition.Y = boundsManoDer.Y - tecla.Dimensiones.Alto;
                        if (tecla.UpdatePosition(newPosition))
                        {
                            pianoWSDuplexClient.PublicarNota(tecla.Nota.ToString());
                        }
                    }
                }
                else
                {
                    tecla.UpdatePosition(new Point3D());
                }
            }
        }
    }
}
