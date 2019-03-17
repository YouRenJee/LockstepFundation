
using MGF.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerStateInGame
{
    idle = 1,
    run = 2,
    attack = 3,
}
[RequireComponent(typeof(Animator))]
public class MGFPlayer : MGFObject
{
    public PlayerStateInGame PS = PlayerStateInGame.idle;
    public int PlayerID = -1;
    public bool IsMine = false;

    public override void Init()
    {
        base.Init();

        base.Tag = "Player";
    }

    //public virtual void HandleOpt(OptionEvent optionEvent)
    //{

    //    Fix64 a = new Fix64(optionEvent.value, false);
    //    switch (optionEvent.opt_type)
    //    {
    //        case OptType.JoyStick:
    //            HandleJoyStick(a);
    //            break;
    //        case OptType.Attack:
    //            HandleAttack(a);
    //            break;
    //        case OptType.skill1:
    //            Handleskill1(a);
    //            break;

    //    }
    //}

    private void HandleAttack(Fix64 value)
    {
        PS = PlayerStateInGame.attack;
        v.Attack();
        StartCoroutine("Shot");
    }


    IEnumerator Shot()
    {
        MGFBullet bt = ObjectPool.CreateObj<MGFBullet>("Bullet");
        bt.MoveTo(tr.Pos + Forward.Nomalize());
        bt.RotateTo(tr.Rot);
        bt.PlayerID = PlayerID;
        bt.v.Hide();
        bt.v.Move();
        bt.v.Rot();
        yield return new WaitForSeconds(0.2f);
        bt.v.Show();



    }

    private void Handleskill1(Fix64 value)
    {
        
    }

    private void HandleJoyStick(Fix64 value)
    {

        if (value == Fix64.Zero)
        {
            PS = PlayerStateInGame.idle;
            return;
        }
        else
        {
            PS = PlayerStateInGame.run;
        }

        base.RotateTo(value);
        base.Move(Forward);
    }


}
