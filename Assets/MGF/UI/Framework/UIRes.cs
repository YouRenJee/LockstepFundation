using System;
using UnityEngine;

namespace MGF.Framework
{
    public static class UIRes
    {
        public static string UIResRoot = "ui/";

        /// <summary>
        /// 加载UI的Prefab
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab(string name)
        {
            GameObject asset = (GameObject)Resources.Load(UIResRoot + name);
            return asset;
        }
    }
}