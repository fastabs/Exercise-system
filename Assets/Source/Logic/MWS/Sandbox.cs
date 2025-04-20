using UnityEngine;

namespace ExerciseSystem
{
    public abstract class Sandbox : MonoBehaviour
    {
        // private readonly List<Widget> _widgets = new();
        //
        // public void RegisterWidget(Widget widget)
        // {
        //     if (_widgets.Contains(widget))
        //         throw new InvalidOperationException($"This {widget.GetType()} already exists in the {nameof(Sandbox)}");
        //     
        //     _widgets.Add(widget);
        // }

        public MailboxEvent OnCreate { get; } = new();
        public MailboxEvent OnDestroy { get; } = new();
    }
}