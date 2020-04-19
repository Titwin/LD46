using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour
{
    public List<RoadNode> neighbours;

    private void Start()
    {
        gameObject.name = "RoadNode" + Traffic.Instance.roadNodes.Count.ToString();
        Traffic.Instance.roadNodes.Add(this);
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        float f = 0.3f;
        Gizmos.color = Color.yellow;
        foreach(RoadNode n in neighbours)
        {
            Gizmos.DrawMesh(GetComponent<MeshFilter>().sharedMesh, transform.position);
            Gizmos.DrawLine(transform.position, n.transform.position);

            Vector3 p = 0.5f * (n.transform.position + transform.position);
            Vector3 v = (n.transform.position - transform.position).normalized;
            Vector3 u = new Vector3(v.y, -v.x, v.z);
            Gizmos.DrawLine(p + f * v, p - f * v + 0.5f * f * u);
            Gizmos.DrawLine(p + f * v, p - f * v - 0.5f * f * u);
        }
    }
}
