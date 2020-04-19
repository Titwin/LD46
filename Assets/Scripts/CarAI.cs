using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Settings")]
    public RoadNode target;
    public Transform sensor;
    public LayerMask obstacleMask;
    public float sensorDistance;
    public float maxSpeed = 4f;
    private Car car;
    public AnimationCurve speedAdjust;

    [Header("Debug")]
    [SerializeField] private Vector2 cmd;
    [SerializeField] private float timer = 0f;
    [SerializeField] private float courtesyTimer = 0f;

    void Start()
    {
        car = GetComponent<Car>();
    }
    public void Initialize()
    {
        RoadNode previous = Traffic.Instance.roadNodes[Random.Range(0, Traffic.Instance.roadNodes.Count)];
        RoadNode initial = previous.neighbours[Random.Range(0, previous.neighbours.Count)];
        target = initial.neighbours[Random.Range(0, initial.neighbours.Count)];

        transform.position = initial.transform.position;
        car.direction = initial.transform.position - previous.transform.position;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (target == null || timer > 100f)
            Initialize();

        // direction and command
        Vector3 d = target.transform.position - transform.position;
        if(d.magnitude < 0.8f)
        {
            timer = 0f;

            if (target.neighbours.Count > 0)
                target = target.neighbours[Random.Range(0, target.neighbours.Count)];
            else
            {
                Debug.LogWarning("error in navmesh : " + target.gameObject.name + ", neighbours " + target.neighbours.Count.ToString());
                Initialize();
            }
            d = target.transform.position - transform.position;
        }
        cmd = new Vector2(d.x, d.y).normalized;
        car.SetInput(cmd);

        // speed adjust
        RaycastHit2D hit = Physics2D.BoxCast(sensor.position, Vector2.one, 0f, sensor.right, sensorDistance, obstacleMask);
        if (hit.collider != null)
        {
            car.speed = speedAdjust.Evaluate(hit.distance / sensorDistance) * maxSpeed;

            if(car.speed == 0f)
            {
                courtesyTimer -= Time.deltaTime;
                if(courtesyTimer < 0f)
                    car.speed = 2 * maxSpeed;
            }
            else
            {
                courtesyTimer = Random.Range(2f, 6f);
            }
        }
        else
        {
            car.speed = maxSpeed;
            courtesyTimer = Random.Range(2f, 6f);
        }
    }
}
