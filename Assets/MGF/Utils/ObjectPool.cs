using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool
{
    private static Dictionary<string, Queue<IRecycleAble>> dic = new Dictionary<string, Queue<IRecycleAble>>();

    public static void AddNewClass(string name, Queue<IRecycleAble>ls) 
    {
        if (!dic.ContainsKey(name))
        {
            dic.Add(name, ls);
        }
    }

    public static void DestoryObj(string name, IRecycleAble o) 
    {
        if (dic.ContainsKey(name))
        {
            dic[name].Enqueue(o);
            o.OnDestory();
        }
    }

    public static T CreateObj<T> (string name) where T : MGFObject
    {
        if (dic.ContainsKey(name))
        {
            if (dic[name].Count > 0)
            {

                IRecycleAble ls = dic[name].Dequeue();
                ls.OnCreate();
                return (T)ls;
            }
        }

        return null;

    }

}
