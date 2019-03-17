using System.Reflection;

namespace MGF.Framework
{
    public abstract class BusinessModule : Module
    {
        private string m_name = null;

        public string Name
        {
            get
            {
                if (m_name == null)
                {
                    m_name = this.GetType().Name;
                }
                return m_name;
            }
        }

        public string Title;

        public BusinessModule()
        {

        }

        internal BusinessModule(string name)
        {
            m_name = name;
        }



        private EventTable m_tblEvent;

        /// <summary>
        /// 实现抽象事件功能
        /// 可以像这样使用：obj.Event("onLogin").AddListener(...)        
        /// 事件的发送方法：this.Event("onLogin").Invoke(args)
        /// 而不需要在编码时先定义好，以提高模块的抽象程度
        /// 但是在模块内部的类不应该过于抽象，比如数据发生更新了，
        /// 在UI类这样使用：obj.onUpdate.AddListener(...)
        /// 这两种方法在使用形式上，保持了一致性！
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ModuleEvent Event(string type)
        {
            return GetEventTable().GetEvent(type);
        }

        internal void SetEventTable(EventTable mgrEvent)
        {
            m_tblEvent = mgrEvent;
        }

        protected EventTable GetEventTable()
        {
            if (m_tblEvent == null)
            {
                m_tblEvent = new EventTable();
            }
            return m_tblEvent;
        }



        /// <summary>
        /// 调用它以创建模块
        /// </summary>
        /// <param name="args"></param>
        public virtual void Create(object args = null)
        {
            this.Log("Create() args = " + args);

        }


        /// <summary>
        /// 调用它以释放模块
        /// </summary>
        public override void Release()
        {
            if (m_tblEvent != null)
            {
                m_tblEvent.Clear();
                m_tblEvent = null;
            }

            base.Release();
        }


        /// <summary>
        /// 当模块收到消息后，对消息进行一些处理
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        internal void HandleMessage(string msg, object[] args)
        {
            this.Log("HandleMessage() msg:{0}, args:{1}", msg, args);

            MethodInfo mi = this.GetType().GetMethod(msg, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null)
            {
                mi.Invoke(this, BindingFlags.NonPublic, null, args, null);
            }
            else
            {
                OnModuleMessage(msg, args);
            }
        }


        /// <summary>
        /// 由派生类去实现，用于处理消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        protected virtual void OnModuleMessage(string msg, object[] args)
        {
            this.Log("OnModuleMessage() msg:{0}, args:{1}", msg, args);
        }


        /// <summary>
        /// 显示业务模块的主UI
        /// 一般业务模块都有UI，这是游戏业务模块的特点
        /// </summary>
        protected virtual void Show(object arg)
        {
            this.Log("Show() arg:{0}", arg);
        }
    }

}
