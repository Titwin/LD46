using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 direction;
    public float directionSpeed = 0.7f;
    public float carSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 d = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        direction = Vector3.MoveTowards(direction, d, directionSpeed * Time.deltaTime);
        transform.right = direction;
        transform.position += direction * carSpeed * Time.deltaTime;
    }
}
