using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Physics;
using MGF.Math;


[RequireComponent(typeof(MGFTransform)), RequireComponent(typeof(MGFBoxCollider2D))]
public class MGFObject : MGFComponet
{    
    public Fix64Vector2 Forward = new Fix64Vector2(0, 1);
    public Fix64Vector2 CollisionDir = new Fix64Vector2(0, 1);

    public bool IsStatic = true;
    public bool IsCollisionAble = false;
    public bool IsTrigger = false;
    public bool IsCollsioning = false;
    public bool IsMoving = false;
    public MGFCollision bc;
    public MGFTransform tr;
    public MGFView v;

    

    internal override string Tag { get { return "MGFObject"; } }

    private Fix64Vector2 initForward = new Fix64Vector2(0, 1);

    //碰撞多边形初始顶点坐标
    private Fix64Vector2[] vertexInit;
    //碰撞多边形旋转后坐标
    private Fix64Vector2[] vertex;

    //最小包围盒顶点坐标
    private Fix64Vector2[] boundings = new Fix64Vector2[4];
    //最小包围盒尺寸
    private Fix64Vector2 halfSize = new Fix64Vector2(0, 0);

    /// <summary>
    /// 初始化组件
    /// </summary>
    internal override void Init()
    {
        bc = GetComponent<MGFCollision>();
        tr = GetComponent<MGFTransform>();
        v = GetComponent<MGFView>();
        bc.Init();
        tr.Init();
        if (v!=null)
        {
            v.Init();
        }
        vertexInit = bc.GetColliderPos();
        vertex = new Fix64Vector2[vertexInit.Length];
        RotateVec(tr.Rot);
        CalcBoundings();
    }

    internal void RotateTo(Fix64 rot)
    {
        if (tr.Rot == rot)
        {
            return;
        }
        tr.Rot = rot;
        RotateVec(tr.Rot);
        CalcBoundings();
    }



    private void RotateVec(Fix64 rot)
    {
        for (int i = 0; i < vertexInit.Length; i++)
        {
            vertex[i] = Fix64Vector2.Rotate(vertexInit[i], -rot);
        }
        Forward = Fix64Vector2.Rotate(initForward, -rot);
    }

    internal void Move(Fix64Vector2 dir)
    {
        if (IsStatic)
        {
            throw new Exception("静态物体不可移动");
        }
        if (IsCollisionAble == false)
        {
            return;
        }
        if (IsCollsioning == false ||IsTrigger)
        {
            tr.Pos += dir / (Fix64)10;
        }
        else 
        {
            tr.Pos += CollisionDir.Nomalize() / (Fix64)20;
        }
        PhysicsManager.Instance.Move(this);
    }

    internal void MoveTo(Fix64Vector2 pos)
    {
        tr.Pos = pos;
        v.MoveTo();
        PhysicsManager.Instance.Move(this);
    }

    /// <summary>
    /// 获得包围盒的世界坐标
    /// </summary>
    /// <returns></returns>
    public Fix64Vector2 GetPos()
    {
        return tr.Pos;
    }

    /// <summary>
    /// 获得多边形定点坐标
    /// </summary>
    /// <returns></returns>
    public Fix64Vector2[] GetVertex()
    {
        return vertex;
    }

    /// <summary>
    /// 计算包围盒尺寸
    /// </summary>
    public void CalcBoundings()
    {
        //step1 分别计算4个轴向上最大/最小的x y 值
        Fix64 MAXX = (Fix64)0;
        Fix64 MAXY = (Fix64)0;
        Fix64 MINX = (Fix64)0;
        Fix64 MINY = (Fix64)0;
        for (int i = 0; i < vertex.Length; i++)
        {
            MAXX = Fix64.Max(MAXX, vertex[i].X);
            MAXY = Fix64.Max(MAXY, vertex[i].Y);
            MINX = Fix64.Min(MINX, vertex[i].X);
            MINY = Fix64.Min(MINY, vertex[i].Y);
        }
        halfSize.X = (MAXX - MINX) / Fix64.Two;
        halfSize.Y = (MAXY - MINY) / Fix64.Two;
    }

    /// <summary>
    /// 获得包围盒的尺寸
    /// </summary>
    /// <returns></returns>
    public Fix64Vector2 GetHalfSize()
    {
        return halfSize;
    }

    public virtual void OnMGFCollision(MGFObject obj)
    {

    }

    public Fix64Vector2 Support(Fix64Vector2 d)
    {
        Fix64 maxDot = Fix64Vector2.Dot(vertex[0], d);
        Fix64Vector2 point = vertex[0];
        for (int i = 1; i < vertex.Length; i++)
        {
            Fix64 now = Fix64Vector2.Dot(vertex[i], d);
            if (now > maxDot)
            {
                maxDot = now;
                point = vertex[i];
            }
        }
        return point + GetPos();
    }

    internal void CalcCollisionDir(MGFObject mgfObject)
    {
        if (IsTrigger) return;
        Fix64Vector2[] vertexO = mgfObject.GetVertex();
        Fix64Vector2 v1 = Fix64Vector2.Rotate(vertexO[0], -(Fix64)90);
        Fix64Vector2 v2 = Fix64Vector2.Rotate(vertexO[1], -(Fix64)90);
        Fix64Vector2 v3 = -v1;
        Fix64Vector2 v4 = -v2;




        Fix64Vector2 dir = GetPos() - mgfObject.GetPos();

        Fix64 d1 = Fix64Vector2.Dot(v1, dir);
        Fix64 d2 = Fix64Vector2.Dot(v2, dir);
        Fix64 d3 = Fix64Vector2.Dot(v3, dir);
        Fix64 d4 = Fix64Vector2.Dot(v4, dir);

        Fix64Vector2 collisionDirVertical = vertexO[0] - vertexO[1];
        Fix64Vector2 collisionDirHorizontal = vertexO[1] - vertexO[2];

        if (d1 >= Fix64.Zero && d4 >= Fix64.Zero)
        {
            if (Fix64Vector2.Dot(collisionDirHorizontal, Forward) > Fix64.Zero)
            {
                CollisionDir = Forward;
            }
            else
            {
                if (Fix64Vector2.Dot(collisionDirVertical, Forward) > Fix64.Zero)
                {
                    CollisionDir = collisionDirVertical;
                }
                else
                {
                    CollisionDir = -collisionDirVertical;
                }
            }

        }
        else if (d1 >= Fix64.Zero && d2 >= Fix64.Zero)
        {
            if (Fix64Vector2.Dot(collisionDirVertical, Forward) < Fix64.Zero)
            {
                CollisionDir = Forward;
            }
            else
            {
                if (Fix64Vector2.Dot(collisionDirHorizontal, Forward) > Fix64.Zero)
                {
                    CollisionDir = collisionDirHorizontal;
                }
                else
                {
                    CollisionDir = -collisionDirHorizontal;
                }
            }

        }
        else if (d2 >= Fix64.Zero && d3 >= Fix64.Zero)
        {
            if (Fix64Vector2.Dot(collisionDirHorizontal, Forward) < Fix64.Zero)
            {
                CollisionDir = Forward;
            }
            else
            {
                if (Fix64Vector2.Dot(collisionDirVertical, Forward) > Fix64.Zero)
                {
                    CollisionDir = collisionDirVertical;
                }
                else
                {
                    CollisionDir = -collisionDirVertical;
                }
            }

        }
        else if (d3 >= Fix64.Zero && d4 >= Fix64.Zero)
        {
            if (Fix64Vector2.Dot(collisionDirVertical, Forward) > Fix64.Zero)
            {
                CollisionDir = Forward;
            }
            else
            {
                if (Fix64Vector2.Dot(collisionDirHorizontal, Forward) > Fix64.Zero)
                {
                    CollisionDir = collisionDirHorizontal;
                }
                else
                {
                    CollisionDir = -collisionDirHorizontal;
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        if (tr == null || vertex == null || vertex.Length == 0)
        {
            return;
        }
        Gizmos.color = Color.green;
        DrawImaginaryCube(GetPos().ToVector2(), halfSize.ToVector2());

        for (int i = 0; i < vertex.Length; i++)
        {
            Gizmos.DrawCube((vertex[i] + GetPos()).ToVector3(), new Vector3(0.3f, 1f, 0.3f));
        }

    }

    /// <summary>
    /// 画一个虚框的正方形
    /// </summary>
    /// <param name="坐标"></param>
    /// <param name="尺寸"></param>
    void DrawImaginaryCube(Vector2 vec1, Vector2 vec2)
    {
        float halfheight = vec2.y;
        float halfweight = vec2.x;

        Gizmos.DrawLine(new Vector3(vec1.x + halfweight, 2, vec1.y + halfheight), new Vector3(vec1.x - halfweight, 2, vec1.y + halfheight));
        Gizmos.DrawLine(new Vector3(vec1.x + halfweight, 2, vec1.y + halfheight), new Vector3(vec1.x + halfweight, 2, vec1.y - halfheight));
        Gizmos.DrawLine(new Vector3(vec1.x + halfweight, 2, vec1.y - halfheight), new Vector3(vec1.x - halfweight, 2, vec1.y - halfheight));
        Gizmos.DrawLine(new Vector3(vec1.x - halfweight, 2, vec1.y - halfheight), new Vector3(vec1.x - halfweight, 2, vec1.y + halfheight));
    }

    internal virtual void HandleFrameEvent()
    {
        return;
    }
}
