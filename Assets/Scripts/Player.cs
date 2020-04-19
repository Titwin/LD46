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

    [Header("Controls settings")]
    public LayerMask ennemyMask;
    public float carEntryRadius;
    public float ennemySearchRadius;

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
        camera.Follow = carController.cameraPivot.transform;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (carController.readInput)
            {
                carController.readInput = false;
                personController.direction = carController.transform.right;
                personController.gameObject.SetActive(true);
                camera.Follow = personController.transform;
            }
            else
            {
                Vector3 car2char = carController.transform.position - personController.transform.position;
                RaycastHit2D hit = Physics2D.BoxCast(personController.transform.position, Vector2.one, ennemySearchRadius, personController.transform.up, 3f, ennemyMask);
                if(hit.collider != null)
                {
                    personController.Attack(hit.collider.transform);
                }
                else if(car2char.sqrMagnitude < carEntryRadius * carEntryRadius)
                {
                    carController.readInput = true;
                    personController.gameObject.SetActive(false);
                    camera.Follow = carController.cameraPivot.transform;
                }
                else
                {
                    //car is too far and victims too !
                }
            }
        }
        music.pitch = carController.readInput ? 1 : 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(personController.transform.position, Vector3.forward, carEntryRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(personController.transform.position, Vector2.one);
        Gizmos.DrawLine(personController.transform.position, personController.transform.position + ennemySearchRadius * personController.transform.up);
    }
}
