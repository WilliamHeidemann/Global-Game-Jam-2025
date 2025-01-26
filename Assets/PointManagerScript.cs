using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManagerScript : MonoBehaviour
{
    public int points = 0;
    public TextMeshPro scoreText;
    private Timer _timer;

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
            switch (bubble)
            {
                case BubbleTypeEnum.Yellow:
                case BubbleTypeEnum.Green:
                    AddPoints(10);
                    break;
                case BubbleTypeEnum.Purple:
                case BubbleTypeEnum.Orange:
                    AddPoints(15);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
}