using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class AnswerVariantButtonWidget : ButtonWidget
    {
        [field: SerializeField] public LeanToggle LeanToggle { get; private set; }
        [field: SerializeField] public LeanBox Background { get; private set; }
        [field: SerializeField] public Color SelectedColor { get; private set; } = Color.yellow;
        [field: SerializeField] public Color RightColor { get; private set; } = Color.green;
        [field: SerializeField] public Color WrongColor { get; private set; } = Color.red;
        
        public string AnswerKey { get; set; }
        public bool IsSelected { get; private set; }

        private Color _defaultColor;

        public void Init()
        {
            _defaultColor = Background.color;
        }
            
        public void SetAsSelected()
        {
            IsSelected = true;
            Background.color = SelectedColor;
        }
        
        public void SetAsUnselected()
        {
            IsSelected = false;
            Background.color = _defaultColor;
        }

        public void SetAsRight()
        {
            IsSelected = false;
            Background.color = RightColor;
        }

        public void SetAsWrong()
        {
            IsSelected = false;
            Background.color = WrongColor;
        }
    }
}