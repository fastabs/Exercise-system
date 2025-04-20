using Lean.Gui;
using TMPro;
using UnityEngine;

namespace ExerciseSystem
{
    public abstract class QuestionWidget : Widget
    {
        [field: SerializeField] public TextMeshProUGUI QuestionTitle { get; private set; }
        [field: SerializeField] public LeanButton Button { get; private set; }
        [field: SerializeField] public QuestionStateWidget QuestionStateWidget { get; private set; }
        [field: SerializeField] public LeanToggle LeanToggle { get; private set; }

        public QuestionState QuestionState { get; set; }
        
        private bool _isResultState;
        
        public abstract bool IsAnswerRight();

        public virtual void SetAsDefault()
        {
            if (!_isResultState)
                QuestionStateWidget.SetAsDefault();
        }

        public virtual void SetAsSelected()
        {
            LeanToggle.On = true;

            if (!_isResultState)
            {
                QuestionState = QuestionState.Answered;
                //QuestionStateWidget.SetAsSelected();
            }
        }

        public virtual void SetAsUnselected()
        {
            LeanToggle.On = true;
            LeanToggle.On = false;
        }

        public void SetAsHasAnswer()
        {
            if (!_isResultState)
            {
                QuestionState = QuestionState.Answered;
                QuestionStateWidget.SetAsHasAnswer();
            }
        }

        public virtual void SetAsResult()
        {
            _isResultState = true;
        }
        
        public virtual void SetAsRightAnswer()
        {
            QuestionStateWidget.SetAsRightAnswer();
        }

        public virtual void SetAsWrongAnswer()
        {
            QuestionStateWidget.SetAsWrongAnswer();
        }

    }
}