using UnityEngine;
using TMPro;

namespace ExerciseSystem
{
    public sealed class TimeResultWidget : ExerciseResultWidget, IExercisePresenter<TimeExerciseResult>
    {
        [field: SerializeField] public TextMeshProUGUI FinalTime { get; private set; }

        public void CreateView(TimeExerciseResult result)
        {
            gameObject.SetActive(true);
            FinalTime.text = result.FinalTime;
        }

        public void DestroyView()
        {
            gameObject.SetActive(false);
        }
    }
}