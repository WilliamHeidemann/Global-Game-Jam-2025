using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UtilityToolkit.Runtime;

public class ClawTrigger : MonoBehaviour
{
    [SerializeField] private ClawMachine _clawMachine;
    private Option<Rigidbody> _bubble = Option<Rigidbody>.None;
    [CanBeNull] private Rigidbody _lastBubble;
    public bool hasBubbles => _bubble.IsSome(out _);

    private void Update()
    {
        if (_bubble.IsSome(out var bubble))
        {
            bubble.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBubbles)
        {
            return;
        }

        if (other.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            if (rigidBody == _lastBubble)
            {
                return;
            }

            rigidBody.GetComponent<SphereCollider>().isTrigger = true;
            rigidBody.isKinematic = true;
            rigidBody.transform.position = transform.position;
            _bubble = Option<Rigidbody>.Some(rigidBody);
            _clawMachine.Grab();
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Bubble"))
    //     {
    //         _bubbles.Remove(other);
    //         if (!hasBubbles)
    //         {
    //             _clawMachine.Release();
    //         }
    //     }
    // }

    public void ReleaseBubble()
    {
        if (!_bubble.IsSome(out Rigidbody bubble))
        {
            return;
        }

        bubble.GetComponent<SphereCollider>().isTrigger = false;
        bubble.isKinematic = false;
        _bubble = Option<Rigidbody>.None;
        _lastBubble = bubble;
    }
}