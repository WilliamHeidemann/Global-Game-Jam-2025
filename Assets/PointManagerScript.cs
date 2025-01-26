using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManagerScript : MonoBehaviour
{
    public class PointTracker
    {
        public TextMeshProUGUI _scoreText;
        public int Points { get; private set; } = 0;

        public void AddPoints(int amount)
        {
            Points += amount;
            _scoreText.text = "Score: " + Points.ToString();
        }
    }
}


