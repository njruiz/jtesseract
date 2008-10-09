using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace JTessaract
{
    public class Box
    {
        private string charactor;

        public string Charactor
        {
            get { return charactor; }
            set { charactor = value; }
        }
        private int x1;

        public int X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        private int y1;

        public int Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        private int x2;

        public int X2
        {
            get { return x2; }
            set { x2 = value; }
        }
        private int y2;

        public int Y2
        {
            get { return y2; }
            set { y2 = value; }
        }
    }
}
