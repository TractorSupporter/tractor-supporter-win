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
        public DateTime CreatedTime;
        public DateTime LastUpdatedPlaceTime;
        public int ConfirmCount;

        public Obstacle2D(double x, double y, DateTime t1, DateTime t2, DateTime t3)
        {
            X = x;
            Y = y;
            LastUpdateTime = t1;
            CreatedTime = t2;
            LastUpdatedPlaceTime = t3;
            ConfirmCount = 1;
        }
    }

}
