using UnityEngine;
using System;

namespace TimeAttackBlock
{
    public class ScoreManager : MonoBehaviour
    {
        public ScoreConfig config;
        public int Score { get; private set; }
        public int BestScore { get; private set; }

        public event Action<int> OnScoreChanged;
        public event Action<int> OnBestUpdated;

        void Start()
        {
            BestScore = SaveManager.Load().bestScore;
        }

        public void Add(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
            if (Score > BestScore)
            {
                BestScore = Score;
                var data = SaveManager.Load();
                data.bestScore = BestScore;
                SaveManager.Save(data);
                OnBestUpdated?.Invoke(BestScore);
            }
        }
    }
}