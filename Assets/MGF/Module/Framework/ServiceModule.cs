using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGF.Framework
{
    public abstract class ServiceModule<T> : Module where T : ServiceModule<T>, new()
    {
        private static T ms_instance = default(T);

        public static T Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new T();
                }
                return ms_instance;
            }

        }

        //public ServiceModule()
        //{

        //    var exp = new Exception("ServiceModule<" + typeof(T).Name + "> 无法直接实例化，因为它是一个单例!");
        //    throw exp;

        //}


    }
}
