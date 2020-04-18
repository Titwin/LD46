using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float blood = 0;

    public PlayerController carController;
    public PlayerControllerFoot personController;

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
                return carController.body.position;
            }
        }
    }
    // Start is called before the first frame update

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
        carController.gameObject.SetActive(true);
        personController.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            carController.gameObject.SetActive(!carController.gameObject.activeSelf);
            personController.gameObject.SetActive(!personController.gameObject.activeSelf);
            if (personController.gameObject.activeSelf)
            {
                personController.body.MovePosition(carController.car.door.position);
                camera.Follow = personController.transform;
            }
            else
            {
                camera.Follow = carController.cameraPivot.transform;
            }
        }
    }
}
