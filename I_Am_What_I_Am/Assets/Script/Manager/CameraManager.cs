using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tool.Module.Message;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Vector3 initPos;
    void Awake()
    {
        initPos = transform.position;
    }

    void Start()
    {
        GameInstance.Connect("camera.shake", OnCameraShake);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("camera.shake", OnCameraShake);
    }

    private void OnCameraShake(IMessage msg)
    {
        transform.DOShakePosition(0.1f, 1f);
        GameInstance.CallLater(0.1f, ()=> transform.position = initPos);
        
    }
}
