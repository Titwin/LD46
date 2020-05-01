using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traffic : MonoBehaviour
{
    public static Traffic Instance;

    private void Awake()
    {
        //if (Instance == null)
        Instance = this;
    }

    public List<RoadNode> roadNodes = new List<RoadNode>();


    public int GetIndex(RoadNode node)
    {
        for (int i = 0; i < roadNodes.Count; i++)
        {
            if (roadNodes[i] == node)
                return i;
        }
        return -1;
    }
}
