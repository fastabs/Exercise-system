using System.IO;
using UnityEngine;
using Leopotam.Ecs;

namespace ExerciseSystem
{
    public sealed class ExerciseViewInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly StaticData _staticData = default;
        private readonly RuntimeData _runtimeData = default;
        
        private MailboxEvent.Listener _observerViewChanged;
        private MailboxEvent.Listener _onAuthChangedListener;
        private MailboxEvent.Listener _onSubscriptionChangedListener;
        private MailboxEvent.Listener _onLicenseChangedListener;
        
        public void Init()
        {
            _observerViewChanged = _runtimeData.UserInterfaceModel.Observer.OnObserverViewChanged.GetListener(true);
            _onAuthChangedListener = _runtimeData.ProfileModel.AuthorizedChanged.GetListener(false);
            _onSubscriptionChangedListener = _runtimeData.ProfileModel.SubscriptionStatusVariantChanged.GetListener(false);
            _onLicenseChangedListener = _runtimeData.ProfileModel.CurrentLicenseChanged.GetListener(false);
            
            var observerSandbox = _staticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            exerciseViewSandbox.FavouriteToggle.Toggle.OnOn.AddListener(AddExerciseToFavourites);
            exerciseViewSandbox.FavouriteToggle.Toggle.OnOff.AddListener(RemoveExerciseFromFavourites);
            exerciseViewSandbox.FavouriteToggle.Button.OnClick.AddListener(() => exerciseViewSandbox.FavouriteToggle.Toggle.Toggle());
        }
        
        public void Run()
        {
            if (_observerViewChanged.IsUnhandled)
            {
                _observerViewChanged.MarkAsHandled();
                
                if (_runtimeData.UserInterfaceModel.Observer.CurrentObserverView != ObserverView.ExerciseView)
                    CloseExerciseView();
            }

            if (_onAuthChangedListener.IsUnhandled)
            {
                _onAuthChangedListener.MarkAsHandled();
                
                CloseExercise();
            }
            
            if (_onSubscriptionChangedListener.IsUnhandled)
            {
                _onSubscriptionChangedListener.MarkAsHandled();
                
                CloseExercise();
            }

            if (_onLicenseChangedListener.IsUnhandled)
            {
                _onLicenseChangedListener.MarkAsHandled();
                
                CloseExercise();
            }
        }
        
        private void CloseExerciseView()
        {
            var observerSandbox = _staticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            var exerciseId = _runtimeData.ApplicationStateModel.CurrentExerciseId;

            if (string.IsNullOrWhiteSpace(exerciseId))
                return;
            
            App.RuntimeData.UserInterfaceModel.Observer.CurrentPoIState = PoIState.Default;
            exerciseViewSandbox.ExerciseTypeLabel.SetTextWithoutLinkTag("@UIElements/Observer/Exercise");
            exerciseViewSandbox.ExerciseName.gameObject.SetActive(false);
            exerciseViewSandbox.CloseExercise();

            var myExerciseId = _runtimeData.UserInterfaceModel.Observer.MyExerciseId;
            if (myExerciseId == exerciseId)
            {
                var deletePath = $"Assets/Resources/Exercises/{myExerciseId}.json";
                File.Delete(deletePath);
                App.RuntimeData.UserInterfaceModel.Observer.MyExerciseId = null;
            }

            _runtimeData.ApplicationStateModel.CurrentExerciseId = null;
            _runtimeData.UserInterfaceModel.Hub.CurrentExerciseId = null;
            App.RuntimeData.ApplicationStateModel.EmptyClickEnabled = true;
            App.RuntimeData.ApplicationStateModel.SceneElementClickEnable = true;
        }

        private void CloseExercise()
        {
            var observerSandbox = _staticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseViewSandbox = observerSandbox.ExerciseViewSandbox;
            var exerciseId = _runtimeData.ApplicationStateModel.CurrentExerciseId;

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
            
            var currentSceneId = App.RuntimeData.ApplicationStateModel.CurrentSceneId;
            var isAvailable = App.StaticData.AccessPassportStorage.IsSceneAvailable(currentSceneId);

            if (isAvailable)
                App.OpenObserver(ObserverView.SceneView);

            App.OpenHub();

            _runtimeData.ApplicationStateModel.CurrentExerciseId = null;
            _runtimeData.UserInterfaceModel.Hub.CurrentExerciseId = null;
            App.RuntimeData.ApplicationStateModel.EmptyClickEnabled = true;
            App.RuntimeData.ApplicationStateModel.SceneElementClickEnable = true;
        }
        
        private void AddExerciseToFavourites()
        {
            var currentExerciseId = _runtimeData.ApplicationStateModel.CurrentExerciseId;
            
            _runtimeData.UserInterfaceModel.Hub.AddExerciseToFavourite(currentExerciseId);
            
            SendFavouritesButtonClickAnalytics(currentExerciseId, true);
        }
        
        private void RemoveExerciseFromFavourites()
        {
            var currentExerciseId = _runtimeData.ApplicationStateModel.CurrentExerciseId;
            
            _runtimeData.UserInterfaceModel.Hub.RemoveExerciseFromFavourite(currentExerciseId);
            
            SendFavouritesButtonClickAnalytics(currentExerciseId, false);
        }
        
        #region Analytics
        
        private void SendFavouritesButtonClickAnalytics(string exerciseId, bool isAdded)
        {
            App.Services.AnalyticsAPI.Builder
                .EventType(AnalyticsAttributes.ObserverExerciseFavouritesClick)
                .EventProperty(AnalyticsAttributes.ExerciseName, exerciseId)
                .EventProperty(AnalyticsAttributes.IsAdded, isAdded)
                .Send();
        }

        #endregion
    }
}