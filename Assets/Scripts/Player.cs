using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float blood = 0;
    public AudioSource music;
    [Header("Linkings")]
    public CarController carController;
    public PlayerController personController;

    new public Cinemachine.CinemachineVirtualCamera camera;

    public static Player main;
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
        main = this;
    }
    private void Start()
    {
        carController.readInput = true;
        personController.gameObject.SetActive(false);
        camera.Follow = carController.car.cameraPivot.transform;
        carController.StartEngine();
    }
    private void Update()
    {
        // player is inside car
        if(carController.readInput)
        {
            personController.transform.position = carController.car.door.position;
            personController.gameObject.SetActive(false);
        }

        // action
        music.pitch = carController.readInput ? 1 : 0.5f;
        camera.m_Lens.FieldOfView = Mathf.MoveTowards(camera.m_Lens.FieldOfView, carController.readInput ? (Mathf.Lerp(48,64,carController.car.currentSpeed/10)) : 32,30*Time.deltaTime);
    }

    public void ExitCar()
    {
        carController.readInput = false;
        personController.direction = carController.transform.right;
        personController.gameObject.SetActive(true);
        camera.Follow = personController.transform;
    }
    public void EnterCar()
    {
        carController.readInput = true;
        personController.gameObject.SetActive(false);
        camera.Follow = carController.car.cameraPivot.transform;
    }
    
}
