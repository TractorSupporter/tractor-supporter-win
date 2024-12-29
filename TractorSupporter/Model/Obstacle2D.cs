using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Model
{
    public class Obstacle2D
    {
        public double X;             
        public double Y;             
        public DateTime LastUpdateTime;
        public int ConfirmCount;

        public Obstacle2D(double x, double y, DateTime t)
        {
            X = x;
            Y = y;
            LastUpdateTime = t;
            ConfirmCount = 1;
        }
    }

}
