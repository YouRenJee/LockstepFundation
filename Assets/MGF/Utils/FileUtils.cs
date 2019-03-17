
using MGF;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace MGF.Utils
{
    public class FileUtils
    {
        private const string LOG_TAG = "FileUtils";

        public static byte[] ReadFile(string fullpath)
        {
            byte[] buffer = null;
            if (File.Exists(fullpath))
            {
                FileStream fs = null;
                try
                {
                    using (fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                    }

                }
                catch (Exception e)
                {
                    Debugger.LogError(LOG_TAG, "ReadFile() Path:{0}, Error:{1}", fullpath, e.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            else
            {
                Debugger.LogError(LOG_TAG, "ReadFile() File is Not Exist: {0}", fullpath);
            }
            return buffer;
        }

        public static string ReadString(string fullpath)
        {
            byte[] buffer = ReadFile(fullpath);
            if (buffer != null)
            {
                return Encoding.UTF8.GetString(buffer);
            }
            return "";
        }

        public static string ReadStringASCII(string fullpath)
        {
            byte[] buffer = ReadFile(fullpath);
            if (buffer != null)
            {
                return Encoding.ASCII.GetString(buffer);
            }
            return "";
        }

        public static int SaveFile(string fullpath, byte[] content)
        {
            if (content == null)
            {
                content = new byte[0];
            }

            string dir = PathUtils.GetParentDir(fullpath);

            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception e)
                {
                    Debugger.LogError(LOG_TAG, "SaveFile() CreateDirectory Error! Dir:{0}, Error:{1}", dir, e.Message);
                    return -1;
                }

            }

            FileStream fs = null;
            try
            {
                using (fs = new FileStream(fullpath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(content, 0, content.Length);
                }
                    
            }
            catch (Exception e)
            {
                Debugger.LogError(LOG_TAG, "SaveFile() Path:{0}, Error:{1}", fullpath, e.Message);
                fs.Close();
                fs.Dispose();
                return -1;
            }
            return content.Length;
        }

        public static int SaveFile(string fullpath, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            return SaveFile(fullpath, buffer);
        }

        public static byte[] SerializeToBinary(object obj)
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, obj);
                data = stream.ToArray();
            }
            return data;
        }

        public static T DeserializeWithBinary<T>(byte[] data)
        {
            T obj;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                BinaryFormatter bf = new BinaryFormatter();
                obj = (T)bf.Deserialize(stream);
            }
            return obj;
        }
    }
}

