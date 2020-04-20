using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public float amplitude = 1f;
    public float speed = 1f;
    public List<Transform> leaves;


    private List<Vector2> phases = new List<Vector2>();
    private List<Vector2> speeds = new List<Vector2>();
    private List<Vector3> initial = new List<Vector3>();
    private float t;

    void Start()
    {
        t = Random.Range(0f, 360f);
        foreach (Transform leaf in leaves)
        {
            initial.Add(leaf.localPosition);
            phases.Add(new Vector2(Random.Range(0f, 360f), Random.Range(0f, 360f)));
            speeds.Add(new Vector2(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        t += speed * Time.deltaTime;
        if (t > 360f)
            t -= 360f;

        for (int i = 0; i < leaves.Count; i++)
        {
            leaves[i].localPosition = initial[i] +
                                      Vector3.up * 0.01f * amplitude * Mathf.Sin(speeds[i].x * (t + phases[i].x)) +
                                      Vector3.right * 0.01f * amplitude * Mathf.Sin(speeds[i].y * (t + phases[i].y));
        }
    }
}
