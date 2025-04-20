using TMPro;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class ResultStatsWidget : ProgressWidget
    {
        private UnityEngine.Gradient _gradient;

        public void Init()
        {
            _gradient = new UnityEngine.Gradient();
            var colors = new[]
            {
                new GradientColorKey(Color.red, 0.0f),
                new GradientColorKey(Color.yellow, 0.5f),
                new GradientColorKey(Color.green, 1.0f)
            };
            
            _gradient.SetKeys(colors, new []{ new GradientAlphaKey(1.0f, 0.0f)});
        }

        public void SetButtonProgress(float value)
        {
            if (_gradient == null)
                Init();
            
            SetButtonProgress01(value);
            Color = _gradient.Evaluate(value);
        }
        
        public void SetProgress(float value, TMP_Text progressLabel)
        {
            if (_gradient == null)
                Init();
            
            SetProgressWithAnimation(value, progressLabel);
            Color = _gradient.Evaluate(value);
        }
    }
}