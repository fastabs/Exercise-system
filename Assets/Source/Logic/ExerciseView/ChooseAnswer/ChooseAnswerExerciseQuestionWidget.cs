using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Gui;
using UnityEngine;
using UnityEngine.Pool;

namespace ExerciseSystem
{
    public sealed class ChooseAnswerExerciseQuestionWidget : QuestionWidget
    {
        [field: SerializeField] public AnswerVariantButtonWidget AnswerVariantPrefab { get; private set; }
        [field: SerializeField] public RectTransform AnswerVariantsContainer { get; private set; }
        [field: SerializeField] public RectTransform AnswerArea { get; private set; }
        [field: SerializeField] public CanvasGroup AnswerAreaCanvasGroup { get; private set; }
        [field: SerializeField] public LeanTrigger NoAnswerTrigger { get; private set; }
        [field: SerializeField] public LeanToggle ResultLeanToggle { get; private set; }

        public bool HasAnswer { get; private set; }
        
        private ObjectPool<AnswerVariantButtonWidget> _answerVariantsPool;
        private List<AnswerVariantButtonWidget> _buttons;
        
        private string[] _answerVariants;
        private string[] _rightAnswers;
        private string _selectedAnswer;
        private string _questionContextId;
        
        private float _defaultAnswerAreaHeight;
        private float _maxAnswerAreaHeight;
        private float _buttonHeight;
        
        private Sequence _sequence;
        
        public void Init(ChooseAnswerExerciseData.QuestionAnswer questionAnswer)
        {
            _answerVariants = questionAnswer.AnswerVariants;
            _rightAnswers = questionAnswer.Answers;
            
            _questionContextId = questionAnswer.ContextId;
            
            InitPool();

            _buttons = new List<AnswerVariantButtonWidget>();

            foreach (var answerVariant in _answerVariants)
            {
                AddAnswerVariant(answerVariant);
            }
        }

        private void AddAnswerVariant(string answerKey)
        {
            var buttonWidget = _answerVariantsPool.Get();

            buttonWidget.Title.SetTextWithoutLinkTag(answerKey);
            buttonWidget.AnswerKey = answerKey;
            
            buttonWidget.Button.OnClick.AddListener(() =>
            {
                if (_selectedAnswer == answerKey)
                    return;
                
                _selectedAnswer = answerKey;
                HasAnswer = true;
                NoAnswerTrigger.gameObject.SetActive(false);
                foreach (var button in _buttons)
                    button.SetAsUnselected();
                
                buttonWidget.SetAsSelected();
                SetAsHasAnswer();
            });

            _buttons.Add(buttonWidget);
        }

        public override bool IsAnswerRight()
        {
            return _rightAnswers.Contains(_selectedAnswer, StringComparer.OrdinalIgnoreCase);
        }

        public override void SetAsResult()
        {
            base.SetAsResult();
            
            foreach (var buttonWidget in _buttons)
            {
                buttonWidget.Button.interactable = false;

                if (buttonWidget.AnswerKey == _rightAnswers[0])
                {
                    buttonWidget.SetAsRight();
                    QuestionState = QuestionState.RightAnswered;
                    continue;
                }
                
                if (buttonWidget.AnswerKey == _selectedAnswer && !IsAnswerRight())
                {
                    buttonWidget.SetAsWrong();
                    QuestionState = QuestionState.WrongAnswered;
                }
            }
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
        
        private void InitPool()
        {
            _answerVariantsPool = new ObjectPool<AnswerVariantButtonWidget>(
                createFunc: () =>
                {
                    var buttonWidget = Instantiate(AnswerVariantPrefab, AnswerVariantsContainer);
                    buttonWidget.Init();
                    return buttonWidget;
                },
                actionOnGet: widget =>
                {
                    widget.gameObject.SetActive(true);
                    widget.Title.text = string.Empty;
                    widget.Button.interactable = true;
                    widget.SetAsUnselected();
                },
                actionOnRelease: widget =>
                {
                    widget.Button.OnClick.RemoveAllListeners();
                    widget.gameObject.SetActive(false);
                },
                actionOnDestroy: widget => { Destroy(widget.gameObject); }
            );
        }
    }
}