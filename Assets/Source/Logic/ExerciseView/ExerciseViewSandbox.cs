using System.Linq;
using System.IO;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ExerciseSystem
{
    public sealed class ExerciseViewSandbox : Sandbox, ILeanWindowReference
    {
        [field: SerializeField] public LeanWindow LeanWindow { get; private set; }
        [field: SerializeField] public FlexBoxWidget FlexBoxWidget { get; private set; }
        
        [field: Header("Header")]
        [field: SerializeField] public TextMeshProUGUI ExerciseTypeLabel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ExerciseName { get; private set; }
        [field: SerializeField] public LeanButton InfoButton { get; private set; }
        [field: SerializeField] public ToggleWidget FavouriteToggle { get; private set; }
        
        [field: Header("ExerciseWidgets")]
        [field: SerializeField] public EnterAnswerExerciseWidget EnterAnswerExerciseWidget { get; private set; }
        [field: SerializeField] public ChooseAnswerExerciseWidget ChooseAnswerExerciseWidget { get; private set; }
        [field: SerializeField] public MatchingExerciseWidget MatchingExerciseWidget { get; private set; }
        [field: SerializeField] public SelectingExerciseWidget SelectingExerciseWidget { get; private set; }
        
        [field: Header("Infrastructure")]
        [field: SerializeField] public InfoExerciseWidget InfoExerciseWidget { get; private set; }
        [field: SerializeField] public CountRightResultWidget CountRightResultWidget { get; private set; }
        // [field: SerializeField] public TimeResultWidget TimeResultWidget { get; private set; }
        
        [field: Header("Footers")]
        [field: SerializeField] public ExerciseResultFooterWidget ExerciseResultFooterWidget { get; private set; }
        [field: SerializeField] public ExerciseStartFooterWidget ExerciseStartFooterWidget { get; private set; }

        public ExerciseType ExerciseType { get; private set; }
        public ExerciseState ExerciseState { get; private set; }
        public int QuestionsCount { get; private set; }
        public float CurrentExerciseViewSize { get; set; }

        private IExercise _currentExercise;
        private bool _isRestarted;

        public void StartEnterAnswerExercise(EnterAnswerExerciseData exerciseData)
        {
            HideAll();
            ExerciseType = ExerciseType.EnterAnswer;
            
            var presenters = new ExercisePresenters<EnterAnswerExerciseData, CountRightExerciseResult>
            {
                InfoPresenter = InfoExerciseWidget,
                InProgressPresenter = EnterAnswerExerciseWidget,
                ResultPresenter = CountRightResultWidget
            };
            
            _currentExercise = new EnterAnswerExercise(presenters, exerciseData);
            
            if (!_isRestarted)
            {
                QuestionsCount = exerciseData.QuestionsAnswers.Length;
                
                var domainLibrary = App.StaticData.DomainLibrary;
                var context = domainLibrary.ContextLibrary.GetContext(exerciseData.QuestionsAnswers[0].ContextId);
                var scene = domainLibrary.CourseLibrary.GetSceneById(context.SceneId);
                
                App.ObserverAPI.SetContext(scene.DefaultContextId);
                ShowInfo();
            }
            else
            {
                _isRestarted = false;
                StartExercise();
            }
        }
        
        public void StartChooseAnswerExercise(ChooseAnswerExerciseData exerciseData)
        {
            HideAll();
            ExerciseType = ExerciseType.ChooseAnswer;
            
            var presenters = new ExercisePresenters<ChooseAnswerExerciseData, CountRightExerciseResult>
            {
                InfoPresenter = InfoExerciseWidget,
                InProgressPresenter = ChooseAnswerExerciseWidget,
                ResultPresenter = CountRightResultWidget
            };
            
            _currentExercise = new ChooseAnswerExercise(presenters, exerciseData);
            
            if (!_isRestarted)
            {
                QuestionsCount = exerciseData.QuestionsAnswers.Length;
                
                var domainLibrary = App.StaticData.DomainLibrary;
                var context = domainLibrary.ContextLibrary.GetContext(exerciseData.QuestionsAnswers[0].ContextId);
                var scene = domainLibrary.CourseLibrary.GetSceneById(context.SceneId);
                
                App.ObserverAPI.SetContext(scene.DefaultContextId);
                ShowInfo();
            }
            else
            {
                _isRestarted = false;
                StartExercise();
            }
        }
        
        public void StartMatchingExercise(MatchingExerciseData exerciseData)
        {
            HideAll();
            ExerciseType = ExerciseType.Matching;
            
            var presenters = new ExercisePresenters<MatchingExerciseData, CountRightExerciseResult>
            {
                InfoPresenter = InfoExerciseWidget,
                InProgressPresenter = MatchingExerciseWidget,
                ResultPresenter = CountRightResultWidget
            };
            
            _currentExercise = new MatchingExercise(presenters, exerciseData);
            
            if (!_isRestarted)
            {
                QuestionsCount = exerciseData.QuestionsAnswers.Length;
                var domainLibrary = App.StaticData.DomainLibrary;
                var context = domainLibrary.ContextLibrary.GetContext(exerciseData.QuestionsAnswers[0].ContextId);
                var scene = domainLibrary.CourseLibrary.GetSceneById(context.SceneId);
                
                App.ObserverAPI.SetContext(scene.DefaultContextId);
                ShowInfo();
            }
            else
            {
                _isRestarted = false;
                StartExercise();
            }
        }
        
        public void StartSelectingExercise(SelectingExerciseData exerciseData)
        {
            HideAll();
            ExerciseType = ExerciseType.Selecting;
            
            var presenters = new ExercisePresenters<SelectingExerciseData, CountRightExerciseResult>
            {
                InfoPresenter = InfoExerciseWidget,
                InProgressPresenter = SelectingExerciseWidget,
                ResultPresenter = CountRightResultWidget
            };
            
            _currentExercise = new SelectingExercise(presenters, exerciseData);
            
            if (!_isRestarted)
            {
                QuestionsCount = exerciseData.QuestionsAnswers.Length;
                var domainLibrary = App.StaticData.DomainLibrary;
                var scene = domainLibrary.CourseLibrary.GetSceneById(exerciseData.SceneId);
                
                App.ObserverAPI.SetContext(scene.DefaultContextId);
                ShowInfo();
            }
            else
            {
                _isRestarted = false;
                StartExercise();
            }
        }

        private void ShowInfo()
        {
            InfoExerciseWidget.Show();
            ExerciseStartFooterWidget.gameObject.SetActive(true);
            FlexBoxWidget.SetValueSmoothly(0.65f);
            
            var currentExerciseId = App.RuntimeData.ApplicationStateModel.CurrentExerciseId;
            if (currentExerciseId == App.RuntimeData.UserInterfaceModel.Observer.MyExerciseId)
            {
                FavouriteToggle.gameObject.SetActive(false);
            }
            else
            {
                FavouriteToggle.gameObject.SetActive(true);
                var dataExercises = App.RuntimeData.UserInterfaceModel.Hub.FavouritesData.Exercises;
                FavouriteToggle.Toggle.On = dataExercises.Contains(currentExerciseId);
            }
            
            var scene = App.StaticData.Scene;
            var switchButton = scene.MainSandbox.SwitchHubObserverButton;
            switchButton.gameObject.SetActive(true);
            switchButton.CanSwitch = true;
            
            ExerciseState = ExerciseState.Info;
            _currentExercise.ShowInfo();
        }
        
        public void StartExercise()
        {
            HideAll();
            App.StaticData.Scene.Logo3D.SetActive(false);
            FavouriteToggle.gameObject.SetActive(false);

            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            
            App.OpenObserver();
            
            var scene = App.StaticData.Scene;
            var mainSandbox = scene.MainSandbox;

            observerSandbox.transform.AsRectTransform().SetAnchoredPositionY(-150f);
            observerSandbox.ViewPresenterWidget.transform.AsRectTransform().SetAnchoredPositionY(125f);
            
            ExerciseState = ExerciseState.InProgress;
            _currentExercise.Start();
            
            mainSandbox.SwitchHubObserverButton.gameObject.SetActive(false);
            mainSandbox.SwitchHubObserverButton.CanSwitch = false;
            mainSandbox.CloseExerciseButtonWidget.gameObject.SetActive(true);
            
            ShowInfoButton();
            StartTimer();
            SendOnExerciseStartedAnalytics();
        }
        
        public void FinishExercise()
        {
            HideAll();
            HideInfoButton();
            
            CountRightResultWidget.Show();
            FlexBoxWidget.SetValueSmoothly(0.65f);
            ExerciseResultFooterWidget.gameObject.SetActive(true);
            
            App.StaticData.Scene.MainSandbox.ExerciseTimer.StopTimer();
            App.StaticData.Scene.MainSandbox.ExerciseTimer.gameObject.SetActive(false);
            
            ExerciseState = ExerciseState.Result;
            _currentExercise.Finish();
            
            App.RuntimeData.UserInterfaceModel.Observer.CurrentPoIState = PoIState.Default;
            SendOnExerciseFinishedAnalytics();
        }

        public void CloseExercise()
        {
            _currentExercise.Close();
            HideAll();
            HideInfoButton();
            ExerciseState = ExerciseState.None;
            _currentExercise = null;
        }

        private void HideAll()
        {
            InfoExerciseWidget.Hide();
            EnterAnswerExerciseWidget.Hide();
            ChooseAnswerExerciseWidget.Hide();
            MatchingExerciseWidget.Hide();
            SelectingExerciseWidget.Hide();
            CountRightResultWidget.Hide();
            ExerciseResultFooterWidget.gameObject.SetActive(false);
            ExerciseStartFooterWidget.gameObject.SetActive(false);
        }

        private void ShowInfoButton()
        {
            InfoButton.gameObject.SetActive(true);
            var icon = InfoButton.gameObject.GetComponentInChildren<Image>();
            icon.color = Color.gray;
            InfoButton.OnClick.RemoveAllListeners();
            InfoButton.OnClick.AddListener(() =>
            {
                if (!InfoExerciseWidget.InfoContainer.gameObject.activeInHierarchy)
                {
                    InfoExerciseWidget.Show();
                    FlexBoxWidget.SetValueSmoothly(0.65f);
                    icon.color = Color.white;
                    InfoExerciseWidget.InfoContainer.gameObject.SetActive(true);
                    return;
                }

                HideInfoWidget();
            });
        }

        public void HideInfoWidget()
        {
            InfoExerciseWidget.Hide();
            FlexBoxWidget.SetValueSmoothly(CurrentExerciseViewSize);
            var icon = InfoButton.gameObject.GetComponentInChildren<Image>();
            icon.color = Color.gray;
            InfoExerciseWidget.InfoContainer.gameObject.SetActive(false);
        }

        private void HideInfoButton()
        {
            InfoButton.gameObject.SetActive(false);
        }

        private void StartTimer()
        {
            var scene = App.StaticData.Scene;
            var mainSandbox = scene.MainSandbox;

            mainSandbox.ExerciseTimer.gameObject.SetActive(true);
            mainSandbox.ExerciseTimer.Reset();
            mainSandbox.ExerciseTimer.StartTimer();
        }
        
        public void Restart()
        {
            _isRestarted = true;
            
            var currentExerciseId = App.RuntimeData.ApplicationStateModel.CurrentExerciseId;
            App.ObserverAPI.StartExercise(currentExerciseId);
        }

        public void CloseExerciseButtonClick()
        {
            var scene = App.StaticData.Scene;
            var mainSandbox = scene.MainSandbox;
            
            mainSandbox.SwitchHubObserverButton.gameObject.SetActive(true);
            mainSandbox.SwitchHubObserverButton.CanSwitch = true;
            mainSandbox.CloseExerciseButtonWidget.gameObject.SetActive(false);
            
            CloseObserverExercise();
            App.ObserverAPI.SetDefaultScene();
            App.RuntimeData.ApplicationStateModel.CurrentSceneId = null;
            App.OpenHub();
            App.ObserverAPI.ResetHighlight();
            
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            observerSandbox.transform.AsRectTransform().SetAnchoredPositionY(0f);
            observerSandbox.ViewPresenterWidget.transform.AsRectTransform().SetAnchoredPositionY(-25f);
        }

        private void CloseObserverExercise()
        {
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            var exerciseId = App.RuntimeData.ApplicationStateModel.CurrentExerciseId;

            if (string.IsNullOrWhiteSpace(exerciseId))
                return;
            
            App.RuntimeData.UserInterfaceModel.Observer.CurrentPoIState = PoIState.Default;
            exerciseViewSandbox.ExerciseName.gameObject.SetActive(false);
            exerciseViewSandbox.CloseExercise();

            var myExerciseId = App.RuntimeData.UserInterfaceModel.Observer.MyExerciseId;
            if (myExerciseId == exerciseId)
            {
                var deletePath = $"{Application.persistentDataPath}/{myExerciseId}.json";
                File.Delete(deletePath);
                App.RuntimeData.UserInterfaceModel.Observer.MyExerciseId = null;
            }

            App.RuntimeData.ApplicationStateModel.CurrentExerciseId = null;
            App.RuntimeData.UserInterfaceModel.Hub.CurrentExerciseId = null;
            App.RuntimeData.ApplicationStateModel.EmptyClickEnabled = true;
            App.RuntimeData.ApplicationStateModel.SceneElementClickEnable = true;
        }
        
        #region Analytics

        private void SendOnExerciseStartedAnalytics()
        {
            App.Services.AnalyticsAPI.Builder
                .EventType(AnalyticsAttributes.ObserverExerciseStarted)
                .EventProperty(AnalyticsAttributes.SceneName, App.RuntimeData.ApplicationStateModel.CurrentSceneId)
                .EventProperty(AnalyticsAttributes.ExerciseName, App.RuntimeData.ApplicationStateModel.CurrentExerciseId)
                .Send();
        }
        
        private void SendOnExerciseFinishedAnalytics()
        {
            App.Services.AnalyticsAPI.Builder
                .EventType(AnalyticsAttributes.ObserverExerciseFinished)
                .EventProperty(AnalyticsAttributes.SceneName, App.RuntimeData.ApplicationStateModel.CurrentSceneId)
                .EventProperty(AnalyticsAttributes.ExerciseName, App.RuntimeData.ApplicationStateModel.CurrentExerciseId)
                .Send();
        }

        #endregion
    }
}
