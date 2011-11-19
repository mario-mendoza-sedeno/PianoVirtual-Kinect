using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using PianoWPFClient.Model;
using PianoWPFClient.Utils;


namespace PianoWPFClient.Utils
{
    public class TecladoDrawer3D:Drawer3D
    {
        public static void Draw(Teclado teclado, Viewport3D viewport3D, Dimensiones dimTeclaBlanca, Dimensiones dimTeclaNegra) {
            Dictionary<string, Tecla> teclas = teclado.Teclas;
            int numTeclas = teclas.Count;
            double separacionTeclas = 0.015;
            double offset = -calcularAnchoTeclado(teclado, dimTeclaBlanca.Ancho, separacionTeclas)/2;
            //Dimensiones auxiliares con margen
            Dimensiones dimTeclaNegraConMargen = new Dimensiones();
            dimTeclaNegraConMargen.Ancho = dimTeclaNegra.Ancho + separacionTeclas * 2;
            dimTeclaNegraConMargen.Profundidad = dimTeclaNegra.Profundidad + separacionTeclas;
            dimTeclaNegraConMargen.Alto = dimTeclaNegra.Alto;
            for (int i = 0, iTeclaBlanca = 0; i < numTeclas; i++)
            {
                Tecla tecla = teclas.ElementAt(i).Value;
                Tecla teclaAnterior = (i == 0) ? null : teclas.ElementAt(i - 1).Value;
                Tecla teclaSiguiente = (i == numTeclas - 1) ? null : teclas.ElementAt(i + 1).Value;
                double posicionX = offset + iTeclaBlanca * dimTeclaBlanca.Ancho;
                posicionX += iTeclaBlanca * separacionTeclas;
                ModelVisual3D model = null;
                // tecla blanca
                if (typeof(TeclaBlanca3D) == tecla.GetType())
                {
                    if (isTeclaBlanca(teclaAnterior))
                    {
                        if (isTeclaBlanca(teclaSiguiente))
                        {
                            //blanca con anterior blanca y siguiente blanca
                            model = DrawTeclaBlancaTipoA(tecla, dimTeclaBlanca, posicionX, BrushesTeclas.Blanco);
                        }
                        else
                        {
                            //blanca con anterior blanca y siguiente negra;
                            model = DrawTeclaBlancaTipoB(tecla, dimTeclaBlanca, dimTeclaNegraConMargen, posicionX, BrushesTeclas.Blanco);
                        }
                    }
                    else
                    {
                        if (isTeclaBlanca(teclaSiguiente))
                        {
                            //blanca con anterior negra y siguiente blanca
                            model = DrawTeclaBlancaTipoC(tecla, dimTeclaBlanca, dimTeclaNegraConMargen, posicionX, BrushesTeclas.Blanco);
                        }
                        else
                        {
                            //blanca con anterior negra y siguiente negra;
                            model = DrawTeclaBlancaTipoD(tecla, dimTeclaBlanca, dimTeclaNegraConMargen, posicionX, BrushesTeclas.Blanco);
                        }
                    }
                    iTeclaBlanca++;
                }
                else //tecla negra
                {
                    posicionX += separacionTeclas;
                    model = DrawTeclaNegra(tecla, dimTeclaNegra, posicionX, BrushesTeclas.Negro);
                }
                viewport3D.Children.Add(model);
            }
        }

        private static bool isTeclaBlanca(Tecla tecla) {
            return (tecla == null || typeof(TeclaBlanca3D) == tecla.GetType()) ? true : false;
        }

        private static double calcularAnchoTeclado(Teclado teclado, double anchoTeclaBlanca, double separacionTeclas)
        {
            int numTeclasBlancas = 0;
            foreach (KeyValuePair<string, Tecla> pair in teclado.Teclas)
            {
                if (typeof(TeclaBlanca3D) == pair.Value.GetType())
                {
                    numTeclasBlancas++;
                }
            }
            return numTeclasBlancas * (anchoTeclaBlanca + separacionTeclas);
        }

/*
 * 
 *       p0______p3     p0___p5 	     p0____p5       p0___p7       p0___p3
 *        /     /        /  /             /   /          /  /          /  /
 *       /     /        /  /__p3    p2 __/   /      p2__/  /__p5    p1/__/p2
 *      /     /        /  p4 /        /  p1 /        / p1 p6 /
 *     /     /        /     /        /     /        /       /
 *  p1/_____/p2    p1/_____/p2    p3/_____/p4    p3/_______/p4
 * 
 */

        /// <summary>
        /// Dibuja una tecla blanca con una tecla anterior blanca y una tecla siguiente blanca
        /// </summary>
        /// <param name="tecla"></param>
        /// <param name="x"></param>
        /// <returns></returns>

        private static ModelVisual3D DrawTeclaBlancaTipoA(Tecla tecla, Dimensiones dimTeclaBlanca, double x, Brush brush)
        {
            Point3D p0 = new Point3D(x, 0, 0);
            Point3D p1 = new Point3D(x + dimTeclaBlanca.Ancho, 0, 0);
            Point3D p2 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaBlanca.Profundidad);
            Point3D p3 = new Point3D(x, 0, dimTeclaBlanca.Profundidad);
            Point3D p4 = new Point3D(x, dimTeclaBlanca.Alto, 0);
            Point3D p5 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, 0);
            Point3D p6 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p7 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);

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
            Model3DGroup teclaModel3D = new Model3DGroup();
            teclaModel3D.Children.Add(DrawCubeModel(x, puntosModelo, brush));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = teclaModel3D;
            tecla.TeclaModel3D = teclaModel3D;
            return model;
        }



        /// <summary>
        ///  Dibuja una tecla blanca con una tecla anterior blanca y una tecla siguiente negra
        /// </summary>
        /// <param name="tecla"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static ModelVisual3D DrawTeclaBlancaTipoB(Tecla tecla, Dimensiones dimTeclaBlanca, Dimensiones dimTeclaNegra, double x, Brush brush)
        {
            double mitadAnchoTeclaNegra = dimTeclaNegra.Ancho / 2;
            double diferenciaAncho = dimTeclaBlanca.Ancho - mitadAnchoTeclaNegra;
            
            //Puntos capa inferior
            Point3D p0 = new Point3D(x, 0, 0);
            Point3D p1 = new Point3D(x + diferenciaAncho, 0, 0);
            Point3D p2 = new Point3D(x + diferenciaAncho, 0, dimTeclaNegra.Profundidad);
            Point3D p3 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaNegra.Profundidad);
            Point3D p4 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaBlanca.Profundidad);
            Point3D p5 = new Point3D(x, 0, dimTeclaBlanca.Profundidad);
            Point3D p6 = new Point3D(x, 0, dimTeclaNegra.Profundidad);
            //Puntos capa superior
            Point3D p7 = new Point3D(x, dimTeclaBlanca.Alto, 0);
            Point3D p8 = new Point3D(x + diferenciaAncho, dimTeclaBlanca.Alto, 0);
            Point3D p9 = new Point3D(x + diferenciaAncho, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p10 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p11 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p12 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p13 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            
            //Modelo trasero
            List<Point3D> puntosModeloTrasero = new List<Point3D>();
            puntosModeloTrasero.Add(p0);
            puntosModeloTrasero.Add(p1);
            puntosModeloTrasero.Add(p2);
            puntosModeloTrasero.Add(p6);
            puntosModeloTrasero.Add(p7);
            puntosModeloTrasero.Add(p8);
            puntosModeloTrasero.Add(p9);
            puntosModeloTrasero.Add(p13);
            Model3DGroup backModel3D = DrawCubeModel(x, puntosModeloTrasero, brush);

            //Modelo delantero
            List<Point3D> puntosModeloDelantero = new List<Point3D>();
            puntosModeloDelantero.Add(p6);
            puntosModeloDelantero.Add(p3);
            puntosModeloDelantero.Add(p4);
            puntosModeloDelantero.Add(p5);
            puntosModeloDelantero.Add(p13);
            puntosModeloDelantero.Add(p10);
            puntosModeloDelantero.Add(p11);
            puntosModeloDelantero.Add(p12);
            Model3DGroup frontModel3D = DrawCubeModel(x, puntosModeloDelantero, brush);

            //Modelo completo
            Model3DGroup teclaModel3D = new Model3DGroup();
            teclaModel3D.Children.Add(backModel3D);
            teclaModel3D.Children.Add(frontModel3D);

            ModelVisual3D model = new ModelVisual3D();
            model.Content = teclaModel3D;
            tecla.TeclaModel3D = teclaModel3D;
            return model;
        }


        /// <summary>
        ///  Dibuja una tecla blanca con una tecla anterior negra y una tecla siguiente blanca
        /// </summary>
        /// <param name="tecla"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static ModelVisual3D DrawTeclaBlancaTipoC(Tecla tecla, Dimensiones dimTeclaBlanca, Dimensiones dimTeclaNegra, double x, Brush brush)
        {
            double mitadAnchoTeclaNegra = dimTeclaNegra.Ancho / 2;
            //Puntos capa inferior
            Point3D p0 = new Point3D(x + mitadAnchoTeclaNegra, 0, 0);
            Point3D p1 = new Point3D(x + dimTeclaBlanca.Ancho, 0, 0);
            Point3D p2 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaNegra.Profundidad);
            Point3D p3 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaBlanca.Profundidad);
            Point3D p4 = new Point3D(x, 0, dimTeclaBlanca.Profundidad);
            Point3D p5 = new Point3D(x, 0, dimTeclaNegra.Profundidad);
            Point3D p6 = new Point3D(x + mitadAnchoTeclaNegra, 0, dimTeclaNegra.Profundidad);
            //Puntos capa superior
            Point3D p7 = new Point3D(x + mitadAnchoTeclaNegra, dimTeclaBlanca.Alto, 0);
            Point3D p8 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, 0);
            Point3D p9 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p10 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p11 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p12 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p13 = new Point3D(x + mitadAnchoTeclaNegra, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);

            //Modelo trasero
            List<Point3D> puntosModeloTrasero = new List<Point3D>();
            puntosModeloTrasero.Add(p0);
            puntosModeloTrasero.Add(p1);
            puntosModeloTrasero.Add(p2);
            puntosModeloTrasero.Add(p6);
            puntosModeloTrasero.Add(p7);
            puntosModeloTrasero.Add(p8);
            puntosModeloTrasero.Add(p9);
            puntosModeloTrasero.Add(p13);
            Model3DGroup backModel3D = DrawCubeModel(x, puntosModeloTrasero, brush);

            //Modelo delantero
            List<Point3D> puntosModeloDelantero = new List<Point3D>();
            puntosModeloDelantero.Add(p5);
            puntosModeloDelantero.Add(p2);
            puntosModeloDelantero.Add(p3);
            puntosModeloDelantero.Add(p4);
            puntosModeloDelantero.Add(p12);
            puntosModeloDelantero.Add(p9);
            puntosModeloDelantero.Add(p10);
            puntosModeloDelantero.Add(p11);
            Model3DGroup frontModel3D = DrawCubeModel(x, puntosModeloDelantero, brush);

            //Modelo completo
            Model3DGroup teclaModel3D = new Model3DGroup();
            teclaModel3D.Children.Add(backModel3D);
            teclaModel3D.Children.Add(frontModel3D);

            ModelVisual3D model = new ModelVisual3D();
            model.Content = teclaModel3D;
            tecla.TeclaModel3D = teclaModel3D;
            return model;
        }

        /// <summary>
        ///   Dibuja una tecla blanca con una tecla anterior negra y una tecla siguiente negra
        /// </summary>
        /// <param name="tecla"></param>
        /// <param name="x"></param>
        /// <param name="dimTeclaBlanca"></param>
        /// <param name="dimTeclaNegra"></param>
        /// <returns></returns>
        private static ModelVisual3D DrawTeclaBlancaTipoD(Tecla tecla, Dimensiones dimTeclaBlanca, Dimensiones dimTeclaNegra, double x, Brush brush)
        {
            double mitadAnchoTeclaNegra = dimTeclaNegra.Ancho / 2;
            double diferenciaAncho = dimTeclaBlanca.Ancho - mitadAnchoTeclaNegra;
            //Puntos capa inferior
            Point3D p0 = new Point3D(x + mitadAnchoTeclaNegra, 0, 0);
            Point3D p1 = new Point3D(x + diferenciaAncho, 0, 0);
            Point3D p2 = new Point3D(x + diferenciaAncho, 0, dimTeclaNegra.Profundidad);
            Point3D p3 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaNegra.Profundidad);
            Point3D p4 = new Point3D(x + dimTeclaBlanca.Ancho, 0, dimTeclaBlanca.Profundidad);
            Point3D p5 = new Point3D(x, 0, dimTeclaBlanca.Profundidad);
            Point3D p6 = new Point3D(x, 0, dimTeclaNegra.Profundidad);
            Point3D p7 = new Point3D(x + mitadAnchoTeclaNegra, 0, dimTeclaNegra.Profundidad);
            //Puntos capa superior
            Point3D p8 = new Point3D(x + mitadAnchoTeclaNegra, dimTeclaBlanca.Alto, 0);
            Point3D p9 = new Point3D(x + diferenciaAncho, dimTeclaBlanca.Alto, 0);
            Point3D p10 = new Point3D(x + diferenciaAncho, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p11 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p12 = new Point3D(x + dimTeclaBlanca.Ancho, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p13 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaBlanca.Profundidad);
            Point3D p14 = new Point3D(x, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);
            Point3D p15 = new Point3D(x + mitadAnchoTeclaNegra, dimTeclaBlanca.Alto, dimTeclaNegra.Profundidad);

            //Modelo trasero
            List<Point3D> puntosModeloTrasero = new List<Point3D>();
            puntosModeloTrasero.Add(p0);
            puntosModeloTrasero.Add(p1);
            puntosModeloTrasero.Add(p2);
            puntosModeloTrasero.Add(p7);
            puntosModeloTrasero.Add(p8);
            puntosModeloTrasero.Add(p9);
            puntosModeloTrasero.Add(p10);
            puntosModeloTrasero.Add(p15);
            Model3DGroup backModel3D = DrawCubeModel(x, puntosModeloTrasero, brush);

            //Modelo delantero
            List<Point3D> puntosModeloDelantero = new List<Point3D>();
            puntosModeloDelantero.Add(p6);
            puntosModeloDelantero.Add(p3);
            puntosModeloDelantero.Add(p4);
            puntosModeloDelantero.Add(p5);
            puntosModeloDelantero.Add(p14);
            puntosModeloDelantero.Add(p11);
            puntosModeloDelantero.Add(p12);
            puntosModeloDelantero.Add(p13);
            Model3DGroup frontModel3D = DrawCubeModel(x, puntosModeloDelantero, brush);

            //Modelo completo
            Model3DGroup teclaModel3D = new Model3DGroup();
            teclaModel3D.Children.Add(backModel3D);
            teclaModel3D.Children.Add(frontModel3D);

            ModelVisual3D model = new ModelVisual3D();
            model.Content = teclaModel3D;
            tecla.TeclaModel3D = teclaModel3D;
            return model;
        }

        /// <summary>
        /// Dibuja una tecla blanca con una tecla anterior blanca y una tecla siguiente blanca
        /// </summary>
        /// <param name="tecla"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static ModelVisual3D DrawTeclaNegra(Tecla tecla, Dimensiones dimTeclaNegra, double x, Brush brush)
        {
            x = x - dimTeclaNegra.Ancho / 2;
            
            Point3D p0 = new Point3D(x , 0, 0);
            Point3D p1 = new Point3D(x + dimTeclaNegra.Ancho, 0, 0);
            Point3D p2 = new Point3D(x + dimTeclaNegra.Ancho, 0, dimTeclaNegra.Profundidad);
            Point3D p3 = new Point3D(x, 0, dimTeclaNegra.Profundidad);
            Point3D p4 = new Point3D(x, dimTeclaNegra.Alto, 0);
            Point3D p5 = new Point3D(x + dimTeclaNegra.Ancho, dimTeclaNegra.Alto, 0);
            Point3D p6 = new Point3D(x + dimTeclaNegra.Ancho, dimTeclaNegra.Alto, dimTeclaNegra.Profundidad);
            Point3D p7 = new Point3D(x, dimTeclaNegra.Alto, dimTeclaNegra.Profundidad);
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
            Model3DGroup teclaModel3D = new Model3DGroup();
            teclaModel3D.Children.Add(DrawCubeModel(x, puntosModelo, brush));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = teclaModel3D;
            tecla.TeclaModel3D = teclaModel3D;
            return model;
        }
    }
}
