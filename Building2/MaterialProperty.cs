using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace Building2
{
    class MaterialProperty
    {
      public  double Kd{get;set;}
      public  double Ks { get; set; }
      public  double Ka{get;set;}
     public   int n{get;set;}
     public float transponency;
     public   Color MaterialColor;
        public MaterialProperty(Color _color,double _Kd, double _Ks, double _Ka, int _n,float _tr) 
        {
            MaterialColor = _color;
            Kd = _Kd;
            Ks = _Ks;
            Ka = _Ka;
            n = _n;
            transponency = _tr;
        }

        public void SetMaterialProperty(double _Kd, double _Ks, double _Ka, int _n, float _tr)
        {
            
            Kd = _Kd;
            Ks = _Ks;
            Ka = _Ka;
            n = _n;
            transponency = _tr;
        }

        public void setMaterialColor(Color _color)
        {
            MaterialColor = _color;
        }
    }
}
