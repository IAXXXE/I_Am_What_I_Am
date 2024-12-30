using System;
using System.Collections;
using System.Collections.Generic;
using Tool.Module.Message;
using UnityEngine;

public class Astar : MonoBehaviour
{
    private AstarPath astarPath;

    // Start is called before the first frame update
    void Start()
    {
        astarPath = GetComponent<AstarPath>();

        GameInstance.Connect("path.scan", OnPathScan);
    }

    void OnDestroy()
    {
        GameInstance.Disconnect("path.scan", OnPathScan);
    }

    private void OnPathScan(IMessage msg)
    {
        astarPath.Scan();
    }
}
