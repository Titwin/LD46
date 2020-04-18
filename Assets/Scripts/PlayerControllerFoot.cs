using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFoot : MonoBehaviour
{
    public Vector2 direction;
    public float directionSpeed = 0.7f;
    public float speed = 1f;

    public Rigidbody2D body;
    private Vector2 fixedUpdateDirection;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.right;
        body = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 d = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        direction = PersoMoveToward(direction, d, directionSpeed * Time.deltaTime);

        transform.up = direction;
        fixedUpdateDirection = direction;

        body.MovePosition(body.position + fixedUpdateDirection * speed * Time.fixedDeltaTime);
        Debug.Log(fixedUpdateDirection * speed * Time.fixedDeltaTime);
    }
    private Vector2 PersoMoveToward(Vector2 current, Vector2 target, float delta)
    {
        Vector2 v = target - current;
        float d = v.magnitude;
        if (delta > d)
            return target;
        else
            return current + delta * v.normalized;
    }
}
