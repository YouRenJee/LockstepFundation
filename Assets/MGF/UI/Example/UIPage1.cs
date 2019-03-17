using MGF.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MGF.Framework.Example
{
    public class UIPage1 : UIPage
    {
        protected override void OnClose(object arg = null)
        {
            base.OnClose(arg);
        }

        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);
        }

        public void OnBtnOpenPage2()
        {
            UIManager.Instance.OpenPage("UIPage2");
        }
    }
}

