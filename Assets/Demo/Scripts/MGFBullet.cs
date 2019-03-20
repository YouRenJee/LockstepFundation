using MGF.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGFBullet : MGFObject, IRecycleAble
{
    public int PlayerID = -1;
    public float m_NowFrame = 0;
    private  float m_aliveTime = 60;

    internal override string Tag { get{ return "BULLET"; }}

    internal override void Init()
    {
        base.Init();
        m_aliveTime = m_aliveTime * Time.fixedDeltaTime;
    }

    internal override void MGFStart()
    {
        m_NowFrame = 0;
    }

    public override void OnMGFCollision(MGFObject obj)
    {
        if (obj is MGFPlayer)
        {
            if (((MGFPlayer)obj).PlayerID == PlayerID)
            {
                return;
            }
        }
        GameSceneManager.Instance.DestroyFromPool(this);
    }

    internal override void HandleFrameEvent()
    {
        
        if (IsCollisionAble == true)
        {
            m_NowFrame+=Time.fixedDeltaTime;
        }
        else
        {
            return;
        }
        if (m_NowFrame >= m_aliveTime)
        {
            GameSceneManager.Instance.DestroyFromPool(this);
            return;
        }

        Move(Forward * 5);

        
    }


    public int InitNum()
    {
        return 20;
    }

    public string RecycleName()
    {
        return Tag;
    }

}
