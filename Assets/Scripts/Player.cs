﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton exposure
    public static Player main;

    [Header("Linkings")]
    public CarController carController;
    public Monster personController;
    public Transform carArrow;
    public AudioSource music;
    public TimeManager time;
    new public Cinemachine.CinemachineVirtualCamera camera;

    [Header("UI")]
    public UIScore uiscore;
    public UIDialog dialog;
    public GameObject deadText;
    public GameObject uiBloodFrame;
    public UIBlood uiBlood;

    [Header("Status/Debug")]
    [SerializeField] bool debug_immortal = false;
    [SerializeField] bool active = true;
    [SerializeField] public float blood = 100;
    [SerializeField] int score;
    public bool walking = false;
    bool alive = true;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            uiscore.AddScore(value - score);
            score = value;
        }
    }
    
    public Vector2 position {
        get {
            if (personController.gameObject.activeSelf)
            {
                return personController.body.position;
            }
            else
            {
                return carController.transform.position;
            }
        }
    }

    public void OnKillPerson(bool car)
    {
        if (car)
        {
            Score += 25;
        }
        else
        {
            Score += 10;
        }
    }
    
    public void OnKillGhoul(bool car)
    {
        if (car)
        {
            Score += 50;
        }
        else
        {
            Score += 100;
        }
    }
    public void OnEatPerson()
    {
        Score += 500;
    }

    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
        }
    }

    private void Awake()
    {
        uiBloodFrame.SetActive(false);
        main = this;
    }
    private void Start()
    {

        
        if (walking)
        {
            carController.Active = false;
            personController.direction = carController.transform.right;
            personController.gameObject.SetActive(true);
            camera.Follow = personController.transform;
            carController.StopEngine();
            walking = true;
        }
        else
        {
            carController.Active = true;
            EnterCar();
        }
        
    }
    private void LateUpdate()
    {
        // player is inside car
        if(carController.Active)
        {
            personController.transform.position = carController.car.door.position;
            personController.gameObject.SetActive(false);
        }

        // action
        music.pitch = Mathf.MoveTowards(music.pitch, carController.Active ? 1 : 0.5f, Time.deltaTime * 2);
        camera.m_Lens.FieldOfView = Mathf.MoveTowards(camera.m_Lens.FieldOfView, carController.Active ? (Mathf.Lerp(48,64,carController.car.currentSpeed/10)) : 32,30*Time.deltaTime);
        if (alive)
        {
            blood -= Time.deltaTime;
            bool died = !debug_immortal && (blood <= 0 || !time.isNight); 
            if (died)
            {
                if (!walking)
                {
                    ExitCar();
                    personController.Die();
                }
                alive = false;
                dialog.ShowText("Arrrrgg !!!", false);
                StartCoroutine(DieAndWait());
            }
            uiBlood.SetValue(Mathf.Clamp01(blood / 100f));
        }
        if (walking)
        {
            carArrow.transform.LookAt(carController.transform.position);
        }
    }
    IEnumerator DieAndWait()
    {
        deadText.SetActive(true);
        yield return new WaitForSeconds(3);
        dialog.ShowText("Press any key to restart", false);
        while (!Controller.PadInputController.input.AnyButton())
        {
            yield return new WaitForEndOfFrame();
        }
        Game.instance.ResetLevel();
    }
    public void Feed(float amount)
    {
        blood = Mathf.Clamp(blood+amount,0,100);
    }
    public void ExitCar()
    {
        carController.Active = false;
        personController.direction = carController.transform.right;
        personController.gameObject.SetActive(true);
        camera.Follow = personController.transform;
        carController.StopEngine();
        walking = true;
    }
    public void EnterCar()
    {
        carController.Active = true;
        personController.gameObject.SetActive(false);
        camera.Follow = carController.car.cameraPivot.transform;
        carController.StartEngine();
        walking = false;
    }
}
