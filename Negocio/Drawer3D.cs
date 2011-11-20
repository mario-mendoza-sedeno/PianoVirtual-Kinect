using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Negocio.Utils
{
    public abstract class Drawer3D
    {

        protected static Model3DGroup DrawCubeModel(double x, List<Point3D> points, Brush brush)
        {
            Model3DGroup model3D = new Model3DGroup();
            // capa trasera
            model3D.Children.Add(CreateTriangleModel(points[0], points[1], points[5], brush));
            model3D.Children.Add(CreateTriangleModel(points[0], points[5], points[4], brush));
            // capa lateral derecho
            if (x > 0)
            {
                model3D.Children.Add(CreateTriangleModel(points[5], points[1], points[2], brush));
                model3D.Children.Add(CreateTriangleModel(points[5], points[2], points[6], brush));
            }
            else
            {
                model3D.Children.Add(CreateTriangleModel(points[5], points[2], points[1], brush));
                model3D.Children.Add(CreateTriangleModel(points[5], points[6], points[2], brush));
            }
            // capa lateral izquierdo
            if (x > 0)
            {
                model3D.Children.Add(CreateTriangleModel(points[0], points[3], points[7], brush));
                model3D.Children.Add(CreateTriangleModel(points[0], points[7], points[4], brush));
            }
            else
            {
                model3D.Children.Add(CreateTriangleModel(points[0], points[7], points[3], brush));
                model3D.Children.Add(CreateTriangleModel(points[0], points[4], points[7], brush));
            }
            // capa superior
            model3D.Children.Add(CreateTriangleModel(points[4], points[7], points[6], brush));
            model3D.Children.Add(CreateTriangleModel(points[4], points[6], points[5], brush));
            // capa inferior
            model3D.Children.Add(CreateTriangleModel(points[2], points[1], points[0], brush));
            model3D.Children.Add(CreateTriangleModel(points[2], points[0], points[3], brush));
            // capa frontal
            model3D.Children.Add(CreateTriangleModel(points[3], points[2], points[6], brush));
            model3D.Children.Add(CreateTriangleModel(points[3], points[6], points[7], brush));
            return model3D;
        }

        protected static GeometryModel3D CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2, Brush brush)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Vector3D normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        protected static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

    }
}
