using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Framework;
using System;
using MGF;

namespace MGF.Framework.Example
{
    public class ModuleExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            ModuleManager.Instance.Init("MGF.Framework");
            ModuleManager.Instance.CreateModule("ModuleA");
            ModuleManager.Instance.CreateModule("ModuleB");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }



    public class ModuleA : BusinessModule
    {
        public override void Create(object args = null)
        {
            base.Create(args);



            //业务层通过ModuleManager通信
            ModuleManager.Instance.Event("ModuleB", "onModuleEventB").AddListener(OnModuleEventB);

            //业务层调用服务层 用过事件监听回调
            ModuleC.Instance.OnEvent.AddListener(OnModuleEventC);
            ModuleC.Instance.DoSomething();

        }

        private void OnModuleEventC(object arg0)
        {
            this.Log("ModuleA Invoke Service ModuleC");
        }

        private void OnModuleEventB(object arg0)
        {
            this.Log("ModuleB Receive Event From ModuleA: " + arg0);
        }
    }


    public class ModuleB : BusinessModule
    {
        public ModuleEvent onModuleEventB { get { return Event("onModuleEventB"); } }

        public override void Create(object args = null)
        {
            base.Create(args);
            onModuleEventB.Invoke("aaaa");
        }

    }



    public class ModuleC : ServiceModule<ModuleC>
    {
        public ModuleEvent OnEvent = new ModuleEvent();

        public void DoSomething()
        {
            OnEvent.Invoke(null);
        }
    }
}
