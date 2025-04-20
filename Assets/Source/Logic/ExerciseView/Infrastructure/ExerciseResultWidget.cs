using System;
using UnityEngine;
using TMPro;

namespace ExerciseSystem
{
    public class ExerciseResultWidget : ExerciseWidget
    {
        [field: SerializeField] public ResultStatsWidget ProgressWidget { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TriesCountLabel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI BestResultLabel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI BestTimeLabel { get; private set; }
        [field: SerializeField] public Transform LoadingTransform { get; private set; }
        [field: SerializeField] public Transform ProgressTransform { get; private set; }
        [field: SerializeField] public Transform ErrorTransform { get; private set; }

        protected void SetBestResult(float value)
        {
            BestResultLabel.SetMonoText($"@UIElements/Exercise/BestResult {value * 100:0}%");
        }

        protected void SetBestTime(float value)
        {
            var timeSpan = TimeSpan.FromSeconds(value);
            BestTimeLabel.SetMonoText($"@UIElements/Exercise/BestTime {timeSpan:mm\\:ss}");
        }
        
        protected void SetTriesCount(int value)
        {
            TriesCountLabel.SetMonoText($"@UIElements/Exercise/TriesCount {value}");
        }
    }
}