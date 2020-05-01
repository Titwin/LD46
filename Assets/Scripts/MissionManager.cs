using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class MissionManager : MonoBehaviour
{
    public TimeManager timeManager;
    public int currentMission = 0;
    public Mission[] missions;
    public Mission mission;
    public UIDialog dialog;
    public Image fade;
    public bool visible = true;
    [SerializeField] Mission.Status status = Mission.Status.Done;
    public Player player;

    public SpriteRenderer locationCursor;
    public Transform arrowPivot;

    public float time = 0;
    public bool full = false;

    public Mission.Status Status
    {
        get => status;
        set
        {
            if (status != value)
            {
                Debug.Log("s:" + status + "->" + value);
                dialog.ShowText(mission.messages[(int)value]);
                status = value;
            }
        }
    }

    private void Start()
    {
        missions = GetComponents<Mission>();
        mission = missions[0];
        Status = Mission.Status.Planned;
    }
    private void LateUpdate()
    {
        Vector2 position = player.position;
        Vector2 direction = Vector2.zero;
        time += Time.deltaTime;
        full = player.blood > 99;
        locationCursor.enabled = true;

        switch (Status)
        {
            case Mission.Status.Planned:
                timeManager.SetTime(timeManager.day, 22, 0);
                timeManager.paused = false;
                player.blood = mission.startBlood;
                player.personController.transform.position = mission.endPosition;
                player.carController.transform.position = mission.basePosition;
                break;
            case Mission.Status.Started:
                direction = mission.basePosition - position;
                locationCursor.transform.position = mission.basePosition;
                break;
            case Mission.Status.Going:
                full = false;
                direction = mission.missionPosition - position;
                locationCursor.transform.position = mission.missionPosition;
                break;
            case Mission.Status.Eating:
                direction = Vector2.zero;
                locationCursor.enabled = false;
                if (!player.personController.animating)
                {
                    Transform closest = player.personController.ClosestPerson();
                    if (closest)
                    {
                        direction = (Vector2)closest.position - position;
                        locationCursor.transform.position = (Vector2)closest.position;
                    }
                }
                break;
            case Mission.Status.Returning:
                direction = mission.endPosition - position;
                locationCursor.transform.position = mission.endPosition;
                break;
            case Mission.Status.Done:
                //nothing
                break;
        }
        if (direction != Vector2.zero)
        {
            arrowPivot.gameObject.SetActive(true);
            arrowPivot.transform.position = position;
            arrowPivot.transform.LookAt(locationCursor.transform.position);
        }
        else
        {
            arrowPivot.gameObject.SetActive(false);
        }
        // checkpoint based changes
        if (direction.sqrMagnitude < 0.5f)
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
                    if (!player.walking)
                        player.ExitCar();
                    time = 0;
                    break;
                case Mission.Status.Eating:
                    // do nothing
                    break;
                case Mission.Status.Returning:

                    if (!player.walking)
                    {
                        player.ExitCar();
                    }
                    Status = Mission.Status.Done;
                    timeManager.paused = true;
                    StartCoroutine(DoNextMission());
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
        if (timeManager.isNight)
        {
            float fadevalue = Mathf.MoveTowards(fade.color.a, visible ? 0 : 1, Time.deltaTime);
            fade.color = new Color(0, 0, 0, fadevalue);
        }else 
        {
            float fadevalue = Mathf.MoveTowards(fade.color.a, 1, Time.deltaTime);
            fade.color = new Color(1, 1, 1, fadevalue);
        }
    }

    IEnumerator DoNextMission()
    {
        player.Active = false;
        visible = false;
        yield return new WaitForSeconds(3);

        ++currentMission;
        currentMission %= missions.Length;
        if (currentMission < missions.Length)
        {
            Status = Mission.Status.Planned;
            mission = missions[currentMission];
            visible = true;
            yield return new WaitForSeconds(1);
            player.Active = true;
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
                Gizmos.DrawLine(position, mission.basePosition);
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
