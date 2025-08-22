using UnityEngine;
using TMPro;

namespace TimeAttackBlock
{
    public class UIController : MonoBehaviour
    {
        [Header("Refs")]
        public TimerManager timer;
        public ScoreManager score;

        [Header("UI")]
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI bestText;

        void Start()
        {
            if (timer != null) timer.OnTimeChanged += OnTimeChanged;
            if (score != null)
            {
                score.OnScoreChanged += OnScoreChanged;
                score.OnBestUpdated += OnBestUpdated;
                bestText.text = $"BEST: {score.BestScore}";
            }
        }

        void OnDestroy()
        {
            if (timer != null) timer.OnTimeChanged -= OnTimeChanged;
            if (score != null)
            {
                score.OnScoreChanged -= OnScoreChanged;
                score.OnBestUpdated -= OnBestUpdated;
            }
        }

        void OnTimeChanged(float t)
        {
            timeText.text = $"TIME: {Mathf.CeilToInt(t)}";
        }

        void OnScoreChanged(int s)
        {
            scoreText.text = $"SCORE: {s}";
        }

        void OnBestUpdated(int b)
        {
            bestText.text = $"BEST: {b}";
        }
    }
}