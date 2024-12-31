using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tool.Module.Message;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public string name;

    public Sprite normal;
    public Sprite injury;

    public List<GameObject> statList = new();

    private Vector3 initPos = new Vector3(-4f, -1, 0);
    private SpriteRenderer enemySprite;

   // Start is called before the first frame update
    void Start()
    {
        enemySprite = transform.Find("_Normal").GetComponent<SpriteRenderer>();

        foreach(Transform child in transform)
        {
            statList.Add(child.gameObject);
        }
        GameInstance.Connect("enemy.init", OnEnemyInit);
        GameInstance.Connect("enemy.action", OnEnemyAction);
        GameInstance.Connect("enemy.injury", OnEnemyInjury);

        GameInstance.Connect("enemy.pre_atk", OnEnemyPreAtk);

        GameInstance.Connect("player.attack", OnPlayerAttack);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("enemy.init", OnEnemyInit);
        GameInstance.Disconnect("enemy.action", OnEnemyAction);
        GameInstance.Disconnect("enemy.injury", OnEnemyInjury);

        GameInstance.Disconnect("enemy.pre_atk", OnEnemyPreAtk);

        GameInstance.Disconnect("player.attack", OnPlayerAttack);
    }

    private void OnEnemyPreAtk(IMessage msg)
    {
        var time = (float)msg.Data;

        StartCoroutine(Attack(time));
    }

    private void OnEnemyInit(IMessage msg)
    {
        Debug.Log("init");
        transform.localPosition = initPos;
        enemySprite.sprite = normal;

        UpdateImage(ActionType.Normal);
    }

    private void OnEnemyInjury(IMessage msg)
    {
        var isInjury = (bool)msg.Data;
        if(isInjury) enemySprite.sprite = injury;
        else enemySprite.sprite = normal;
    }

    private void OnPlayerAttack(IMessage msg)
    {
        var random = UnityEngine.Random.Range(0, 10);
        switch(GameInstance.Instance.mode)
        {
        case Mode.Easy:
            if(random < 9)  BeHit();
            else Avoid();
            break;
        case Mode.Normal:
            if(random < CombatManager.Instance.splitRatio * 12 + 1 || CombatManager.Instance.currTurn == CombatTurn.Enemy )
            {
                BeHit();
            }
            else
            {
                Avoid();
            }
            break;
        case Mode.Hard:
            if(random < CombatManager.Instance.splitRatio * 10 + 1 || CombatManager.Instance.currTurn == CombatTurn.Enemy )
            {
                BeHit();
            }
            else
            {
                Avoid();
            }
            break;
        }
        
    }

    private IEnumerator Attack(float time)
    {
        yield return new WaitForSeconds(time);
        if(!GameInstance.Instance.inCombat) yield break;

        if(CombatManager.Instance.currTurn == CombatTurn.Enemy)
        {
            UpdateImage(ActionType.PreAttack);
            yield return new WaitForSeconds(0.5f);
        }

        UpdateImage(ActionType.Attack);
        GameInstance.CallLater(0.2f, () => UpdateImage(ActionType.Normal));
        GameInstance.Signal("enemy.attack");

        yield break;
    }

    private void OnEnemyAction(IMessage msg)
    {
        var actionType = (ActionType)msg.Data;
        Debug.Log("enemy" + actionType);
        
        UpdateImage(actionType);

    }

    private void Avoid()
    {
        UpdateImage(ActionType.Avoid);
        GameInstance.Instance.audioManager.PlayAudio("avoid");
        GameInstance.CallLater(0.2f, () => UpdateImage(ActionType.Normal));

        switch(GameInstance.Instance.mode)
        {
        case Mode.Normal:
            var random = UnityEngine.Random.Range(0, 10);
            if(random > 5) StartCoroutine(Attack(0.5f));
            break;
        case Mode.Hard:
            StartCoroutine(Attack(0.5f));
            break;
        }
        
    }

    private void BeHit()
    {
        GameInstance.Signal("score.up", "player");
        GameInstance.Signal("camera.shake");
        GameInstance.Instance.audioManager.PlayAudio("hit");
        if(CombatManager.Instance.playerScore >= 10)
        {
            Lose();
            return;
        }
        UpdateImage(ActionType.BeHit);
        GameInstance.CallLater(0.2f, () => UpdateImage(ActionType.Normal));
    }

    private void Lose()
    {
        // StopAllCoroutines();
        UpdateImage(ActionType.Lose);
        transform.DOLocalMoveX(-5.5f, 2f);
        transform.DOLocalMoveY(-15f, 0.5f).SetDelay(2f);
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
