using System;
using System.Collections.Generic;
using UnityEngine;

public class ClawTrigger : MonoBehaviour
{
    [SerializeField] private ClawMachine _clawMachine;
    private readonly HashSet<Collider> _bubbles = new();
    public bool hasBubbles => _bubbles.Count > 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bubble"))
        {
            _bubbles.Add(other);
            _clawMachine.Grab();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bubble"))
        {
            _bubbles.Remove(other);
            if (!hasBubbles)
            {
                _clawMachine.Release();
            }
        }
    }
}