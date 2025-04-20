using System.Collections.Immutable;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ExerciseSystem
{
    public sealed class EnterAnswerExerciseWidget : ExerciseWidget, IExercisePresenter<EnterAnswerExerciseData>
    {
        [field: SerializeField] public ScrollRect LatinExerciseScrollView { get; private set; }
        [field: SerializeField] public Transform FooterContainer { get; private set; }
        
        [FormerlySerializedAs("latinTranslationQuestionWidgetPrefab")]
        [Header("Prefabs")]
        [SerializeField] private EnterAnswerQuestionWidget enterAnswerQuestionWidgetPrefab;
        [SerializeField] private ExerciseInProgressFooterWidget exerciseFooterWidgetPrefab;

        public ImmutableList<EnterAnswerQuestionWidget> Questions { get; private set; }
        
        private ImmutableList<string> _contextIds;
        private ExerciseInProgressFooterWidget _footer;
        private int _currentQuestionIndex;
        
        public void CreateView(EnterAnswerExerciseData data)
        {
            OpenExerciseWidget();
            
            Questions = ImmutableList<EnterAnswerQuestionWidget>.Empty;
            _contextIds = ImmutableList<string>.Empty;
            _currentQuestionIndex = 0;
            CreateQuestions(data);
            ShowPoI(data);
            
            _footer = Instantiate(exerciseFooterWidgetPrefab, FooterContainer);
            _footer.NextButton.OnClick.AddListener(OnNextQuestion);
            _footer.PreviousButton.OnClick.AddListener(OnPreviousQuestion);
            _footer.FinishButton.OnClick.AddListener(OnExerciseFinish);

            App.RuntimeData.UserInterfaceModel.ForceApplyTheme(_footer.gameObject);
        }
        
        private void CreateQuestions(EnterAnswerExerciseData exerciseData)
        {
            var questionsAnswers = exerciseData.QuestionsAnswers;

            for (var i = 0; i < questionsAnswers.Length; i++)
            {
                var questionWidget = Instantiate(enterAnswerQuestionWidgetPrefab, LatinExerciseScrollView.content);
                questionWidget.Init(questionsAnswers[i]);
                
                Questions = Questions.Add(questionWidget);
                _contextIds = _contextIds.Add(questionsAnswers[i].ContextId);
                
                if (i > 0)
                    questionWidget.gameObject.SetActive(false);
            }
        }
        
        private void OnNextQuestion()
        {
            Questions[_currentQuestionIndex].SetAsHasAnswer();
            
            Questions[_currentQuestionIndex].gameObject.SetActive(false);
            _currentQuestionIndex += 1;
            Questions[_currentQuestionIndex].gameObject.SetActive(true);

            App.ObserverAPI.SetContext(_contextIds[_currentQuestionIndex]);
            var posIContainer = App.StaticData.Scene.MainSandbox.ObserverSandbox.PoIContainer;
            posIContainer.AddPoI(_contextIds[_currentQuestionIndex]);

            if (_currentQuestionIndex == 1)
            {
                _footer.PreviousButton.gameObject.SetActive(true);
                _footer.NextButton.transform.AsRectTransform().offsetMin = new Vector2(100, 10);
            }
            
            if (_currentQuestionIndex == Questions.Count - 1)
            {
                _footer.NextButton.gameObject.SetActive(false);
                _footer.FinishButton.gameObject.SetActive(true);
            }
            
            var exerciseViewSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            if (exerciseViewSandbox.InfoExerciseWidget.InfoContainer.gameObject.activeInHierarchy)
                exerciseViewSandbox.HideInfoWidget();
        }
        
        private void OnPreviousQuestion()
        {
            if (_currentQuestionIndex == Questions.Count - 1)
            {
                _footer.NextButton.gameObject.SetActive(true);
                _footer.FinishButton.gameObject.SetActive(false);
            }
            
            Questions[_currentQuestionIndex].gameObject.SetActive(false);
            _currentQuestionIndex -= 1;
            Questions[_currentQuestionIndex].gameObject.SetActive(true);
            
            App.ObserverAPI.SetContext(_contextIds[_currentQuestionIndex]);
            var posIContainer = App.StaticData.Scene.MainSandbox.ObserverSandbox.PoIContainer;
            posIContainer.AddPoI(_contextIds[_currentQuestionIndex]);
            
            if (_currentQuestionIndex < 1)
            {
                _footer.PreviousButton.gameObject.SetActive(false);
                _footer.NextButton.transform.AsRectTransform().offsetMin = new Vector2(0, 10);
            }
            
            var exerciseViewSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            if (exerciseViewSandbox.InfoExerciseWidget.InfoContainer.gameObject.activeInHierarchy)
                exerciseViewSandbox.HideInfoWidget();
        }

        private void OnExerciseFinish()
        {
            Questions[_currentQuestionIndex].SetAsHasAnswer();
            
            App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox.FinishExercise();
        }

        private void OpenExerciseWidget()
        {
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            
            exerciseViewSandbox.FlexBoxWidget.SetValueSmoothly(0.27f);
            exerciseViewSandbox.CurrentExerciseViewSize = 0.27f;
            exerciseViewSandbox.LatinTranslationExerciseWidget.Show();
        }

        private void ShowPoI(EnterAnswerExerciseData data)
        {
            var domainLibrary = App.StaticData.DomainLibrary;
            var context = domainLibrary.ContextLibrary.GetContext(data.QuestionsAnswers[0].ContextId);
            App.RuntimeData.ApplicationStateModel.CurrentSceneId = context.SceneId;
            App.RuntimeData.UserInterfaceModel.Hub.CurrentSceneId = context.SceneId;
            
            App.ObserverAPI.SetContext(data.QuestionsAnswers[0].ContextId);
            var posIContainer = App.StaticData.Scene.MainSandbox.ObserverSandbox.PoIContainer;
            posIContainer.AddPoI(data.QuestionsAnswers[0].ContextId);
            App.RuntimeData.UserInterfaceModel.Observer.CurrentPoIState = PoIState.Question;
        }
        
        public void DestroyView()
        {
            foreach (var question in Questions) 
                Destroy(question.gameObject);
            
            Destroy(_footer.gameObject);
        }
    }
}