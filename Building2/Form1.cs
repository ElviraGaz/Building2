using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
    public partial class Form1 : Form
    {
        //--ПОЛЯ----
        #region Fields
        double zoom = 0.55;
        double X, Y, Z;
        int AngleX = 0;
        int AngleY = -45;
        int AngleZ = 0;

      
        double stepWindowTop1, stepWindowTop2, stepWindowLeft1;
        public int imageId = 0; // индефекатор текстуры
       public uint mGlTextureObject = 0;

       
        private bool textureIsLoad = false; // флаг - загружена ли текстура  
      //----------
        House house = new House(1,1,1,1,1,1);
        Color buildingColor, windowColor, roofColor,foubdationColor; // цвет здания и цвет окна и цвет крыши и цвет фундамента
        Color directColor, ambientColor; // Цвет точечного и рассеянного источника света


        MaterialProperty buildingMaterial;
        MaterialProperty windowMaterial;
        //Параметры освещения
        float ka, kd, ks; // коэффициенты фонового, диффузного и отраженного освещеий
        int n; // степень сфокусированности бликов
        float transparency; // степень прозрачночти
        double maxX, maxY, maxZ;
        //текстура

        const int GLASS_TEXTURE=0;
        const int COM4_TEXTURE=1;
        const int COM6_TEXTURE=2;
        const int COM9_TEXTURE = 5;
        const int NUM_TEXTURES=5;
        uint[]  textureObjects=new uint[NUM_TEXTURES];
        public uint[] mGlTextureObject1 = new uint[NUM_TEXTURES];
        string[] TextureUri = { "C:/t1.jpg", "C:/t2.jpg", "C:/grass.jpg","C:/wind.jpg","C:/t11.jpg" };
        
        
       

            House house1;
            House house2;
            House house3;
            House house4;
            House house5;
            House house6;
            House house7;
            House house8;
            House house9;
            House house10;
            House house11;
            House house12;


            House house13;
            House house14;
           

       // BuildingGenerator buildingGenerator;
        bool clicked; // Флаг нажатия на кнопку мыши (нажата = true, иначе false)
        #endregion

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
            initializeOpenGL();
            
            stepWindowTop1  = (double) trackBarStepWin1.Value/100;
            stepWindowTop2  = (double)trackBarStepWinTop2.Value/100;
            stepWindowLeft1 = (double) trackBar2.Value/100;

            buildingColor = new Color();
            buildingColor = Color.FromArgb(60, 40, 12);
            windowColor = new Color();
            windowColor = Color.FromArgb(25, 25, 25);
            roofColor = new Color();
            roofColor = Color.FromArgb(56, 56, 56);
            foubdationColor = new Color();
            foubdationColor = Color.FromArgb(57, 57, 57);

            windowMaterial   = new MaterialProperty(windowColor, 1, 1, 1, 58,1);
            buildingMaterial = new MaterialProperty(buildingColor, 0.46, 0.01, 0.23, 128,1);
            maxX = trackBarMaxX.Value * Math.PI / 180;
            maxY = trackBarMaxY.Value * Math.PI / 180;
            maxZ = trackBarMaxZ.Value * Math.PI / 180;

            house.InputDate(trackBarCountX.Value, trackBarCountY.Value, trackBarCountZ.Value, trackBarMaxX.Value, trackBarMaxY.Value, trackBarMaxZ.Value,trackBarH.Value,trackBarRoof.Value,0.2f,1);
            house.InputSteps(stepWindowTop1,stepWindowTop2,stepWindowLeft1);
            house.modelingHouse();
            SetColors();
            X = 0; Y = -3; Z = -20;
            TEXTURE();
            #region LabelInit
            labelh.Text = trackBarMaxY.Value.ToString();
            labell.Text = trackBarMaxX.Value.ToString();
            labelsh.Text =trackBarMaxZ.Value.ToString() ;

            labeldl.Text = trackBarCountX.Value.ToString();
            labelet.Text = trackBarCountY.Value.ToString();
            labelshr.Text = trackBarCountZ.Value.ToString();

            labelr1.Text = trackBarRT.Value.ToString();
            labelg1.Text = trackBarGT.Value.ToString();
            labelb1.Text = trackBarBT.Value.ToString();

            labelr2.Text = trackBarRR.Value.ToString();
            labelg2.Text = trackBarGR.Value.ToString();
            labelb2.Text = trackBarBR.Value.ToString();

            labelKa.Text = "1";
            labelKd.Text = "1";
            labelKs.Text = "1";
            labeln.Text = "1";
            labelt.Text = "1";


            labelr3.Text = trackBarRb.Value.ToString();
            labelg3.Text = trackBarGb.Value.ToString();
            labelb3.Text = trackBarBb.Value.ToString();

            labelr4.Text = trackBarRw.Value.ToString();
            labelg4.Text = trackBarGw.Value.ToString();
            labelb4.Text = trackBarBw.Value.ToString();


            labelXL.Text = trackBarXLight.Value.ToString();
            labelYL.Text = trackBarYLight.Value.ToString();
            labelZL.Text = trackBarZLight.Value.ToString();
            
            labelX.Text = X.ToString();
            labelY.Text = Y.ToString();
            labelZ.Text = Z.ToString();
            labelZoom.Text = zoom.ToString();
            labelQ.Text = stepWindowTop1.ToString();
            labelE.Text = stepWindowLeft1.ToString();
            labelW.Text = stepWindowTop2.ToString();
#endregion
        }
    
//==================ИНИЦИАЛИЗАЦИЯ OPENGL================//
        private void initializeOpenGL()
        {
            // инициализация бибилиотеки glut 
            Glut.glutInit();
            // инициализация режима экрана 
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            // инициализация библиотеки openIL
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);

            // установка цвета очистки экрана (RGBA) 
            Gl.glClearColor(1f, 1f, 1f, 0);

            // установка порта вывода 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            // активация проекционной матрицы 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очистка матрицы 
            Gl.glLoadIdentity();

            // установка перспективы 
            Glu.gluPerspective(45, (float)AnT.Width / (float)AnT.Height, 0.1, 2000);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glShadeModel(Gl.GL_SMOOTH);
            // начальная настройка параметров openGL (освещение и первый источник света)             
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
           
            // Включаем прозрачность
            Gl.glEnable(Gl.GL_BLEND);
            // Функция смешивающая для каждого x,y значения буфера
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
         
            Gl.glEnable(Gl.GL_DEPTH_TEST);	// Hidden surface removal
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL);
       
        }

        
        //========================ОТРИСОВКА СЦЕНЫ==================================

        private void AnT_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT|Gl.GL_STENCIL_BUFFER_BIT);
            // и масштабирование объекта           
            // очищение текущей матрицы 
            Gl.glLoadIdentity();
            // производим перемещение, в зависимости от значений, полученных при перемещении ползунков 
            Gl.glTranslated(X, Y, Z);
            Gl.glScaled(zoom, zoom, zoom);
          //  Gl.glPushMatrix();


          //  Gl.glTranslated(trackBarXLight.Value, 0, 0);
          //  Gl.glTranslated(0, trackBarYLight.Value, 0);
          //  Gl.glTranslated(0, 0, trackBarZLight.Value);
          //  Gl.glColor3ub((byte)trackBarRT.Value, (byte)trackBarGT.Value, (byte)trackBarBT.Value);
          //  Glut.glutSolidSphere(1, 32, 32);
          //  Gl.glClear(Gl.GL_COLOR);
          //  Gl.glPopMatrix();       

            Gl.glPushMatrix();
            Gl.glTranslated(trackBarXLight.Value, 0, 0);
            Gl.glTranslated(0, trackBarYLight.Value, 0);
            Gl.glTranslated(0, 0, trackBarZLight.Value);
            Gl.glColor3ub((byte)trackBarRT.Value, (byte)trackBarGT.Value, (byte)trackBarBT.Value);
           // Glut.glutSolidSphere(1, 32, 32);
            Gl.glClear(Gl.GL_COLOR);
             
            Gl.glPopMatrix();
            // Задаем свойтсва источника света 
            setLightSource();


            // Включаем нормализацию векторов
            Gl.glEnable(Gl.GL_NORMALIZE);

            //Повороты
            Gl.glRotated(AngleX, 1, 0, 0);
            Gl.glRotated(AngleY, 0, 1, 0);
            Gl.glRotated(AngleZ, 0, 0, 1);

          
            //Моделирование здания
            if ( radioButton3.Checked)
            {
                
                //Моделирование здания(Текстура)
               if (textureIsLoad && checkBox1.Checked)
                {
                   Gl.glPushMatrix();
                   Gl.glTranslated(0, 1, 0);
                   Gl.glTranslated(0.01, 0, 0.01);
                   house.drawWithTextureMY(mGlTextureObject, house.arrayPointsBack, house.rectangleBack, Side.BACK);
                   Gl.glTranslated(0, 0, -0.02);
                   house.drawWithTextureMY(mGlTextureObject, house.arrayPointsTop, house.rectangleTop, Side.TOP);
                   house.drawWithTextureMY(mGlTextureObject, house.arrayPointsRight, house.rectangleRight, Side.RIGHT);
                   Gl.glTranslated(-0.02, 0, 0);
                   house.drawWithTextureMY(mGlTextureObject, house.arrayPointsLeft, house.rectangleLeft, Side.LEFT);
                   Gl.glPopMatrix();
                   Gl.glPushMatrix();
                   Gl.glTranslated(0, 1, 0);
                   house.DrawALLWindowWithTexture(mGlTextureObject1[3], stepWindowTop1, stepWindowTop2, stepWindowLeft1);
                   Gl.glPopMatrix();
                   house.DrawHouse1(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false); 
                   Gl.glPopMatrix();
                   Gl.glDisable(Gl.GL_LIGHTING);
                   Gl.glClear(Gl.GL_COLOR);               
                   DrawGround();                  
                }


                //Моделирование здания(закраска)
                else if (!checkBox1.Checked)
                {

                  Gl.glPushMatrix();
                  house.StencilTest();
                  Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
                  house.DrawBuildins(buildingColor, windowColor,roofColor,foubdationColor, directColor, buildingMaterial, windowMaterial);
                  Gl.glDisable(Gl.GL_LIGHTING);
                  Gl.glClear(Gl.GL_COLOR);
                  Gl.glPopMatrix();
                  Gl.glTranslated(0, -1, 0);
                  DrawGround();
                 
                }

            }

                //Студ Городок
            else if(radioButtonStudCity.Checked)
            {             
                DrawPlane(); 
                DrawStudCity();   
            }

              //Городок
            else if(radioButton1.Checked)
            {
                

                //====== Непосредственное стирание

               // Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);              
                InitStreet(out house1, out house2, out house3, out house4, out house5, out house6,out house7,out house8,out house9,out house10,out house11,out house12,out house13,out house14);                           
                DrawStreet();
                Gl.glPushMatrix();
                Gl.glTranslated(0,-4,0);
                DrawPlane1();
                Gl.glPopMatrix();
            }

            // завершаем рисование 
            Gl.glFlush();
            // обновлем элемент openGlCtrl
            AnT.Invalidate();

        }

      
        //Рисуем студ городок
        #region StudCity()


        private void DrawStudCity()
        {

            House Com1 = new House(22, 5, 3, 2200, 500, 500);
            House Com2 = new House(22, 5, 3, 2200, 500, 500);
            House Com3 = new House(22, 5, 3, 2200, 500, 500);
            House Com4 = new House(22, 5, 3, 2200, 500, 500);


            House Com5 = new House(22, 5, 3, 2200, 500, 700);
            House Com6 = new House(10, 12, 5, 900, 1200, 700);
            House Com7 = new House(10, 12, 5, 900, 1200, 700);
            House Com8 = new House(10, 12, 5, 900, 1200, 700);


            House Com9 = new House(10, 12, 5, 1200, 1000, 500);

            Com1.modelingHouse();
            Com2.modelingHouse();
            Com3.modelingHouse();
            Com4.modelingHouse();
            Com5.modelingHouse();
            Com6.modelingHouse();
            Com7.modelingHouse();
            Com8.modelingHouse();
            Com9.modelingHouse();
            Gl.glPushMatrix();
            Gl.glTranslated(-18, 0, 10);
            
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject1[0]);
        
            Com1.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com1.drawWithTextureMY(mGlTextureObject1[0], Com1.arrayPointsTop, Com1.rectangleTop, Side.TOP);
            Com1.drawWithTextureMY(mGlTextureObject1[0], Com1.arrayPointsLeft, Com1.rectangleLeft, Side.LEFT);
            Com1.drawWithTextureMY(mGlTextureObject1[0], Com1.arrayPointsRight, Com1.rectangleRight, Side.RIGHT);
            Com1.drawWithTextureMY(mGlTextureObject1[0], Com1.arrayPointsBack, Com1.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glTranslated(0, 0, -25);

            Com2.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);

            Gl.glPushMatrix(); 
            Gl.glTranslated(0, 1, 0);
            Com2.drawWithTextureMY(mGlTextureObject1[0], Com2.arrayPointsTop, Com2.rectangleTop, Side.TOP);
            Com2.drawWithTextureMY(mGlTextureObject1[0], Com2.arrayPointsLeft, Com2.rectangleLeft, Side.LEFT);
            Com2.drawWithTextureMY(mGlTextureObject1[0], Com2.arrayPointsRight, Com2.rectangleRight, Side.RIGHT);
            Com2.drawWithTextureMY(mGlTextureObject1[0], Com2.arrayPointsBack, Com2.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glTranslated(0, 0, -25);

            Com3.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com3.drawWithTextureMY(mGlTextureObject1[0], Com3.arrayPointsTop, Com3.rectangleTop, Side.TOP);
            Com3.drawWithTextureMY(mGlTextureObject1[0], Com3.arrayPointsLeft, Com3.rectangleLeft, Side.LEFT);
            Com3.drawWithTextureMY(mGlTextureObject1[0], Com3.arrayPointsRight, Com3.rectangleRight, Side.RIGHT);
            Com3.drawWithTextureMY(mGlTextureObject1[0], Com3.arrayPointsBack, Com3.rectangleBack, Side.BACK);

            Gl.glPopMatrix();
            Gl.glTranslated(0, 0, -25);
            
            Com4.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com4.drawWithTextureMY(mGlTextureObject1[0], Com4.arrayPointsTop, Com4.rectangleTop, Side.TOP);
            Com4.drawWithTextureMY(mGlTextureObject1[0], Com4.arrayPointsLeft, Com4.rectangleLeft, Side.LEFT);
            Com4.drawWithTextureMY(mGlTextureObject1[0], Com4.arrayPointsRight, Com4.rectangleRight, Side.RIGHT);
            Com4.drawWithTextureMY(mGlTextureObject1[0], Com4.arrayPointsBack, Com4.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glTranslated(50, 0, 70);
           
            Com5.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com5.drawWithTextureMY(mGlTextureObject1[0], Com5.arrayPointsTop, Com5.rectangleTop, Side.TOP);
            Com5.drawWithTextureMY(mGlTextureObject1[0], Com5.arrayPointsLeft, Com5.rectangleLeft, Side.LEFT);
            Com5.drawWithTextureMY(mGlTextureObject1[0], Com5.arrayPointsRight, Com5.rectangleRight, Side.RIGHT);
            Com5.drawWithTextureMY(mGlTextureObject1[0], Com5.arrayPointsBack, Com5.rectangleBack, Side.BACK);

            Gl.glPopMatrix();
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject1[1]);
            Gl.glTranslated(30, 0, -14);
            
            Com6.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
           // Com6.DrawHouse(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Com6.drawWithTextureMY(mGlTextureObject1[1], Com6.arrayPointsTop, Com6.rectangleTop, Side.TOP);
            Com6.drawWithTextureMY(mGlTextureObject1[1], Com6.arrayPointsLeft, Com6.rectangleLeft, Side.LEFT);
            Com6.drawWithTextureMY(mGlTextureObject1[1], Com6.arrayPointsRight, Com6.rectangleRight, Side.RIGHT);
            Com6.drawWithTextureMY(mGlTextureObject1[1], Com6.arrayPointsBack, Com6.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glTranslated(-10, 1, -35);

            Com7.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com7.drawWithTextureMY(mGlTextureObject1[1], Com7.arrayPointsTop, Com7.rectangleTop, Side.TOP);
            Com7.drawWithTextureMY(mGlTextureObject1[1], Com7.arrayPointsLeft, Com7.rectangleLeft, Side.LEFT);
            Com7.drawWithTextureMY(mGlTextureObject1[1], Com7.arrayPointsRight, Com7.rectangleRight, Side.RIGHT);
            Com7.drawWithTextureMY(mGlTextureObject1[1], Com7.arrayPointsBack, Com7.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glTranslated(-10, 0, -35);

            Com8.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com8.drawWithTextureMY(mGlTextureObject1[1], Com8.arrayPointsTop, Com8.rectangleTop, Side.TOP);
            Com8.drawWithTextureMY(mGlTextureObject1[1], Com8.arrayPointsLeft, Com8.rectangleLeft, Side.LEFT);
            Com8.drawWithTextureMY(mGlTextureObject1[1], Com8.arrayPointsRight, Com8.rectangleRight, Side.RIGHT);
            Com8.drawWithTextureMY(mGlTextureObject1[1], Com8.arrayPointsBack, Com8.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glPopMatrix();
            
            //******
            Gl.glTranslated(100, 0, -30);
            Com9.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsTop, Com9.rectangleTop, Side.TOP);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsLeft, Com9.rectangleLeft, Side.LEFT);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsRight, Com9.rectangleRight, Side.RIGHT);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsBack, Com9.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glPopMatrix();
            Com9.InputDate(7, 10, 15, 400, 1000, 1700,0,0,0,-1);
            Com9.modelingHouse();
            Gl.glTranslated(5, 0, -21);
            Com9.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsTop, Com9.rectangleTop, Side.TOP);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsLeft, Com9.rectangleLeft, Side.LEFT);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsRight, Com9.rectangleRight, Side.RIGHT);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsBack, Com9.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glPopMatrix();



            Com9.InputDate(7, 10, 5, 1100, 1000, 500,0,0,0,-1);
            Com9.modelingHouse();
            Gl.glTranslated(-10, 0, -5);
            Com9.DrawRoofAndFoundation(buildingColor, windowColor, directColor, buildingMaterial, windowMaterial, false);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 1, 0);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsTop, Com9.rectangleTop, Side.TOP);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsLeft, Com9.rectangleLeft, Side.LEFT);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsRight, Com9.rectangleRight, Side.RIGHT);
            Com9.drawWithTextureMY(mGlTextureObject1[4], Com9.arrayPointsBack, Com9.rectangleBack, Side.BACK);
            Gl.glPopMatrix();
            Gl.glPopMatrix();


        }

        #endregion

        #region DrawSteet
        //New Method
        private void DrawStreet()
        { 

            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            buildingColor = Color.FromArgb(255, 127, 80);
            roofColor = Color.FromArgb(139, 35, 35);
            foubdationColor = Color.FromArgb(28,28,28);
            DrawHouse(-25, 1, 30,house1);

            buildingColor = Color.FromArgb(238, 232, 170);
            roofColor = Color.FromArgb(238, 118, 0);
           
            DrawHouse(-25, 1, -6, house2);
            buildingColor = Color.FromArgb(233, 150, 122);
            roofColor = Color.FromArgb(108, 123, 139);
            foubdationColor = Color.FromArgb(139, 119, 101);
            DrawHouse(-25, 1, -60,house3);
            buildingColor = Color.FromArgb(255, 228, 181);
            DrawHouse(-15, 1, -80,house7);
            buildingColor = Color.FromArgb(250, 240, 230);
            DrawHouse(-25, 1, -100,house4);


            buildingColor = Color.FromArgb(176, 48, 96);
            roofColor = Color.FromArgb(165, 42, 42);
            DrawHouse( 15, 1, -100,house5);


            buildingColor = Color.FromArgb(244, 164,96);
            roofColor = Color.FromArgb(131, 139, 139);
            foubdationColor = Color.FromArgb(165, 42, 42);
           // windowColor = Color.FromArgb(23, 10, 100);
            DrawHouse(60, 1, -100, house8);


            buildingColor = Color.FromArgb(23, 10, 100);
            roofColor = Color.FromArgb(233, 150, 122);
            foubdationColor = Color.FromArgb(28, 28, 28);
            DrawHouse(100, 1, -100,house6);



            buildingColor = Color.FromArgb(255, 140, 0 );
            roofColor = Color.FromArgb(34, 139, 34 );
            DrawHouse(100, 1, -40, house1);
            buildingColor=Color.FromArgb(255, 69, 0 );
            roofColor = Color.FromArgb(139, 69, 19 );
            DrawHouse(70,  1, -40,house2);
            buildingColor=Color.FromArgb(255, 255, 224 );
            roofColor = Color.FromArgb(131, 139, 139);
            DrawHouse(37,1,-40,house9);
            roofColor = Color.FromArgb(219, 112, 147  );
            buildingColor=Color.FromArgb(255, 228, 196);
            DrawHouse(5, 1, -40, house10);
            buildingColor = Color.FromArgb(243, 112, 151);
            roofColor = Color.FromArgb(252,114,45);
            DrawHouse(5, 1, 0, house11);
            roofColor = Color.FromArgb(91, 85, 87);
            DrawHouse(5, 1, 30, house11);
            roofColor = Color.FromArgb(139, 35, 35);
            buildingColor = Color.FromArgb(173, 214, 255);
            DrawHouse(40, 1, 10, house12);
            buildingColor = Color.FromArgb(249,175,90);
            roofColor = Color.FromArgb(21,21,19);
            DrawHouse(70, 1, 10, house14);
            buildingColor = Color.FromArgb(255, 246, 143 );
            roofColor = Color.FromArgb(250, 128, 114);
            DrawHouse(100, 1, 10, house13);
        }

        private void DrawHouse(int dx,int dy,int dz,House house)
        {
            Gl.glPushMatrix();
            Gl.glTranslated(dx, dy, dz);
            house.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);
            Gl.glPopMatrix();
        }

        private static void InitStreet(out House house1, out House house2, out House house3, out House house4, out House house5, out House house6,out House house7,out House house8,out House house9,out House house10,out House house11,out House house12,out House house13,out House house14)
        {
            house1 = new House(4, 5, 7, 1500, 1500, 2200);
            house2 = new House(7, 10, 5, 2500, 4500, 2000);
            house3 = new House(10, 15, 20, 2000, 7500, 2000);
            house4 = new House(2, 5, 2, 1500, 4000, 1000);
            house5 = new House(7, 10, 7, 4500, 7000, 3000);
            house6 = new House(7, 10, 7, 1500, 3000, 3000);

            house7 = new House(7, 10, 7, 1500, 3000, 3000);
            house8 = new House(7, 10, 7, 1500, 3000, 3000);


            house9 = new House();
            house10 = new House();
            house11 = new House();
            house12 = new House();
            house13 = new House();
            house14 = new House();

            //Input Data to house


            //house1
            house1.InputDate(5, 5, 5, 1500, 1700, 1500, 70, 25,1f,5);
            house1.InputSteps(0.5, 0.7, 0.5);
            
            house1.modelingHouse();
            //house2
            house2.InputDate(5, 6, 5, 1700, 2500, 2000, 100, 25,0.5f,5);
            house2.InputSteps(0.4, 0.6, 0.4);
            house2.modelingHouse();
            //house3
            house3.InputDate(9, 20, 9, 1500, 5000, 3000, 0, 0, 0.5f,5);
            house3.InputSteps(0.5, 0.5, 0.5);
            house3.modelingHouse();
            //house4
            house4.InputDate(1, 35, 1, 2000, 9000, 1000, 0, 0,0.5f,5);
            house4.InputSteps(0, 0.8, 0);
            house4.modelingHouse();


            house7.InputDate(10, 45, 10, 1500, 7000, 1000, 0, 0, 0.5f,5);
            house7.InputSteps(0.4, 0.5, 0.4);
            house7.modelingHouse();


            //house5
            house5.InputDate(7, 10, 7, 2500, 4000, 3000, 170, 50,0.5f,5);
            house5.InputSteps(0.5, 0.4, 0.5);
            house5.modelingHouse();
            //house6
            house6.InputDate(7, 10, 7, 1500, 3000, 3000, 100, 20,0.5f,5);
            house6.InputSteps(0.5, 0.4, 0.5);
            house6.modelingHouse();

            house8.InputDate(7, 25, 7, 2300, 6000, 3500, 150, 20, 0.5f, 5);
            house8.InputSteps(0.5, 0.5, 0.5);
            house8.modelingHouse();


            house9.InputDate(7, 15, 7, 1800, 3800, 2500, 0, 0, 0.5f, 5);
            house9.InputSteps(0.7, 0.7, 0.7);
            house9.modelingHouse();


            house10.InputDate(7, 5, 7, 1700, 2200, 1500, 100, 20, 0.5f, 5);
            house10.InputSteps(0.5, 0.7, 0.5);
            house10.modelingHouse();


            //3-й ряд
            house11.InputDate(7, 3, 7, 1700, 1200, 1500, 100, 0, 0.5f, 5);
            house11.InputSteps(0.5, 0.7, 0.5);
            house11.modelingHouse();


            house12.InputDate(7, 5, 7, 1500, 2200, 2500, 0, 0, 0.5f, 5);
            house12.InputSteps(0.5, 0.7, 0.5);
            house12.modelingHouse();


            house13.InputDate(7, 3, 7, 1500, 1000, 2500, 100, 50, 0.5f, 5);
            house13.InputSteps(0.5, 0.4, 0.5);
            house13.modelingHouse();

            house14.InputDate(7, 5, 7, 1500, 1900, 2500, 0, 0, 0.5f, 5);
            house14.InputSteps(0.5, 0.7, 0.5);
            house14.modelingHouse();
            //===============================================

            Gl.glPushMatrix();
            Gl.glTranslated(-25, 1, 30);
            house1.StencilTest();
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glTranslated(-25, 1, -6);
            house2.StencilTest();
            // house2.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-25, 1, -60);
            house3.StencilTest();
            // house3.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-15, 1, -80);
            house7.StencilTest();
            // house4.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-25, 1, -100);
            house4.StencilTest();
            // house4.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(15, 1, -100);
            house5.StencilTest();
            // house5.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(60, 1, -100);
            house8.StencilTest();
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glTranslated(100, 1, -100);
            house6.StencilTest();
            //house6.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glTranslated(100,1,-40);
            house1.StencilTest();
            // house1.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();


            //house2.InputDate(15, 5, 5, 3500, 2700, 1500, 70, 25, 1f, 5);
            //house2.modelingHouse();
            Gl.glPushMatrix();
            Gl.glTranslated(70, 1, -40);
            house2.StencilTest();
            // house1.DrawBuildins(buildingColor, windowColor, roofColor, foubdationColor, directColor, buildingMaterial, windowMaterial);//DrawPrim();//DrawSMth();
            Gl.glPopMatrix();



            Gl.glPushMatrix();
            Gl.glTranslated(37, 1, -40);
            house9.StencilTest();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(5, 1, -40);
            house10.StencilTest();
            Gl.glPopMatrix();


            //3-й ряд.

            Gl.glPushMatrix();
            Gl.glTranslated(5, 1, 0);
            house11.StencilTest();
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            Gl.glTranslated(5, 1, 30);
            house11.StencilTest();
            Gl.glPopMatrix();



            Gl.glPushMatrix();
            Gl.glTranslated(40, 1, 10);
            house12.StencilTest();
            Gl.glPopMatrix();



            Gl.glPushMatrix();
            Gl.glTranslated(70, 1, 10);
            house14.StencilTest();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(100, 1, 10);
            house13.StencilTest();
            Gl.glPopMatrix();
            //
        }
        #endregion

        #region DrawPlanes
        private void DrawPlane1()
        {
          
            Gl.glPushMatrix();
            Gl.glTranslated(50, 0, -20);
            double a = 80;
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3b(0, 1, 0);
          
            Gl.glVertex3d(a, -0.1, a);
           
            Gl.glVertex3d(a, -0.1, -a);
            Gl.glVertex3d(-a, -0.1, -a);
            Gl.glVertex3d(-a, -0.1, a);

            Gl.glEnd();
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }


        private void DrawPlane()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject1[2]);
            Gl.glPushMatrix();
            Gl.glTranslated(50, 0, -20);
            double a = 80;
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glNormal3b(0, 1, 0);
            Gl.glTexCoord2d(0, 0);
            Gl.glVertex3d(a, -0.1, a);
            Gl.glTexCoord2d(1, 0);
            Gl.glVertex3d(a, -0.1, -a);
            Gl.glTexCoord2d(1, 1);
            Gl.glVertex3d(-a, -0.1, -a);
            Gl.glTexCoord2d(0, 1);
            Gl.glVertex3d(-a, -0.1, a);

            Gl.glEnd();
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        #endregion
        //Загружаем текстуры для студ городка
        #region multiTexture

        public void TEXTURE() 
        {
            // Set up texture maps
            Gl.glEnable(Gl.GL_TEXTURE_2D);
                //n-идентификаторов
            Gl.glGenTextures(NUM_TEXTURES, textureObjects);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND0_RGB_ARB, Gl.GL_SRC_COLOR);
            for (int i = 0; i < NUM_TEXTURES; i++)
                textureLoadNext(TextureUri[i],i);    
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        private void textureLoadNext(string url, int i)
        {

            // создаем изображение с индификатором imageId
            Il.ilGenImages(i, out imageId);
            // делаем изображение текущим
            Il.ilBindImage(imageId);

            // пробуем загрузить изображение
            if (Il.ilLoadImage(url))
            {
                // если загрузка прошла успешно
                // сохраняем размеры изображения
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                // определяем число бит на пиксель
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp) // в зависимости оп полученного результата
                {
                    // создаем текстуру используя режим GL_RGB или GL_RGBA
                    case 24:
                        mGlTextureObject1[i] = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject1[i] = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
                // активируем флаг, сигнализирующий загрузку текстуры
                textureIsLoad = true;
                // очищаем память
                Il.ilDeleteImages(i, ref imageId);
            }
        }




      

  #endregion
            //------------------------------------ОСВЕЩЕНИЕ---------------------------------------
        #region Light
        //-------------Свойство материала------------------

        private void SetColors()
        {
            buildingColor = Color.FromArgb(trackBarRb.Value, trackBarGb.Value, trackBarBb.Value);
            windowColor = Color.FromArgb(trackBarRw.Value, trackBarGw.Value, trackBarBw.Value);
            roofColor = Color.FromArgb(trackBarRroof.Value, trackBarGroof.Value, trackBarBroof.Value);
            foubdationColor = Color.FromArgb(trackBarRf.Value,trackBarGf.Value,trackBarBf.Value);
            directColor = Color.FromArgb(trackBarRT.Value, trackBarGT.Value, trackBarBT.Value);
            ambientColor = Color.FromArgb(trackBarRR.Value, trackBarGR.Value, trackBarBR.Value);

            pictureBoxB.BackColor = buildingColor;
            pictureBoxW.BackColor = windowColor;
            pictureBoxT.BackColor = directColor;
            pictureBoxR.BackColor = ambientColor;
            pictureBoxRoof.BackColor = roofColor;
            pictureBoxFound.BackColor = foubdationColor;

           labelr1.Text= trackBarRT.Value.ToString();
           labelg1.Text= trackBarGT.Value.ToString();
           labelb1.Text = trackBarBT.Value.ToString();

           labelr2.Text = trackBarRR.Value.ToString();
           labelg2.Text = trackBarGR.Value.ToString();
           labelb2.Text = trackBarBR.Value.ToString();

           labelr3.Text = trackBarRb.Value.ToString();
           labelg3.Text = trackBarGb.Value.ToString();
           labelb3.Text = trackBarBb.Value.ToString();

           labelr4.Text = trackBarRw.Value.ToString();
           labelg4.Text = trackBarGw.Value.ToString();
           labelb4.Text = trackBarBw.Value.ToString();



           label140.Text = trackBarRroof.Value.ToString();
           label141.Text = trackBarGroof.Value.ToString();
           label142.Text = trackBarBroof.Value.ToString();


           label143.Text = trackBarRf.Value.ToString();
           label144.Text = trackBarGf.Value.ToString();
           label145.Text = trackBarBf.Value.ToString();

        


        }

        //Цвет источников света
        private void setLightSource()
        { // Enable lighting
            Gl.glEnable(Gl.GL_LIGHTING);
            float[] ambientLS = { (float)ambientColor.R / 255, (float)ambientColor.G / 255, (float)ambientColor.B / 255, 1.0f };
            float[] directLS = { (float)directColor.R / 255, (float)directColor.G / 255, (float)directColor.B / 255, 1.0f };
            float[] positionLS = { (float)trackBarXLight.Value, (float)trackBarYLight.Value, (float)trackBarZLight.Value, 1.0f };
            
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambientLS);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, directLS);
            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, directLS);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, positionLS);
            Gl.glLightModelf(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);

            // Enable color tracking
          //Gl.glEnable(Gl.GL_COLOR_MATERIAL);
        //    float[] mat_emission = {1f, 0f, 0f, 1.0f};
            // Set Material properties to follow glColor values
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE);
         //   Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION,mat_emission);
        }

        #endregion
        //====================================ВАЖНЫЕ СОБЫТИЯ===============================
        #region Events
        //Изменение свойств материала
       private void onMaterialPropertyChanched(object sender, EventArgs e)
       {
           ka = (float)trackBarKa.Value / 100;
           kd = (float)trackBarKd.Value / 100;
           ks = (float)trackBarKs.Value / 100;
           n = trackBarn.Value;
           transparency = (float)trackBarP.Value / 100;
           if (radioButtonWall.Checked)
               buildingMaterial.SetMaterialProperty(kd, ks, ka, n, transparency);
           if (radioButtonWindow.Checked)
               windowMaterial.SetMaterialProperty(kd, ks, ka, n, transparency);
           AnT.Invalidate();

           labelKa.Text = ka.ToString();
           labelKd.Text = kd.ToString();
           labelKs.Text= ks.ToString(); 
           labeln.Text= n.ToString();
           labelt.Text = transparency.ToString();
       }


       //Установить нужные значения свойств
       private void onRadioButtonChecked(object sender, EventArgs e)
       {
           if (radioButtonWall.Checked)
           {
               trackBarKa.Value = (int)(buildingMaterial.Ka * 100);
               trackBarKd.Value = (int)(buildingMaterial.Kd * 100);
               trackBarKs.Value = (int)(buildingMaterial.Ks * 100);
               trackBarn.Value = buildingMaterial.n;
               trackBarP.Value = (int)(buildingMaterial.transponency * 100);
           }
           if (radioButtonWindow.Checked)
           {
               trackBarKa.Value = (int)(windowMaterial.Ka * 100);
               trackBarKd.Value = (int)(windowMaterial.Kd * 100);
               trackBarKs.Value = (int)(windowMaterial.Ks * 100);
               trackBarn.Value = windowMaterial.n;
               trackBarP.Value = (int)(windowMaterial.transponency * 100);
           }
       }


       //Изменение параметров ДОМА
       private void onParametrChanched(object sender, EventArgs e)
       {
           maxX = trackBarMaxX.Value * Math.PI / 180;
           maxY = trackBarMaxY.Value * Math.PI / 180;
           maxZ = trackBarMaxZ.Value * Math.PI / 180;
           house.InputDate(trackBarCountX.Value, trackBarCountY.Value, trackBarCountZ.Value, trackBarMaxX.Value, trackBarMaxY.Value, trackBarMaxZ.Value,trackBarH.Value,trackBarRoof.Value,0.2f,1);
           house.modelingHouse();
           AnT.Refresh();

           labelh.Text = trackBarMaxY.Value.ToString();
           labell.Text = trackBarMaxX.Value.ToString();
           labelsh.Text = trackBarMaxZ.Value.ToString();


            labeldl.Text=trackBarCountX.Value.ToString();
            labelet.Text=trackBarCountY.Value.ToString();
            labelshr.Text = trackBarCountZ.Value.ToString();


            label28.Text = trackBarH.Value.ToString();
            label29.Text = trackBarRoof.Value.ToString();
       }



      //Изменение цвета
       private void onColorChanged(object sender, EventArgs e)
       {
           SetColors();
           AnT.Refresh();
          
       }


       private void onBackGroundColorChanched(object sender,EventArgs e) {
           Gl.glClearColor((float)(trackBar1.Value)/100.0f, (float)trackBar3.Value/100.0f,(float)trackBar4.Value/100.0f, 0);

           label146.Text = trackBar1.Value.ToString();
           label147.Text = trackBar3.Value.ToString();
           label148.Text = trackBar4.Value.ToString();

       }

       private void onLocationChanged(object sender, EventArgs e)
       {
           X = trackBarX.Value;
           Y = trackBarY.Value;
           Z = trackBarZ.Value;
           labelX.Text = X.ToString();
           labelY.Text = Y.ToString();
           labelZ.Text = Z.ToString();
       }

   //--------------Параметры окошек(высота/ширина)-----------------------
       private void onStepChanged(object sender, EventArgs e)
       {
           stepWindowTop1 = (double)trackBarStepWin1.Value/100;
           stepWindowLeft1 =(double) trackBar2.Value/100;    
           stepWindowTop2 = (double)trackBarStepWinTop2.Value/100;


           labelQ.Text = stepWindowTop1.ToString();
           labelE.Text = stepWindowLeft1.ToString();
           labelW.Text = stepWindowTop2.ToString();

           house.InputSteps(stepWindowTop1,stepWindowTop2,stepWindowLeft1);
           AnT.Invalidate();
       }

      


       private void button1_Click(object sender, EventArgs e)
       {
           house.findTextureCoord();
           textureLoad();
           AnT.Refresh();
       }

       //--------Повороты
       #region Rotation
       private void trackBarRx_Scroll(object sender, EventArgs e)
        {
            AngleX = trackBarRx.Value;
            labelRx.Text = AngleX.ToString();
            AnT.Invalidate(); 
        }

        private void trackBarRy_Scroll(object sender, EventArgs e)
        {
            AngleY = trackBarRy.Value;
            labelRy.Text = AngleY.ToString();
            AnT.Invalidate(); 
        }

        private void trackBarRz_Scroll(object sender, EventArgs e)
        {
            AngleZ = trackBarRz.Value;
            labelRz.Text = AngleZ.ToString();
            AnT.Invalidate();
        }


        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            zoom = (double)trackBarZoom.Value / 100;
            labelZoom.Text = zoom.ToString();
        }



        private void trackBarXLight_Scroll(object sender, EventArgs e)
        {
            labelXL.Text = trackBarXLight.Value.ToString();

        }

        private void trackBarYLight_Scroll(object sender, EventArgs e)
        {
            labelYL.Text = trackBarYLight.Value.ToString();
        }

        private void trackBarZLight_Scroll(object sender, EventArgs e)
        {
            labelZL.Text = trackBarZLight.Value.ToString();
        }




        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            zoom = 0.55;
            trackBarZoom.Value = 55;
            AngleX = 0;
            AngleY = -45;
            AngleZ = 0;
            trackBarRx.Value = AngleX;
            trackBarRy.Value = AngleY;
            trackBarRz.Value = AngleZ;
            labelZoom.Text = zoom.ToString();
            buildingMaterial.Ka = 0.46;
            X = 0; Y = -3; Z = -20;

            groupBox7.Enabled = true;
            groupBox10.Enabled = true;
            groupBox6.Enabled = true;
            groupBox8.Enabled = true;
            groupBox5.Enabled = true;
        }

        private void AnT_MouseDown(object sender, MouseEventArgs e)
        {
            clicked = true;
        }

        private void AnT_MouseUp(object sender, MouseEventArgs e)
        {
            clicked = false;
        }

        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            if (clicked)
            {

                AngleX = e.X % 360;
                AngleY = e.Y % 360;
                trackBarRx.Value = AngleX;
                trackBarRy.Value = AngleY;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            zoom = 0.06;
            buildingMaterial.Ka = 0.46;
            X = -2;
            Y = -4;
            Z = -19;
            AngleX = 5;
            AngleY = -54;
            AngleZ = 2;

            trackBarRx.Value = AngleX;
            trackBarRy.Value = AngleY;
            trackBarRz.Value = AngleZ;

            trackBarX.Value = (int)X;
            trackBarY.Value = (int)Y;
            trackBarZ.Value = (int)Z;

            
            SetColors();
            groupBox7.Enabled = false;
            groupBox10.Enabled = false;
            groupBox6.Enabled = false;
            groupBox8.Enabled = false;
            groupBox5.Enabled = true;
           
            tabControl1.SelectTab(1);
            
            
        }
       #endregion
        #endregion
        //Наложение текстуры
        #region Texture
        //================ТЕКСТУРИРОВАНИЕ=================
        /* Обработка пункта меню загрузки изображения
      * */
        private void textureLoad()
        {
            // открываем окно выбора файла
            DialogResult res = openFileDialogTextureName.ShowDialog();

            // есл файл выбран - и возвращен результат OK
            if (res == DialogResult.OK)
            {
                // создаем изображение с индификатором imageId
                Il.ilGenImages(1, out imageId);
                // делаем изображение текущим
                Il.ilBindImage(imageId);
                // адрес изображения полученный с помощью окна выбра файла
                string url = openFileDialogTextureName.FileName;
                // пробуем загрузить изображение
                if (Il.ilLoadImage(url))
                {
                    // если загрузка прошла успешно
                    // сохраняем размеры изображения
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    // определяем число бит на пиксель
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                    switch (bitspp) // в зависимости оп полученного результата
                    {
                        // создаем текстуру используя режим GL_RGB или GL_RGBA
                        case 24:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                            break;
                        case 32:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                            break;
                    }
                    // активируем флаг, сигнализирующий загрузку текстуры
                    textureIsLoad = true;
                    // очищаем память
                    Il.ilDeleteImages(1, ref imageId);
                }
            }

        }

      


        /* Создание текстуры в памяти openGL
         * */
        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {
            // индетефекатор текстурного объекта
            uint texObject;
            // генерируем текстурный объект
            Gl.glGenTextures(1, out texObject);
            // устанавливаем режим упаковки пикселей
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);
            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);



            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            //Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE0_RGB_ARB, Gl.GL_TEXTURE0);
            //Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND0_RGB_ARB, Gl.GL_SRC_COLOR);


            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE0_RGB_ARB, Gl.GL_TEXTURE0);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND0_RGB_ARB, Gl.GL_SRC_COLOR);

            //Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE1_RGB_ARB, Gl.GL_TEXTURE2);
            //Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND1_RGB_ARB, Gl.GL_SRC_COLOR);

            //Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE2_RGB_ARB, Gl.GL_TEXTURE1);
            //Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND2_RGB_ARB, Gl.GL_SRC_COLOR);
            // создаем RGB или RGBA текстуру
            switch (Format)
            {
                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
            }
            // возвращаем индетефекатор текстурного объекта
            return texObject;
        }

        #endregion

        #region Useful
        void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            //Описываем свойства материала
            float[] color = new float[4] { 1, 0, 0, 1 }; //Красный цвет
            float[] shininess = new float[1] { 30 };
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, color); //Цвет чайника
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, color); //Отраженный свет
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininess); //Степень отраженного света
            //----------------------------
            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, -6);
            Gl.glRotated(AngleX, 1.0, 0.0, 0.0);
            Gl.glRotated(AngleY, 0.0, 1.0, 0.0);
            Gl.glRotated(AngleZ, 0.0, 0.0, 1.0);
            // рисуем чайник с помощью библиотеки FreeGLUT 
            Glut.glutSolidTeapot(1);
            Gl.glPopMatrix();
            Gl.glFlush();
            AnT.Invalidate();
        }






        void SetupRC()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH);
            // Light values and coordinates
            float[] whiteLight = { 0.45f, 0.45f, 0.45f, 1.0f };
            float[] sourceLight = { 0.25f, 0.25f, 0.25f, 1.0f };
            float[] lightPos = { trackBarXLight.Value, trackBarYLight.Value, trackBarZLight.Value, 0.0f };

        //    Gl.glEnable(Gl.GL_DEPTH_TEST);	// Hidden surface removal

            // Enable lighting
            Gl.glEnable(Gl.GL_LIGHTING);

            // Setup and enable light 0
            //Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, whiteLight);
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_TWO_SIDE, whiteLight);
            
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, sourceLight);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, sourceLight);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);
            Gl.glEnable(Gl.GL_LIGHT0);

            // Enable color tracking
          //  Gl.glEnable(Gl.GL_COLOR_MATERIAL);

            // Set Material properties to follow glColor values
        //    Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE);

            // Black blue background
            Gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }







       

        #region Ground
        void DrawGround2()
        {
            float fExtent = 20.0f;
            float fStep = 1.0f;
            float y = -0.1f;
            float iStrip, iRun;

            for (iStrip = -fExtent; iStrip <= fExtent; iStrip += fStep)
            {
                Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                Gl.glNormal3f(0.0f, 1.0f, 0.0f);   // All Point up

                for (iRun = fExtent; iRun >= -fExtent; iRun -= fStep)
                {
                    Gl.glVertex3f(iStrip, y, iRun);
                    Gl.glVertex3f(iStrip + fStep, y, iRun);
                }
                Gl.glEnd();
            }
        }


        //Рисует сетку
        void DrawGround()
        {
            Gl.glColor3ub(0,0,0);
            float fExtent = 20.0f;
            float fStep = 1.0f;
            float y = -0.4f;
            float iLine;

            Gl.glBegin(Gl.GL_LINES);
            for (iLine = -fExtent; iLine <= fExtent; iLine += fStep)
            {
                Gl.glVertex3f(iLine, y, fExtent);    // Draw Z lines
                Gl.glVertex3f(iLine, y, -fExtent);

                Gl.glVertex3f(fExtent, y, iLine);
                Gl.glVertex3f(-fExtent, y, iLine);
            }

            Gl.glEnd();
        }


        #endregion

        private void radioButtonStudCity_CheckedChanged(object sender, EventArgs e)
        {
            zoom = 0.06;
            trackBarZoom.Value = 6;

            labelZoom.Text = zoom.ToString();
            buildingMaterial.Ka = 1;
            AngleX = 23;
            AngleY = 45;
            AngleZ = 11;
            X = -1; Y = -1; Z = -6;
            trackBarRx.Value = AngleX;
            trackBarRy.Value = AngleY;
            trackBarRz.Value = AngleZ;

            trackBarX.Value = (int)X;
            trackBarY.Value = (int)Y;
            trackBarZ.Value = (int)Z;

            groupBox7.Enabled = false;
            groupBox10.Enabled = false;
            groupBox6.Enabled = false;
            groupBox8.Enabled = false;
            groupBox5.Enabled = false;
            tabControl1.SelectTab(1);
        }

      
        


        #endregion

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        

      

        

       
      
    }
}
