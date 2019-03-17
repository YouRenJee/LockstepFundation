//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MGF.Math;
//using System;
//using MGF.Physics;

//public class SphereCollider2D : Collider2DBase
//{


//    public Fix64 Radius = new Fix64(0.5f);


//    public override bool Check(Collider2DBase obj)
//    {


//        if (Fix64Vector2.Distance(GetPos(), obj.GetPos()) > (Radius + obj.GetHalfHeight()))
//        {
//            return false;
//        }
//        else
//        {
//            return true;
//        }

//    }

//    public override Fix64Vector2 GetPos()
//    {
//        return new Fix64Vector2(new Fix64(transform.localPosition.x), new Fix64(transform.localPosition.z));
//    }

//    internal override Fix64 GetHalfHeight()
//    {
//        return Radius;
//    }

//    internal override Fix64 GetHalfWidth()
//    {
//        return Radius;
//    }




//    void OnDrawGizmosSelected()
//    {

//        Gizmos.DrawSphere(transform.position, 0.5f);
//        Gizmos.color = Color.green;

//        if (Quadtree.dic.Count == 0)
//        {
//            return;
//        }
//        if (Quadtree.dic.ContainsKey(this))
//        {
//            Quadtree.Rectangle rt = Quadtree.dic[this].GetCenter();
//            Gizmos.DrawCube(new Vector3((float)rt.bounds.X, 0.1f, (float)rt.bounds.Y), new Vector3((float)rt.GetHalfWidth() * 2, 0, (float)rt.GetHalfWidth() * 2));

//        }
//    }

//}

