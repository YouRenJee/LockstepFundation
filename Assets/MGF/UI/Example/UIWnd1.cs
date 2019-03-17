using MGF;
using MGF.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MGF.Framework.Example
{
    public class UIWnd1 : UIWindow
    {
        public void OnBtnOpenWnd1()
        {
            UIManager.Instance.OpenWindow("UIWnd1").onClose += OnWnd1Close;
        }

        private void OnWnd1Close(object arg)
        {
            //this.Log("OnWnd1Close()");
        }

        public void OnBtnOpenWidget1()
        {
            UIManager.Instance.OpenWidget("UIWidget1");
        }
    }
}

