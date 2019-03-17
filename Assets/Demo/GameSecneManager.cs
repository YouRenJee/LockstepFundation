using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Framework;
using MGF.Physics;
using MGF.Math;
using System;

public class GameSecneManager : GameSingletom<GameSecneManager>
{
    public Fix64Vector2 RecyclePoint = new Fix64Vector2(300, 300);

    private GameObject Player;
    private GameObject Plane;
    private GameObject tp;
    private GameObject Bullet;
    private Queue<OptionEvent> q = new Queue<OptionEvent>();

    public int syncFrameid = 0;
    private float stickDir = 0;


    private Dictionary<int, MGFPlayer> players = new Dictionary<int, MGFPlayer>();

    private List<MGFObject> dynamicObj = new List<MGFObject>();
    public Quadtree qt;


    public void SetDir(float dir)
    {
        stickDir = dir;
    }

    public void InputKey(OptionEvent opt)
    {
        q.Enqueue(opt);
    }

    public void GameInit()
    {
        Player = Resources.Load<GameObject>("Player/DefaultAvatar");
        tp = Resources.Load<GameObject>("Ground/Ground");
        Bullet = Resources.Load<GameObject>("Player/Shuriken");
        Plane = Instantiate(tp,Vector3.zero,Quaternion.identity).gameObject;
        CreatePlayer();
        CreateBullet(10);
        qt = new Quadtree(0, GetSecneSize(50, 50));
        AddObj();
    }

    private void CreatePlayer()
    {
        List<RoomPlayerInfo> info = RuntimeData.RoomListInfo;
        for (int i = 0; i < info.Count; i++)
        {
            GameObject obj = Instantiate(Player, Plane.transform, true);
            obj.transform.position = Vector3.zero;
            var mgf = obj.GetComponent<MGFPlayer>();
            mgf.PlayerID = info[i].player_id;
            if (info[i].player_id == RuntimeData.UserID)
            {
                mgf.IsMine = true;
                Camera.main.transform.SetParent(Plane.transform);
                Camera.main.gameObject.AddComponent<PlayerCamera>().Init(mgf);
            }
            players.Add(info[i].player_id, mgf);
        }

    }

    private void CreateBullet(int num)
    {
        Queue<IRecycleAble> ls = new Queue<IRecycleAble>();
        for (int i = 0; i < num; i++)
        {
            var l = Instantiate(Bullet, RecyclePoint.ToVector3(), Quaternion.identity, Plane.transform).GetComponent<MGFObject>();
            l.IsCollisionAble = false;
            l.IsTrigger = true;
            l.IsStatic = false;
            ls.Enqueue((IRecycleAble)l);
        }
        ObjectPool.AddNewClass("Bullet", ls);
    }

    private void AddObj()
    {
        obj = FindObjectsOfType<MGFObject>();
        cnt = obj.Length;
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i].Init();

            if (obj[i].IsStatic == false)
            {
                dynamicObj.Add(obj[i]);
            }
            if (obj[i].IsCollisionAble == true)
            {
                qt.Insert(obj[i]);
            }
            
        }
    }

    private int cnt = 0;
    private MGFObject[] obj;

    public MGFObject[] GetObj()
    {
        return obj;
    }

    public void NewMGFObj()
    {


    }

    public void DestroyMGFObj()
    {

    }

    private Rectangle GetSecneSize(int w,int h)
    {
        Fix64Vector2 halfSize = new Fix64Vector2(0, 0);
        Fix64Vector2 center = new Fix64Vector2((Fix64)Plane.transform.position.x, (Fix64)Plane.transform.position.z);
        halfSize.X = (Fix64)w;
        halfSize.Y = (Fix64)h;
        Rectangle rt = new Rectangle(center, halfSize);
        return rt;
    }

        

    public void MoveObj(MGFObject obj)
    {
        if (!Quadtree.dic.ContainsKey(obj) || Quadtree.dic[obj] == null)
        {
            qt.Insert(obj);
        }
        else
        {
            qt.Move(obj);
        }
        
    }

    private void OnGUI()
    {
        GUIStyle s = new GUIStyle();
        s.fontSize = 60;
        GUILayout.Label(syncFrameid.ToString(), s);
        
    }

    /// <summary>
    /// 进行包围盒的检测
    /// </summary>
    private bool CheckBoundings(MGFObject a, MGFObject b)
    {
        Fix64Vector2 posA = a.GetPos();
        Fix64Vector2 posB = b.GetPos();
        return Fix64.Abs(posA.X - posB.X) < a.GetHalfSize().X + b.GetHalfSize().X &&
             Fix64.Abs(posA.Y - posB.Y) < a.GetHalfSize().Y + b.GetHalfSize().Y;

    }

    private List<MGFObject> lcd = new List<MGFObject>();



    private void CheckCollision()
    {
        
        for (int i = 0; i < dynamicObj.Count; i++)
        {
            if (dynamicObj[i].IsCollisionAble == false)
            {
                continue;
            }
            if (lcd.Count != 0)
            {
                lcd.Clear();
            }
            //获得需要进行碰撞检测的list
            qt.Retrieve(lcd, dynamicObj[i]);

            bool flag = false;
            for (int j = 0; j < lcd.Count; j++)
            {
                //先计算包围盒是否碰撞 再通过GJK检测多边形是否碰撞
                if (CheckBoundings(lcd[j], dynamicObj[i]))
                {
                    if (GJK(dynamicObj[i], lcd[j]))
                    {
                        flag = true;
                        dynamicObj[i].CalcCollisionDir(lcd[j]);
                        dynamicObj[i].OnMGFCollision(lcd[j]);


                    }

                }
            }
            if (flag)
            {
                dynamicObj[i].PL = ObjectState.BeCollision;
            }
            else
            {
                dynamicObj[i].PL = ObjectState.None;
            }
        }

    }

    private bool GJK(MGFObject a, MGFObject b)
    {
        Simplex s = new Simplex();
        Fix64Vector2 dir = a.GetPos() - b.GetPos();
        s.Add(Support(a, b, dir));
        dir = -dir;

        while (true)
        {
            s.Add(Support(a, b, dir));
            if (Fix64Vector2.Dot(s.GetLast(), dir) <= Fix64.Zero)
            {
                return false;
            }
            else
            {
                if (ContainsOrigin(s,ref dir))
                {
                    return true;
                }
            }
        }
    }

    private bool ContainsOrigin(Simplex s, ref Fix64Vector2 dir)
    {

        var a = s.GetLast();
        var AO = -a;
        var b = s.GetB();
        var AB = b - a;
        if (s.Count() == 3)
        {
            var c = s.GetC();
            var AC = c - a;

            var abPerp = CalcNomal(AC,AB, AB);
            var acPerp = CalcNomal(AB,AC, AC);

            if (Fix64Vector2.Dot(abPerp, AO) > Fix64.Zero)
            {
                s.Remove(c);
                dir = abPerp;
            }
            else
            {
                if (Fix64Vector2.Dot(acPerp, AO) > Fix64.Zero)
                {
                    s.Remove(b);
                    dir = acPerp;
                }
                else
                {
                    return true;
                }
            }
        }
        else
        {
            
            var abPerp = CalcNomal(AB,AO, AB);
            dir = abPerp;
        }
        return false;
    }

    private Fix64Vector2 CalcNomal(Fix64Vector2 a, Fix64Vector2 b, Fix64Vector2 c)
    {
        Fix64 z = a.X * b.Y - a.Y * b.X;

        return new Fix64Vector2(-z*c.Y, z*c.X);
    }

    public class Simplex
    {
        private List<Fix64Vector2> list = new List<Fix64Vector2>();

        public void Add(Fix64Vector2 point)
        {
            list.Add(point);
        }


        public Fix64Vector2 GetLast()
        {
            return list[list.Count - 1];
        }

        public Fix64Vector2 GetB()
        {
            return list[list.Count - 2];
        }
        public Fix64Vector2 GetC()
        {
            return list[list.Count - 3];
        }

        public void Remove(Fix64Vector2 point)
        {
            list.Remove(point);
        }

        public int Count()
        {
            return list.Count;
        }
    }

    private Fix64Vector2 Support(MGFObject a, MGFObject b, Fix64Vector2 d)
    {
        Fix64Vector2 InA = a.Support(d);
        Fix64Vector2 InB = b.Support(-d);

        
        return InA - InB;
    }

    /// <summary>
    /// 画出四叉树以及动态物体所在节点
    /// </summary>
    private void OnDrawGizmos()
    {
        if (qt==null)
        {
            return;
        }
        DrawQt(qt);

        DrawDynamicObj();
    }

    private void DrawDynamicObj()
    {
        Gizmos.color = new Color(0.5f,0.9f,0f,0.3f);
        for (int i = 0; i < dynamicObj.Count; i++)
        {
            if (Quadtree.dic.ContainsKey(dynamicObj[i])==false || Quadtree.dic[dynamicObj[i]]==null)
            {
                continue;
            }
            Fix64Vector2 center = Quadtree.dic[dynamicObj[i]].GetCenter().center;
            Fix64Vector2 halfSize = Quadtree.dic[dynamicObj[i]].GetCenter().halfSize;
            Gizmos.DrawCube(center.ToVector3(),(halfSize*2).ToVector3());
        }
    }

    private void DrawQt(Quadtree qqt)
    {
        Gizmos.color = Color.red;
        if (qqt != null)
        {
            if (qqt.nodes == null)
            {
                return;
            }
            else
            {
                if (qqt.nodes[0] == null)
                {
                    return;
                }
            }
            float ctX = (float)qqt.GetCenter().center.X;
            float ctY = (float)qqt.GetCenter().center.Y;
            float width = (float)qqt.GetCenter().halfSize.X;
            float height = (float)qqt.GetCenter().halfSize.Y;
            Vector3 vf = new Vector3(ctX - width, 1, ctY);
            Vector3 vt = new Vector3(ctX + width, 1, ctY);
            Vector3 hf = new Vector3(ctX, 1, ctY - height);
            Vector3 ht = new Vector3(ctX, 1, ctY + height);

            Gizmos.DrawLine(vf, vt);
            Gizmos.DrawLine(hf, ht);
            for (int i = 0; i < 4; i++)
            {
                if (qqt.nodes[i] != null)
                {
                    DrawQt(qqt.nodes[i]);
                }
            }
        }
    }

    public void OnLogicUpdate(LogicFrame udata)
    {

        SyncOpts(udata);
        CollectPlayerOpt();
    }

    private void CollectPlayerOpt()
    {
        NextFrameOpt nextFrame = new NextFrameOpt();
        nextFrame.frameid = syncFrameid + 1;
        nextFrame.zid = RuntimeData.RoomID;
        nextFrame.uid = RuntimeData.UserID;
        OptionEvent stick = new OptionEvent();


        stick.player_id = RuntimeData.UserID;   
        stick.opt_type = OptType.JoyStick;
        Fix64 a = new Fix64(stickDir);
        stick.value = a.GetRaw();
        nextFrame.opts.Add(stick);
        for (int i = 0; i < q.Count; i++)
        {
            nextFrame.opts.Add(q.Dequeue());
        }
        
        NetworkManager.Instance.UdpSendProtobufCmd(Stype.game_server, Cmd.eNextFrameOpt, nextFrame);
    }

    private void SyncOpts(LogicFrame frame)
    {
        for (int i = 0; i < frame.unsync_frames.Count; i++)
        {
            if (syncFrameid >= frame.unsync_frames[i].frameid)
            {
                continue;
            }
            OnSyncFrameOpts(frame.unsync_frames[i].opts);
        }
        syncFrameid = frame.frameid;
    }

    private void OnSyncFrameOpts(List<OptionEvent> opts)
    {
        //处理玩家操作
        for (int i = 0; i < opts.Count; i++)
        {
            players[opts[i].player_id].HandleOpt(opts[i]);
        }

        //处理帧事件
        for (int i = 0; i < dynamicObj.Count; i++)
        {
            dynamicObj[i].HandleFrameEvent();
        }
        //计算碰撞
        CheckCollision();
        //SyncTower();
    }
}






