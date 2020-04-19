using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Links")]
    public Transform door;
    public Transform cameraPivot;
    // Start is called before the first frame update

    [Header("Controls settings")]
    public Vector2 direction;
    public float rotationSpeed = 0.7f;
    public float speed = 1f;
    public float acceleration = 1f;
    public float deceleration = 1f;

    private Rigidbody2D body;
    private Vector2 lastNonZeroDirection;
    private Vector2 deltaPosition;

    private Vector2 input;

    [Header("Debug")]
    [SerializeField] private float currentSpeed;
    void Start()
    {
        direction = Vector3.zero;
        lastNonZeroDirection = Vector3.right;
        deltaPosition = Vector2.zero;
        body = GetComponent<Rigidbody2D>();
    }

    public void SetInput(Vector2 input)
    {
        this.input = input;
    }
    void FixedUpdate()
    {
        // compute direction
        if (input != Vector2.zero)
            input.Normalize();
        float angle = Vector2.SignedAngle(direction, input);
        if (input.sqrMagnitude != 0f)
        {
            float da = (angle > 0f ? 1 : -1) * rotationSpeed * Time.deltaTime;
            direction = Rotate(lastNonZeroDirection, Mathf.Abs(da) < Mathf.Abs(angle) ? da : angle);
        }

        // compute  speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, input.magnitude * speed, (currentSpeed < input.magnitude * speed ? acceleration : deceleration) * Time.deltaTime);

        // compute deltaPosition position
        deltaPosition = direction * currentSpeed * Time.fixedDeltaTime;

        // aiming
        if (direction.sqrMagnitude > 0.000001f)
            lastNonZeroDirection = direction;
        transform.right = lastNonZeroDirection;

        body.MovePosition(body.position + deltaPosition);
        cameraPivot.transform.position = body.position + direction * currentSpeed * 0.5f;
    }
    private void LateUpdate()
    {
        input = Vector2.zero;
    }
    private float PersoMoveTowards(float current, float target, float delta)
    {
        float v = target - current;
        if (delta > v)
            return target;
        else
            return current + delta;
    }
    Vector2 Rotate(Vector2 aPoint, float aDegree)
    {
        float rad = aDegree * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);
        return new Vector2(aPoint.x * c - aPoint.y * s, aPoint.y * c + aPoint.x * s);
    }
}
