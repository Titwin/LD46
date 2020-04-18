using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Mission mission;

    public Mission.Status status;
    public Player player;
    private void LateUpdate()
    {
        Vector2 position = player.position;
        Vector2 direction = Vector2.zero;
        switch (status)
        {
            case Mission.Status.Planned:
                direction = mission.basePosition- position;
                break;
            case Mission.Status.Started:
                //nothing
                break;
            case Mission.Status.Going:
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
        if(direction.sqrMagnitude < 1)
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
                    break;
                case Mission.Status.Eating:
                    status = Mission.Status.Returning;
                    break;
                case Mission.Status.Returning:
                    status = Mission.Status.Done;
                    break;
                case Mission.Status.Done:
                    //nothing
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
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
                //nothing
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
