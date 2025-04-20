using System;
using System.Collections.Immutable;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Lean.Gui;

namespace ExerciseSystem
{
    public sealed class CountRightResultWidget : ExerciseResultWidget, IExercisePresenter<CountRightExerciseResult>
    {
        [field: SerializeField] public TextMeshProUGUI PercentLabel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI CountRightLabel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI CurrentTryTimeLabel { get; private set; }
        [field: SerializeField] public ScrollRect QuestionsScrollView { get; private set; }
        [field: SerializeField] public LeanToggle QuestionsToggle { get; private set; }

        private ImmutableList<EnterAnswerQuestionWidget> _enterQuestions;
        private ImmutableList<ChooseAnswerExerciseQuestionWidget> _chooseAnswerQuestions;
        private ImmutableList<MatchingQuestionBlockWidget> _matchingQuestions;
        private ImmutableList<SelectingQuestionWidget> _selectingQuestions;
        
        public void CreateView(CountRightExerciseResult result)
        {
            _enterQuestions = ImmutableList<EnterAnswerQuestionWidget>.Empty;
            _chooseAnswerQuestions = ImmutableList<ChooseAnswerExerciseQuestionWidget>.Empty;
            _matchingQuestions = ImmutableList<MatchingQuestionBlockWidget>.Empty;
            _selectingQuestions = ImmutableList<SelectingQuestionWidget>.Empty;
            
            QuestionsScrollView.content.SetSizeDeltaY(0f);
            QuestionsScrollView.content.SetAnchoredPositionY(0f);
                
            SetCountRight(result.RightAnswersCount, result.QuestionsCount);
            var percentage = result.RightAnswersCount / (float) result.QuestionsCount;
            SetProgress(percentage);
            SetCurrentTryTime();
            var bestResult = result.BestResult / (float) result.QuestionsCount;
            SetBestResult(bestResult);
            SetTriesCount(result.TriesCount);
            SetBestTime(result.BestTime);
        }

        public void DestroyView()
        {
            QuestionsToggle.TurnOff();
            
            foreach (var question in _enterQuestions) 
                Destroy(question.gameObject);
            
            foreach (var question in _chooseAnswerQuestions) 
                Destroy(question.gameObject);
            
            foreach (var question in _matchingQuestions) 
                Destroy(question.gameObject);
            
            foreach (var question in _selectingQuestions) 
                Destroy(question.gameObject);
        }

        public void ShowLatinTranslationQuestions(ImmutableList<EnterAnswerQuestionWidget> questions)
        {
            foreach (var question in questions)
            {
                var questionWidget = Instantiate(question, QuestionsScrollView.content);
                questionWidget.gameObject.SetActive(true);
                questionWidget.transform.AsRectTransform().sizeDelta = new Vector2(0f, 370f);
                
                questionWidget.Button.OnClick.AddListener(() => 
                {
                    foreach (var latinQuestionWidget in _enterQuestions)
                    {
                        latinQuestionWidget.SetAsUnselected();
                    }
                    questionWidget.SetAsSelected();
                    question.SetContext();
                });
                
                questionWidget.SetAsHasAnswer();
                questionWidget.SetAsResult();
                if (question.IsAnswerRight())
                    questionWidget.SetAsRightAnswer();
                else
                    questionWidget.SetAsWrongAnswer();
                
                _enterQuestions = _enterQuestions.Add(questionWidget);
            }
            
            var size = _enterQuestions.Count * 370f;
            QuestionsScrollView.content.SetSizeDeltaY(size + 100f);
            questions[0].SetSceneDefaultContext();
        }
        
        public void ShowChooseAnswerQuestions(ImmutableList<ChooseAnswerExerciseQuestionWidget> questions)
        {
            foreach (var question in questions)
            {
                question.SetAsResult();
                var questionWidget = Instantiate(question, QuestionsScrollView.content);
                questionWidget.gameObject.SetActive(true);
                questionWidget.transform.AsRectTransform().sizeDelta = new Vector2(0f, 520f);
                questionWidget.Button.gameObject.GetComponent<LeanBox>().enabled = true;
                questionWidget.Button.enabled = true;
                questionWidget.Button.interactable = true;
                questionWidget.Button.OnClick.AddListener(() => 
                {
                    foreach (var chooseQuestionWidget in _chooseAnswerQuestions)
                    {
                        chooseQuestionWidget.ResultLeanToggle.On = false;
                    }
                    questionWidget.ResultLeanToggle.On = true;
                    question.SetContext();
                });
                
                if (question.IsAnswerRight())
                    questionWidget.SetAsRightAnswer();
                else
                    questionWidget.SetAsWrongAnswer();
                
                _chooseAnswerQuestions = _chooseAnswerQuestions.Add(questionWidget);
            }

            var size = _chooseAnswerQuestions.Count * 520f;
            QuestionsScrollView.content.SetSizeDeltaY(size + 100f);
            questions[0].SetSceneDefaultContext();
        }
        
        public void ShowMatchingQuestions(MatchingQuestionWidget questions)
        {
            questions.SetAsResult();
            var questionBlocks = questions.QuestionBlocks;
            var color = questionBlocks[0].AnswerArea.AnswerVariant.Background.color;
            color.a = 1f;
            
            foreach (var questionBlock in questionBlocks)
            {
                var question = Instantiate(questionBlock, QuestionsScrollView.content);
                var answer = questionBlock.AnswerArea.CurrentMatchingAnswerWidget.Answer;
                question.AnswerArea.AnswerVariant.Title.SetTextWithoutLinkTag(answer);
                question.AnswerArea.AnswerVariant.Background.color = color;
                
                if (questionBlock.IsRightAnswered)
                    question.SetAsRight();
                else
                    question.SetAsWrong();
                
                question.ContextButton.OnClick.AddListener(() =>
                {
                    foreach (var blockWidget in _matchingQuestions)
                    {
                        blockWidget.LeanToggle.On = false;
                    }
                    question.LeanToggle.On = true;
                
                    App.ObserverAPI.SetContext(questionBlock.questionContextId);
                });
                _matchingQuestions = _matchingQuestions.Add(question);

                question.LeanToggle.On = false;
            }
            
            QuestionsScrollView.content.SetSizeDeltaY(_matchingQuestions.Count * 149f);
            
            var domainLibrary = App.StaticData.DomainLibrary;
            var context = domainLibrary.ContextLibrary.GetContext(questions.QuestionBlocks[0].questionContextId);
            var scene = domainLibrary.CourseLibrary.GetSceneById(context.SceneId);
                
            App.ObserverAPI.SetContext(scene.DefaultContextId);
        }
        
        public void ShowSelectingQuestions(ImmutableList<SelectingQuestionWidget> questions)
        {
            foreach (var question in questions)
            {
                var questionWidget = Instantiate(question, QuestionsScrollView.content);
                questionWidget.gameObject.SetActive(true);
                questionWidget.transform.AsRectTransform().sizeDelta = new Vector2(0f, 300f);
                
                questionWidget.Button.OnClick.AddListener(() =>
                {
                    foreach (var selectingQuestion in _selectingQuestions)
                    {
                        selectingQuestion.SetAsUnselected();
                    }
                    questionWidget.SetAsSelected();
                    
                    if (!string.IsNullOrWhiteSpace(question.SelectedContextId))
                        App.ObserverAPI.SetContext(question.SelectedContextId);
                });
                
                questionWidget.SetAsHasAnswer();
                questionWidget.SetAsResult();
                if (question.IsAnswerRight())
                {
                    questionWidget.SetAsRightAnswer();
                }
                else
                {
                    questionWidget.SetAsWrongAnswer();
                    
                    var currentSceneId = App.RuntimeData.ApplicationStateModel.CurrentSceneId;
                    var poIs = App.StaticData.DomainLibrary.PoILibrary.GetPoIsBySceneId(currentSceneId);
                    var contextId = string.Empty;
            
                    foreach (var poi in poIs)  
                    {
                        if (poi.SceneElementId != question.RightAnswer)
                            continue;
                
                        contextId = poi.ContextId;
                        break;
                    }
            
                    questionWidget.ShowAnswerButton.gameObject.SetActive(true);
                    questionWidget.ShowAnswerButton.OnClick.AddListener(() =>
                    {
                        foreach (var selectingQuestion in _selectingQuestions)
                        {
                            selectingQuestion.SetAsUnselected();
                        }
                        questionWidget.SetAsSelected();
                        
                        App.ObserverAPI.SetContext(contextId);
                    });
                }

                _selectingQuestions = _selectingQuestions.Add(questionWidget);
            }
            
            QuestionsScrollView.content.SetSizeDeltaY(_selectingQuestions.Count * 308f);
            App.ObserverAPI.ResetHighlight();
            
            var domainLibrary = App.StaticData.DomainLibrary;
            var sceneId = App.RuntimeData.ApplicationStateModel.CurrentSceneId;
            var scene = domainLibrary.CourseLibrary.GetSceneById(sceneId);
            App.ObserverAPI.SetContext(scene.DefaultContextId);
        }
        
        private void SetCountRight(int rightAnswers, int total)
        {
            CountRightLabel.text = $"{rightAnswers}/{total}";
        }
        
        private void SetProgress(float value)
        {
            ProgressWidget.SetProgress(value, PercentLabel);
            //PercentLabel.text = $"{value * 100:0}%";
        }

        private void SetCurrentTryTime()
        {
            var currentTime = App.StaticData.Scene.MainSandbox.ExerciseTimer.CurrentTime;
            var timeSpan = TimeSpan.FromSeconds(currentTime);
            CurrentTryTimeLabel.SetMonoText($"@UIElements/Exercise/CurrentTryTime {timeSpan:mm\\:ss}");
        }
    }
}