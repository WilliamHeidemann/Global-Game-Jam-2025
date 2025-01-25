using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtils;

public class ClawMachine : MonoBehaviour
{
    private Quaternion[] _initialRotations;
    private Quaternion[] _targetRotations;
    
    [SerializeField] private Rigidbody _clawMachine;
    [SerializeField] private ClawTrigger _clawTrigger;
    private float _initialDepth;
    private float _targetDepth;
    private bool hasReachedTop => Mathf.Abs(_clawMachine.transform.localPosition.y - _initialDepth) < 0.1f;
    private bool hasReachedBottom => Mathf.Abs(_clawMachine.position.y - _targetDepth) < 0.1f;

    private bool _isGrabbing;
    
    private InputAction _moveInputAction;
    private InputAction _interactInputAction;

    [SerializeField] private float _translationSpeed;
    [SerializeField] private float _dropDepth;
    [SerializeField] private float _pressure;

    public AudioSource soundEffectSource;

    
    [SerializeField] private Transform _pistonBone;
    [SerializeField] private Transform _clawFingersStart;
    [SerializeField] private Transform _clawFingersEnd;
    
    private void Start()
    {
        _initialDepth = _clawMachine.transform.localPosition.y;
        _targetDepth = _initialDepth - _dropDepth;
        
        _moveInputAction = InputSystem.actions.FindAction("Move");
        _interactInputAction = InputSystem.actions.FindAction("Jump");
        
        Release();
    }

    private void Update()
    {
        if (hasReachedTop)
        {
            TranslateClaw();
        }

        if (_interactInputAction.triggered && hasReachedTop)
        {
            if (_isGrabbing)
            {
                Release();
            }
            else
            {
                LowerClaw();
            }
        }
    }

    private void TranslateClaw()
    {
        var value = _moveInputAction.ReadValue<Vector2>();
        Vector3 translation = new Vector3(value.x, 0, value.y) * (Time.deltaTime * _translationSpeed);
        _clawMachine.position += translation;

        if(Mathf.Abs(translation.x) != 0.0f && !soundEffectSource.isPlaying)
        {
            Debug.Log("Played effect 0");
            SoundManager.instance.PlaySoundEffect(0);
        } 
        else if (Mathf.Abs(translation.z) != 0.0f && !soundEffectSource.isPlaying)
        {
            Debug.Log("Played effect 1");
            SoundManager.instance.PlaySoundEffect(1);
        }

        bool isMoving = Mathf.Abs(translation.x) > 0.0f || Mathf.Abs(translation.z) > 0.0f;
        if (!isMoving && soundEffectSource.isPlaying)
        {
            Debug.Log("Stopping");
            soundEffectSource.Stop();
        }
    }

    private async void LowerClaw()
    {
        while (!hasReachedBottom && !_clawTrigger.hasBubbles)
        {
            Vector3 currentPosition = _clawMachine.position;
            _clawMachine.position = Vector3.MoveTowards(currentPosition, currentPosition.With(y: _targetDepth),
                Time.deltaTime);
            await Awaitable.NextFrameAsync();
        }


        await Awaitable.WaitForSecondsAsync(.5f);
        if (!_clawTrigger.hasBubbles)
        {
            Grab();
        }

        await Awaitable.WaitForSecondsAsync(1f);
        RaiseClaw();
    }

    private async void RaiseClaw()
    {
        while (!hasReachedTop)
        {
            Vector3 currentPosition = _clawMachine.position;
            _clawMachine.position = Vector3.MoveTowards(currentPosition, currentPosition.With(y: _initialDepth),
                Time.deltaTime);
            await Awaitable.NextFrameAsync();
        }

        if (!_clawTrigger.hasBubbles)
        {
            await Awaitable.WaitForSecondsAsync(.5f);
            Release();
        }
    }

    public void Grab()
    {
        _isGrabbing = true;
        _pistonBone.position = _clawFingersEnd.position;
        _pistonBone.DOMove(_clawFingersStart.position, .5f).SetEase(Ease.OutBounce);
    }

    public void Release()
    {
        _isGrabbing = false;
        _pistonBone.position = _clawFingersStart.position;
        _pistonBone.DOMove(_clawFingersEnd.position, .5f).SetEase(Ease.OutBounce);
    }
}