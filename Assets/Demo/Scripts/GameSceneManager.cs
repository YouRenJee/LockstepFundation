
using System.Collections.Generic;
using UnityEngine;
using MGF.Framework;
using MGF.Physics;
using MGF.Math;

public class GameSceneManager : SceneSingletom<GameSceneManager>
{
    private Fix64Vector2 m_RecyclePoint = new Fix64Vector2(300, 300);

    
    public GameObject Player;
    public GameObject Ground;
    public int X=0;
    public int Y=0;

    private ObjectPool m_Op;

    //private GameObject Bullet;

    //public void InputKey(OptionEvent opt)
    //{
    //    q.Enqueue(opt);
    //}

    //public void SetDir(float dir)
    //{
    //    stickDir = dir;
    //}


    private Rectangle GetSecneSize()
    {
        Fix64Vector2 halfSize = new Fix64Vector2(0, 0);
        Fix64Vector2 center = new Fix64Vector2((Fix64)Ground.transform.position.x, (Fix64)Ground.transform.position.z);
        halfSize.X = (Fix64)X;
        halfSize.Y = (Fix64)Y;
        Rectangle rt = new Rectangle(center, halfSize);
        return rt;
    }

    public void GameInit()
    {
        CreatePlayer();
        InitObjctsInScene();
    }

    private void CreatePlayer()
    {

    }

    private void InitObjctsInScene()
    {
        MGFObject[] obj = FindObjectsOfType<MGFObject>();
        PhysicsManager.Instance.Init(obj, GetSecneSize());
    }








    //private void CreateBullet(int num)
    //{
    //    Queue<IRecycleAble> ls = new Queue<IRecycleAble>();
    //    for (int i = 0; i < num; i++)
    //    {
    //        var l = Instantiate(Bullet, RecyclePoint.ToVector3(), Quaternion.identity, Plane.transform).GetComponent<MGFObject>();
    //        l.IsCollisionAble = false;
    //        l.IsTrigger = true;
    //        l.IsStatic = false;
    //        ls.Enqueue((IRecycleAble)l);
    //    }
    //    ObjectPool.AddNewClass("Bullet", ls);
    //}



    //private int cnt = 0;
    //private MGFObject[] obj;

    //public MGFObject[] GetObj()
    //{
    //    return obj;
    //}

    //public void DestroyMGFObj()
    //{

    //}




    //public void MoveObj(MGFObject obj)
    //{
    //    if (!Quadtree.dic.ContainsKey(obj) || Quadtree.dic[obj] == null)
    //    {
    //        qt.Insert(obj);
    //    }
    //    else
    //    {
    //        qt.Move(obj);
    //    }

    //}

    //private void OnGUI()
    //{
    //    GUIStyle s = new GUIStyle();
    //    s.fontSize = 60;
    //    GUILayout.Label(syncFrameid.ToString(), s);

    //}
    /// <summary>
    /// 画出四叉树以及动态物体所在节点
    /// </summary>
    //private void OnDrawGizmos()
    //{
    //    if (qt == null)
    //    {
    //        return;
    //    }
    //    DrawQt(qt);

    //    DrawDynamicObj();
    //}

    //private void DrawDynamicObj()
    //{
    //    Gizmos.color = new Color(0.5f, 0.9f, 0f, 0.3f);
    //    for (int i = 0; i < dynamicObj.Count; i++)
    //    {
    //        if (Quadtree.dic.ContainsKey(dynamicObj[i]) == false || Quadtree.dic[dynamicObj[i]] == null)
    //        {
    //            continue;
    //        }
    //        Fix64Vector2 center = Quadtree.dic[dynamicObj[i]].GetCenter().center;
    //        Fix64Vector2 halfSize = Quadtree.dic[dynamicObj[i]].GetCenter().halfSize;
    //        Gizmos.DrawCube(center.ToVector3(), (halfSize * 2).ToVector3());
    //    }
    //}

    //private void DrawQt(Quadtree qqt)
    //{
    //    Gizmos.color = Color.red;
    //    if (qqt != null)
    //    {
    //        if (qqt.nodes == null)
    //        {
    //            return;
    //        }
    //        else
    //        {
    //            if (qqt.nodes[0] == null)
    //            {
    //                return;
    //            }
    //        }
    //        float ctX = (float)qqt.GetCenter().center.X;
    //        float ctY = (float)qqt.GetCenter().center.Y;
    //        float width = (float)qqt.GetCenter().halfSize.X;
    //        float height = (float)qqt.GetCenter().halfSize.Y;
    //        Vector3 vf = new Vector3(ctX - width, 1, ctY);
    //        Vector3 vt = new Vector3(ctX + width, 1, ctY);
    //        Vector3 hf = new Vector3(ctX, 1, ctY - height);
    //        Vector3 ht = new Vector3(ctX, 1, ctY + height);

    //        Gizmos.DrawLine(vf, vt);
    //        Gizmos.DrawLine(hf, ht);
    //        for (int i = 0; i < 4; i++)
    //        {
    //            if (qqt.nodes[i] != null)
    //            {
    //                DrawQt(qqt.nodes[i]);
    //            }
    //        }
    //    }
    //}

    //public void OnLogicUpdate(LogicFrame udata)
    //{

    //    SyncOpts(udata);
    //    CollectPlayerOpt();
    //}

    //private void CollectPlayerOpt()
    //{
    //    NextFrameOpt nextFrame = new NextFrameOpt();
    //    nextFrame.frameid = syncFrameid + 1;
    //    nextFrame.zid = RuntimeData.RoomID;
    //    nextFrame.uid = RuntimeData.UserID;
    //    OptionEvent stick = new OptionEvent();


    //    stick.player_id = RuntimeData.UserID;
    //    stick.opt_type = OptType.JoyStick;
    //    Fix64 a = new Fix64(stickDir);
    //    stick.value = a.GetRaw();
    //    nextFrame.opts.Add(stick);
    //    for (int i = 0; i < q.Count; i++)
    //    {
    //        nextFrame.opts.Add(q.Dequeue());
    //    }

    //    NetworkManager.Instance.UdpSendProtobufCmd(Stype.game_server, Cmd.eNextFrameOpt, nextFrame);
    //}

    //private void SyncOpts(LogicFrame frame)
    //{
    //    for (int i = 0; i < frame.unsync_frames.Count; i++)
    //    {
    //        if (syncFrameid >= frame.unsync_frames[i].frameid)
    //        {
    //            continue;
    //        }
    //        OnSyncFrameOpts(frame.unsync_frames[i].opts);
    //    }
    //    syncFrameid = frame.frameid;
    //}

    //private void OnSyncFrameOpts(List<OptionEvent> opts)
    //{
    //    处理玩家操作
    //    for (int i = 0; i < opts.Count; i++)
    //    {
    //        players[opts[i].player_id].HandleOpt(opts[i]);
    //    }

    //    处理帧事件
    //    for (int i = 0; i < dynamicObj.Count; i++)
    //    {
    //        dynamicObj[i].HandleFrameEvent();
    //    }
    //    计算碰撞
    //    CheckCollision();
    //    SyncTower();
    //}
}






