using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tool;
using Tool.Util;
using Tool.Module.Message;
// using UnityEngine.AddressableAssets;
// using UnityEngine.SceneManagement;

public enum Mode
{
    Easy,
    Normal,
    Hard,
}

public class GameInstance : Singleton<GameInstance>
{
    // public Player player;
    // public Cursor cursor;
    public Mode mode;
    public bool inCombat;

    public AudioManager audioManager;

    private new void Awake()
    {
        base.Awake();
        MessageDispatcher.Init(gameObject);

        Application.targetFrameRate = 30;
        mode = Mode.Normal;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Easy mode");
            mode = Mode.Easy;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Normal mode");
            mode = Mode.Normal;
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Hard mode");
            mode = Mode.Hard;
        }
    }

    public void ChangeMode(int diff)
    {
        if(diff > 0)
        {
            if(mode == Mode.Easy) mode = Mode.Normal;
            if(mode == Mode.Normal) mode = Mode.Hard;
        }
        else if(diff < 0)
        {
            if(mode == Mode.Normal) mode = Mode.Easy;
            if(mode == Mode.Hard) mode = Mode.Normal;
        }
    }

    #region Coroutine
    public static void CallLater(float delay, Action action)
    {
        Instance.StartCoroutine(Instance.CorCallLater(delay, action));
    }
    // CallLater(0.1f, ()) => );
    // CallLater(0.1f, () => 
    // {
            // code
    // });

    public static void CallNextFrame(Action action)
    {
        Instance.StartCoroutine(Instance.CorCallNextFrame(action));
    }
    public static void CallWaitFrames(float frames, Action action)
    {
        Instance.StartCoroutine(Instance.CorCallWaitFrames(frames, action));
    }
    private IEnumerator CorCallLater(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
    private IEnumerator CorCallNextFrame(Action action)
    {
        yield return null;
        action();
    }
    private IEnumerator CorCallWaitFrames(float frames, Action action)
    {
        Debug.Assert(frames > 0);
        for (int i = 0; i < frames; i++)
            yield return null;
        action();
    }
    #endregion

    #region Message
    public static void Signal(string msg, object data = null, object src = null, float delay = 0.0f)
    {
        MessageDispatcher.SendMessage(src, msg, data, delay);
    }

    public static void Connect(string msg, MessageHandler handler)
    {
        MessageDispatcher.AddListener(msg, handler, true);
    }

    public static void Disconnect(string msg, MessageHandler handler)
    {
        MessageDispatcher.RemoveListener(msg, handler, true);
    }
    #endregion
}
