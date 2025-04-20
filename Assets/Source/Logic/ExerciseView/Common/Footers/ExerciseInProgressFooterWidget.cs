using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class ExerciseInProgressFooterWidget : ExerciseFooterWidget
    {
        [field: SerializeField] public LeanButton NextButton { get; private set; }
        [field: SerializeField] public LeanButton PreviousButton { get; private set; }
        [field: SerializeField] public LeanButton FinishButton { get; private set; }
    }
}