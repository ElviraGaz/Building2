using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// для работы с библиотекой OpenGL
using Tao.OpenGl;
// для работы с библиотекой FreeGLUT
using Tao.FreeGlut;
// для работы с элементом управления SimpleOpenGLControl
using Tao.Platform.Windows;
using Tao.FreeType;
// для загрузки текстур и работы с ними
using Tao.DevIl;
using System.Drawing;



//Класс Жилой дом с окнами
namespace Building2
{
    class House
    {

        #region Field
        public List<Rect> rectangleTop;
        public List<Rect> rectangleLeft;
        public List<Rect> rectangleRight;
        public List<Rect> rectangleBack;
        int countX; //Число окон по длине  здания
        int countY; //Число окон по высоте здания
        int countZ; //Число окон по ширине(глубине) здания
        int MaxDepth;     //глубина   Z
        int MaxWidht;    // длина     X
        int MaxHeight;  //  высота    Y
        
        double step1, step2, step3;
        public Vertex[,] arrayPointsTop;
        public Vertex[,] arrayPointsLeft;
        public Vertex[,] arrayPointsRight;
        public Vertex[,] arrayPointsBack;
        double h = 0;
        double roofAng = 0.25;

        //ширина рамы
        float windowFrame = 0.2f;
        int foundHeight = 1;

        #endregion

        #region Properties
        public Color buildingColor { get; set; } //стены
        public Color windowColor { get; set; } //окна
        public Color foundColor { get; set; } //фундамент.
        public Color roofColor { get; set; } //крыша
        
        #endregion


        public House()
        {
           
            rectangleTop = new List<Rect>();
            rectangleLeft = new List<Rect>();
            rectangleRight = new List<Rect>();
            rectangleBack = new List<Rect>();

            arrayPointsTop = new Vertex[countX + 1, countY + 1];
            arrayPointsBack = new Vertex[countX + 1, countY + 1];
            arrayPointsLeft = new Vertex[countZ + 1, countY + 1];
            arrayPointsRight = new Vertex[countZ + 1, countY + 1];

        }


        //Конструктор дом без окон
        public House(int CX, int CY, int CZ, int MaxX, int MaxY, int MaxZ)
        {
            countX = CX;
            countY = CY;
            countZ = CZ;
            MaxWidht = MaxX;
            MaxHeight = MaxY;
            MaxDepth = MaxZ;
            rectangleTop = new List<Rect>();
            rectangleLeft = new List<Rect>();
            rectangleRight = new List<Rect>();
            rectangleBack = new List<Rect>();

            arrayPointsTop  = new Vertex[countX + 1, countY + 1];
            arrayPointsBack = new Vertex[countX + 1, countY + 1];
            arrayPointsLeft = new Vertex[countZ + 1, countY + 1];
            arrayPointsRight = new Vertex[countZ + 1, countY + 1];
            
        }



        //Конструктор дом с окнами
        public House(int CX, int CY, int CZ, int MaxX, int MaxY, int MaxZ,double _step1,double _step2,double _step3)
        {
            countX = CX;
            countY = CY;
            countZ = CZ;
            MaxWidht = MaxX;
            MaxHeight = MaxY;
            MaxDepth = MaxZ;

            rectangleTop = new List<Rect>();
            rectangleLeft = new List<Rect>();
            rectangleRight = new List<Rect>();
            rectangleBack = new List<Rect>();
            step1 = _step1;
            step2 = _step2;
            step3 = _step3;
           
            arrayPointsTop = new Vertex[countX + 1, countY + 1];
            arrayPointsBack = new Vertex[countX + 1, countY + 1];
            arrayPointsLeft = new Vertex[countZ + 1, countY + 1];
            arrayPointsRight = new Vertex[countZ + 1, countY + 1];
           
        }



        //Ввод данных
        public void InputDate(int CX,int CY,int CZ,int MaxX,int MaxY,int MaxZ,double _h,double _roofAng,float windowFrame,int foundHeight)
        {
            rectangleTop.Clear();
            rectangleLeft.Clear();
            rectangleRight.Clear();
            rectangleBack.Clear();

            countX = CX;
            countY = CY;
            countZ = CZ;         
            MaxWidht = MaxX;
            MaxHeight = MaxY;
            MaxDepth=MaxZ;
            h = _h/10;
            roofAng = _roofAng/100;
            arrayPointsTop = new Vertex[countX + 1, countY + 1];
            arrayPointsBack = new Vertex[countX + 1, countY + 1];
            arrayPointsLeft = new Vertex[countZ + 1, countY + 1];
            arrayPointsRight = new Vertex[countZ + 1, countY + 1];
            this.windowFrame = windowFrame;
            this.foundHeight = foundHeight;
        }


        public void InputDate(int CX, int CY, int CZ, int MaxX, int MaxY, int MaxZ,double step1,double step2,double step3,double _h,double _roofAng,float windowFrame,int foundHeight)
        {
            InputDate(CX, CY, CZ, MaxX, MaxY, MaxZ,_h,_roofAng,windowFrame,foundHeight);
            this.step1 = step1;
            this.step2 = step2;
            this.step3 = step3;

          
        }



        public void InputSteps(double step1,double step2,double step3)
        {
            this.step1 = step1;
            this.step2 = step2;
            this.step3 = step3;
        }


        #region Calculation
        //Лицевая и задняя сторона
        private void calculatePointsFrontal(double x,double y,double z, Vertex[,] arr)
        {
            
            double dU = 0;
            double dV = 0;

            dU = (((double)MaxWidht / 180) * Math.PI) / countX;
            dV = (((double)MaxHeight / 180) * Math.PI) / countY;
         
            for (int i = 0; i < countX + 1; i++)
                for (int j = 0; j < countY + 1; j++)
                {
                    Vertex v = new Vertex(x + i * dU, y + j * dV,z);
                    arr[i, j] = v;
                   
                }
        }


        //Боквые стороны
        private void calculatePointsCross(double x, double y, double z, Vertex[,] arr)
        {
            
            double dU = 0;
            double dV = 0;

            dU = (((double)MaxDepth / 180) * Math.PI) / countZ;
            dV = (((double)MaxHeight / 180) * Math.PI) / countY;
           
            for (int i = 0; i < countZ + 1; i++)
                for (int j = 0; j < countY + 1; j++)
                {
                    Vertex v = new Vertex(x, y + j * dV, z + i * dU);
                    arr[i, j] = v;

                }
        }



        public void createRect(List<Rect> list,int c1,int c2)
        {

            for (int i = 0; i < c1; i++)
                for (int j = 0; j < c2; j++)
                    list.Add(new Rect(i, j, i + 1, j, i + 1, j + 1, i, j + 1));

        }

        #endregion


        // Моделирование дома
        public void modelingHouse()
        {
            calculatePointsFrontal(0,0,Z(),arrayPointsTop);
            calculatePointsFrontal(0, 0, 0, arrayPointsBack);
            calculatePointsCross  (X(), 0,0,arrayPointsLeft);
            calculatePointsCross  (0, 0, 0, arrayPointsRight);
            createRect(rectangleTop,countX,countY);
            createRect(rectangleBack, countX, countY);
            createRect(rectangleLeft,countZ,countY);
            createRect(rectangleRight, countZ, countY);
        }


        public void InputColor() { 
        
        }


        #region DrawHouse


        public void DrawHouse1(Color buildingColor, Color windowColor, Color directColor, MaterialProperty buildingMaterial, MaterialProperty windowMaterial, bool flag)
        {

            Gl.glClear(Gl.GL_COLOR);
            Color PlaneColor = Color.FromArgb(90, 90, 90);
            setMaterial(PlaneColor, directColor, buildingMaterial);
            //DrawGround2();
            Gl.glPushMatrix();

            Gl.glClear(Gl.GL_COLOR);
            Color foundColor = Color.FromArgb(56, 56, 56);
            setMaterial(foundColor, directColor, buildingMaterial);
            //Фундамент


            DrawWall(Gl.GL_QUADS, X(), 1, Z());

            Gl.glTranslated(0, 1, 0);
            Gl.glClear(Gl.GL_COLOR);
            if (flag)
            {
                //Стены
                setMaterial(buildingColor, directColor, buildingMaterial);
                Gl.glPushMatrix();
                Gl.glTranslated(0.01, 0, 0.01);
                DrawWall(Gl.GL_QUADS, X() - 0.02, Y(), Z() - 0.02);
                Gl.glPopMatrix();
                Gl.glClear(Gl.GL_COLOR);
            }
            setMaterial(windowColor, directColor, windowMaterial);
            if (flag)
                DrawALLWindow(step1, step2, step3);


            // Gl.glTranslated(0, maxY, 0);
            Color roofColor = Color.FromArgb(56, 56, 56);
            setMaterial(roofColor, directColor, buildingMaterial);
            //Крыша

            Gl.glTranslated(-0.1, Y(), 0);
            DrawWall(Gl.GL_QUADS, X() + 0.2, 0.2, Z());
            Gl.glTranslated(-0.15, 0.2, 0);
            DrawWall(Gl.GL_QUADS, X() + 0.5, 0.2, Z());


            Gl.glPopMatrix();

        }



       #region DrawBulding
       public void StencilTest()
       {
           // разрешаем тест трафарета
           Gl.glEnable(Gl.GL_STENCIL_TEST);  
           Gl.glStencilFunc(Gl.GL_ALWAYS, 1, 1);
           Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE);
           Gl.glPushMatrix();
           Gl.glTranslated(0.02, 0, 0.02);
           DrawWall(Gl.GL_QUADS, X() - 0.04, Y(), Z() - 0.04);
           Gl.glPopMatrix();   
           Gl.glPushMatrix();
           DrawALLWindow(step1, step2, step3);
           Gl.glPopMatrix();
          
       }
        //Прорисовка здания
      public void DrawBuildins(Color buildingColor, Color windowColor, Color roofColor, Color foundColor, Color directColor, MaterialProperty buildingMaterial, MaterialProperty windowMaterial)
       {
           Gl.glEnable(Gl.GL_STENCIL_TEST);

           //Wall
           Gl.glPushMatrix();
          // Gl.glTranslated(0, foundHeight, 0);
           setMaterial(buildingColor, directColor, buildingMaterial);
           Gl.glStencilFunc(Gl.GL_EQUAL, 1, 255);
           Gl.glColor3d(1, 1, 1);
           
           DrawWall(Gl.GL_QUADS, X(), Y(), Z());
           Gl.glPopMatrix();

           Gl.glDisable(Gl.GL_STENCIL_TEST);

           //foundation 
           Gl.glColor3d(0, 1, 0);
           setMaterial(foundColor, directColor, buildingMaterial);
           Gl.glPushMatrix();
           Gl.glTranslated(-0.05, 0, -0.05);
           DrawWall(Gl.GL_QUADS, X() + 0.1, -foundHeight, Z() + 0.1);
           Gl.glPopMatrix();

           //window
           Gl.glPushMatrix();
           setMaterial(windowColor, directColor, buildingMaterial);
           DrawALLWindow2(step1, step2, step3, directColor, windowMaterial, windowColor);
           setMaterial(roofColor, directColor, buildingMaterial);
           Gl.glPopMatrix();

           //roof
           Gl.glPushMatrix();
           Gl.glTranslated(-0.1, Y(), -0.1);

           DrawWall(Gl.GL_QUADS, X() + 0.2, 0.2, Z() + 0.2);
           Gl.glTranslated(-0.15, 0.2, -0.15);
           DrawWall(Gl.GL_QUADS, X() + 0.5, 0.2, Z() + 0.5);
           Gl.glPopMatrix();
           if (h != 0)
               RoofTreangle();
           
          
          
          
        

       }



       #endregion

        //Нормаль методом Ньюэла
       public double[] getNormal(double[] A, double[] B, double[] C)
       {
           double a, b, c;
           a = (A[1] - B[1]) * (A[2] + B[2]) + (B[1] - C[1]) * (B[2] + C[2]) + (C[1] - A[1]) * (C[2] + A[2]);
           b = (A[2] - B[2]) * (A[0] + B[0]) + (B[2] - C[2]) * (B[0] + C[0]) + (C[2] - A[2]) * (C[0] + A[0]);
           c = (A[0] - B[0]) * (A[1] + B[1]) + (B[0] - C[0]) * (B[1] + C[1]) + (C[0] - A[0]) * (C[1] + A[1]);
           double[] n = new double[]{a, b, c};
          
           return n;
       }

       // Треугольная крыша
       private void RoofTreangle()
       {

           double[] T1 = new double[] { 0, 0.2, Z()};
           double[] T2 = new double[] { X(), 0.2, Z() };
           double[] T3 = new double[] { X(), 0.2, 0 };
           double[] T4 = new double[] { 0, 0.2, 0 };
           double[] T5 = new double[] { X() / 4, h+0.2, Z() / 2 };
           double[] T6 = new double[] { 3 * X() / 4, h+0.2, Z() / 2 };


           double[] n1 = new double[3];
           double[] n2 = new double[3];
           double[] n3 = new double[3];
           double[] n4 = new double[3];

           n1 = getNormal(T1,T4,T5);
           n2 = getNormal(T6,T2,T3);
           n3 = getNormal(T4,T5,T6);
           n4 = getNormal(T1,T5,T6);

           Gl.glTranslated(0, Y()+0.2, 0);
          
           //top
           
           Gl.glBegin(Gl.GL_POLYGON);
           Gl.glNormal3d(n4[0],n4[1],n4[2]);
           Gl.glVertex3d(0, 0.2, Z() );
           Gl.glVertex3d(X() , 0.2, Z() );
           Gl.glVertex3d(X()*(1-roofAng) , h+0.2, Z() / 2 );
           Gl.glVertex3d(X() *roofAng , h+0.2, Z() / 2 );
           Gl.glEnd();
           //left
           
           Gl.glBegin(Gl.GL_TRIANGLES);
           Gl.glNormal3d(n1[0], n1[1], n1[2]);
           Gl.glVertex3d(0 , 0.2, Z() );
           Gl.glVertex3d(0 , 0.2, 0 );
           Gl.glVertex3d(X() *roofAng , h+0.2, Z() / 2 );

           //right
           Gl.glNormal3d(n2[0], n2[1], n2[2]);
           Gl.glVertex3d(X(), 0.2, Z() );
           Gl.glVertex3d(X() , 0.2, 0 );
           Gl.glVertex3d(X() *(1-roofAng) , h+0.2, Z() / 2 );
           Gl.glEnd();
           //back
           Gl.glBegin(Gl.GL_POLYGON);
           Gl.glNormal3d(n3[0], n3[1], n3[2]);
           Gl.glVertex3d(0 , 0.2, 0);
           Gl.glVertex3d(X() , 0.2, 0 );
           Gl.glVertex3d(X()*(1-roofAng) , h+0.2, Z() / 2 );
           Gl.glVertex3d(X()*roofAng , h+0.2, Z() / 2 );
           Gl.glEnd();
       }




       public void DrawRoofAndFoundation(Color buildingColor, Color windowColor, Color directColor, MaterialProperty buildingMaterial, MaterialProperty windowMaterial, bool YESdrawWindow)
       {
           Gl.glPushMatrix();
           Gl.glClear(Gl.GL_COLOR);
           Color foundColor = Color.FromArgb(105, 105, 105);
           setMaterial(foundColor, directColor, buildingMaterial);
           //Фундамент
           DrawWall(Gl.GL_QUADS, X(), 1, Z());

           Gl.glTranslated(0, 1, 0);
           Gl.glClear(Gl.GL_COLOR);
           //Стены


           Gl.glClear(Gl.GL_COLOR);
           setMaterial(windowColor, directColor, windowMaterial);



           Color roofColor = Color.FromArgb(56, 56, 56);
           setMaterial(roofColor, directColor, buildingMaterial);
           //Крыша

           Gl.glTranslated(-0.1, Y(), 0);
           DrawWall(Gl.GL_QUADS, X() + 0.2, 0.2, Z());
           Gl.glTranslated(-0.15, 0.2, 0);
           DrawWall(Gl.GL_QUADS, X() + 0.5, 0.2, Z());


           Gl.glPopMatrix();

       }



        #endregion
  

        //Рисуем параллелипипед
        public void DrawWall(int i,double X, double Y, double Z)
        {

            Gl.glShadeModel(Gl.GL_SMOOTH);
            //Gl.glStencilFunc(Gl.GL_EQUAL, 2, 1);
            //Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE);
            //TOP
            Gl.glBegin(i);
            Gl.glNormal3f(0, 0, 1);

            Gl.glVertex3d(X, Y, Z);
            Gl.glVertex3d(0, Y, Z );
            Gl.glVertex3d(0, 0, Z );
            Gl.glVertex3d(X, 0, Z);
            Gl.glEnd();

           //RIGHT
            Gl.glBegin(i);
            Gl.glNormal3f(1, 0, 0);

            Gl.glVertex3d(X, Y, Z);
            Gl.glVertex3d(X, 0, Z);
            Gl.glVertex3d(X, 0, 0);
            Gl.glVertex3d(X, Y, 0);
            Gl.glEnd();

             //LEFT
            Gl.glBegin(i);
            Gl.glNormal3f(1, 0, 0);
            Gl.glVertex3d(0, 0, Z);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, Y, 0);
            Gl.glVertex3d(0, Y, Z);
            Gl.glEnd();


            Gl.glBegin(i);
            //BOTTOM
            Gl.glNormal3f(0, 1, 0);

            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, 0, Z);
            Gl.glVertex3d(X, 0, Z);
            Gl.glVertex3d(X, 0, 0);
            Gl.glEnd();

            
            Gl.glBegin(i);
            //UP
            Gl.glNormal3f(0, 1, 0);

            Gl.glVertex3d(0, Y, 0);
            Gl.glVertex3d(0, Y, Z);
            Gl.glVertex3d(X, Y, Z);
            Gl.glVertex3d(X, Y, 0);
            Gl.glEnd();
            ////
            //BACK
            Gl.glBegin(i);
            Gl.glNormal3f(0, 0, 1);

            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(X , 0, 0);
            Gl.glVertex3d(X , Y, 0);
            Gl.glVertex3d(0, Y, 0);
            Gl.glEnd();

        }


        //Задаем свойства материала 
       public void setMaterial(Color color, Color directColor, MaterialProperty material)
        {
           
            float[] ambient = { (float)(material.Ka * color.R / 255), (float)(material.Ka * color.G / 255), (float)(material.Ka * color.B / 255), material.transponency };   
            float[] diffuse = { (float)(material.Kd * color.R / 255), (float)(material.Kd * color.G / 255), (float)(material.Kd * color.B / 255), material.transponency };
            float[] specularBack = { (float)(material.Ks * directColor.R / 255), (float)(material.Ks * directColor.G / 255), (float)(material.Ks * directColor.B / 255), material.transponency };
            float[] specularFront = { (float)(material.Ks * directColor.R / 255), (float)(material.Ks * directColor.G / 255), (float)(material.Ks * directColor.B / 255), material.transponency };

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, ambient);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, diffuse);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, specularBack);

            Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, material.n);
            //Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, 1);
          
        }

      #region DrawWindow

       //Нарисовать все окна
       public void DrawALLWindow(double stepTop1, double stepTop2, double stepLeft)
       {

           DrawWindows(rectangleBack, arrayPointsBack, stepTop1, stepTop2, true);
           DrawWindows(rectangleTop, arrayPointsTop, stepTop1, stepTop2, true);

           DrawWindows(rectangleLeft, arrayPointsLeft, stepLeft, stepTop2, false);
           DrawWindows(rectangleRight, arrayPointsRight, stepLeft, stepTop2, false);

       }



       public void DrawWindows(List<Rect> list, Vertex[,] arr, double step1, double step2, bool flag)
       {
           foreach (Rect p in list)
               DrawWindow(arr, p, step1, step2, flag);
       }




//============Нарисовать окно!!!==========================================================
      public void DrawWindow(Vertex[,] arr, Rect p, double step1, double step2, bool flag)
      {
          double x1, x2, x3, x4, z1, z2, z3, z4;
          if (flag)
          {
              Gl.glNormal3f(0, 0, 1);
              step1 = ((((double)MaxWidht / 180) * Math.PI) / countX) * step1;
              step1 /= 2;
          }
          else
          {
              Gl.glNormal3f(1, 0, 0);
              step1 = ((((double)MaxDepth / 180) * Math.PI) / countZ) * step1;
              step1 /= 2;
          }
          step2 = ((((double)MaxHeight / 180) * Math.PI) / countY) * step2;
          step2 /= 2;
          if (flag)
          {
              x1 = arr[p.i1, p.j1].X + step1;
              x2 = arr[p.i2, p.j2].X - step1;
              x3 = arr[p.i3, p.j3].X - step1;
              x4 = arr[p.i4, p.j4].X + step1;

              z1 = arr[p.i1, p.j1].Z;
              z2 = arr[p.i2, p.j2].Z;
              z3 = arr[p.i3, p.j3].Z;
              z4 = arr[p.i4, p.j4].Z;
          }
          else
          {
              x1 = arr[p.i1, p.j1].X;
              x2 = arr[p.i2, p.j2].X;
              x3 = arr[p.i3, p.j3].X;
              x4 = arr[p.i4, p.j4].X;


              z1 = arr[p.i1, p.j1].Z + step1;
              z2 = arr[p.i2, p.j2].Z - step1;
              z3 = arr[p.i3, p.j3].Z - step1;
              z4 = arr[p.i4, p.j4].Z + step1;
          }

          double y1 = arr[p.i1, p.j1].Y + step2;
          double y2 = arr[p.i2, p.j2].Y + step2;
          double y3 = arr[p.i3, p.j3].Y - step2;
          double y4 = arr[p.i4, p.j4].Y - step2;


          //Тарфарет
          Gl.glShadeModel(Gl.GL_SMOOTH);
          Gl.glStencilFunc(Gl.GL_ALWAYS, 2, 1);
          Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE);

          Gl.glBegin(Gl.GL_QUADS);
          Gl.glVertex3f((float)x1, (float)y1, (float)z1);
          Gl.glVertex3f((float)x2, (float)y2, (float)z2);
          Gl.glVertex3f((float)x3, (float)y3, (float)z3);
          Gl.glVertex3f((float)x4, (float)y4, (float)z4);
          Gl.glEnd();

      }

     

     //WINDOW IN
      public void DrawALLWindow2(double stepTop1, double stepTop2, double stepLeft,Color direct,MaterialProperty mat,Color win)
      {

          DrawWindows2(rectangleTop, arrayPointsTop, stepTop1, stepTop2, true,0,0,-windowFrame,direct,mat,win);
          DrawWindows2(rectangleBack, arrayPointsBack, stepTop1, stepTop2, true,0,0,windowFrame,direct,mat,win);
          DrawWindows2(rectangleLeft, arrayPointsLeft, stepLeft, stepTop2, false,-windowFrame,0,0,direct,mat,win);
          DrawWindows2(rectangleRight, arrayPointsRight, stepLeft, stepTop2, false,windowFrame,0,0,direct,mat,win);
      }



      public void DrawWindows2(List<Rect> list, Vertex[,] arr, double step1, double step2, bool flag, float dx, float dy, float dz, Color direct, MaterialProperty mat,Color win)
      {
          foreach (Rect p in list)
              DrawWindow2(arr, p, step1, step2, flag,dx,dy,dz,direct,mat,win);
      }
      public void DrawWindow2(Vertex[,] arr, Rect p, double step1, double step2, bool flag, float dx, float dy, float dz, Color direct, MaterialProperty mat,Color win)
      {
          double x1, x2, x3, x4, z1, z2, z3, z4;
          if (flag)
          {
              Gl.glNormal3f(0, 0, 1);
              step1 = ((((double)MaxWidht / 180) * Math.PI) / countX) * step1;
              step1 /= 2;
          }
          else
          {
              Gl.glNormal3f(1, 0, 0);
              step1 = ((((double)MaxDepth / 180) * Math.PI) / countZ) * step1;
              step1 /= 2;
          }
          step2 = ((((double)MaxHeight / 180) * Math.PI) / countY) * step2;
          step2 /= 2;
          if (flag)
          {
              x1 = arr[p.i1, p.j1].X + step1;
              x2 = arr[p.i2, p.j2].X - step1;
              x3 = arr[p.i3, p.j3].X - step1;
              x4 = arr[p.i4, p.j4].X + step1;

              z1 = arr[p.i1, p.j1].Z;
              z2 = arr[p.i2, p.j2].Z;
              z3 = arr[p.i3, p.j3].Z;
              z4 = arr[p.i4, p.j4].Z;
          }
          else
          {
              x1 = arr[p.i1, p.j1].X;
              x2 = arr[p.i2, p.j2].X;
              x3 = arr[p.i3, p.j3].X;
              x4 = arr[p.i4, p.j4].X;


              z1 = arr[p.i1, p.j1].Z + step1;
              z2 = arr[p.i2, p.j2].Z - step1;
              z3 = arr[p.i3, p.j3].Z - step1;
              z4 = arr[p.i4, p.j4].Z + step1;
          }

          double y1 = arr[p.i1, p.j1].Y + step2;
          double y2 = arr[p.i2, p.j2].Y + step2;
          double y3 = arr[p.i3, p.j3].Y - step2;
          double y4 = arr[p.i4, p.j4].Y - step2;


          DrawRama(x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4, dx, dy, dz,direct,mat,win);
      }


      private void DrawRama(double x1, double x2, double x3, double x4, double y1, double y2, double y3, double y4, double z1, double z2, double z3, double z4, float dx, float dy, float dz, Color direct, MaterialProperty mat,Color win) 
      {
          Gl.glBegin(Gl.GL_QUADS);
          //UP
          setMaterial(win, direct, mat);
          Gl.glVertex3f((float)x1+dx, (float)y1, (float)z1 +dz);
          Gl.glVertex3f((float)x2+dx, (float)y2, (float)z2 +dz);
          Gl.glVertex3f((float)x3+dx, (float)y3, (float)z3 +dz);
          Gl.glVertex3f((float)x4+dx, (float)y4, (float)z4 +dz);
          Color roofColor = Color.FromArgb(37, 30, 35);
          setMaterial(roofColor, direct, mat);
          Gl.glVertex3f((float)x1, (float)y1, (float)z1);
          Gl.glVertex3f((float)x1+dx, (float)y1, (float)z1 +dz);
          Gl.glVertex3f((float)x2+dx, (float)y2, (float)z2 +dz);
          Gl.glVertex3f((float)x2, (float)y2, (float)z2);
          //
          Gl.glVertex3f((float)x1, (float)y3, (float)z1);
          Gl.glVertex3f((float)x1 + dx, (float)y3, (float)z1 + dz);
          Gl.glVertex3f((float)x2 + dx, (float)y3, (float)z2 + dz);
          Gl.glVertex3f((float)x2, (float)y3, (float)z2);
         

          Gl.glVertex3f((float)x1, (float)y1, (float)z1);
          Gl.glVertex3f((float)x1, (float)y4, (float)z1);
          Gl.glVertex3f((float)x1+dx, (float)y4, (float)z1 +dz);
          Gl.glVertex3f((float)x1+dx, (float)y1, (float)z1 +dz);
          //
          Gl.glVertex3f((float)x2, (float)y2, (float)z2);
          Gl.glVertex3f((float)x2, (float)y4, (float)z2);
          Gl.glVertex3f((float)x2 + dx, (float)y4, (float)z2 + dz);
          Gl.glVertex3f((float)x2 + dx, (float)y1, (float)z2 + dz);
          //
          Gl.glEnd();
         
      }
       #endregion
    
      #region Texture


      //Текстура
      


     
        //Рисуем с текстурой

       public void drawWithTextureMY(uint mGlTextureObject, Vertex[,] arr,List<Rect> list,Side S)
       {
         
            // включаем режим текстурирования
           Gl.glEnable(Gl.GL_TEXTURE_2D);
            // включаем режим текстурирования , указывая индификатор mGlTextureObject
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);
           
            //Gl.glLightModelf(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);
           // Gl.glShadeModel(Gl.GL_SMOOTH);
            if (S == Side.TOP)
                Gl.glNormal3f(0, 0, 1);
            if (S == Side.BACK)
                Gl.glNormal3f(0, 0, 1);
            if (S == Side.LEFT)
                Gl.glNormal3f(-1, 0, 0);
            if (S == Side.RIGHT)
                Gl.glNormal3f(-1, 0, 0);
            foreach (Rect p in list)
            {
           
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, 0);
                Gl.glVertex3f((float)arr[p.i1, p.j1].X, (float)arr[p.i1, p.j1].Y, (float)arr[p.i1, p.j1].Z);

                Gl.glTexCoord2d(1,0);
                Gl.glVertex3f((float)arr[p.i2, p.j2].X, (float)arr[p.i2, p.j2].Y, (float)arr[p.i2, p.j2].Z);

                Gl.glTexCoord2d(1,1);
                Gl.glVertex3f((float)arr[p.i3, p.j3].X, (float)arr[p.i3, p.j3].Y, (float)arr[p.i3, p.j3].Z);

                Gl.glTexCoord2d(0,1);
                Gl.glVertex3f((float)arr[p.i4, p.j4].X, (float)arr[p.i4, p.j4].Y, (float)arr[p.i4, p.j4].Z);

                Gl.glEnd();
            }
                 // отключаем режим текстурирования
            Gl.glDisable(Gl.GL_TEXTURE_2D);
       }




       public void DrawALLWindowWithTexture(uint mGlTextureObject, double stepTop1, double stepTop2, double stepLeft)
       {
           DrawWindowsWithTexture(mGlTextureObject,rectangleBack, arrayPointsBack, stepTop1, stepTop2, true);
           DrawWindowsWithTexture(mGlTextureObject, rectangleTop, arrayPointsTop, stepTop1, stepTop2, true);

           DrawWindowsWithTexture(mGlTextureObject, rectangleLeft, arrayPointsLeft, stepLeft, stepTop2, false);
           DrawWindowsWithTexture(mGlTextureObject, rectangleRight, arrayPointsRight, stepLeft, stepTop2, false);

       }

       public void DrawWindowsWithTexture(uint mGlTextureObject,List<Rect> list, Vertex[,] arr, double step1, double step2, bool flag)
       {
           foreach (Rect p in list)
               drawWindowWithTexture(mGlTextureObject,arr, p, step1, step2, flag);
       }


       public void drawWindowWithTexture(uint mGlTextureObject, Vertex[,] arr, Rect p,double step1,double step2,bool flag)
       {

           // включаем режим текстурирования
           Gl.glEnable(Gl.GL_TEXTURE_2D);
           // включаем режим текстурирования , указывая индификатор mGlTextureObject
           Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);

         
           
        
            double x1, x2, x3, x4, z1, z2, z3, z4;
            if (flag)
            {
                Gl.glNormal3f(0, 0, 1);
                step1 = ((((double)MaxWidht / 180) * Math.PI) / countX) * step1;
            }
            else
            {
                Gl.glNormal3f(1, 0, 0);
                step1 = ((((double)MaxDepth / 180) * Math.PI) / countZ) * step1;
            }
                step2 = ((((double)MaxHeight / 180) * Math.PI) /countY)*step2;
                if (flag)
                {
                    x1 = arr[p.i1, p.j1].X + step1;
                    x2 = arr[p.i2, p.j2].X - step1;
                    x3 = arr[p.i3, p.j3].X - step1;
                    x4 = arr[p.i4, p.j4].X + step1;

                    z1 = arr[p.i1, p.j1].Z;
                    z2 = arr[p.i2, p.j2].Z;
                    z3 = arr[p.i3, p.j3].Z;
                    z4 = arr[p.i4, p.j4].Z;
                }
                else
                {
                    x1 = arr[p.i1, p.j1].X;
                    x2 = arr[p.i2, p.j2].X;
                    x3 = arr[p.i3, p.j3].X;
                    x4 = arr[p.i4, p.j4].X;


                    z1 = arr[p.i1, p.j1].Z + step1;
                    z2 = arr[p.i2, p.j2].Z - step1;
                    z3 = arr[p.i3, p.j3].Z - step1;
                    z4 = arr[p.i4, p.j4].Z + step1;
                }

                double y1 = arr[p.i1, p.j1].Y + step2;
                double y2 = arr[p.i2, p.j2].Y + step2;
                double y3 = arr[p.i3, p.j3].Y - step2;
                double y4 = arr[p.i4, p.j4].Y - step2;      

                Gl.glShadeModel(Gl.GL_SMOOTH);
               
          

               Gl.glBegin(Gl.GL_QUADS);
               Gl.glTexCoord2d(0, 0);
               Gl.glVertex3f((float)x1, (float)y1, (float)z1);
               

               Gl.glTexCoord2d(1, 0);
            Gl.glVertex3f((float)x2, (float)y2, (float)z2);
               

               Gl.glTexCoord2d(1, 1);
                Gl.glVertex3f((float)x3, (float)y3, (float)z3);

               Gl.glTexCoord2d(0, 1);
               Gl.glVertex3f((float)x4, (float)y4, (float)z4);

               Gl.glEnd();
           
           // отключаем режим текстурирования
           Gl.glDisable(Gl.GL_TEXTURE_2D);
       }
       
      


       /* Находит координаты на текстуре, соответсвующие вершинам полигона
        * */
        public void findTextureCoord()
        {
            float maxU = MaxWidht;
            float maxV = MaxHeight;
            int coutU = countX;
            int coutV = countY;
            float deltaU = maxU / (coutU );
            float deltaV = maxV / (coutV);
            float curU = 0;
            float curV = 0;

            for (int u = 0; u < coutU+1; u++)
            {
                for (int v = 0; v < coutV+1; v++)
                {
                    arrayPointsTop[u, v].textureX = curU / maxU; // Xi = Ui / (maxU - minU)
                    arrayPointsTop[u, v].textureY = curV / maxV; // Yi = Vi / (maxV - minV)
                    curV += deltaV;
                }
                curV = 0;
                curU += deltaU;
            }
        }



        public void findTextureCoordMY()
        {
            float maxU = MaxWidht;
            float maxV = MaxHeight;
            int coutU = countX;
            int coutV = countY;
            float deltaU = maxU / (coutU);
            float deltaV = maxV / (coutV);
            float curU = 0;
            float curV = 0;

            for (int u = 0; u < coutU + 1; u++)
            {
                for (int v = 0; v < coutV + 1; v++)
                {
                    arrayPointsTop[u, v].textureX = curU ; // Xi = Ui / (maxU - minU)
                    arrayPointsTop[u, v].textureY = curV ; // Yi = Vi / (maxV - minV)
                    curV += deltaV;
                }
                curV = 0;
                curU += deltaU;
            }
        }
       #endregion

      #region Getter
        //Параметры ШИРИНА ВЫСОТА ДЛИНА дома
        public double X()
        {

            double dU = 0;
            dU = (((double)MaxWidht / 180) * Math.PI) / countX;
            double x = dU * (countX);
            return x;
        }

        public double Y()
        {

            double dU = 0;
            dU = (((double)MaxHeight / 180) * Math.PI) / countY;
            double y = dU * (countY);
            return y;
        }


        public double Z()
        {

            double dU = 0;
            dU = (((double)MaxDepth / 180) * Math.PI) / countZ;
            double z = dU * (countZ);
            return z;
        }


        public void setWindowFrame(float value)
        {
            windowFrame = value;
        }

        #endregion




    }
}
