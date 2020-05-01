using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{   
    [SerializeField] LevelState volatileContainer;
    public LevelState state;

    public bool initialized = false;
    public bool active = false;
    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        volatileContainer.gameObject.SetActive(false);
        initialized = true;
    }
    public void StartLevel()
    {
        if (!initialized)
        {
            Initialize();
        }
        state = Instantiate<LevelState>(volatileContainer,volatileContainer.transform.parent,true);
        state.gameObject.SetActive(true);
        state.workflow.visible = true;
    }

    public void Restart()
    {
        Unload();
        StartLevel();
    }

    public void Unload()
    {
        Destroy(state.gameObject);
        active = false;
    }
}
