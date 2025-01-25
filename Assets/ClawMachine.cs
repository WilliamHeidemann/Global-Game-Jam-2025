using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtils;

public class ClawMachine : MonoBehaviour
{
    [SerializeField] private Transform[] _hinges;
    private Quaternion[] _initialRotations;
    private Quaternion[] _targetRotations;
    
    [SerializeField] private Rigidbody _clawMachine;
    [SerializeField] private ClawTrigger _clawTrigger;
    private float _initialPipePosition;
    private float _targetPipePosition;
    private bool hasReachedTop => Mathf.Abs(_clawMachine.position.y - _initialPipePosition) < 0.1f;
    private bool hasReachedBottom => Mathf.Abs(_clawMachine.position.y - _targetPipePosition) < 0.1f;
    private InputAction _moveInputAction;
    private InputAction _interactInputAction;

    [SerializeField] private float _translationSpeed;
    [SerializeField] private float _dropDepth;
    [SerializeField] private float _pressure;

    public AudioSource soundEffectSource;

    private void Start()
    {
        _initialRotations = new Quaternion[_hinges.Length];
        for (int i = 0; i < _hinges.Length; i++)
        {
            _initialRotations[i] = _hinges[i].rotation;
        }

        _targetRotations = new Quaternion[_hinges.Length];
        for (int i = 0; i < _hinges.Length; i++)
        {
            _targetRotations[i] = _initialRotations[i] * Quaternion.Euler(_pressure, 0, 0);
        }

        _initialPipePosition = _clawMachine.position.y;
        _targetPipePosition = _initialPipePosition - _dropDepth;

        _moveInputAction = InputSystem.actions.FindAction("Move");
        _interactInputAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        if (hasReachedTop)
        {
            TranslateClaw();
        }

        if (_interactInputAction.triggered && hasReachedTop)
        {
            if (_clawTrigger.hasBubbles)
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
            _clawMachine.position = Vector3.MoveTowards(currentPosition, currentPosition.With(y: _targetPipePosition),
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
            _clawMachine.position = Vector3.MoveTowards(currentPosition, currentPosition.With(y: _initialPipePosition),
                Time.deltaTime);
            await Awaitable.NextFrameAsync();
        }

        if (!_clawTrigger.hasBubbles)
        {
            await Awaitable.WaitForSecondsAsync(.5f);
            Release();
        }
    }

    public async void Release()
    {
        while (!IsNotGrabbing())
        {
            for (var i = 0; i < _hinges.Length; i++)
            {
                _hinges[i].rotation = Quaternion.RotateTowards(_hinges[i].rotation, _initialRotations[i], 1f);
            }

            await Awaitable.NextFrameAsync();
        }
        
        bool IsNotGrabbing()
        {
            var angle = Quaternion.Angle(_hinges[0].rotation, _initialRotations[0]);
            return angle < 1f;
        }
    }

    public async void Grab()
    {
        while (!IsGrabbing())
        {
            for (var i = 0; i < _hinges.Length; i++)
            {
                _hinges[i].rotation = Quaternion.RotateTowards(_hinges[i].rotation, _targetRotations[i], .3f);
            }

            await Awaitable.NextFrameAsync();
        }
        bool IsGrabbing()
        {
            var angle = Quaternion.Angle(_hinges[0].rotation, _targetRotations[0]);
            return angle < 1f;
        }
    }
}