﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Mission mission;

    public Mission.Status status;
    public Player player;

    public float time = 0;
    public bool full = false;

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 30, 100, 20), "status:" + status);
        GUI.Label(new Rect(10, 50, 100, 20), "time:" + time);
        GUI.Label(new Rect(10, 70, 100, 20), "full:" + full);
    }

    private void LateUpdate()
    {
        Vector2 position = player.position;
        Vector2 direction = Vector2.zero;
        time += Time.deltaTime;
        switch (status)
        {
            case Mission.Status.Planned:
                direction = mission.basePosition - position;
                break;
            case Mission.Status.Started:
                //nothing
                break;
            case Mission.Status.Going:
                full = false;
                direction = mission.missionPosition - position;
                break;
            case Mission.Status.Eating:
                direction = mission.missionPosition - position;
                break;
            case Mission.Status.Returning:
                direction = mission.basePosition - position;
                break;
            case Mission.Status.Done:
                //nothing
                break;
        }
        // checkpoint based changes
        if (direction.sqrMagnitude < 1)
        {
            switch (status)
            {
                case Mission.Status.Planned:
                    status = Mission.Status.Started;
                    break;
                case Mission.Status.Started:
                    status = Mission.Status.Going;
                    break;
                case Mission.Status.Going:
                    status = Mission.Status.Eating;
                    time = 0;
                    break;
                case Mission.Status.Eating:
                    // do nothing
                    break;
                case Mission.Status.Returning:
                    status = Mission.Status.Done;
                    break;
                case Mission.Status.Done:
                    //nothing
                    break;
            }
        } // timer based actions
        if (direction.sqrMagnitude < 1)
        {
            switch (status)
            {
                case Mission.Status.Planned:
                    break;
                case Mission.Status.Started:
                    break;
                case Mission.Status.Going:
                    break;
                case Mission.Status.Eating:
                    if (full || time > mission.timeOutTimer)
                    {
                        status = Mission.Status.Returning;
                    }
                    break;
                case Mission.Status.Returning:
                    break;
                case Mission.Status.Done:
                    break;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Vector2 position = player.position;
        Gizmos.DrawWireCube(mission.basePosition, Vector3.one);
        Gizmos.DrawWireCube(mission.missionPosition, Vector3.one);
        Gizmos.DrawLine(mission.basePosition, mission.missionPosition);

        switch (status)
        {
            case Mission.Status.Planned:
                Gizmos.DrawLine(position,mission.basePosition);
                break;
            case Mission.Status.Started:
                //nothing
                break;
            case Mission.Status.Going:
                Gizmos.DrawLine(position, mission.missionPosition);
                break;
            case Mission.Status.Eating:
                Gizmos.DrawWireSphere(mission.missionPosition, 5);
                break;
            case Mission.Status.Returning:
                Gizmos.DrawLine(position, mission.basePosition);
                break;
            case Mission.Status.Done:
                //nothing
                break;
        }
    }

}
