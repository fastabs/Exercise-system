using UnityEngine;
using UnityEngine.UI;

namespace ExerciseSystem
{
    public sealed class MatchingExerciseWidget : ExerciseWidget, IExercisePresenter<MatchingExerciseData>
    {
        [field: SerializeField] public ScrollRect MatchingScrollView { get; private set; }
        [field: SerializeField] public Transform FooterContainer { get; private set; }
        
        [Header("Prefabs")]
        [SerializeField] private MatchingQuestionWidget matchingQuestionWidgetPrefab;
        [SerializeField] private ExerciseCheckResultFooterWidget exerciseFooterWidgetPrefab;

        public MatchingQuestionWidget MatchingQuestionWidget { get; private set; }
        private MatchingQuestionWidget _questions;
        private ExerciseCheckResultFooterWidget _footer;
        
        public void CreateView(MatchingExerciseData data)
        {
            MatchingScrollView.content.SetSizeDeltaY(0f);
            
            _questions = Instantiate(matchingQuestionWidgetPrefab, MatchingScrollView.content);
            _questions.CreateQuestions(data);
            MatchingQuestionWidget = _questions;

            OpenExerciseWidget();
            ShowPoI(data);
            MatchingScrollView.content.SetSizeDeltaY(_questions.QuestionBlocks.Count * 141f);
            
            _footer = Instantiate(exerciseFooterWidgetPrefab, FooterContainer);
            _footer.CheckResultButton.OnClick.AddListener(OnCheckResult);
        }

        private void OnCheckResult()
        {
            App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox.FinishExercise();
        }
        
        private void OpenExerciseWidget()
        {
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            
            exerciseViewSandbox.FlexBoxWidget.SetValueSmoothly(0.5f);
            exerciseViewSandbox.CurrentExerciseViewSize = 0.5f;
            exerciseViewSandbox.MatchingExerciseWidget.Show();
        }

        private void ShowPoI(MatchingExerciseData data)
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
            Destroy(_questions.gameObject);
            Destroy(_footer.gameObject);
        }
    }
}