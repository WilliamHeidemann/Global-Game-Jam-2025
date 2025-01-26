using System;
using UnityEngine;
using UnityEngine.UI;
using static PointManagerScript;

public class GoalScript : MonoBehaviour
{
    private PointManagerScript _pointManager;
    [SerializeField] private BubbleTypeEnum _type;
    [SerializeField] private bool _isTrashHole;

    private void Awake()
    {
        _pointManager = FindFirstObjectByType<PointManagerScript>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BubbleType bubbleType))
        {
            _pointManager.CalculateScore(bubbleType.Type, _type, _isTrashHole);
        }
    }
}