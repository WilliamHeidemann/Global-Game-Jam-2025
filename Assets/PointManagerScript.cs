using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManagerScript : MonoBehaviour
{
    public int points = 0;
    public TextMeshPro scoreText;
    private Timer _timer;

    [SerializeField] private TextMeshProUGUI _finalScoreText;
    
    private void Awake()
    {
        _timer = FindFirstObjectByType<Timer>();
    }

    private void Start()
    {
        AddPoints(0);
    }

    public void CalculateScore(BubbleTypeEnum bubble, BubbleTypeEnum hole, bool isTrashHole)
    {
        if (isTrashHole)
        {
            AddPoints(1);
            SoundManager.instance.PlaySoundEffect(5);
            return;
        }

        if (bubble == hole)
        {
            _timer.AddTime(5);
            SoundManager.instance.PlaySoundEffect(3);
            AddPoints(10);
        }
        else
        {
            AddPoints(2);
            SoundManager.instance.PlaySoundEffect(4);
        }
    }

    private void AddPoints(int amount)
    {
        points += amount;
        scoreText.text = "Score: " + points.ToString();
    }

    public void SetPointFinalText()
    {
        _finalScoreText.text = "Score: " + points.ToString();
    }
}