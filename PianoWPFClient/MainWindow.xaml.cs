using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// namespaces
using Coding4Fun.Kinect.Wpf;
using KinectNui = Microsoft.Research.Kinect.Nui;
using System.Windows.Media.Media3D;
using System.ServiceModel;
using Negocio;
using Negocio.Model;
using KinectManipulation;

namespace PianoWPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        #region Campos
        JuegoPiano juego;
        Dimensiones factorConversion;
        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            juego = new JuegoPiano(viewport3D);
            Kinect.KinectDetection();
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Se ponen dimensiones menores al teclado (x= -14 +14, y= -2 +2, z= 0-12)
            factorConversion = new Dimensiones(12, 1.5, 12);

            juego.SetIniciarSesionCompletedAction(delegate() {
                conectarWS.Content = "Desconectar";
            });

            juego.SetFinalizarSesionCompletedAction(delegate(){
                conectarWS.Content = "Conectar";
            });

            juego.IniciarSesion();

            // Leer el angulo de elavación actual del kinect 
            elevationAngleSlider.Value = Kinect.ElevationAngle;

            // Establecer un Action para cuando un frame de video esta listo
            Kinect.BitmapSourceActionForVideo = (Action<BitmapSource>)delegate(BitmapSource bitmapSource)
            {
                videoImage.Source = bitmapSource;
            };

            // Establecer un Action para cuando un frame de profundidad esta listo
            Kinect.BitmapSourceActionForDepth = (Action<BitmapSource>)delegate(BitmapSource bitmapSource)
            {
                depthImage.Source = bitmapSource;
            };

            //Establecer un Action para cuando el skeleton este listo
            Kinect.TrackedUsersActionForSkeleton = (Action<List<KinectUser>>)delegate(List<KinectUser> kinectUsers)
            {
                foreach (KinectUser kinectUser in kinectUsers)
                {
                    juego.UpdatePosition(kinectUser.TrackingID, escalarPosicion(kinectUser.HandLeft), escalarPosicion(kinectUser.HandRight));
                }
            };

            //Establecer un Action para cuando se detecta un usuario nuevo
            Kinect.AddKinectUserAction = (Action<KinectUser>)delegate(KinectUser kinectUser)
            {
                juego.addJugador(kinectUser.TrackingID);
            };

            //Establecer un Action para cuando se deja de detectar un usuario nuevo
            Kinect.RemoveKinectUserAction = (Action<KinectUser>)delegate(KinectUser kinectUser)
            {
                juego.removeJugador(kinectUser.TrackingID);
            };
        }

        private Point3D escalarPosicion(KinectNui.Vector posicion) {
            double z = 2 * factorConversion.Profundidad * posicion.Z - 3 * factorConversion.Profundidad;
            return new Point3D(2 * posicion.X * factorConversion.Ancho, 2 * posicion.Y * factorConversion.Ancho, z);
        }

        private void elevationAngleSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Kinect.ElevationAngle = (int)((Slider)sender).Value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Kinect.Uninitialize();
            juego.FinalizarSesion();
        }

        private void conectarWS_Click(object sender, RoutedEventArgs e)
        {
            if (juego.Connected)
            {
                conectarWS.Content = "Desconectando ...";
                juego.FinalizarSesion();
            }
            else {
                conectarWS.Content = "Conectando ...";
                juego.IniciarSesion();
            }
        }
    }
}
