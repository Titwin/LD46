﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public enum Status
    {
        Planned, Started, Going, Eating, Returning, Done
    }

    public Status status;

    public Vector2 basePosition;
    public Vector2 missionPosition;
    public Vector2 endPosition;
    public float timeOutTimer = 10;
    public float startBlood = 100;
    public string[] messages;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(basePosition, Vector3.one);
        Gizmos.DrawWireCube(missionPosition, Vector3.one);
        Gizmos.DrawLine(basePosition, missionPosition);
        Gizmos.DrawLine(endPosition, missionPosition);
    }
}
