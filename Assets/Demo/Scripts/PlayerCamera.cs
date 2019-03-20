using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private bool IsInit = false;

    public Transform Player;

    private Vector3 _offset;
    public void Init()
    {
        IsInit = true;
        _offset = new Vector3(0,15,-12);
        transform.position = Vector3.Lerp(transform.position, Player.position + _offset, Time.deltaTime * 10);
    }

    private void LateUpdate()
    {
        if (IsInit==false)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, Player.position + _offset, Time.deltaTime * 10);
        
    }


}
