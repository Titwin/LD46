using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float blood = 100;
    public AudioSource music;
    public UIBlood uiBlood;
    [Header("Linkings")]
    public CarController carController;
    public Monster personController;

    public bool walking = false;
    new public Cinemachine.CinemachineVirtualCamera camera;

    public static Player main;
    public GameObject uiBloodFrame;

    public Transform carArrow;
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

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "blood:"+blood);
    }

    private void Awake()
    {
        uiBloodFrame.SetActive(false);
        main = this;
    }
    private void Start()
    {

        carController.readInput = true;
        if (walking)
        {
            carController.readInput = false;
            personController.direction = carController.transform.right;
            personController.gameObject.SetActive(true);
            camera.Follow = personController.transform;
            carController.StopEngine();
            walking = true;
        }
        else
        {
            EnterCar();
        }
        
    }
    private void LateUpdate()
    {
        // player is inside car
        if(carController.readInput)
        {
            personController.transform.position = carController.car.door.position;
            personController.gameObject.SetActive(false);
        }

        // action
        music.pitch = Mathf.MoveTowards(music.pitch, carController.readInput ? 1 : 0.5f, Time.deltaTime * 2);
        camera.m_Lens.FieldOfView = Mathf.MoveTowards(camera.m_Lens.FieldOfView, carController.readInput ? (Mathf.Lerp(48,64,carController.car.currentSpeed/10)) : 32,30*Time.deltaTime);

        blood -= Time.deltaTime;
        if (blood <= 0)
        {
            personController.Die();
        }
        uiBlood.SetValue(Mathf.Clamp01(blood / 100f));

        if (walking)
        {
            carArrow.transform.LookAt(carController.transform.position);
        }
    }

    public void Feed(float amount)
    {
        blood = Mathf.Clamp(blood+amount,0,100);
    }
    public void ExitCar()
    {
        carController.readInput = false;
        personController.direction = carController.transform.right;
        personController.gameObject.SetActive(true);
        camera.Follow = personController.transform;
        carController.StopEngine();
        walking = true;
    }
    public void EnterCar()
    {
        carController.readInput = true;
        personController.gameObject.SetActive(false);
        camera.Follow = carController.car.cameraPivot.transform;
        carController.StartEngine();
        walking = false;
    }
}
