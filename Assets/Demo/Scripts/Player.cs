
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

[RequireComponent(typeof(PlayerView))]
public class Player : MGFObject
{
    public int PlayerID = 1;
    private PlayerView pv;
    internal override void Init()
    {
        Fix64 x = new Fix64(32);
        base.Init();
        pv = GetComponent<PlayerView>();
    }

    internal void Skill()
    {
        Bullet bt= GameSceneManager.Instance.CreateFromPool<Bullet>("BULLET",GetPos() + Forward.Nomalize() , tr.Rot);
        bt.PlayerID = PlayerID;
        pv.Skill();
    }
}
