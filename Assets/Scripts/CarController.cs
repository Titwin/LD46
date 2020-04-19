using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Linking")]
    public Car car;
    
    public bool readInput = true;

    private void FixedUpdate()
    {
        if (readInput)
        {
            car.SetInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
    }
}
