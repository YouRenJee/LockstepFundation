using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Framework;
using MGF.Math;
using System;

namespace MGF.Physics
{
    public class PhysicsManager : ServiceModule<PhysicsManager>
    {


        private HashSet<MGFObject> m_Objects = new HashSet<MGFObject>();
        private HashSet<MGFObject> m_DynamicObjs = new HashSet<MGFObject>();
        private Quadtree m_Qt;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mgfList"></param>
        /// <param name="planePos"></param>
        public void Init(MGFObject[] mgfList, Rectangle planePos)
        {
            m_Qt = new Quadtree(0, planePos);
            if (mgfList != null && mgfList.Length != 0)
            {

                for (int i = 0; i < mgfList.Length; i++)
                {
                    mgfList[i].Init();
                    mgfList[i].MGFEnable();
                    m_Objects.Add(mgfList[i]);
                    if (mgfList[i].IsCollisionAble != false)
                    {
                        m_Qt.Insert(mgfList[i]);
                    }
                    if (mgfList[i].IsStatic == false)
                    {
                        m_DynamicObjs.Add(mgfList[i]);
                    }
                }

            }
        }

        /// <summary>
        /// 添加物体
        /// </summary>
        /// <param name="mGFObject"></param>
        public void AddObject(MGFObject mGFObject)
        {
            m_Qt.Insert(mGFObject);
        }

        /// <summary>
        /// 销毁物体
        /// </summary>
        /// <param name="mGFObject"></param>
        public void DesObject(MGFObject mGFObject)
        {
            m_Qt.Remove(mGFObject);
            Quadtree.dic[mGFObject] = null;
        }

        /// <summary>
        /// 获得所有物体
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MGFObject> GetAllObjects()
        {
            return m_Objects;
        }

        /// <summary>
        /// 获得所有动态物体
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MGFObject> GetAllDynamicObjs()
        {
            return m_DynamicObjs;
        }

        /// <summary>
        /// 获得所有物体
        /// </summary>
        /// <returns></returns>
        internal Quadtree GetQuadtree()
        {
            return m_Qt;
        }

        /// <summary>
        /// 射线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<MGFObject> RayCast2D(Fix64Vector2 start, Fix64Vector2 end)
        {
            HashSet<MGFObject> ls = new HashSet<MGFObject>();
            foreach (var item in m_Objects)
            {
                Fix64Vector2 hit;
                Fix64Vector2[] vertex = item.GetVertex();
                Fix64Vector2[] vertexN = new Fix64Vector2[vertex.Length];
                for (int j = 0; j < vertexN.Length; j++)
                {
                    vertexN[j] = vertex[j] + item.GetPos();

                }
                for (int j = 0; j < 4; j++)
                {
                    int nt = j + 1;
                    if (j == 3)
                    {
                        nt = 0;
                    }
                    if (MGFPhysics.lineIntersection(start, end, vertexN[j], vertexN[nt], out hit))
                    {
                        Debug.Log(item.name + " line P1" + vertexN[j] + " P2 " + vertexN[nt] + " hit " + hit);
                        ls.Add(item);
                        break;
                    }
                }
            }
            return ls;
        }

        private HashSet<MGFObject> lcd = new HashSet<MGFObject>();
        /// <summary>
        /// 碰撞检测
        /// </summary>
        public void CheckCollision()
        {
            foreach (var item in m_DynamicObjs)
            {
                if (item.IsCollisionAble == false)
                {
                    continue;
                }
                if (lcd.Count != 0)
                {
                    lcd.Clear();
                }
                //获得需要进行碰撞检测的list
                m_Qt.Retrieve(lcd, item);
                bool flag = false;
                foreach (var item2 in lcd)
                {
                    if (MGFPhysics.CheckBoundings(item2, item))
                    {
                        if (MGFPhysics.GJK(item, item2))
                        {
                            flag = true;
                            item.CalcCollisionDir(item2);
                            item.OnMGFCollision(item2);
                            Debug.Log(item.name + " " + item2.name);
                        }
                    }
                }
                item.IsCollsioning = flag;
            }
            
        }

        /// <summary>
        /// 物体移动时调用
        /// </summary>
        /// <param name="mgf"></param>
        public void Move(MGFObject mgf)
        {
            m_Qt.Move(mgf);
        }
    }
}

