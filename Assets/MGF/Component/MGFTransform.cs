using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;


public class MGFTransform : MGFComponet 
{
    public Fix64Vector2 Pos = new Fix64Vector2(0,0);
    public Fix64 Rot = Fix64.Zero;





    public override void Init()
    {
        Pos.X = (Fix64) transform.position.x;
        Pos.Y = (Fix64) transform.position.z;
        Rot = (Fix64)transform.rotation.eulerAngles.y;
    }



}
