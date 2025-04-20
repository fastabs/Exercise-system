using TMPro;
using System.IO;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class InfoExerciseWidget : ExerciseResultWidget, IExercisePresenter<CountRightExerciseResult>
    {
        [field: Header("Info")]
        [field: SerializeField] public Transform InfoContainer { get; set; }
        [field: SerializeField] public TextMeshProUGUI ChooseAnswerDescription { get; private set; }
        [field: SerializeField] public TextMeshProUGUI EnterDescription { get; private set; }
        [field: SerializeField] public TextMeshProUGUI MatchingDescription { get; private set; }
        [field: SerializeField] public TextMeshProUGUI SelectingDescription { get; private set; }
        [field: SerializeField] public TextMeshProUGUI QuestionCount { get; private set; }
        
        [field: Header("Result")]
        [field: SerializeField] public Transform ResultContainer { get; set; }
        [field: SerializeField] public TextMeshProUGUI PercentLabel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI CountRightLabel { get; private set; }
        
        public void CreateView(CountRightExerciseResult result)
        {
            HideInfoView();
            
            var exerciseViewSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            var exerciseType = exerciseViewSandbox.ExerciseType;
            switch (exerciseType)
            {
                case ExerciseType.ChooseAnswer:
                    ChooseAnswerDescription.gameObject.SetActive(true);
                    QuestionCount.transform.AsRectTransform().SetAnchoredPositionY(-247f);
                    break;
                case ExerciseType.EnterAnswer:
                    EnterDescription.gameObject.SetActive(true);
                    QuestionCount.transform.AsRectTransform().SetAnchoredPositionY(-272f);
                    break;
                case ExerciseType.Matching:
                    MatchingDescription.gameObject.SetActive(true);
                    QuestionCount.transform.AsRectTransform().SetAnchoredPositionY(-370f);
                    break;
                case ExerciseType.Selecting:
                    SelectingDescription.gameObject.SetActive(true);
                    QuestionCount.transform.AsRectTransform().SetAnchoredPositionY(-284f);
                    break;
            }
            
            QuestionCount.SetMonoText($"@ExerciseInfo/QuestionCount {exerciseViewSandbox.QuestionsCount}");
            
            var exerciseId = App.RuntimeData.ApplicationStateModel.CurrentExerciseId;
            var filePath = $"ExercisesResult/{exerciseId}_result.json";
            var resultPath = Path.Combine(Application.persistentDataPath, filePath);
            
            if (!File.Exists(resultPath))
            {
                InfoContainer.gameObject.SetActive(true);
                return;
            }
            
            ResultContainer.gameObject.SetActive(true);
            SetCountRight(result.BestResult, result.QuestionsCount);
            var percentage = result.BestResult / (float) result.QuestionsCount;
            SetProgress(percentage);
            
            var bestResult = result.BestResult / (float) result.QuestionsCount;
            SetBestResult(bestResult);
            SetTriesCount(result.TriesCount);
            SetBestTime(result.BestTime);
        }

        public void DestroyView()
        {
            InfoContainer.gameObject.SetActive(false);
            ResultContainer.gameObject.SetActive(false);
        }

        private void HideInfoView()
        {
            InfoContainer.gameObject.SetActive(false);
            ChooseAnswerDescription.gameObject.SetActive(false);
            EnterDescription.gameObject.SetActive(false);
            MatchingDescription.gameObject.SetActive(false);
            SelectingDescription.gameObject.SetActive(false);
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
    }
}