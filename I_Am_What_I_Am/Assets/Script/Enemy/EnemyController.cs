using System;
using System.Collections;
using System.Collections.Generic;
using Tool.Module.Message;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public string name;

    public List<GameObject> statList = new();

   // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            statList.Add(child.gameObject);
        }
        GameInstance.Connect("enemy.action", OnEnemyAction);

        GameInstance.Connect("player.attack", OnPlayerAttack);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("enemy.action", OnEnemyAction);

        GameInstance.Disconnect("player.attack", OnPlayerAttack);
    }

    private void OnPlayerAttack(IMessage msg)
    {
        var random = UnityEngine.Random.Range(0, 10);
        if(random <= 3)
        {
            UpdateImage(ActionType.Avoid);
        }
        else
        {
            BeHit();
        }

        GameInstance.CallLater(0.2f, () => UpdateImage(ActionType.Normal));

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        var random = UnityEngine.Random.Range(0.5f, 1.5f);

        yield return new WaitForSeconds(random);

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

    private void BeHit()
    {
        GameInstance.Signal("score.up", "player");
        GameInstance.Signal("camera.shake");
        UpdateImage(ActionType.BeHit);
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
