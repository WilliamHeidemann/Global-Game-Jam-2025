using System;
using UnityEngine;
using UnityEngine.Events;

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
}
