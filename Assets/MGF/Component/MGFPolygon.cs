using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF;
using MGF.Math;


[ExecuteInEditMode]
public class MGFPolygon : MGFCollision
{
    public bool EditMode = false;
    public Fix64Vector2[] vertex;


    public override Fix64Vector2[] GetColliderPos()
    {
        throw new System.NotImplementedException();
    }

}
