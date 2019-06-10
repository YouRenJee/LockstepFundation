using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MGFComponet : MonoBehaviour
{
    internal virtual void Init()
    {
    }

    internal virtual void MGFEnable()
    {

    }

    internal virtual void MGFDestroy()
    { }

    internal virtual string Tag => this.name;
}
