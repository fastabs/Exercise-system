using System.IO;
using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class CloseExerciseButtonWidget : Widget
    {
        [field: SerializeField] public LeanButton Button { get; private set; }
        
        private void Awake()
        {
            Button.OnClick.AddListener(CloseExercise);
        }

        private void CloseExercise()
        {
            App.ShowMessageBox()
                .SetTitle("@UIElements/Observer/ExerciseClosing")
                .SetDescription("@UIElements/Observer/AreYouSureYouWantToClose")
                .SetPositiveText("@UIElements/MessageBox/Ok")
                .SetNegativeText("@UIElements/MessageBox/Cancel")
                .SetPositiveAction(() =>
                {
                    SendOnExerciseClosedAnalytics();
                    
                    var scene = App.StaticData.Scene;
                    var mainSandbox = scene.MainSandbox;
            
                    mainSandbox.SwitchHubObserverButton.gameObject.SetActive(true);
                    mainSandbox.CloseExerciseButtonWidget.gameObject.SetActive(false);
                    mainSandbox.SwitchHubObserverButton.CanSwitch = true;
                    mainSandbox.ExerciseTimer.StopTimer();
                    mainSandbox.ExerciseTimer.gameObject.SetActive(false);
                    
                    //App.OpenObserver(ObserverView.SceneView);
                    CloseObserverExercise();
                    App.ObserverAPI.SetDefaultScene();
                    App.ObserverAPI.ResetHighlight();
                    App.OpenHub();
                    
                    var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
                    observerSandbox.transform.AsRectTransform().SetAnchoredPositionY(0f);
                    observerSandbox.ViewPresenterWidget.transform.AsRectTransform().SetAnchoredPositionY(-25f);
                })
                .Show();
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

        private void SendOnExerciseClosedAnalytics()
        {
            App.Services.AnalyticsAPI.Builder
                .EventType(AnalyticsAttributes.ObserverExerciseClosed)
                .EventProperty(AnalyticsAttributes.SceneName, App.RuntimeData.ApplicationStateModel.CurrentSceneId)
                .EventProperty(AnalyticsAttributes.ExerciseName, App.RuntimeData.ApplicationStateModel.CurrentExerciseId)
                .Send();
        }

        #endregion
    }
}