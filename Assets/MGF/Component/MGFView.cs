using System;
using MGF.Math;
using UnityEngine;


[RequireComponent(typeof(MGFObject))]
public class MGFView : MGFComponet
{
    MGFObject obj;

    Animator ar;


    bool isPlayer = false;
    public override void Init()
    {


    }

    public void Move()
    {
        transform.position = obj.tr.Pos.ToVector3Zero();
    }

    public void Rot()
    {
        transform.rotation = Quaternion.Euler(0, (float)obj.tr.Rot, 0);
    }



    private void Update()
    {
        if (obj == null || obj.IsStatic)
        {
            return;
        }

        if (isPlayer)
        {
        //    switch (obj1.PS)
        //    {
        //        case PlayerStateInGame.idle:
        //            ar.SetBool("IdleToWalk", false);
        //            transform.position = obj.tr.Pos.ToVector3Zero();
        //            break;
        //        case PlayerStateInGame.run:
        //            ar.SetBool("IdleToWalk", true);
        //            transform.position = Vector3.Lerp(transform.position, obj.tr.Pos.ToVector3Zero(), Time.deltaTime * 10);
        //            break;
        //    }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, obj.tr.Pos.ToVector3Zero(), Time.deltaTime * 10);
        }

        //旋转视图
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, (float)obj.tr.Rot, 0), Time.deltaTime * 10);
    }
    internal void Attack()
    {
        ar.SetTrigger("Attack");
    }

    public void Hide()
    {
        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            item.enabled = false;
        }

    }

    public void Show()
    {
        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            item.enabled = true;
        }
    }

}
