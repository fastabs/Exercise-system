using Leopotam.Ecs;

namespace ExerciseSystem
{
    public sealed class ExercisesInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly StaticData _staticData = default;
        private readonly RuntimeData _runtimeData = default;

        private MailboxEvent.Listener _onCurrentLicenseChanged;
        private MailboxEvent.Listener _exerciseResultChanged;
        
         public void Init()
         {
             var userInterfaceModel = _runtimeData.UserInterfaceModel;
             _exerciseResultChanged = userInterfaceModel.Observer.OnExerciseResultChanged.GetListener(true);
             _onCurrentLicenseChanged = _runtimeData.ProfileModel.CurrentLicenseChanged.GetListener(false);
             
             Refill();
         }

        public void Run()
        {
            if (_onCurrentLicenseChanged.IsUnhandled)
            {
                _onCurrentLicenseChanged.MarkAsHandled();

                Refill();
            }
            
            if (_exerciseResultChanged.IsUnhandled)
            {
                _exerciseResultChanged.MarkAsHandled();

                UpdateExerciseResult();
            }
        }

        private void Refill()
        { 
            var enableExercisesFeature = _runtimeData.RemoteConfigAPI.EnableExercisesFeature;
            if (!enableExercisesFeature)
                return;
            
            var hubSandbox = _staticData.Scene.MainSandbox.HubSandbox;
            var exercisesSandbox = hubSandbox.HubExercisesSandbox;

            var courseLibrary = _staticData.DomainLibrary.CourseLibrary;
            var courses = courseLibrary.Courses;

            exercisesSandbox.Clear();

            foreach (var course in courses)
            {
                var exerciseIds = course.ExerciseIds;
                if (exerciseIds.Length == 0)
                    continue;

                var verticalContainer = exercisesSandbox.AddExercisesContainer(course.CourseName);
                exercisesSandbox.WaitOneFrame(() =>
                {
                    verticalContainer.ExpandButton.OnClick.AddListener(() =>
                    {
                        if (verticalContainer.IsCollapsed)
                        {
                            verticalContainer.Expand();
                            verticalContainer.ChangeSize();
                        }
                        else
                        {
                            verticalContainer.Collapse();
                            verticalContainer.ChangeSize(0);
                        }

                        SendContainerClickAnalytics(course.CourseName, !verticalContainer.IsCollapsed);
                    });

                    foreach (var exerciseId in exerciseIds)
                    {
                        verticalContainer.AddExercise(exerciseId);
                    }

                    verticalContainer.Expand();
                    verticalContainer.ChangeSize();
                });
            }
            
            exercisesSandbox.AddCreateButton();
        }
        
        private void UpdateExerciseResult()
        {
            var enableExercisesFeature = _runtimeData.RemoteConfigAPI.EnableExercisesFeature;
            if (!enableExercisesFeature)
                return;
            
            var hubSandbox = _staticData.Scene.MainSandbox.HubSandbox;
            var exercisesContainers = hubSandbox.HubExercisesSandbox.gameObject.GetComponentsInChildren<ExercisesCollapsableContainer>();
            
            var currentExerciseId = _runtimeData.ApplicationStateModel.CurrentExerciseId;
            var newResult = _runtimeData.UserInterfaceModel.Observer.UpdatedExerciseResult;

            foreach (var exercisesContainer in exercisesContainers)
            {
                exercisesContainer.UpdateResult(currentExerciseId, newResult);
            }
        }
        
        #region Analitycs

        private void SendContainerClickAnalytics(string containerName, bool isExpand)
        {
            App.Services.AnalyticsAPI.Builder
                .EventType(AnalyticsAttributes.HubTabExercisesContainerClick)
                .EventProperty(AnalyticsAttributes.ContainerName, containerName)
                .EventProperty(AnalyticsAttributes.IsExpand, isExpand)
                .Send();
        }

        #endregion
    }
}