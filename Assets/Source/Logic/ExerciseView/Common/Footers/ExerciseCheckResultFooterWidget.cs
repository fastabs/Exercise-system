using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class ExerciseCheckResultFooterWidget : ExerciseFooterWidget
    {
        [field: SerializeField] public LeanButton CheckResultButton { get; private set; }
    }
}