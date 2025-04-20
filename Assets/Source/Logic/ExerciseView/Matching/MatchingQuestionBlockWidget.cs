using UnityEngine;
using Lean.Gui;
using TMPro;

namespace ExerciseSystem
{
    public sealed class MatchingQuestionBlockWidget : Widget
    {
        [field: SerializeField] public TextMeshProUGUI Title { get; private set; }
        [field: SerializeField] public LeanButton ContextButton { get; private set; }
        [field: SerializeField] public MatchingAnswerAreaWidget AnswerArea { get; private set; }
        [field: SerializeField] public QuestionStateWidget QuestionStateWidget { get; private set; }
        [field: SerializeField] public LeanToggle LeanToggle { get; private set; }

        public string questionContextId;
        public string RightAnswer { get; set; }

        public bool IsRightAnswered => AnswerArea.CurrentMatchingAnswerWidget.Answer == RightAnswer;

        public void SetAsRight()
        {
            AnswerArea.SetAsRight();
        }
        
        public void SetAsWrong()
        {
            AnswerArea.SetAsWrong();
        }
    }
}