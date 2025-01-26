using System;
using DG.Tweening;
using UnityEngine;

public class StickMover : MonoBehaviour
{
    public Transform Point1;
    public Transform Point2;

    private bool goingForward;

    private void Start()
    {
        Move();
    }

    private void Move()
    {
        var endPoint = goingForward ? Point1 : Point2;
        transform.DOMove(endPoint.position, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            goingForward = !goingForward;
            Move();
        });
    }
    
    
}
