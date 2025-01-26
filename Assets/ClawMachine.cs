using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtils;

public class ClawMachine : MonoBehaviour
{
    private Quaternion[] _initialRotations;
    private Quaternion[] _targetRotations;

    [SerializeField] private Rigidbody _clawMachine;
    [SerializeField] private ClawTrigger _clawTrigger;
    private float _initialY;
    private float _targetY;

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

    [SerializeField] private Rig _clawRig;

    private void Start()
    {
        _initialY = _clawMachine.position.y;
        _targetY = _initialY - _dropDepth;

        _moveInputAction = InputSystem.actions.FindAction("Move");
        _interactInputAction = InputSystem.actions.FindAction("Jump");

        _pistonBone.DOMoveY(_clawFingersEnd.position.y, .5f).SetEase(Ease.OutBounce);
    }

    private void Update()
    {
        if (!_isAtTop)
        {
            return;
        }
        
        TranslateClaw();

        if (!_interactInputAction.triggered)
        {
            return;
        }
        
        if (_isGrabbing)
        {
            Release();
        }
        else
        {
            LowerClaw();
        }
    }

    private void TranslateClaw()
    {
        var value = _moveInputAction.ReadValue<Vector2>();
        Vector3 translation = new Vector3(value.x, 0, value.y) * (Time.deltaTime * _translationSpeed);
        _clawMachine.position += translation;
        
        _clawMachine.position = _clawMachine.position.With(x: Mathf.Clamp(_clawMachine.position.x, -1, 1), z: Mathf.Clamp(_clawMachine.position.z, 0.36f, 1.06f));
        

        if (Mathf.Abs(translation.x) != 0.0f && !soundEffectSource.isPlaying)
        {
            SoundManager.instance.PlayClawMovementSound(0);
        }
        else if (Mathf.Abs(translation.z) != 0.0f && !soundEffectSource.isPlaying)
        {
            SoundManager.instance.PlayClawMovementSound(1);
        }

        bool isMoving = Mathf.Abs(translation.x) > 0.0f || Mathf.Abs(translation.z) > 0.0f;
        if (!isMoving && soundEffectSource.isPlaying)
        {
            soundEffectSource.Stop();
        }
    }

    private void LowerClaw()
    {
        _isAtTop = false;
        _clawMachine.position = _clawMachine.position.With(y: _initialY);
        _clawMachine.DOMoveY(_targetY, 1f)
            .OnComplete(async () =>
            {
                await Awaitable.WaitForSecondsAsync(.5f);
                if (!_clawTrigger.hasBubbles)
                {
                    Grab();
                }
                else
                {
                    RaiseClaw();
                }
            });
    }

    private void RaiseClaw()
    {
        _clawMachine.position = _clawMachine.position.With(y: _targetY);
        _clawMachine.DOMoveY(_initialY, 1f)
            .OnComplete(async () =>
            {
                if (!_clawTrigger.hasBubbles)
                {
                    await Awaitable.WaitForSecondsAsync(.5f);
                    Release();
                }
                else
                {
                    _isAtTop = true;
                }
            });
    }

    public void Grab()
    {
        _isGrabbing = true;
        _pistonBone.DOMoveY(_clawFingersStart.position.y, .5f)
            .SetEase(Ease.OutBounce);
        _clawRig.weight = 1f;
        DOTween.To(() => _clawRig.weight, x => _clawRig.weight = x, 0f, 1f)
            .SetEase(Ease.OutBounce)
            .OnComplete(RaiseClaw);
    }

    public void Release()
    {
        _pistonBone.DOMoveY(_clawFingersEnd.position.y, .5f).SetEase(Ease.OutBounce);
        _clawRig.weight = 0f;
        _clawTrigger.ReleaseBubble();
        DOTween.To(() => _clawRig.weight, x => _clawRig.weight = x, 1f, 1f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                _isAtTop = true;
                _isGrabbing = false;
            });
    }
}