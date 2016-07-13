using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace Building2
{
    class Rect
    {
        public int i1;
        public int i2;
        public int i3;
        public int i4;
        public int j1;
        public int j2;
        public int j3;
        public int j4;
        //цвет
        Vertex n;
        Color сolor;

        public Rect(int _indexPoint1i, int _indexPoint1j, int _indexPoint2i, int _indexPoint2j, int _indexPoint3i, int _indexPoint3j, int _indexPoint4i, int _indexPoint4j)
        {
         this.i1 = _indexPoint1i;
         this.i2 = _indexPoint2i;
         this.i3 = _indexPoint3i;
         this.i4 = _indexPoint4i;
         this.j1 = _indexPoint1j;
         this.j2 = _indexPoint2j;
         this.j3 = _indexPoint3j;
         this.j4 = _indexPoint4j;
         }
    }
}
