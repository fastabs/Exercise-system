using UnityEngine;
using Lean.Gui;

namespace ExerciseSystem
{
    public class ExerciseWidget : Widget
    {
        [field: SerializeField] public LeanToggle LeanToggle { get; private set; }
        
        public void Show()
        {
            LeanToggle.On = true;
        }

        public void Hide()
        {
            LeanToggle.On = false;
        }
    }
}