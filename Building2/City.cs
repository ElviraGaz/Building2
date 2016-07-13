using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
// для работы с библиотекой OpenGL
using Tao.OpenGl;
// для работы с библиотекой FreeGLUT
using Tao.FreeGlut;
// для работы с элементом управления SimpleOpenGLControl
using Tao.Platform.Windows;
using Tao.FreeType;
// для загрузки текстур и работы с ними
using Tao.DevIl;

namespace Building2
{
    class City
    {

        int countX;
        int countZ;
       
        int MaxLenght;     // глубина
        int MaxWidht;    //ширина

        public Vertex[,] arrayPointsGround;
        public List<Rect> rectangleGround;
        double n, t;
        Random r = new Random();
        double dU, dV;
        House house = new House(1, 1, 1, 1, 1, 1, 1, 1, 1);

         public City(int CX, int CZ, int MaxX, int MaxZ)
        {
            countX = CX;
          
            countZ = CZ;
            MaxWidht = MaxZ;
            MaxLenght = MaxX;
            rectangleGround = new List<Rect>();
            arrayPointsGround = new Vertex[countX + 1, countZ + 1];
            dU = (((double)MaxLenght / 180) * Math.PI) / countX;
            dV = (((double)MaxWidht / 180) * Math.PI) / countZ;
            double n = r.NextDouble();
            double t = r.NextDouble();
        }

         public void InputData(int CX, int CZ, int MaxX, int MaxZ)
         {
             countX = CX;
             countZ = CZ;
             MaxWidht = MaxZ;
             MaxLenght = MaxX;
             rectangleGround.Clear();
             arrayPointsGround = new Vertex[countX + 1, countZ + 1];
            
         }

         private void calculatePointsGround(double x, double y, double z, Vertex[,] arr)
         {     

             for (int i = 0; i < countX + 1; i++)
                 for (int j = 0; j < countZ + 1; j++)
                 {
                     Vertex v = new Vertex(x + i * dU, y + j * dV, z);
                     arr[i, j] = v;

                 }
         }




         public void createRect(List<Rect> list, int c1, int c2)
         {

             for (int i = 0; i < c1; i++)
                 for (int j = 0; j < c2; j++)
                     list.Add(new Rect(i, j, i + 1, j, i + 1, j + 1, i, j + 1));

         }



         // Моделирование дома
         public void modelingCity()
         {
             calculatePointsGround(0, 0, 0, arrayPointsGround);    
             createRect(rectangleGround, countX, countZ);
           
         }






    }
}
