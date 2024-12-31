using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public List<AudioClip> audioList;
    public List<AudioClip> bgmList;

    public List<AudioClip> hitList;

    private AudioSource audioSource;
    private AudioSource bgmSource;

    private Dictionary<string, AudioClip> audioDict = new();

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        audioDict["start"] = audioList[0];
        audioDict["def"] = audioList[1];
        audioDict["avoid"] = audioList[2];
        audioDict["hit"] = audioList[3];
        audioDict["ko"] = audioList[4];
        bgmSource = transform.Find("_BGM").GetComponent<AudioSource>();
        bgmSource.Play();
    }

    public void PlayAudio(string id)
    {
        if(id == "hit")
        {
            var random = UnityEngine.Random.Range(0,hitList.Count);
            audioSource.clip = hitList[random];
        }
        else
        {
            audioSource.clip = audioDict[id];
        }
        audioSource.Play();
    }

    public void PlayAudio(int idx)
    {
        audioSource.clip = audioList[idx];
        audioSource.Play();
    }

    public void PlayBGM(int idx)
    {
        bgmSource.clip = bgmList[idx];
        bgmSource.Play();
    }

    public void ChangeBGM(int idx)
    {
        StartCoroutine(StopBGM());
        GameInstance.CallLater(1f, () =>
        {
            bgmSource.volume = 1;
            bgmSource.clip = bgmList[idx];
            bgmSource.Play();
        });
    }

    private IEnumerator StopBGM()
    {
        while(bgmSource.volume > 0)
        {
            bgmSource.volume -= 0.05f;
            yield return null;
        }
        yield break;
    }

}