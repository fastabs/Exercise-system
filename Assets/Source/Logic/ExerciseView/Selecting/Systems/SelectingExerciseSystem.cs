using Leopotam.Ecs;

namespace ExerciseSystem
{
    public sealed class SelectingExerciseSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly StaticData _staticData = default;
        private readonly RuntimeData _runtimeData = default;

        private MailboxEvent.Listener _onContextChanged;
        
        public void Init()
        {
            _onContextChanged = _runtimeData.ApplicationStateModel.OnContextChanged.GetListener(false);
        }
        
        public void Run()
        {
            var exerciseId = _runtimeData.ApplicationStateModel.CurrentExerciseId;
            if (string.IsNullOrWhiteSpace(exerciseId))
                return;
            
            var exerciseSandbox = _staticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            var exerciseType = exerciseSandbox.ExerciseType;
            if (exerciseType != ExerciseType.Selecting)
                return;
            
            var exerciseState = exerciseSandbox.ExerciseState;
            if (exerciseState != ExerciseState.InProgress)
                return;

            if (_onContextChanged.IsUnhandled)
            {
                _onContextChanged.MarkAsHandled();

                SetSelected();
            }
        }
        
        private void SetSelected()
        {
            var currentSceneId = _runtimeData.ApplicationStateModel.CurrentSceneId;
            var scene = _staticData.DomainLibrary.CourseLibrary.GetSceneById(currentSceneId);
            if (scene.DefaultContextId == _runtimeData.ApplicationStateModel.CurrentContextId)
                return;
            
            var sceneElementId = _runtimeData.ApplicationStateModel.HighlightedSceneElementId;
            var mainSandbox = _staticData.Scene.MainSandbox;
            var exerciseViewSandbox = mainSandbox.ObserverSandbox.ExerciseViewSandbox;
            var selectExerciseWidget = exerciseViewSandbox.SelectingExerciseWidget;
            
            selectExerciseWidget.SetSelectedId(sceneElementId);
        }
    }
}