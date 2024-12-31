using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEnd : MonoBehaviour
{
    public void AnimEndAction()
    {
        GameInstance.Signal("anim.end");
    }
}
