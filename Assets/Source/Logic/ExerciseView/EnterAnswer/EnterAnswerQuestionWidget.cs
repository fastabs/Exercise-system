using System;
using System.Linq;
using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class EnterAnswerQuestionWidget : QuestionWidget
    {
        [field: SerializeField] public InputFieldWidget AnswerInputField { get; private set; }
        [field: SerializeField] public LeanBox AnswerArea { get; private set; }
        
        
        private string _questionContextId;
        private string[] _rightAnswers;
        
        public void Init(EnterAnswerExerciseData.QuestionAnswer questionAnswer)
        {
            var localizationText = App.RuntimeData.Localizer.Localize(questionAnswer.Question);
            QuestionTitle.text = LocalizationText.FromJSON(localizationText).Text;
            _rightAnswers = questionAnswer.Answers;
            
            _questionContextId = questionAnswer.ContextId;
        }
        
        public override void SetAsResult()
        {
            base.SetAsResult();
            AnswerInputField.InputField.readOnly = true;
            AnswerArea.raycastTarget = false;
            Button.interactable = true;
            Button.OnClick.AddListener(SetAsSelected);
        }
        
        public override void SetAsRightAnswer()
        {
            base.SetAsRightAnswer();
            AnswerInputField.Visibility = StatusVisibility.Success;
        }
        
        public override void SetAsWrongAnswer()
        {
            base.SetAsWrongAnswer();
            AnswerInputField.Visibility = StatusVisibility.Error;
        }

        public void SetContext()
        {
            App.ObserverAPI.SetContext(_questionContextId);
        }

        public void SetSceneDefaultContext()
        {
            var domainLibrary = App.StaticData.DomainLibrary;
            var context = domainLibrary.ContextLibrary.GetContext(_questionContextId);
            var scene = domainLibrary.CourseLibrary.GetSceneById(context.SceneId);
                
            App.ObserverAPI.SetContext(scene.DefaultContextId);
        }

        public override bool IsAnswerRight()
        {
            return _rightAnswers.Contains(AnswerInputField.Text.Trim(), StringComparer.OrdinalIgnoreCase);
        }
    }
}