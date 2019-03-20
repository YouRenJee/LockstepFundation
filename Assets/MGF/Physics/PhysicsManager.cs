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

        public event Action<MGFObject> OnCollision = null;

        private List<MGFObject> m_Objects = new List<MGFObject>();
        private List<MGFObject> m_DynamicObj = new List<MGFObject>();
        private Quadtree m_Qt;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mgfList"></param>
        /// <param name="planePos"></param>
        public void Init(MGFObject[] mgfList, Rectangle planePos)
        {
            m_Qt = new Quadtree(0, planePos);
            if (mgfList != null && mgfList.Length!=0)
            {
                
                for (int i = 0; i < mgfList.Length; i++)
                {
                    mgfList[i].Init();
                    m_Objects.Add(mgfList[i]);
                    m_Qt.Insert(mgfList[i]);
                    if (mgfList[i].IsStatic == false)
                    {
                        m_DynamicObj.Add(mgfList[i]);
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
            m_Objects.Add(mGFObject);
            m_Qt.Insert(mGFObject);
            if (mGFObject.IsStatic == false)
            {
                m_DynamicObj.Add(mGFObject);
            }
        }

        /// <summary>
        /// 销毁物体
        /// </summary>
        /// <param name="mGFObject"></param>
        public void DesObject(MGFObject mGFObject)
        {
            m_Objects.Remove(mGFObject);
            if (m_DynamicObj.Contains(mGFObject))
            {
                m_DynamicObj.Remove(mGFObject);
            }
            m_Qt.Remove(mGFObject);

        }

        /// <summary>
        /// 射线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<MGFObject> RayCast2D(Fix64Vector2 start, Fix64Vector2 end)
        {
            List<MGFObject> ls = new List<MGFObject>();
            for (int i = 0; i < m_Objects.Count; i++)
            {
                Fix64Vector2 hit;
                Fix64Vector2[] vertex = m_Objects[i].GetVertex();
                Fix64Vector2[] vertexN = new Fix64Vector2[vertex.Length];
                for (int j = 0; j < vertexN.Length; j++)
                {
                    vertexN[j] = vertex[j] + m_Objects[i].GetPos();

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
                        Debug.Log(m_Objects[i].name + " line P1" + vertexN[j] + " P2 " + vertexN[nt] + " hit " + hit);
                        ls.Add(m_Objects[i]);
                        break;
                    }
                }
            }
            return ls;
        }

        private List<MGFObject> lcd = new List<MGFObject>();
        
        /// <summary>
        /// 碰撞检测
        /// </summary>
        private void CheckCollision()
        {
            for (int i = 0; i < m_DynamicObj.Count; i++)
            {
                if (m_DynamicObj[i].IsCollisionAble == false)
                {
                    continue;
                }
                if (lcd.Count != 0)
                {
                    lcd.Clear();
                }
                //获得需要进行碰撞检测的list
                m_Qt.Retrieve(lcd, m_DynamicObj[i]);
               // bool flag = false;
                for (int j = 0; j < lcd.Count; j++)
                {
                    //先计算包围盒是否碰撞 再通过GJK检测多边形是否碰撞
                    if (MGFPhysics.CheckBoundings(lcd[j], m_DynamicObj[i]))
                    {
                        if (MGFPhysics.GJK(m_DynamicObj[i], lcd[j]))
                        {
                            //flag = true;
                            m_DynamicObj[i].CalcCollisionDir(lcd[j]);
                            m_DynamicObj[i].OnMGFCollision(lcd[j]);
                        }
                    }
                }
                //if (flag)
                //{
                //    m_DynamicObj[i].PL = ObjectState.BeCollision;
                //}
                //else
                //{
                //    m_DynamicObj[i].PL = ObjectState.None;
                //}
            }
        }
    }
}

