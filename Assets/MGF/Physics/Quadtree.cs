using System;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;
using UnityEngine.Assertions;

namespace MGF.Physics
{
    public class Rectangle
    {
        public Fix64Vector2 center;
        public Fix64Vector2 halfSize;

        public Rectangle(Fix64Vector2 center, Fix64Vector2 halfSize)
        {
            this.center = center;
            this.halfSize = halfSize;

        }
    }

    internal class Quadtree
    {
        internal static Dictionary<MGFObject, Quadtree> dic = new Dictionary<MGFObject, Quadtree>();

        private Quadtree[] nodes = null;
        private int MAX_OBJECTS = 3;
        private int MAX_LEVELS = 5;
        private int _level = 0;
        private List<MGFObject> objects;
        private Rectangle rect;


        internal Quadtree[] GetNodes()
        {
            return nodes;
        }

        /// <summary>
        /// 获得当前节点中心点
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetCenter()
        {
            return rect;
        }

        /// <summary>
        /// 判断当前一个节点是否为空
        /// </summary>
        /// <returns></returns>
        internal bool IsNull()
        {
            bool isnull = true;
            if (objects.Count == 0)
            {
                if (nodes[0] == null)
                {
                    return true;
                }
                for (int i = 0; i < nodes.Length; i++)
                {
                    isnull = nodes[i].IsNull();
                    if (isnull == false)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return isnull;
        }

        /// <summary>
        /// 生成一棵树
        /// </summary>
        /// <param name="level">层</param>
        /// <param name="pBounds">中心点</param>
        internal Quadtree(int level, Rectangle pBounds)
        {
            objects = new List<MGFObject>();
            nodes = new Quadtree[4];
            rect = pBounds;
            _level = level;
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        /// <param name="pRect"></param>
        internal void Remove(MGFObject pRect)
        {
            if (objects.Contains(pRect))
            {
                objects.Remove(pRect);
            }
            else
            {
                if (dic[pRect] !=null)
                {
                    Assert.IsTrue(dic[pRect].objects.Contains(pRect));
                    dic[pRect].Remove(pRect);
                }
            }
        }

        /// <summary>
        /// 清空树
        /// </summary>
        internal void Clear()
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

        /// <summary>
        /// 分裂树
        /// </summary>
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

        /// <summary>
        /// 获得物体所在象限
        /// </summary>
        /// <param name="pRect"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 插入物体
        /// </summary>
        /// <param name="pRect"></param>
        internal void Insert(MGFObject pRect)
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
                for (int i = objects.Count - 1; i >= 0; i--)
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

        /// <summary>
        /// 获得所有可能发生碰撞物体
        /// </summary>
        /// <param name="lcd"></param>
        /// <param name="pRect"></param>
        internal void Retrieve(List<MGFObject> lcd, MGFObject pRect)
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
                if (objects[i] != pRect && objects[i].IsCollisionAble == true)
                {
                    lcd.Add(objects[i]);
                }
            }


        }

        /// <summary>
        /// 获得物体所在Quadtree节点
        /// </summary>
        /// <param name="pRect"></param>
        /// <returns></returns>
        private Quadtree RetrieveQt(MGFObject pRect)
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
        /// 物体发生移动时调用
        /// </summary>
        /// <param name="pRect"></param>
        internal void Move(MGFObject pRect)
        {
            Quadtree now = RetrieveQt(pRect);
            //计算该物体目前所在的位置是否与前一帧所在位置一致
            if (!dic.ContainsKey(pRect) || dic[pRect]==null)
            {
                return;
            }
            if (now != dic[pRect])
            {
                //不一致时重新插入物体
                dic[pRect].Remove(pRect);
                now.Insert(pRect);
            }
        }
    }

}