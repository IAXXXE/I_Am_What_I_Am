using System;
using System.Collections;
using System.Collections.Generic;
using Tool.Module.Message;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject title;
    public GameObject arena;

    public List<GameObject> scenes;

    void Start()
    {
        GameInstance.Connect("scene.load", OnSceneLoad);
        GameInstance.Connect("scene.unload", OnSceneUnload);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("scene.load", OnSceneLoad);
        GameInstance.Disconnect("scene.unload", OnSceneUnload);
    }

    public void StartGame()
    {
        GameInstance.Signal("fade.out");
        GameInstance.CallLater(1f, () => {
            title.gameObject.SetActive(false);
            arena.gameObject.SetActive(true);
        });
        GameInstance.CallLater(1.5f, () => GameInstance.Signal("fade.in"));
    }

    public void ReturnTitle()
    {
        GameInstance.Signal("fade.out");
        GameInstance.CallLater(1f, () => {
            title.gameObject.SetActive(true);
            arena.gameObject.SetActive(false);
        });
        GameInstance.CallLater(1.5f, () => GameInstance.Signal("fade.in"));
    }


    private void OnSceneLoad(IMessage msg)
    {
        var sceneName = (string)msg.Data;

        for (int i = 0; i < scenes.Count; i ++)
        {
            if(scenes[i].name == sceneName) scenes[i].gameObject.SetActive(true);
            else scenes[i].gameObject.SetActive(false);
        }

    }

    private void OnSceneUnload(IMessage msg)
    {        
        var sceneName = (string)msg.Data;

        for (int i = 0; i < scenes.Count; i ++)
        {
            if(scenes[i].name == sceneName) scenes[i].gameObject.SetActive(false);
        }
    }
}
