using System;
using System.Collections;
using System.Collections.Generic;
using Tool.Module.Message;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CursorStat
{
    Normal,
    Down,
    Up
}

public class Cursor : MonoBehaviour
{
    public Camera mainCamera;
    public Vector2 offset = new Vector2(0, 0);

    private CursorStat currStat;
    private Transform trail;
    private Image cursorImage;

    private Vector3 downPos;
    private Vector3 upPos;

    // Start is called before the first frame update
    void Start()
    {
        currStat  = CursorStat.Normal;

        GameInstance.Connect("stat.change", OnStatChange);

        trail = transform.Find("_Trail");
        cursorImage = transform.Find("_Image").GetComponent<Image>();
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("stat.change", OnStatChange);
    }

    private void OnStatChange(IMessage msg)
    {
        currStat = (CursorStat)msg.Data;
    }

    void Update()
    {
        // transform
        if(Input.GetMouseButtonDown(0))
        {
            downPos = Input.mousePosition;
            trail.gameObject.SetActive(true);
        }

        if(Input.GetMouseButtonUp(0))
        {
            upPos = Input.mousePosition;
            trail.gameObject.SetActive(false);

            if(upPos.x < downPos.x) DoAction(ActionType.Attack);
            else DoAction(ActionType.Avoid);

            trail.GetComponent<TrailRenderer>().Clear();
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        var pos = new Vector3(targetPosition.x + offset.x, targetPosition.y + offset.y, targetPosition.z);
        transform.position = pos;
    }

    private void DoAction(ActionType actionType)
    {
        GameInstance.Signal("player.action", actionType);
    }
}
