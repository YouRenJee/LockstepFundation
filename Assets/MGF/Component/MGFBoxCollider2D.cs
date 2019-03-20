using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;

public class MGFBoxCollider2D : MGFCollision 
{
    //矩形碰撞盒大小
    private Fix64Vector2 halfSize = new Fix64Vector2(0, 0);
    
    //矩形碰撞体有4个顶点
    private Fix64Vector2[] vertex = new Fix64Vector2[4];


    internal override string Tag { get { return "MGFBoxCollider2D"; } }
    internal override void Init()
    {
        Quaternion qua = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (GetComponent<MeshFilter>() != null)
        {
            halfSize.X = (Fix64)(GetComponent<MeshFilter>().mesh.bounds.size.x * transform.localScale.x / 2);
            halfSize.Y = (Fix64)(GetComponent<MeshFilter>().mesh.bounds.size.z * transform.localScale.z / 2);
        }
        else
        {
            halfSize.X = (Fix64)(GetComponent<BoxCollider>().bounds.size.x / 2);
            halfSize.Y = (Fix64)(GetComponent<BoxCollider>().bounds.size.z / 2);
        }

        vertex[0] = Fix64Vector2.SetVertex(halfSize.X, halfSize.Y);
        vertex[1] = Fix64Vector2.SetVertex( halfSize.X, -halfSize.Y);
        vertex[2] = Fix64Vector2.SetVertex(-halfSize.X, -halfSize.Y);
        vertex[3] = Fix64Vector2.SetVertex( -halfSize.X, halfSize.Y);
        transform.rotation = qua;
    }

    public override Fix64Vector2[] GetColliderPos()
    {
        return vertex;
    }
}
