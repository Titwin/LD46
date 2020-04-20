using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject cover;
   
    public Level levelTemplate;
    public Level currentLevel;

    bool paused = false;
    public static Game instance;
    // Start is called before the first frame update
    void Start()
    {
        cover.SetActive(true);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (cover.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                cover.SetActive(false);

                currentLevel = Instantiate<Level>(levelTemplate);
                currentLevel.gameObject.SetActive(true);
                currentLevel.player.gameObject.SetActive(true);
                currentLevel.workflow.visible = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            paused = !paused;
        }
    }
    public void Reset()
    {
        Destroy(currentLevel.gameObject);

        currentLevel = Instantiate<Level>(levelTemplate);
        currentLevel.gameObject.SetActive(true);
        currentLevel.player.gameObject.SetActive(true);
        currentLevel.workflow.visible = true;
    }
}
