using System;
using System.Collections;
using System.Collections.Generic;
using Tool.Module.Message;
using Unity.VisualScripting;
using UnityEngine;

public enum ActionType
{
    Normal,
    Attack,
    Avoid,
    BeHit,
}

public enum PlayerStat
{
    Normal,
    Wait,
    BeHit,
    Avoid,
}

public class PlayerController : MonoBehaviour
{
    public List<GameObject> statList = new();

    public PlayerStat currStat = PlayerStat.Normal;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            statList.Add(child.gameObject);
        }
        GameInstance.Connect("player.action", OnPlayerAction);
        GameInstance.Connect("enemy.attack", OnEnemyAttack);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("player.action", OnPlayerAction);
        GameInstance.Disconnect("enemy.attack", OnEnemyAttack);
    }

    private void OnPlayerAction(IMessage msg)
    {
        if(currStat != PlayerStat.Normal) return;

        var actionType = (ActionType)msg.Data;
        Debug.Log(actionType);
        if(actionType == ActionType.Attack)
        {
            GameInstance.Signal("player.attack");
        }
        else if(actionType == ActionType.Avoid)
        {
            Avoid();
        }

        UpdateImage(actionType);
        GameInstance.CallLater(0.3f, () => UpdateImage(ActionType.Normal));

    }

    private void OnEnemyAttack(IMessage msg)
    {
        if(currStat == PlayerStat.Avoid) return;

        BeHit();
    }

    private void Avoid()
    {
        currStat = PlayerStat.Avoid;
        GameInstance.CallLater(0.3f, () => currStat = PlayerStat.Normal);
    }

    private void BeHit()
    {
        GameInstance.Signal("score.up", "enemy");
        GameInstance.Signal("camera.shake");
        UpdateImage(ActionType.BeHit);
        currStat = PlayerStat.BeHit;

        GameInstance.CallLater(0.3f, () => {
            UpdateImage(ActionType.Normal);
            currStat = PlayerStat.Normal;
        });
    }

    private void UpdateImage(ActionType actionType)
    {
        for(int i = 0; i < statList.Count; i++)
        {
            if(statList[i].name == "_" + actionType.ToString()) statList[i].SetActive(true);
            else statList[i].SetActive(false);
        }

    }
}
