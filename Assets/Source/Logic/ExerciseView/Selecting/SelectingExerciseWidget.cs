using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Zun010.LeoEcsExtensions;
using System.Collections.Immutable;

namespace ExerciseSystem
{
    public sealed class SelectingExerciseWidget : ExerciseWidget, IExercisePresenter<SelectingExerciseData>
    {
        [field: SerializeField] public ScrollRect SelectingScrollView { get; private set; }
        [field: SerializeField] public SliderSceneElementsActionSandbox SliderSceneElementsAction { get; private set; }
        [field: SerializeField] public Transform FooterContainer { get; private set; }
        
        [Header("Prefabs")]
        [SerializeField] private SelectingQuestionWidget selectingQuestionWidgetPrefab;
        [SerializeField] private ExerciseInProgressFooterWidget exerciseFooterWidgetPrefab;

        public ImmutableList<SelectingQuestionWidget> Questions { get; private set; }
        
        private ExerciseInProgressFooterWidget _footer;
        private string _exerciseSceneId;
        private int _currentQuestionIndex;
        
        public void CreateView(SelectingExerciseData data)
        {
            OpenExerciseWidget();
            
            Questions = ImmutableList<SelectingQuestionWidget>.Empty;
            _exerciseSceneId = data.SceneId;
            _currentQuestionIndex = 0;
            
            CreateQuestions(data);
            ShowPoI(data);
            
            _footer = Instantiate(exerciseFooterWidgetPrefab, FooterContainer);
            _footer.NextButton.OnClick.AddListener(OnNextQuestion);
            _footer.PreviousButton.OnClick.AddListener(OnPreviousQuestion);
            _footer.FinishButton.OnClick.AddListener(OnExerciseFinish);
            App.RuntimeData.UserInterfaceModel.ForceApplyTheme(_footer.gameObject);
            
            ShowSlider();
        }

        private void CreateQuestions(SelectingExerciseData exerciseData)
        {
            var questionsAnswers = exerciseData.QuestionsAnswers;

            for (var i = 0; i < questionsAnswers.Length; i++)
            {
                var questionWidget = Instantiate(selectingQuestionWidgetPrefab, SelectingScrollView.content);
                questionWidget.Init(questionsAnswers[i]);
                Questions = Questions.Add(questionWidget);
                
                if (i > 0)
                    questionWidget.gameObject.SetActive(false);
            }
        }
        
        private void SelectElement(string selectedElementId, string sceneId)
        {
            var observerAPI = App.ObserverAPI;
            if (string.IsNullOrWhiteSpace(selectedElementId))
            {
                observerAPI.ResetHighlight();

                var scene = App.StaticData.DomainLibrary.CourseLibrary.GetSceneById(sceneId);
                observerAPI.SetContext(scene.DefaultContextId);
                    
                return;
            }

            var poIs = App.StaticData.DomainLibrary.PoILibrary.GetPoIsBySceneId(sceneId);
                
            foreach (var poi in poIs)  
            {
                if (poi.SceneElementId != selectedElementId)
                    continue;
                    
                App.ObserverAPI.SetContext(poi.ContextId);
                App.RuntimeData.ApplicationStateModel.HighlightedSceneElementId = poi.SceneElementId;
                return;
            }
        }
        
        public void SetSelectedId(string id)
        {
            if (Questions[_currentQuestionIndex] == null)
                return;
            
            Questions[_currentQuestionIndex].SelectedId = id;
            Questions[_currentQuestionIndex].SelectedContextId = App.RuntimeData.ApplicationStateModel.CurrentContextId;
            Questions[_currentQuestionIndex].SetAsHasAnswer();
        }
        
        private void OnNextQuestion()
        {
            Questions[_currentQuestionIndex].SetAsHasAnswer();
            
            Questions[_currentQuestionIndex].gameObject.SetActive(false);
            _currentQuestionIndex += 1;
            Questions[_currentQuestionIndex].gameObject.SetActive(true);

            SelectElement(Questions[_currentQuestionIndex].SelectedId, _exerciseSceneId);
            SliderSceneElementsAction.ActionSliderWidget.Slider.value = 0;

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
            
            SelectElement(Questions[_currentQuestionIndex].SelectedId, _exerciseSceneId);
            SliderSceneElementsAction.ActionSliderWidget.Slider.value = 0;
            
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
            
            exerciseViewSandbox.FlexBoxWidget.SetValueSmoothly(0.23f);
            exerciseViewSandbox.CurrentExerciseViewSize = 0.23f;
            exerciseViewSandbox.SelectingExerciseWidget.Show();
        }

        private void ShowSlider()
        {
            var entity = App.ObserverAPI.GetCurrentSceneEntity();
            var actionsComponent = entity.Req<ActionsComponent>();
            var actions = actionsComponent.Actions;

            var sliderActions = actions.OfType<SliderSceneElementsAction>().Count();

            if (sliderActions == 0)
            {
                SelectingScrollView.transform.AsRectTransform().offsetMin = new Vector2(0f, 100f);
                SliderSceneElementsAction.gameObject.SetActive(false);
                return;
            }
            
            
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            
            exerciseViewSandbox.FlexBoxWidget.SetValueSmoothly(0.32f);
            exerciseViewSandbox.CurrentExerciseViewSize = 0.32f;
            SelectingScrollView.transform.AsRectTransform().offsetMin = new Vector2(0f, 220f);
            SliderSceneElementsAction.gameObject.SetActive(true);
        }
        
        private void ShowPoI(SelectingExerciseData data)
        {
            App.RuntimeData.ApplicationStateModel.CurrentSceneId = data.SceneId;
            App.RuntimeData.UserInterfaceModel.Hub.CurrentSceneId = data.SceneId;
            
            var scene = App.StaticData.DomainLibrary.CourseLibrary.GetSceneById(data.SceneId);
            App.ObserverAPI.SetContext(scene.DefaultContextId);
            var poIContainer = App.StaticData.Scene.MainSandbox.ObserverSandbox.PoIContainer;
            poIContainer.ShowAll();
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