using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject cover;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        cover.SetActive(true);
        player.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cover.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                player.gameObject.SetActive(true);
                cover.SetActive(false);
            }
        }
    }
}
