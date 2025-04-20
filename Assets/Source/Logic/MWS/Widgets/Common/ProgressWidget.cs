using DG.Tweening;
using Lean.Gui;
using TMPro;
using UnityEngine;

namespace ExerciseSystem
{
    public class ProgressWidget : Widget
    {
        [field: SerializeField] public LeanCircle DownloadProgressCircle { get; private set; }
        [field: SerializeField] public RectTransform LineEndDot { get;  private set; }
        [field: SerializeField] public LeanCircle LineStartDotCircle { get;  private set; }
        [field: SerializeField] public LeanCircle LineEndDotCircle { get;  private set; }

        public float Progress01 { get; private set; }

        public Color Color
        {
            get => DownloadProgressCircle.color;
            set
            {
                DownloadProgressCircle.color = value;
                LineStartDotCircle.color = value;
                LineEndDotCircle.color = value;
            }
        }
        
        public void ShowProgress()
        {
            gameObject.SetActive(true);
        }
        
        public void HideProgress()
        {
            gameObject.SetActive(false);
        }

        public void SetButtonProgress01(float value)
        {
            if (value is float.NaN or > 1f)
                return;
            
            LineEndDotCircle.gameObject.SetActive(value > 0);
            LineStartDotCircle.gameObject.SetActive(value > 0);
            DownloadProgressCircle.Fill = value;
            var eulerAngles = LineEndDot.localEulerAngles;
            eulerAngles.z = -360f * value;
            LineEndDot.localEulerAngles = eulerAngles;
            Progress01 = value;
        }

        protected void SetProgressWithAnimation(float value, TMP_Text progressLabel)
        {
            if (value is float.NaN or > 1f)
                return;
            
            LineEndDotCircle.gameObject.SetActive(value > 0);
            LineStartDotCircle.gameObject.SetActive(value > 0);
            
            var startValue = 0f;
            DOTween.To(() => startValue, x => 
                {
                    startValue = x;
                    DownloadProgressCircle.Fill = x;
                    
                    var eulerAngles = LineEndDot.localEulerAngles;
                    eulerAngles.z = -360f * x;
                    LineEndDot.localEulerAngles = eulerAngles;
                    progressLabel.text = $"{x * 100:0}%";
                    
                }, value, 1.5f)
                .SetEase(Ease.OutQuad);
            
            Progress01 = value;
        }
    }
}