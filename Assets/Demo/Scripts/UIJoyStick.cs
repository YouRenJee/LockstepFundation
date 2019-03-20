
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIJoyStick : MonoBehaviour
{

    private Canvas m_Cs;
    public Transform Stick;
    public float MaxR = 80;
    public Queue<Command> Commands = new Queue<Command>();



    private Vector2 m_TouchDir = Vector2.up;
    private static Command skill = new Command(CommandType.Skill);
    public static Command stick = new Command(CommandType.JoyStick);
    public void OnSkillClick()
    {
        Commands.Enqueue(skill);
    }




    private float VectorAngle(Vector2 from, Vector2 to)
    {
        float angle;

        Vector3 cross = Vector3.Cross(from, to);
        angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }

    public float Dir
    {
        get
        {
            return VectorAngle(Vector2.up, m_TouchDir);
        }
    }

    public void Init()
    {
        m_Cs = GameObject.Find("UIRoot").GetComponent<Canvas>();
        Stick.localPosition = Vector2.zero;
        m_TouchDir = Vector2.up;
    }


    public void OnStickDrag()
    {
        Vector2 pos = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, Input.mousePosition, m_Cs.worldCamera, out pos);

        float len = pos.magnitude;
        if (len <= 0)
        {
            m_TouchDir = Vector2.up;
            return;
        }

        m_TouchDir.x = pos.x / len; 
        m_TouchDir.y = pos.y / len; 

        if (len >= MaxR)
        { 
            pos.x = pos.x * MaxR / len;
            pos.y = pos.y * MaxR / len;
        }

        Stick.localPosition = pos;
        

    }


    public void OnEndDrag()
    {
        Stick.localPosition = Vector2.zero;
        m_TouchDir = Vector2.up;
    }

}
