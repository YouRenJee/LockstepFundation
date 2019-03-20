//using MGF.Physics;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MGFBullet : MGFObject,IRecycleAble
//{
//    public override void Init()
//    {
//        base.Init();
//        Tag = "Bullet";
//    }

//    //属于哪个Player
//    public int PlayerID = -1;
//    private int aliveTime = 50;
//    private int NowFrame = 0;

//    public void OnCreate()
//    {
        
//        IsCollisionAble = true;
//        NowFrame = 0;
//    }

//    public void OnDestory()
//    {
//        v.Hide();
//        IsCollisionAble = false;
//        tr.Pos = GameSecneManager.Instance.RecyclePoint;
        
//        Quadtree.dic[this].Remove(this);
//        Quadtree.dic[this] = null;
//    }
//    public override void OnMGFCollision(MGFObject obj)
//    {
//        if (obj is MGFPlayer)
//        {
//            if (((MGFPlayer)obj).PlayerID == PlayerID)
//            {
//                return;
//            } 
//        }
//        ObjectPool.DestoryObj("Bullet", this);
//    }

//    internal override void HandleFrameEvent()
//    {

//        if (IsCollisionAble == true)
//        {
//            NowFrame++;
//        }
//        else
//        {
//            return;
//        }
//        if (NowFrame>=aliveTime)
//        {
//            ObjectPool.DestoryObj("Bullet", this);
//            return;
//        }
        
//        Move(Forward*5);
//    }

    


//}
