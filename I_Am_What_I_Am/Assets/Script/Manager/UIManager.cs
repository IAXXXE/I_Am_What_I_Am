using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tool.Module.Message;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Transform fadeTrans;

    void Start()
    {
        fadeTrans = transform.Find("_Fade");

        GameInstance.Connect("fade.out", OnFadeOut);
        GameInstance.Connect("fade.in", OnFadeIn);

        fadeTrans.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("fade.out", OnFadeOut);
        GameInstance.Disconnect("fade.in", OnFadeIn);
    }

    private void OnFadeOut(IMessage msg)
    {
        fadeTrans.gameObject.SetActive(true);
        fadeTrans.GetComponent<Image>().DOFade(1f, 1f);
    }
    private void OnFadeIn(IMessage msg)
    {
        fadeTrans.GetComponent<Image>().DOFade(0f, 1f);
        GameInstance.CallLater(1f, () => fadeTrans.gameObject.SetActive(false));
    }

}
