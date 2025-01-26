using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UiFunctionality : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPause;
    
    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _onPause?.Invoke();
        }
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(0);
    }
}
