using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;
using MGF.Physics;

namespace MGF.Physics
{
    internal static class MGFPhysics
    {

        internal static bool lineIntersection(Fix64Vector2 p0, Fix64Vector2 p1,
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

        internal static bool CheckBoundings(MGFObject a, MGFObject b)
        {
            Fix64Vector2 posA = a.GetPos();
            Fix64Vector2 posB = b.GetPos();
            return Fix64.Abs(posA.X - posB.X) < a.GetHalfSize().X + b.GetHalfSize().X &&
                 Fix64.Abs(posA.Y - posB.Y) < a.GetHalfSize().Y + b.GetHalfSize().Y;

        }

        public static bool GJK(MGFObject a, MGFObject b)
        {
            Simplex s = new Simplex();
            Fix64Vector2 dir = a.GetPos() - b.GetPos();
            s.Add(Support(a, b, dir));
            dir = -dir;

            while (true)
            {
                s.Add(Support(a, b, dir));
                if (Fix64Vector2.Dot(s.GetLast(), dir) <= Fix64.Zero)
                {
                    return false;
                }
                else
                {
                    if (ContainsOrigin(s, ref dir))
                    {
                        return true;
                    }
                }
            }
        }

        private static bool ContainsOrigin(Simplex s, ref Fix64Vector2 dir)
        {

            var a = s.GetLast();
            var AO = -a;
            var b = s.GetB();
            var AB = b - a;
            if (s.Count() == 3)
            {
                var c = s.GetC();
                var AC = c - a;

                var abPerp = CalcNomal(AC, AB, AB);
                var acPerp = CalcNomal(AB, AC, AC);

                if (Fix64Vector2.Dot(abPerp, AO) > Fix64.Zero)
                {
                    s.Remove(c);
                    dir = abPerp;
                }
                else
                {
                    if (Fix64Vector2.Dot(acPerp, AO) > Fix64.Zero)
                    {
                        s.Remove(b);
                        dir = acPerp;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {

                var abPerp = CalcNomal(AB, AO, AB);
                dir = abPerp;
            }
            return false;
        }

        private static Fix64Vector2 CalcNomal(Fix64Vector2 a, Fix64Vector2 b, Fix64Vector2 c)
        {
            Fix64 z = a.X * b.Y - a.Y * b.X;

            return new Fix64Vector2(-z * c.Y, z * c.X);
        }

        private static Fix64Vector2 Support(MGFObject a, MGFObject b, Fix64Vector2 d)
        {
            Fix64Vector2 InA = a.Support(d);
            Fix64Vector2 InB = b.Support(-d);
            return InA - InB;
        }

        private class Simplex
        {
            private List<Fix64Vector2> list = new List<Fix64Vector2>();

            public void Add(Fix64Vector2 point)
            {
                list.Add(point);
            }


            public Fix64Vector2 GetLast()
            {
                return list[list.Count - 1];
            }

            public Fix64Vector2 GetB()
            {
                return list[list.Count - 2];
            }
            public Fix64Vector2 GetC()
            {
                return list[list.Count - 3];
            }

            public void Remove(Fix64Vector2 point)
            {
                list.Remove(point);
            }

            public int Count()
            {
                return list.Count;
            }
        }
    }

}

