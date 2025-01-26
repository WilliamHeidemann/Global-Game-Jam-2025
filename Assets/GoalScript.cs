using UnityEngine;
using UnityEngine.UI;
using static PointManagerScript;

public class GoalScript : MonoBehaviour
{
    private PointManagerScript pointManager;
    private PointTracker _pointsTracker;

    void Start()
    {
        _pointsTracker = new PointTracker();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            Debug.Log("Goal!!");
            _pointsTracker.AddPoints(1);
        }
    }
}
