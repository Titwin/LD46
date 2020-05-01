using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Menu menu;

    public Level level;

    bool paused = false;
    public static Game instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    { 
        menu.GoToCover();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (level.active) {
                ExitLevel();
                menu.GoToMainMenu();
            }
            else {
                Exit();
            }
        }
    }
    public void StartLevel()
    {
        level.gameObject.SetActive(true);
        level.StartLevel();
    }
    public void ResetLevel()
    {
        level.Restart();
    }
    public bool ExitLevel()
    {
        if (level.active)
        {
            level.Unload();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
