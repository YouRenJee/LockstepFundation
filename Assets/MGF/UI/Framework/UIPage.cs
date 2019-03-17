using UnityEngine;
using UnityEngine.UI;

namespace MGF.Framework
{
    public class UIPage : UIPanel
    {
        /// <summary>
        /// 返回按钮，大部分Page都会有返回按钮
        /// </summary>
        [SerializeField]
        private Button m_btnGoBack;

        /// <summary>
        /// 打开UI的参数
        /// </summary>
        protected object m_openArg;

        /// <summary>
        /// 该UI的当前实例是否曾经被打开过
        /// </summary>
        private bool m_isOpenedOnce;

        /// <summary>
        /// 当UIPage被激活时调用
        /// </summary>
        protected void OnEnable()
        {
            this.Log("OnEnable()");
            if (m_btnGoBack != null)
            {
                m_btnGoBack.onClick.AddListener(OnBtnGoBack);
            }

#if UNITY_EDITOR
            if (m_isOpenedOnce)
            {
                //如果UI曾经被打开过，
                //则可以通过UnityEditor来快速触发Open/Close操作
                //方便调试
                OnOpen(m_openArg);
            }
#endif
        }

        /// <summary>
        /// 当UI不可用时调用
        /// </summary>
        protected void OnDisable()
        {
            this.Log("OnDisable()");
#if UNITY_EDITOR
            if (m_isOpenedOnce)
            {
                //如果UI曾经被打开过，
                //则可以通过UnityEditor来快速触发Open/Close操作
                //方便调试
                OnClose();
            }
#endif
            if (m_btnGoBack != null)
            {
                m_btnGoBack.onClick.RemoveAllListeners();
            }
        }


        /// <summary>
        /// 当点击“返回”时调用
        /// 但是并不是每一个Page都有返回按钮
        /// </summary>
        private void OnBtnGoBack()
        {
            this.Log("OnBtnGoBack()");
            UIManager.Instance.GoBackPage();
        }

        /// <summary>
        /// 调用它打开UIPage
        /// </summary>
        /// <param name="arg"></param>
        public sealed override void Open(object arg = null)
        {
            this.Log("Open()");
            m_openArg = arg;
            m_isOpenedOnce = false;

            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }

            OnOpen(arg);
            m_isOpenedOnce = true;
        }

        public sealed override void Close(object arg = null)
        {
            this.Log("Close()");
            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }

            OnClose(arg);
        }

    }
}