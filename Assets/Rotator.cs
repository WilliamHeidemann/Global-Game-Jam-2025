using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float _speed;

    void Start()
    {
        _speed = Random.Range(50f, 100f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * _speed, Space.World);
    }
}