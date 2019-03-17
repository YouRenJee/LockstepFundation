using MGF.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGF.Framework.Example
{
    public class UIExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            UIManager.Instance.Init("ui/");
            UIManager.MainPage = "UIPage1";
            UIManager.Instance.EnterMainPage();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

