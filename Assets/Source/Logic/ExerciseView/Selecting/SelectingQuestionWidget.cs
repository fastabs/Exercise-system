using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class SelectingQuestionWidget : QuestionWidget
    {
        [field: SerializeField] public LeanTrigger NoAnswerTrigger { get; private set; }
        [field: SerializeField] public LeanButton ShowAnswerButton { get; private set; }
        
        public string SelectedId { get; set; }
        public string SelectedContextId { get; set; }
        public string RightAnswer { get; private set; }

        public void Init(SelectingExerciseData.QuestionAnswer questionAnswer)
        {
            QuestionTitle.SetMonoText(questionAnswer.Question);
            RightAnswer = questionAnswer.Answer;
        }
        
        public override bool IsAnswerRight()
        {
            return SelectedId == RightAnswer;
        }
    }
}