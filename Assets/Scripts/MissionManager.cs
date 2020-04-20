using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Mission mission;
    public UIDialog dialog;

    private Mission.Status status = Mission.Status.Done;
    public Player player;


    public SpriteRenderer locationCursor;
    public Transform arrowPivot;

    public float time = 0;
    public bool full = false;

    public Mission.Status Status {
        get => status;
        set { 
            if (status != value)
            {
                dialog.ShowText(mission.messages[(int)value]);
                status = value;
            }
        } }

    private void Start()
    {
        Status = Mission.Status.Planned;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 30, 100, 20), "status:" + Status);
        GUI.Label(new Rect(10, 50, 100, 20), "time:" + time);
        GUI.Label(new Rect(10, 70, 100, 20), "full:" + full);
    }

    private void LateUpdate()
    {
        Vector2 position = player.position;
        Vector2 direction = Vector2.zero;
        time += Time.deltaTime;

        switch (Status)
        {
            case Mission.Status.Planned:
                direction = mission.basePosition - position;
                locationCursor.transform.position = mission.basePosition;
                break;
            case Mission.Status.Started:
                //nothing
                break;
            case Mission.Status.Going:
                full = false;
                direction = mission.missionPosition - position;
                locationCursor.transform.position = mission.missionPosition;
                break;
            case Mission.Status.Eating:
                direction = mission.missionPosition - position;
                break;
            case Mission.Status.Returning:
                direction = mission.basePosition - position;
                locationCursor.transform.position = mission.basePosition;
                break;
            case Mission.Status.Done:
                //nothing
                break;
        }
        arrowPivot.transform.position = position;
        arrowPivot.transform.LookAt(locationCursor.transform.position);
        // checkpoint based changes
        if (direction.sqrMagnitude < 1)
        {
            switch (Status)
            {
                case Mission.Status.Planned:
                    Status = Mission.Status.Started;
                    break;
                case Mission.Status.Started:
                    Status = Mission.Status.Going;
                    break;
                case Mission.Status.Going:
                    Status = Mission.Status.Eating;
                    time = 0;
                    break;
                case Mission.Status.Eating:
                    // do nothing
                    break;
                case Mission.Status.Returning:
                    Status = Mission.Status.Done;
                    break;
                case Mission.Status.Done:
                    //nothing
                    break;
            }
        } // timer based actions
        //if (direction.sqrMagnitude < 1)
        {
            switch (Status)
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
                        Status = Mission.Status.Returning;
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

        switch (Status)
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
