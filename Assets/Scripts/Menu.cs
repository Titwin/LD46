using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] Game game;
    [SerializeField] GameObject cover;
    [SerializeField] GameObject main;
    public void GoToCover()
    {
        ToggleMenu(true);
        cover.SetActive(true);
        main.SetActive(false);
    }

    public void GoToMainMenu()
    {
        ToggleMenu(true);
        cover.SetActive(false);
        main.SetActive(true);
    }

    private void Update()
    {
        if (cover.activeSelf && Controller.InputController.input.AnyButton())
        {
            GoToMainMenu();
        }
    }
    public void StartGame()
    {
        ToggleMenu(false);
        game.StartLevel();
    }

    public void DoExit()
    {
        cover.SetActive(false);
        main.SetActive(false);
        game.Exit();
    }

    public void ToggleMenu(bool visibility)
    {
        this.gameObject.SetActive(visibility);
    }
}
