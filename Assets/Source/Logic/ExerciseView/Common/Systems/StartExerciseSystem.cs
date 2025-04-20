using System;
using UnityEngine;
using Leopotam.Ecs;

namespace ExerciseSystem
{
    public sealed class StartExerciseSystem : IEcsRunSystem
    {
        private readonly EcsFilter<StartExerciseRequest> _startExerciseFilter = default;

        private readonly StaticData _staticData = default;
        private readonly RuntimeData _runtimeData = default;
        
        public void Run()
        {
            foreach (var index in _startExerciseFilter)
            {
                var exerciseId = _startExerciseFilter.Get1(index).ExerciseId;
                
                _startExerciseFilter.GetEntity(index).Destroy();

                StartExercise(exerciseId);
            }
        }
        
        private void StartExercise(string exerciseId)
        {
            App.RuntimeData.ApplicationStateModel.EmptyClickEnabled = false;
            App.RuntimeData.ApplicationStateModel.SceneElementClickEnable = false;
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            
            var hub = App.RuntimeData.UserInterfaceModel.Hub;
            if (!string.IsNullOrWhiteSpace(hub.CurrentExerciseId))
                exerciseViewSandbox.CloseExercise();
            
            App.OpenObserver(ObserverView.ExerciseView);
            App.ObserverAPI.SetExercise(exerciseId);
            hub.CurrentExerciseId = exerciseId;
            
            var exerciseTypeString = exerciseId.Split('_')[0];
            var exerciseType = Enum.Parse<ExerciseType>(exerciseTypeString);
            exerciseViewSandbox.ExerciseType.SetTextWithoutLinkTag($"@UIElements/Exercise/{exerciseTypeString}");
            
            exerciseViewSandbox.ExerciseName.gameObject.SetActive(true);
            exerciseViewSandbox.ExerciseName.SetTextWithoutLinkTag($"@UIElements/Exercise/{exerciseType}");
            
            switch (exerciseType)
            {
                case ExerciseType.EnterAnswer:
                    var enterRepository = new ResourcesExerciseRepository<EnterAnswerExerciseData, CountRightExerciseResult>(exerciseId);
                    var enterData = enterRepository.GetData();
                    exerciseViewSandbox.StartLatinTranslationExercise(enterData);
                    break;
                case ExerciseType.ChooseAnswer:
                    var chooseRepository = new ResourcesExerciseRepository<ChooseAnswerExerciseData, CountRightExerciseResult>(exerciseId);
                    var chooseData = chooseRepository.GetData();
                    exerciseViewSandbox.StartChooseAnswerExercise(chooseData);
                    break;
                case ExerciseType.Matching:
                    var matchingRepository = new ResourcesExerciseRepository<MatchingExerciseData, CountRightExerciseResult>(exerciseId);
                    var matchingData = matchingRepository.GetData();
                    exerciseViewSandbox.StartMatchingExercise(matchingData);
                    break;
                case ExerciseType.Selecting:
                    var selectingRepository = new ResourcesExerciseRepository<SelectingExerciseData, CountRightExerciseResult>(exerciseId);
                    var selectingData = selectingRepository.GetData();
                    exerciseViewSandbox.StartSelectingExercise(selectingData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}