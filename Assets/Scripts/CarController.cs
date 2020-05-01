using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Controller;
public class CarController : MonoBehaviour
{
    [Header("Linking")]
    public Car car;
    public Player player;
    bool readInput = true;
    bool firstFrame = true;
    public bool Active
    {
        get { return readInput; }
        set
        {
            if(value!= readInput)
            {
                if (!readInput && value)
                    firstFrame = true;
                readInput = value;
            }
            
        }
    }
    private AudioListener audioListener;

    void Start()
    {
        audioListener = GetComponent<AudioListener>();
    }

    private void Update()
    {
        if (readInput && player.Active)
        {
            if (firstFrame)
            {
                firstFrame = false;
            }
            else
            {
                if (PadInputController.input.GetButtonDownA())
                {
                    player.ExitCar();
                }
                else if (PadInputController.input.GetButtonB())
                {
                    car.SetInput(Vector2.zero);
                }
                else
                {
                    Vector2 input = PadInputController.input.GetInputAxis();
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
