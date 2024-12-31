using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tool.Module.Message;
using UnityEngine;

public enum ActionType
{
    Normal,
    PreAttack,
    Attack,
    Avoid,
    BeHit,
    Lose
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

    public bool isBack = false;

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
        if(actionType == ActionType.Attack)
        {
            if(CombatManager.Instance.currTurn != CombatTurn.Player && isBack != true) return;
            GameInstance.Signal("player.attack");
            GameInstance.CallLater(0.3f, () => UpdateImage(ActionType.Normal));
        }
        else if(actionType == ActionType.Avoid)
        {
            Avoid();
        }

        UpdateImage(actionType);
    }

    private void OnEnemyAttack(IMessage msg)
    {
        if(currStat == PlayerStat.Avoid) 
        {
            GameInstance.Instance.audioManager.PlayAudio("def");
            currStat = PlayerStat.Normal;
            isBack = true;
            GameInstance.CallLater(2f, () => isBack = false);
            return;
        }
        BeHit();
    }

    private void Avoid()
    {
        currStat = PlayerStat.Avoid;
        GameInstance.CallLater(0.5f, () => currStat = PlayerStat.Normal);
        GameInstance.CallLater(0.5f, () => UpdateImage(ActionType.Normal));
    }

    private void BeHit()
    {
        GameInstance.Signal("score.up", "enemy");
        GameInstance.Signal("camera.shake");
        UpdateImage(ActionType.BeHit);
        currStat = PlayerStat.BeHit;
        GameInstance.Instance.audioManager.PlayAudio("hit");

        if(CombatManager.Instance.enemyScore >= 10)
        {
            Lose();
            return;
        }

        GameInstance.CallLater(0.3f, () => {
            UpdateImage(ActionType.Normal);
            currStat = PlayerStat.Normal;
        });
    }

    private void Lose()
    {
        // StopAllCoroutines();
        UpdateImage(ActionType.Lose);

        transform.DOLocalMoveY(-7f, 1f);
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
