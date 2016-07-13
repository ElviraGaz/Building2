using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Building2
{
    class Vertex
    {
        public double X;
        public double Y;
        public double Z;
        public double u, v;


        public float textureX, textureY;
        public static int N = 4;
        Vertex n;


        public Vertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            
        }


        public Vertex(Vertex vertex)
        {
            this.X = vertex.X;
            this.Y = vertex.Y;
            this.Z = vertex.Z;
           
        }


        public Vertex Normal
        {
            get
            {
                return n;
            }
        }

        public double[] getVector()
        {
            double[] vector = new double[N];
            vector[0] = X;
            vector[1] = Y;
            vector[2] = Z;
           
            return vector;
        }

        public void setVector(double[] vector)
        {
            X = vector[0];
            Y = vector[1];
            Z = vector[2];
           
        }


        //вычисляет косинус через скалярное произведение
        public static double Cos(Vertex a, Vertex b)
        {
            return (a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (a.LenthVector() * b.LenthVector());
        }

        //Длина вектора
        public double LenthVector()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }



        public void Normalize()
        {
            double l = LenthVector();
            if (l != 0)
            {
                X /= l;
                Y /= l;
                Z /= l;
            };
        }



    }
}
