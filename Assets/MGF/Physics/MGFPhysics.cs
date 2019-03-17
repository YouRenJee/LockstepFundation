using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;
using MGF.Physics;

namespace MGF.Physics
{
    public static class MGFPhysics
    {

        public static bool lineIntersection(Fix64Vector2 p0, Fix64Vector2 p1,
               Fix64Vector2 p2, Fix64Vector2 p3, out Fix64Vector2 point)
        {
            Fix64 s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, s_numer, t_numer, denom, t;
            s10_x = p1.X - p0.X;
            s10_y = p1.Y - p0.Y;
            s32_x = p3.X - p2.X;
            s32_y = p3.Y - p2.Y;
            point = new Fix64Vector2(0, 0);
            denom = s10_x * s32_y - s32_x * s10_y;
            if (denom == Fix64.Zero)//平行或共线
                return false; // Collinear
            bool denomPositive = denom > Fix64.Zero;

            s02_x = p0.X - p2.X;
            s02_y = p0.Y - p2.Y;
            s_numer = s10_x * s02_y - s10_y * s02_x;
            if ((s_numer < Fix64.Zero) == denomPositive)//参数是大于等于0且小于等于1的，分子分母必须同号且分子小于等于分母
                return false; // No collision

            t_numer = s32_x * s02_y - s32_y * s02_x;
            if ((t_numer < Fix64.Zero) == denomPositive)
                return false; // No collision

            if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
                return false; // No collision
                              // Collision detected
            t = t_numer / denom;
            point.X = p0.X + (t * s10_x);
            point.Y = p0.Y + (t * s10_y);
            return true;
        }
        public static List<MGFObject> RayCast2D(Fix64Vector2 start, Fix64Vector2 end)
        {
            List<MGFObject> ls = new List<MGFObject>();
            MGFObject[] obj = GameSecneManager.Instance.GetObj();
            for (int i = 0; i < obj.Length; i++)
            {
                Fix64Vector2 hit;
                Fix64Vector2[] vertex = obj[i].GetVertex();
                Fix64Vector2[] vertexN = new Fix64Vector2[vertex.Length];
                for (int j = 0; j < vertexN.Length; j++)
                {
                    vertexN[j] = vertex[j] + obj[i].GetPos();
                    
                }
                for (int j = 0; j < 4; j++)
                {
                    int nt = j + 1;
                    if (j == 3)
                    {
                        nt = 0;
                    }
                    if (lineIntersection(start, end, vertexN[j], vertexN[nt], out hit))
                    {
                        Debug.Log(obj[i].name + " line P1" + vertexN[j] + " P2 " + vertexN[nt] + " hit " + hit);
                        ls.Add(obj[i]);
                        break;
                    }
                }
            }
            return ls;
        }
                

    }

}

