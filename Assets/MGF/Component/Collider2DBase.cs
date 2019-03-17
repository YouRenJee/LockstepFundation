using MGF.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MGF.Physics.Quadtree;

public abstract class Collider2DBase : MonoBehaviour
{
    public bool EnableCheck = true;
    public bool IsStatic = true;
    

    public abstract bool Check(Collider2DBase obj);
    public abstract Fix64Vector2 GetPos();

    internal abstract Fix64 GetHalfHeight();

    internal abstract Fix64 GetHalfWidth();

}
