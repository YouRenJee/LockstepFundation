using System;
using MGF.Math;
using UnityEngine;


public class MGFView : MGFComponet
{
    protected MGFObject m_Obj;

    internal override string Tag { get { return "MGFView"; }}

    internal override void Init()
    {
        m_Obj = GetComponent<MGFObject>();
    }

    public void MoveTo()
    {
        transform.position = m_Obj.tr.Pos.ToVector3Zero();
    }

    public void Rot()
    {
        transform.rotation = Quaternion.Euler(0, (float)m_Obj.tr.Rot, 0);
    }




    protected virtual void DefaultViewBehaviour()
    {
        if (m_Obj == null || m_Obj.IsStatic)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, m_Obj.tr.Pos.ToVector3Zero(), Time.deltaTime * 20);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, (float)m_Obj.tr.Rot, 0), Time.deltaTime * 10);
    }

    private void Update()
    {
        DefaultViewBehaviour();
    }


    public void Hide()
    {
        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            item.enabled = false;
        }

    }

    public void Show()
    {
        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            item.enabled = true;
        }
    }

}
