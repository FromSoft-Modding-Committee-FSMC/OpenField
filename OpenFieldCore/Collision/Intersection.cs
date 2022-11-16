using System;
using System.Collections.Generic;
using System.Text;

using OFC.Mathematics;

namespace OFC.Collision
{
    public class Intersection
    {
        public static bool CircleCircle(Vector2s aOrigin, float aRadius, Vector2s bOrigin, float bRadius)
        {
            float D = Vector2s.EuclideanDistance(ref aOrigin, ref bOrigin);

            if(D > (aRadius + bRadius)) { return false; }   //Circles do not intersect.
            return false;
        }
    }
}
