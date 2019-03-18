using MGF.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectPool
{

    private Dictionary<string, Queue<IRecycleAble>> rcDic = new Dictionary<string, Queue<IRecycleAble>>();
    
    /// <summary>
    /// 增加一种新的对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rc"></param>
    public void AddNewObj(string name, Queue<IRecycleAble> rc)
    {
        if (!rcDic.ContainsKey(name))
        {
            rcDic.Add(name, rc);
        }
        else
        {
            for (int i = 0; i < rc.Count; i++)
            {
                rcDic[name].Enqueue(rc.Dequeue());
            }
        }
    }


    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rc"></param>
    public void DestoryObj(string name, IRecycleAble rc)
    {
        Assert.IsTrue(rcDic.ContainsKey(name));

        rcDic[name].Enqueue(rc);
        rc.OnDestory();

    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T CreateObj<T>(string name) where T : MGFObject
    {
        Assert.IsTrue(rcDic.ContainsKey(name));
        if (rcDic[name].Count > 0)
        {
            IRecycleAble ls = rcDic[name].Dequeue();
            ls.OnCreate();
            return (T)ls;
        }
        else
        {
            throw new Exception("no obj exist!!!");
        }

    }

}
