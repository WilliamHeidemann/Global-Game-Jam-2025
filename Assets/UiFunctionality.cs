using System;
using UnityEngine;

public class UiFunctionality : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    
    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    }
}
