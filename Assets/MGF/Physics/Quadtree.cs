using System;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;

namespace MGF.Physics
{
    public class Rectangle
    {
        public Fix64Vector2 center; //原点
        public Fix64Vector2 halfSize;


        public Rectangle(Fix64Vector2 center, Fix64Vector2 halfSize)
        {
            this.center = center;
            this.halfSize = halfSize;

        }
    }


    public class Quadtree 
    {



        public static Dictionary<MGFObject, Quadtree> dic = new Dictionary<MGFObject, Quadtree>();
        private int MAX_OBJECTS = 3;
        private int MAX_LEVELS = 5;

        private int _level = 0;        // 子节点深度
        private List<MGFObject> objects;     // 物体数组
        private Rectangle rect;
        public Quadtree[] nodes = null; // 四个子节点

        public Rectangle GetCenter()
        {
            return rect;
        }

        public bool IsNull()
        {
            bool isnull = true;
            if (objects.Count == 0)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i] != null)
                    {
                        isnull = nodes[i].IsNull();
                        if (isnull == false)
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
            return isnull;
        }

        public Quadtree(int level, Rectangle pBounds)
        {
            objects = new List<MGFObject>();
            nodes = new Quadtree[4];
            rect = pBounds;
            _level = level;
        }

        public void Remove(MGFObject pRect)
        {
            
            objects.Remove(pRect);
        }

        public void Clear()
        {
            objects.Clear();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }

        private void Split()
        {
            Fix64 subWidth = rect.halfSize.X / (Fix64)2;
            Fix64 subHeight = rect.halfSize.Y / (Fix64)2;
            Fix64Vector2 subSize = new Fix64Vector2(subWidth, subHeight);
            Fix64 X = rect.center.X;
            Fix64 Y = rect.center.Y;

            nodes[0] = new Quadtree(_level + 1, new Rectangle(new Fix64Vector2(X + subWidth, Y + subHeight), subSize));
            nodes[1] = new Quadtree(_level + 1, new Rectangle(new Fix64Vector2(X - subWidth, Y + subHeight), subSize));
            nodes[2] = new Quadtree(_level + 1, new Rectangle(new Fix64Vector2(X - subWidth, Y - subHeight), subSize));
            nodes[3] = new Quadtree(_level + 1, new Rectangle(new Fix64Vector2(X + subWidth, Y - subHeight), subSize));
        }

        private int GetIndex(MGFObject pRect)
        {
            int index = -1;
            // 中线
            Fix64 verticalMidpoint = rect.center.X;
            Fix64 horizontalMidpoint = rect.center.Y;
            Fix64Vector2 pHalfSize = pRect.GetHalfSize();
            Fix64Vector2 pos = pRect.GetPos();
            // 物体完全位于上面两个节点所在区域
            bool topQuadrant = (pos.Y > horizontalMidpoint && pos.Y - pHalfSize.Y > horizontalMidpoint);
            // 物体完全位于下面两个节点所在区域
            bool bottomQuadrant = (pos.Y < horizontalMidpoint && pos.Y + pHalfSize.Y < horizontalMidpoint);

            // 物体完全位于左面两个节点所在区域
            if (pos.X < verticalMidpoint && (pos.X + pHalfSize.X) < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1; // 处于左上节点 
                }
                else if (bottomQuadrant)
                {
                    index = 2; // 处于左下节点
                }
            }
            // 物体完全位于右面两个节点所在区域
            else if (pos.X > verticalMidpoint && pos.X - pHalfSize.X > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0; // 处于右上节点
                }
                else if (bottomQuadrant)
                {
                    index = 3; // 处于右下节点
                }
            }

            return index;
        }

        public void Insert(MGFObject pRect)
        {
            // 插入到子节点
            if (nodes[0] != null)
            {
                int index = GetIndex(pRect);

                if (index != -1)
                {
                    nodes[index].Insert(pRect);
                    return;
                }
            }

            // 还没分裂或者插入到子节点失败，只好留给父节点了
            objects.Add(pRect);
            if (pRect.IsStatic == false)
            {
                if (dic.ContainsKey(pRect))
                {
                    dic[pRect] = this;
                }
                else
                {
                    dic.Add(pRect, this);
                }
            }

            // 超容量后如果没有分裂则分裂
            if (objects.Count > MAX_OBJECTS && _level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }
                // 分裂后要将父节点的物体分给子节点们
                for (int i = objects.Count-1; i>=0; i--)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].Insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                }
            }
        }



        public void Retrieve(List<MGFObject> lcd, MGFObject pRect)
        {
            int index = GetIndex(pRect); //判断属于当前节点的那个nodes上


            if (index != -1 && nodes[0] != null) //如果不在当前节点的边界上 且当前节点不是最后一个子节点 则继续加入子节点物体
            {
                nodes[index].Retrieve(lcd, pRect);
            }
            else if (nodes[0] != null) //如果在当前节点边界上 并且子节点不为空 那么继续交给子节点判断
            {
                for (int i = 0; i < 4; i++)
                {
                    nodes[i].Retrieve(lcd, pRect);
                }
            }


            //加入当前节点上的物体
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i]!= pRect && objects[i].IsCollisionAble == true)
                {
                    lcd.Add(objects[i]);
                } 
            }
            

        }

        public Quadtree RetrieveQt(MGFObject pRect)
        {
            Quadtree qt = this;
            int index = GetIndex(pRect);
            if (index != -1 && nodes[0] != null)
            {
                qt = nodes[index].RetrieveQt(pRect);
            }
            return qt;

        }


        /// <summary>
        /// 物体发生移动时
        /// </summary>
        /// <param name="pRect"></param>
        public void Move(MGFObject pRect)
        {
            //计算该物体目前所在的位置是否与前一帧所在位置一致

            Quadtree now = RetrieveQt(pRect);
            if (now != dic[pRect])
            {
                dic[pRect].Remove(pRect);
                now.Insert(pRect);
            }
        }



    }

}