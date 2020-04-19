using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Linking")]
    public Car car;
    public Player player;
    public bool readInput = true;

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
}
