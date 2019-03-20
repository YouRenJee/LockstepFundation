
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
public class MGFPlayer : MGFObject
{
    public int PlayerID = 1;
    private PlayerView pv;
    internal override void Init()
    {
        base.Init();
        pv = GetComponent<PlayerView>();
    }

    internal void Skill()
    {
        MGFBullet bt= GameSceneManager.Instance.CreateFromPool<MGFBullet>("BULLET",GetPos() + Forward.Nomalize() , tr.Rot);
        bt.PlayerID = PlayerID;
        pv.Skill();
    }
}
