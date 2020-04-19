using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Linking")]
    public Car car;
    public Player player;
    public bool readInput = true;
    private AudioListener audioListener;

    void Start()
    {
        audioListener = GetComponent<AudioListener>();
    }

    private void FixedUpdate()
    {
        

        if (readInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ExitCar();
            }
            else
            {
                car.SetInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            }
        }
        else
        {
            car.SetInput(Vector2.zero);
        }
    }

    public void StartEngine()
    {
        car.StartEngine();

        if (audioListener)
        {
            Destroy(audioListener);
            audioListener = null;
        }
        audioListener = gameObject.AddComponent<AudioListener>() as AudioListener;
    }

    public void StopEngine()
    {
        car.StopEngine();

        if (audioListener)
        {
            Destroy(audioListener);
            audioListener = null;
        }            
    }
}
