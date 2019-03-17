using MGF;
using System;
using UnityEngine.Events;

namespace MGF.Framework
{

    public abstract class Module
    {
        /// <summary>
        /// 调用它以释放模块
        /// </summary>
        public virtual void Release()
        {
            this.Log("Release");
        }
    }
}
