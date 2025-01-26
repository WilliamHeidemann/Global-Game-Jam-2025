using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManagerScript : MonoBehaviour
{
    public int points = 0;
    public TextMeshPro scoreText;

    public void AddPoints(int amount)
    {
         points += amount;
         scoreText.text = "Score: " + points.ToString();
    }
}


