using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DG.Tweening;
using Tool.Module.Message;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public bool inCombat;

    // public Transform playerLine;
    // public Transform enemyLine;
    public Material barMaterial;

    public Transform playerIcon;
    public Transform enemyIcon;
    public Transform resultTrans;

    private int playerScore;
    private int enemyScore;


    private string enemyID;

    // Start is called before the first frame update
    void Start()
    {
        inCombat = true;

        InitCombat();
        GameInstance.Connect("score.up", OnScoreUp);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("score.up", OnScoreUp);
    }

    private void OnScoreUp(IMessage msg)
    {
        var id = (string)msg.Data;

        if(id == "player")
        {
            playerScore ++;
            playerIcon.DOShakeScale(1.2f, 0.3f);
        }
        else
        {
            enemyScore ++;
            enemyIcon.DOShakeScale(1.2f, 0.3f);
        }

        UpdateBar();
        Debug.Log("pp : " + playerScore + " ep : " + enemyScore);
        CheckScore();
    }

    public void UpdateBar()
    {
        float splitRatio = ((float)enemyScore) / (playerScore + enemyScore);
        Debug.Log("split : " + splitRatio);
        barMaterial.SetFloat("_SplitRatio", splitRatio);
    }

    public void CheckScore()
    {
        if(playerScore >= 10)
        {
            inCombat = false;
            resultTrans.gameObject.SetActive(true);
            resultTrans.Find("_Win").gameObject.SetActive(true);
            resultTrans.Find("_Lose").gameObject.SetActive(false);
        }
        else if (enemyScore >= 10)
        {
            inCombat = false;
            resultTrans.gameObject.SetActive(true);
            resultTrans.Find("_Win").gameObject.SetActive(false);
            resultTrans.Find("_Lose").gameObject.SetActive(true);
        }
    }

    public void InitCombat()
    {
        playerScore = 1;
        enemyScore = 1;

        UpdateBar();
    }

    public void LoadEnemy(string enemy)
    {
        enemyID = enemy;
 
    }

    public void ReStart()
    {
        GameInstance.Signal("fade.out");
        GameInstance.CallLater(1f, () => {
            resultTrans.gameObject.SetActive(false);
            InitCombat();
        });
        GameInstance.CallLater(1.5f, () => GameInstance.Signal("fade.in"));
    }
}
