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
using Microsoft.Research.Kinect.Nui;
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
        Dimensiones _escala;
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
            juego.IniciarSesion();
            

            _escala = new Dimensiones(5, 5, 5);

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
                //foreach (KinectUser kinectUser in kinectUsers)
                //{
                    KinectUser kinectUser = kinectUsers[0];
                    Point3D handLeft = new Point3D(kinectUser.HandLeft.X * _escala.Ancho, kinectUser.HandLeft.Y * _escala.Ancho, kinectUser.HandLeft.Z * _escala.Profundidad);
                    Point3D handRight = new Point3D(kinectUser.HandRight.X * _escala.Ancho, kinectUser.HandRight.Y * _escala.Ancho, kinectUser.HandRight.Z * _escala.Profundidad);
                    juego.UpdatePosition(kinectUser.TrackingID, handLeft, handRight);
                //}
            };

            //Establecer un Action para cuando se detecta un usuario nuevo
            Kinect.AddKinectUserAction = (Action<KinectUser>)delegate(KinectUser kinectUser)
            {
                juego.addJugador(kinectUser.TrackingID);
            };
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

    }
}
