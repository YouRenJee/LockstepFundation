
using UnityEngine;
using MGF.Framework;
using MGF.Physics;
using MGF.Math;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CommandType
{
    JoyStick = 1,
    Skill = 2
}

public class Command
{
    public CommandType CmdType;
    public float value;
    public Command(CommandType tp)
    {
        CmdType = tp;
    }
}

public class GameSceneManager : SceneSingletom<GameSceneManager>
{
    
    public MGFPlayer Player;
    public GameObject Bullet;
    public GameObject Ground;
    public PlayerCamera APlayerCamera;
    public UIJoyStick JoyStick;
    public int X=0;
    public int Y=0;

    private ObjectPool m_Op;
    private Fix64Vector2 m_RecyclePoint = new Fix64Vector2(300, 300);
    private Queue<Command> m_Opts = new Queue<Command>();

    //private GameObject Bullet;

    //public void InputKey(OptionEvent opt)
    //{
    //    q.Enqueue(opt);
    //}

    //public void SetDir(float dir)
    //{
    //    stickDir = dir;
    //}

    //private void Start()
    //{
    //    var type = typeof(IRecycleAble);

    //    List<Type> types = new List<Type>();

    //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    //    {
    //        foreach (var tp in assembly.GetTypes())
    //        {
    //            if (type.IsAssignableFrom(tp))
    //            {
    //                if (tp.IsClass && !tp.IsAbstract)
    //                {
    //                    types.Add(tp);
    //                }
    //            }
    //        }
    //    }
    //    types.ForEach((t) =>
    //    {
    //        var instance = Activator.CreateInstance(t) as IRecycleAble;
    //        Debug.Log(instance.RecycleName());
    //        Debug.Log(instance.InitNum());
    //    });

    //}

    private void Start()
    {
        GameInit();
    }

    public void GameInit()
    {
        m_Op = new ObjectPool();
        APlayerCamera.Init();
        JoyStick.Init();
        CreatePlayer();
        CreateBullet();
        InitObjctsInScene();
    }

    private void CreateBullet()
    {
        IRecycleAble ra = Bullet.GetComponent<MGFBullet>();
        Queue<IRecycleAble> q = new Queue<IRecycleAble>();
        for (int i = 0; i < ra.InitNum(); i++)
        {
            MGFBullet bt = Instantiate(Bullet, m_RecyclePoint.ToVector3(),Quaternion.identity).GetComponent<MGFBullet>();
            bt.IsCollisionAble = false;
            q.Enqueue(bt);
        }
        m_Op.AddNewObj(ra.RecycleName(), q);
    }

    private void CreatePlayer()
    {

    }

    private void InitObjctsInScene()
    {
        MGFObject[] obj = FindObjectsOfType<MGFObject>();
        PhysicsManager.Instance.Init(obj, GetSecneSize());
    }

    private Rectangle GetSecneSize()
    {
        Fix64Vector2 halfSize = new Fix64Vector2(0, 0);
        Fix64Vector2 center = new Fix64Vector2((Fix64)Ground.transform.position.x, (Fix64)Ground.transform.position.z);
        halfSize.X = (Fix64)X;
        halfSize.Y = (Fix64)Y;
        Rectangle rt = new Rectangle(center, halfSize);
        return rt;
    }

    public T CreateFromPool<T>(string name, Fix64Vector2 fv,Fix64 rot) where T : MGFObject
    {
        
        T obj = m_Op.CreateObj<T>(name);
        obj.IsCollisionAble = true;
        PhysicsManager.Instance.AddObject(obj);
        obj.v.Show();
        obj.MoveTo(fv);
        obj.RotateTo(rot);
        obj.MGFStart();
        return obj;
    }

    public void DestroyFromPool(MGFObject obj)
    {
        Debug.Log(((MGFBullet)obj).m_NowFrame);
        obj.v.Hide();
        obj.IsCollisionAble = false;
        m_Op.DestoryObj(obj.Tag, obj as IRecycleAble);
        obj.MGFDestroy();
        obj.MoveTo(m_RecyclePoint);
        PhysicsManager.Instance.DesObject(obj);

    }


    /// <summary>
    /// 画出四叉树以及动态物体所在节点
    /// </summary>
    private void OnDrawGizmos()
    {
        Quadtree qt = PhysicsManager.Instance.GetQuadtree();
        if (qt == null)
        {
            return;
        }
        DrawQt(qt);

        DrawDynamicObj();
    }

    private void DrawDynamicObj()
    {
        List<MGFObject> dynamicObj = PhysicsManager.Instance.GetAllDynamicObjs();
        Gizmos.color = new Color(0.5f, 0.9f, 0f, 0.3f);
        for (int i = 0; i < dynamicObj.Count; i++)
        {
            if (Quadtree.dic.ContainsKey(dynamicObj[i]) == false || Quadtree.dic[dynamicObj[i]] == null)
            {
                continue;
            }
            Fix64Vector2 center = Quadtree.dic[dynamicObj[i]].GetCenter().center;
            Fix64Vector2 halfSize = Quadtree.dic[dynamicObj[i]].GetCenter().halfSize;
            Gizmos.DrawCube(center.ToVector3(), (halfSize * 2).ToVector3());
        }
    }

    private void DrawQt(Quadtree qqt)
    {
        Gizmos.color = Color.red;
        if (qqt != null)
        {
            if (qqt.GetNodes() == null)
            {
                return;
            }
            else
            {
                if (qqt.GetNodes()[0] == null)
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
                if (qqt.GetNodes()[i] != null)
                {
                    DrawQt(qqt.GetNodes()[i]);
                }
            }
        }
    }


    /// <summary>
    /// FixedUpdate模拟帧事件
    /// </summary>
    private void FixedUpdate()
    {
        HandleCommand();
        PhysicsManager.Instance.CheckCollision();
        CollectPlayerOpt();
    }

    private void CollectPlayerOpt()
    {
        Command cm = UIJoyStick.stick;
        cm.value = JoyStick.Dir;
        m_Opts.Enqueue(cm);
        if (JoyStick.Commands.Count!=0)
        {
            for (int i = 0; i < JoyStick.Commands.Count; i++)
            {
                m_Opts.Enqueue(JoyStick.Commands.Dequeue());
            }
        }
    }

    private void HandleCommand()
    {
        HandlePlayer();
        HandleOthers();
    }

    private void HandleOthers()
    {
        List<MGFObject> obj = PhysicsManager.Instance.GetAllObjects();
        for (int i = 0; i < obj.Count; i++)
        {
            obj[i].HandleFrameEvent();
        }
    }

    private void HandlePlayer()
    {
        for(int i = 0; i < m_Opts.Count; i++)
        {
            Command cm = m_Opts.Dequeue();
            switch (cm.CmdType)
            {
                case CommandType.JoyStick:
                    Fix64 value = (Fix64)cm.value;
                    if (value != Fix64.Zero)
                    {
                        Player.IsMoving = true;
                        Player.RotateTo(value);
                        Player.Move(Player.Forward);
                    }
                    else
                    {
                        Player.IsMoving = false;
                    }
                    break;
                case CommandType.Skill:
                    Player.Skill();
                    break;
            }

        }
    }
}






