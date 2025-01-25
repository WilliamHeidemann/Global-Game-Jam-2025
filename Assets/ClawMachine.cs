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

    private bool _isAtTop = true;
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
        _initialDepth = _clawMachine.position.y;
        _targetDepth = _initialDepth - _dropDepth;
        
        _moveInputAction = InputSystem.actions.FindAction("Move");
        _interactInputAction = InputSystem.actions.FindAction("Jump");
        
        Release();
    }

    private void Update()
    {
        if (_isAtTop)
        {
            TranslateClaw();
        }

        if (_interactInputAction.triggered && _isAtTop)
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

    private void LowerClaw()
    {
        _isAtTop = false;
        _clawMachine.position = _clawMachine.position.With(y: _initialDepth);
        _clawMachine.DOMoveY(_targetDepth, 1f)
            .OnComplete(async () =>
            {
                await Awaitable.WaitForSecondsAsync(.5f);
                if (!_clawTrigger.hasBubbles)
                {
                    Grab();
                }
                await Awaitable.WaitForSecondsAsync(1f);
                RaiseClaw();
            });
    }

    private void RaiseClaw()
    {
        _clawMachine.position = _clawMachine.position.With(y: _targetDepth);
        _clawMachine.DOMoveY(_initialDepth, 1f)
            .OnComplete(async () =>
            {
                if (!_clawTrigger.hasBubbles)
                {
                    await Awaitable.WaitForSecondsAsync(.5f);
                    Release();
                }
                _isAtTop = true;
            });
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