using DG.Tweening;
using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class MatchingAnswerAreaWidget : Widget
    {
        [field: SerializeField] public LeanBox Background { get; private set; }
        [field: SerializeField] public MatchingAnswerWidget AnswerVariant { get; private set; }
        [field: SerializeField] public ThemeColorVariant EnteredColor { get; private set; } = ThemeColorVariant.Accent;
        [field: SerializeField] public Color RightColor { get; private set; }
        [field: SerializeField] public Color WrongColor { get; private set; }

        private Sequence _changeColorSequence;
        private bool _isResult;
        
        private MatchingAnswerWidget _currentMatchingAnswerWidget;
        
        public MatchingAnswerWidget CurrentMatchingAnswerWidget
        {
            get => _currentMatchingAnswerWidget;
            set
            {
                _currentMatchingAnswerWidget = value;
                HidePreview();
                
                if (_currentMatchingAnswerWidget == null)
                    return;
                
                var position = _currentMatchingAnswerWidget.transform.position;
                position.y = transform.position.y;
                _currentMatchingAnswerWidget.transform.position = position;
            }
        }

        public void SetAsEntered()
        {
            var themeLibrary = App.StaticData.ThemeLibrary;
            var themeAsset = themeLibrary.GetAssetByIndex(PlayerPrefsProvider.CurrentThemeIndex);
            var variant = themeAsset.GetColorByVariant(EnteredColor);

            if (_changeColorSequence.IsActive())
                _changeColorSequence.Kill();
            
            _changeColorSequence = DOTween.Sequence()
                .Append(Background.DOColor(variant, 0.2f));
        }

        public void SetAsDefault()
        {
            if (_isResult)
                return;
            
            var themeColor = Background.GetComponent<ThemeColor>();
            var themeLibrary = App.StaticData.ThemeLibrary;
            var themeAsset = themeLibrary.GetAssetByIndex(PlayerPrefsProvider.CurrentThemeIndex);
            var variant = themeAsset.GetColorByVariant(themeColor.ColorVariant);
            
            if (_changeColorSequence.IsActive())
                _changeColorSequence.Kill();
            
            _changeColorSequence = DOTween.Sequence()
                .Append(Background.DOColor(variant, 0.2f));
        }

        public void SetAsRight()
        {
            if (_changeColorSequence.IsActive())
                _changeColorSequence.Kill();
            
            Background.color = RightColor;
        }

        public void SetAsWrong()
        {
            if (_changeColorSequence.IsActive())
                _changeColorSequence.Kill();

            Background.color = WrongColor;
        }

        public void SetAsResult()
        {
            _isResult = true;
        }

        public void ShowPreview(string title)
        {
            AnswerVariant.Title.text = title;
            AnswerVariant.Background.DOFade(.6f, .2f);
        }
        
        public void HidePreview()
        {
            AnswerVariant.Title.text = string.Empty;
            AnswerVariant.Background.DOFade(0, .2f);
        }
    }
}