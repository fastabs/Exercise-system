using Lean.Gui;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class ExerciseStartFooterWidget: ExerciseFooterWidget
    {
        [field: SerializeField] public LeanButton StartButton { get; private set; }
    }
}