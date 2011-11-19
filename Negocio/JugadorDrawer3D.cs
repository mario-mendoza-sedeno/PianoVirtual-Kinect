using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PianoWPFClient.Model;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace PianoWPFClient.Utils
{
    public class JugadorDrawer3D:Drawer3D
    {
        public static void Draw(Jugador jugador, Viewport3D viewport3D, Dimensiones dimMano)
        {
            //mano izq
            ModelVisual3D modelManoIzq = DrawMano(jugador, dimMano, Brushes.Blue);
            viewport3D.Children.Add(modelManoIzq);
            jugador.ManoIzquierda = (Model3DGroup)modelManoIzq.Content;

            //mano der
            ModelVisual3D modelManoDer = DrawMano(jugador, dimMano, Brushes.Green);
            viewport3D.Children.Add(modelManoDer);
            jugador.ManoDerecha = (Model3DGroup)modelManoDer.Content;
        }

        private static ModelVisual3D DrawMano(Jugador jugador, Dimensiones dimMano, Brush brush)
        {
            Point3D p0 = new Point3D(0, 0, 0);
            Point3D p1 = new Point3D(dimMano.Ancho, 0, 0);
            Point3D p2 = new Point3D(dimMano.Ancho, 0, dimMano.Profundidad);
            Point3D p3 = new Point3D(0, 0, dimMano.Profundidad);
            Point3D p4 = new Point3D(0, dimMano.Alto, 0);
            Point3D p5 = new Point3D(dimMano.Ancho, dimMano.Alto, 0);
            Point3D p6 = new Point3D(dimMano.Ancho, dimMano.Alto, dimMano.Profundidad);
            Point3D p7 = new Point3D(0, dimMano.Alto, dimMano.Profundidad);
            /*
            //Modelo
            List<Point3D> puntosModelo = new List<Point3D>();
            puntosModelo.Add(p0);
            puntosModelo.Add(p1);
            puntosModelo.Add(p2);
            puntosModelo.Add(p3);
            puntosModelo.Add(p4);
            puntosModelo.Add(p5);
            puntosModelo.Add(p6);
            puntosModelo.Add(p7);
            
            Model3DGroup manoModel3D = new Model3DGroup();
            manoModel3D.Children.Add(DrawCubeModel(0, puntosModelo, brush));

            ModelVisual3D modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = manoModel3D;
            return modelVisual3D;
            */

            
             Model3DGroup cube = new Model3DGroup();
            //front side triangles
            cube.Children.Add(CreateTriangleModel(p3, p2, p6, brush));
            cube.Children.Add(CreateTriangleModel(p3, p6, p7, brush));
            //right side triangles
            cube.Children.Add(CreateTriangleModel(p2, p1, p5, brush));
            cube.Children.Add(CreateTriangleModel(p2, p5, p6, brush));
            //back side triangles
            cube.Children.Add(CreateTriangleModel(p1, p0, p4, brush));
            cube.Children.Add(CreateTriangleModel(p1, p4, p5, brush));
            //left side triangles
            cube.Children.Add(CreateTriangleModel(p0, p3, p7, brush));
            cube.Children.Add(CreateTriangleModel(p0, p7, p4, brush));
            //top side triangles
            cube.Children.Add(CreateTriangleModel(p7, p6, p5, brush));
            cube.Children.Add(CreateTriangleModel(p7, p5, p4, brush));
            //bottom side triangles
            cube.Children.Add(CreateTriangleModel(p2, p3, p0, brush));
            cube.Children.Add(CreateTriangleModel(p2, p0, p1, brush));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = cube;
            return model; 

            /*
            Model3DGroup cube = new Model3DGroup();
            cube.Children.Add(CreateTriangleModel(p2, p1, p0, brush));
            cube.Children.Add(CreateTriangleModel(p2, p0, p3, brush));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = cube;
            return model;*/
        }
    }
}
