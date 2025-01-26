using UnityEngine;

public class BubbleDestroyer : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bubble"))
        {
            Destroy(collision.gameObject);
        }
    }
}
