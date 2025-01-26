using System;
using UnityEngine;
using UtilityToolkit.Runtime;

public class Timer : MonoBehaviour
{
    private CountdownTimer _timer;
    private float _initialScale;
    [SerializeField] private Gradient _gradient;
    private Material _material;
    [SerializeField] private float _time;
    private bool _hasStarted;
    
    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void Start()
    {
        _timer = new CountdownTimer(_time);
        _initialScale = transform.localScale.y;
        _material.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        if (!_hasStarted) return;
        float fraction = 1f - _timer.FractionDone;
        if (fraction < 0f) return;
        transform.localScale = new Vector3(transform.localScale.x, fraction / _initialScale, transform.localScale.z);
        Color color = _gradient.Evaluate(fraction);
        _material.SetColor("_EmissionColor", color);
    }

    public void AddTime(float secondsToAdd)
    {
        _timer.AddTime(secondsToAdd);
    }

    public void StartTimer()
    {
        _hasStarted = true;
    }
    
    public void StopTimer()
    {
        _hasStarted = false;
    }
}
