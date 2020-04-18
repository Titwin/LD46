using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float blood = 0;
    // Start is called before the first frame update

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "blood:"+blood);
    }
}
