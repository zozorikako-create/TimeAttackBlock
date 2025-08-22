using UnityEngine;
using System;

namespace TimeAttackBlock
{
    public class TimerManager : MonoBehaviour
    {
        public GameConfig config;
        public float CurrentTime { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<float> OnTimeChanged;
        public event Action OnTimeUp;

        void Awake()
        {
            if (config == null)
            {
                Debug.LogWarning("GameConfig 未設定。仮で100秒を使用します。");
                CurrentTime = 100f;
            }
            else
            {
                CurrentTime = Mathf.Min(config.initialTime, config.maxTime);
            }
            IsRunning = true;
            OnTimeChanged?.Invoke(CurrentTime);
        }

        void Update()
        {
            if (!IsRunning) return;
            CurrentTime -= Time.deltaTime;
            if (CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                IsRunning = false;
                OnTimeChanged?.Invoke(CurrentTime);
                OnTimeUp?.Invoke();
            }
            else
            {
                OnTimeChanged?.Invoke(CurrentTime);
            }
        }

        public void AddTime(int sec)
        {
            var max = (config != null) ? config.maxTime : 100f;
            CurrentTime = Mathf.Min(CurrentTime + sec, max);
            OnTimeChanged?.Invoke(CurrentTime);
        }
    }
}