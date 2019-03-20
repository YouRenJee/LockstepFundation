using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MGFView
{
    Animator m_anim;

    internal override void Init()
    {
        base.Init();
        m_anim = GetComponent<Animator>();
    }

    public void Skill()
    {
        m_anim.SetTrigger("Attack");
    }

    protected override void DefaultViewBehaviour()
    {
        base.DefaultViewBehaviour();

        if (m_Obj.IsMoving)
        {
            m_anim.SetBool("IdleToWalk", true);
        }
        else
        {
            m_anim.SetBool("IdleToWalk", false);
        }

    }

    private void Update()
    {
        DefaultViewBehaviour();
    }



}
