
using System;
using System.Diagnostics;
using System.IO;

namespace MGF
{
    public class Debugger
    {
        
        public static bool EnableLog = true;
        public static bool EnableTime = false;
        public static bool EnableSave = false;
        public static bool EnableStack = false;
        public static bool EnablePrefix = false;
        public static string LogFileDir = UnityEngine.Application.dataPath+"/Log/";
        public static string LogFileName = "";
        private static string Prefix = "> ";
        public static bool UseUnityEngine = true;


        public static void Init()
        {
            if (!UseUnityEngine)
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                LogFileDir = path + "/DebuggerLog/";
            }
        }
        
        private static void Internal_Log(string msg, object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.Log(msg, (UnityEngine.Object)context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private static void Internal_LogWarning(string msg, object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.LogWarning(msg, (UnityEngine.Object)context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private static void Internal_LogError(string msg, object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.LogError(msg, (UnityEngine.Object)context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }


        //----------------------------------------------------------------------
        [Conditional("ENABLE_LOG")]
        public static void Log(string tag, string methodName, string message = "")
        {

            message = GetLogText(tag, methodName, message);
            Internal_Log(message);
            LogToFile("[I]" + message);
        }

        [Conditional("ENABLE_LOG")]
        public static void Log(string tag, string methodName, string format, params object[] args)
        {
            string message = GetLogText(tag, methodName, string.Format(format, args));
            //string message = string.Format(format, args);
            Internal_Log(message);
            LogToFile("[I]" + message);
        }

        public static void LogError(string tag, string methodName, string message)
        {
            message = GetLogText(tag, methodName, message);
            Internal_LogError(Prefix + message);
            LogToFile("[E]" + message,true);
        }

        public static void LogError(string tag, string methodName, string format, params object[] args)
        {
            string message = GetLogText(tag, methodName, string.Format(format, args));
            Internal_LogError(Prefix + message);
            LogToFile("[E]" + message,true);
        }


        public static void LogWarning(string tag, string methodName, string message)
        {
            message = GetLogText(tag, methodName, message);
            Internal_LogWarning(Prefix + message);
            LogToFile("[W]" + message);
        }

        public static void LogWarning(string tag, string methodName, string format, params object[] args)
        {
            string message = GetLogText(tag, methodName, string.Format(format, args));
            Internal_LogWarning(Prefix + message);
            LogToFile("[W]" + message);
        }



        //----------------------------------------------------------------------
        private static string GetLogText(string tag, string methodName, string message)
        {
            string str = "";
            if (Debugger.EnableTime)
            {
                DateTime now = DateTime.Now;
                str = now.ToString("HH:mm:ss.fff") + "     ";
            }
            if (Debugger.EnablePrefix)
            {
                str = Prefix +str + tag + "::" + methodName + "() ";
            }
           
            return str + message;
        }



        //----------------------------------------------------------------------
        internal static string CheckLogFileDir()
        {
            if (string.IsNullOrEmpty(LogFileDir))
            {
                //该行代码无法在线程中执行！
                try
                {
                    if (UseUnityEngine)
                    {
                        LogFileDir = UnityEngine.Application.persistentDataPath + "/DebuggerLog/";
                    }
                    else
                    {
                        string path = System.AppDomain.CurrentDomain.BaseDirectory;
                        LogFileDir = path + "/DebuggerLog/";
                    }
                }
                catch (Exception e)
                {
                    Internal_LogError("Debugger::CheckLogFileDir() " + e.Message + e.StackTrace);
                    return "";
                }
            }

            try
            {
                if (!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }
            }
            catch (Exception e)
            {
                Internal_LogError("Debugger::CheckLogFileDir() " + e.Message + e.StackTrace);
                return "";
            }

            return LogFileDir;
        }

        internal static string GenLogFileName()
        {
            DateTime now = DateTime.Now;
            string filename = now.GetDateTimeFormats('s')[0].ToString();//2005-11-05T14:06:25
            filename = filename.Replace("-", "_");
            filename = filename.Replace(":", "_");
            filename = filename.Replace(" ", "");
            filename += ".log";

            return filename;
        }

        private static void LogToFile(string message, bool EnableStack = false)
        {
            if (!EnableSave)
            {
                return;
            }


            LogFileName = GenLogFileName();
            LogFileDir = CheckLogFileDir();


            string fullpath = LogFileDir + LogFileName;

           
            using (StreamWriter sw = new StreamWriter(fullpath,true))
            {
                sw.WriteLine(message);
                if ((EnableStack || Debugger.EnableStack) && UseUnityEngine)
                {
                    sw.WriteLine(UnityEngine.StackTraceUtility.ExtractStackTrace());
                }
            }
        }
    }
}
