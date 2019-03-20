using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGF.Framework;

public class CoroutineTool : UnitySingletom<CoroutineTool>
{
    public void Coroutine()
    {
        StartCoroutine(ICoroutine());
    }


    IEnumerator ICoroutine()
    {
        yield return null;
    }
}
