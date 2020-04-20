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
        if (readInput && player.Active)
        {
            if (player.GetButtonDown1())
            {
                player.ExitCar();
            }
            else
            {
                Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                car.SetInput(input);

                /*Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                car.SetInput(input);

                if(input == Vector2.zero)
                {
                    float x = Input.GetKey(KeyCode.D) ? 1f : (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A) ? -1f : 0f);
                    float y = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
                    car.SetInput(new Vector2(x, y));
                }*/
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
