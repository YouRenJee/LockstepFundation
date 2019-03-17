using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Math;

public abstract class MGFCollision : MGFComponet
{
    /// <summary>
    /// 获得该多边形碰撞体初始状态(旋转为0)的顶点坐标
    /// </summary>
    /// <returns></returns>
    public abstract Fix64Vector2[] GetColliderPos();

}
