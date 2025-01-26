using UnityEngine;
using UnityEngine.UI;
using static PointManagerScript;

public class GoalScript : MonoBehaviour
{
    public PointManagerScript pointManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            Debug.Log("Goal!!");
            pointManager.AddPoints(1);
        }
    }
}