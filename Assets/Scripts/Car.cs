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
    public bool tankControls = false;

    private Rigidbody2D body;
    private Vector2 lastNonZeroDirection;
    private Vector2 deltaPosition;

    [SerializeField] private ParticleSystem[] wheelmarks;
    [Header("Audio")]
    public AudioSource motorAudioSource;
    public AudioSource doorsAudioSource;
    public AudioClip idleMotorAudioClip;
    public AudioClip openingCarAudioClip;
    public AudioClip closingCarAudioClip;

    [Header("Debug")]
    [SerializeField] public float currentSpeed;
    [SerializeField] private Vector2 input;

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

    public void StartEngine()
    {
        doorsAudioSource.PlayOneShot(openingCarAudioClip);
        motorAudioSource.clip = idleMotorAudioClip;
        motorAudioSource.Play();
    }

    public void StopEngine()
    {
        doorsAudioSource.PlayOneShot(closingCarAudioClip);
        motorAudioSource.Stop();
    }

    Vector3 rotationPivot;
    void Update()
    {
        // compute direction
        if (input != Vector2.zero)
            input.Normalize();
        if (tankControls)
        {
            rotationPivot = this.transform.position + this.transform.right * 0.5f;
            //currentSpeed = body.velocity.magnitude;
            float steer = input.x;
            float gas = input.y;
            //bool backwards = Mathf.Sign(acceleration) < 0;
            float targetSpeed = gas == 0 ? 0 : (gas > 0 ? speed : -speed);
            //if (acceleration != 0)

            bool drift = currentSpeed > 0 && targetSpeed <= 0;


            steer *= Mathf.Abs(drift ? currentSpeed * 2 : currentSpeed);
            transform.RotateAround(rotationPivot, Vector3.back, steer * rotationSpeed / 360f);

            bool stetch = currentSpeed < speed / 4 && gas > 0f  || (currentSpeed>0 && targetSpeed <= 0 && Mathf.Abs(steer)>0.8f);

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (targetSpeed != 0 ? acceleration : deceleration) * Time.deltaTime);

            direction = transform.right;
            body.velocity = direction * currentSpeed;

            foreach (var w in wheelmarks)
            {
                if (drift)
                {
                    w.Play();
                }
                else
                {
                    w.Pause();
                }
            }
        }
        else
        {
            float angle = Vector2.SignedAngle(direction, input);
            if (input.sqrMagnitude != 0f)
            {
                float da = (angle > 0f ? 1 : -1) * rotationSpeed * Time.deltaTime;
                direction = Rotate(lastNonZeroDirection, Mathf.Abs(da) < Mathf.Abs(angle) ? da : angle);
            }

            // compute  speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, input.magnitude * speed, (currentSpeed < input.magnitude * speed ? acceleration : deceleration) * Time.deltaTime);

            // compute deltaPosition position
            deltaPosition = direction * currentSpeed;

            // aiming
            if (direction.sqrMagnitude > 0.000001f)
                lastNonZeroDirection = direction;
            transform.right = lastNonZeroDirection;

            body.MovePosition(body.position + deltaPosition * Time.fixedDeltaTime);
        }
        if (cameraPivot)
            cameraPivot.transform.position = body.position + direction * currentSpeed * 0.5f;

        // Engine sound
        motorAudioSource.pitch = 0.75f + currentSpeed / 6f;
    }

    Vector2 Rotate(Vector2 aPoint, float aDegree)
    {
        float rad = aDegree * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);
        return new Vector2(aPoint.x * c - aPoint.y * s, aPoint.y * c + aPoint.x * s);
    }

    private void OnDrawGizmos()
    {
        if (body)
        {
            Gizmos.DrawSphere(rotationPivot, 0.05f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(body.position, body.position + direction);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(body.position, body.position + deltaPosition);
            if (cameraPivot)
            {
                Gizmos.DrawSphere(cameraPivot.position, 0.05f);
            }

        }
    }
}
