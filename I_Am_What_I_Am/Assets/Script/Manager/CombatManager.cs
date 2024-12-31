using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DG.Tweening;
using TMPro;
using Tool.Module.Message;
using Unity.VisualScripting;
using UnityEngine;

public enum CombatTurn
{
    Enemy,
    Player
}

public class CombatManager : Singleton<CombatManager>
{
    public Material barMaterial;

    public CombatTurn currTurn;

    public Transform playerIcon;
    public Transform enemyIcon;
    public Transform resultTrans;
    public Transform winTrans;
    public GameObject flash;
    public GameObject preAnim;

    public Transform pointerTrans;

    private TextMeshProUGUI playerText;
    private TextMeshProUGUI enemyText;
    public int playerScore;
    public int enemyScore;
    public float splitRatio;

    private string enemyID;
    private bool firstTurn;

    // Start is called before the first frame update
    void Start()
    {
        GameInstance.Connect("score.up", OnScoreUp);
        GameInstance.Connect("anim.end", OnAnimEnd);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("score.up", OnScoreUp);
        GameInstance.Disconnect("anim.end", OnAnimEnd);
    }

    public void OnEnable()
    {
        InitCombat();
    }

    private void OnAnimEnd(IMessage msg)
    {
        OnAnimEnd();
    }

    private void OnScoreUp(IMessage msg)
    {
        var id = (string)msg.Data;

        if(id == "player")
        {
            playerScore ++;
            // playerIcon.DOShakeScale(1.2f, 0.2f);
            SetScore("player", playerScore);
            flash.SetActive(true);
            if(playerScore < 10) GameInstance.CallLater(0.2f, () => flash.SetActive(false));
        }
        else
        {
            enemyScore ++;
            // enemyIcon.DOShakeScale(1.2f, 0.2f);
            SetScore("enemy", enemyScore);
        }

        UpdateBar();
        CheckScore();
    }

    public void UpdateBar()
    {
        splitRatio = ((float)enemyScore + 1) / (playerScore + enemyScore + 2);
        barMaterial.SetFloat("_SplitRatio", splitRatio);
    }

    public void CheckScore()
    {
        if(playerScore >= 10)
        {
            GameInstance.Instance.inCombat = false;
            if(enemyScore == 0) 
            {
                GameInstance.CallLater(0.1f, () => GameInstance.Instance.audioManager.PlayAudio("ko"));
            }

            if(GameInstance.Instance.mode == Mode.Hard)
            {
                GameInstance.Signal("fade.out");
                GameInstance.CallLater(1f, () => {
                    winTrans.gameObject.SetActive(true);
                });
                GameInstance.CallLater(1.5f, () => GameInstance.Signal("fade.in"));
                
            }
            else
            {
                resultTrans.gameObject.SetActive(true);
                resultTrans.Find("_Win").gameObject.SetActive(true);
                resultTrans.Find("_Lose").gameObject.SetActive(false);
            }
            StopCoroutine(StartEnemyTurn());
            StopCoroutine(StartPlayerTurn());
            pointerTrans.gameObject.SetActive(false);
            GameInstance.Instance.ChangeMode(1);
        }
        else if (enemyScore >= 10)
        {
            GameInstance.Instance.inCombat = false;
            resultTrans.gameObject.SetActive(true);
            resultTrans.Find("_Win").gameObject.SetActive(false);
            resultTrans.Find("_Lose").gameObject.SetActive(true);
            // StopAllCoroutines();
            StopCoroutine(StartEnemyTurn());
            StopCoroutine(StartPlayerTurn());
            pointerTrans.gameObject.SetActive(false);
            GameInstance.Instance.ChangeMode(-1);
        }
    }

    public void InitCombat()
    {
        playerText = playerIcon.Find("_Text").GetComponent<TextMeshProUGUI>();
        enemyText = enemyIcon.Find("_Text").GetComponent<TextMeshProUGUI>();
        GameInstance.Instance.inCombat = true;
        SetScore("player", 0);
        SetScore("enemy", 0);
        UpdateBar();

        Debug.Log("INIT");
        GameInstance.Signal("enemy.init");
        winTrans.gameObject.SetActive(false);
        transform.Find("_Enemy").localPosition = new Vector3(-4f, -1, 0);
        transform.Find("_Player").localPosition = new Vector3(4.7f, -2f, 0);
        pointerTrans.localPosition = new Vector3(0, pointerTrans.localPosition.y, 0);
        pointerTrans.gameObject.SetActive(true);

        preAnim.SetActive(true);
        GameInstance.Instance.audioManager.ChangeBGM(1);
        firstTurn = true;
        currTurn = CombatTurn.Enemy;
    }

    public void OnAnimEnd()
    {

        preAnim.SetActive(false);
        switch(GameInstance.Instance.mode)
        {
        case Mode.Easy:
            StartCoroutine(StartPlayerTurn());
            break;
        case Mode.Normal:
            StartCoroutine(StartPlayerTurn());
            break;
        case Mode.Hard:
            StartCoroutine(StartEnemyTurn());
            break;
        }
        
    }

    public float moveTime = 20f;
    private IEnumerator StartEnemyTurn()
    {
        currTurn = CombatTurn.Enemy;
        var enemyTime = splitRatio * moveTime;
        pointerTrans.localPosition = new Vector3(1360f * splitRatio - 680f, pointerTrans.localPosition.y, 0);
        pointerTrans.DOLocalMoveX(-680f, enemyTime);

        if(firstTurn == true && GameInstance.Instance.mode != Mode.Easy)
        {
            GameInstance.Signal("enemy.pre_atk", 0.3f);
        }

        if(enemyTime > 7f && GameInstance.Instance.mode != Mode.Easy)
        {
            var attackTime1 = UnityEngine.Random.Range(0.5f, enemyTime - 4f);
            GameInstance.Signal("enemy.pre_atk", attackTime1);

            GameInstance.Signal("enemy.pre_atk", attackTime1 + 4f);
        }
        else
        {
            var attackTime = UnityEngine.Random.Range(0.2f, enemyTime - 1f);
            GameInstance.Signal("enemy.pre_atk", attackTime);
        }

        yield return new WaitForSeconds(enemyTime);
        yield return StartPlayerTurn();
    }

    private IEnumerator StartPlayerTurn()
    {
        currTurn = CombatTurn.Player;
        var playerTime = (1 - splitRatio) * moveTime;
        pointerTrans.localPosition = new Vector3(1360f * splitRatio - 680f, pointerTrans.localPosition.y, 0);
        pointerTrans.DOLocalMoveX(680f, playerTime);
        
        yield return new WaitForSeconds(playerTime);
        yield return StartEnemyTurn();
    }


    public void LoadEnemy(string enemy)
    {
        enemyID = enemy;
    }

    public void SetScore(string id, int score)
    {
        if(id == "player")
        {
            playerScore = score;
            // playerIcon.DOShakeScale(1.1f, 0.1f);
            playerText.text = playerScore.ToString();

           if(playerScore >= 5) GameInstance.Signal("enemy.injury", true);
        }
        else
        {
            enemyScore = score;;
            // enemyIcon.DOShakeScale(1.1f, 0.1f);
            enemyText.text = enemyScore.ToString();
        }
    }

    public void ReStart()
    {
        GameInstance.Signal("fade.out");
        GameInstance.CallLater(1f, () => {
            InitCombat();
            resultTrans.gameObject.SetActive(false);
        });
        GameInstance.CallLater(1.5f, () => GameInstance.Signal("fade.in"));
    }
}
