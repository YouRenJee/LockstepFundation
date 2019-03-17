
using MGF.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MGF.Framework
{
    public class UIManager : ServiceModule<UIManager>
    {
        public const string LOG_TAG = "UIManager";

        public static string MainScene = "Main";
        public static string MainPage = "UIHomePage";




        class UIPageTrack
        {
            public string name;
            public string scene;
        }


        private Stack<UIPageTrack> m_pageTrackStack;
        private UIPageTrack m_currentPage;
        private Action<string> sceneLoaded;
        private List<UIPanel> m_listLoadedPanel;

        public UIManager()
        {
            m_pageTrackStack = new Stack<UIPageTrack>();
            m_listLoadedPanel = new List<UIPanel>();
        }

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="uiResRoot">UI资源的根目录，默认为"ui/"</param>
        public void Init(string uiResRoot)
        {
            

            UIRes.UIResRoot = uiResRoot;

            //监听UnityScene加载事件
            SceneManager.sceneLoaded += (scene, mode) =>
            {

                sceneLoaded(scene.name);
              
            };
        }


        /// <summary>
        /// 加载UI，如果UIRoot下已经有了，则直接取UIRoot下的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        private T Load<T>(string name) where T : UIPanel
        {
            string[] s = name.Split('/');
            T ui = UIRoot.Find<T>(s[s.Length-1]);
            if (ui == null)
            {
                GameObject original = UIRes.LoadPrefab(name);
                if (original != null)
                {
                    GameObject go = GameObject.Instantiate(original);
                    ui = go.GetComponent<T>();
                    if (ui != null)
                    {
                        go.name = s[s.Length - 1];
                        UIRoot.AddChild(ui);
                    }
                    else
                    {
                        this.LogError("Load() Prefab没有增加对应组件: " + name);
                    }
                }
                else
                {
                    this.LogError("Load() Res Not Found: " + name);
                }
            }

            if (ui != null)
            {
                if (m_listLoadedPanel.IndexOf(ui) < 0)
                {
                    m_listLoadedPanel.Add(ui);
                }
            }

            return ui;
        }

        private T Open<T>(string name, object arg = null) where T : UIPanel
        {
            T ui = Load<T>(name);
            if (ui != null)
            {
                ui.Open(arg);
            }
            else
            {
                this.LogError("Open() Failed! Name:{0}", name);
            }
            return ui;
        }

        private void CloseAllLoadedPanels()
        {
            for (int i = 0; i < m_listLoadedPanel.Count; i++)
            {
                if (m_listLoadedPanel[i] == null)
                {
                    m_listLoadedPanel.RemoveAt(i);
                    i--;
                    continue;
                }
                if (m_listLoadedPanel[i].IsOpen)
                {
                    m_listLoadedPanel[i].Close();
                   
                }
                
            }
        }

        //=======================================================================

        /// <summary>
        /// 进入主Page
        /// 会清空Page堆栈
        /// </summary>
        public void EnterMainPage()
        {
            m_pageTrackStack.Clear();
            OpenPageWorker(MainScene, MainPage, null);
        }


        //=======================================================================
        #region UIPage管理
        /// <summary>
        /// 打开Page
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="page"></param>
        /// <param name="arg"></param>
        public void OpenPage(string scene, string page, object arg = null)
        {
            //this.Log(LOG_TAG, "OpenPage() scene:{0}, page:{1}, arg:{2} ", scene, page, arg);

            if (m_currentPage != null)
            {
                m_pageTrackStack.Push(m_currentPage);
            }

            OpenPageWorker(scene, page, arg);
        }

        public void OpenPage(string page, object arg = null)
        {
            this.OpenPage(MainScene, page, arg);
        }

        /// <summary>
        /// 返回上一个Page
        /// </summary>
        public void GoBackPage()
        {
            //this.Log(LOG_TAG, "GoBackPage()");
            if (m_pageTrackStack.Count > 0)
            {
                var track = m_pageTrackStack.Pop();
                OpenPageWorker(track.scene, track.name, null);
            }
            else if (m_pageTrackStack.Count == 0)
            {
                EnterMainPage();
            }
        }

        private void OpenPageWorker(string scene, string page, object arg)
        {
           //this.Log(LOG_TAG, "OpenPageWorker() scene:{0}, page:{1}, arg:{2} ", scene, page, arg);

            string oldScene = SceneManager.GetActiveScene().name;

            m_currentPage = new UIPageTrack();
            m_currentPage.scene = scene;
            m_currentPage.name = page;

            //关闭当前Page时打开的所有UI
            CloseAllLoadedPanels();


            if (oldScene == scene)
            {
                Open<UIPage>(page, arg);
            }
            else
            {
                sceneLoaded = (sceneName) =>
                {
                    if (sceneName == scene)
                    {
                        sceneLoaded = null;
                        Open<UIPage>(page, arg);
                    }
                };
                SceneManager.LoadScene(scene);
            }
        }



        #endregion

        //=======================================================================

        #region UIWindow管理

        public UIWindow OpenWindow(string name, object arg = null)
        {
            UIWindow ui = Open<UIWindow>(name, arg);
            return ui;
        }


        #endregion

        //=======================================================================

        #region UIWidget管理

        public UIWidget OpenWidget(string name, object arg = null)
        {
            UIWidget ui = Open<UIWidget>(name, arg);
            return ui;
        }



        #endregion
    }
}


