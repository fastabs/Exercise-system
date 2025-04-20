using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class ExerciseResultFooterWidget : ExerciseFooterWidget
    {
        [field: SerializeField] public LeanButton TryMoreButton { get; private set; }
        [field: SerializeField] public LeanButton CloseExerciseButton { get; private set; }
    }
}