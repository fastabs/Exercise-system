using System;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ExerciseSystem
{
    public sealed class MatchingAnswerWidget : Widget, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field: SerializeField] public TextMeshProUGUI Title { get; private set; }
        [field: SerializeField] public LeanBox Background { get; private set; }
        
        public string Answer
        {
            get => _answer;
            set
            {
                _answer = value;
                Title.SetTextWithoutLinkTag(value);
            }
        }
        private string _answer;

        private Action<PointerEventData> _beginDragAction;
        private Action<PointerEventData> _dragAction;
        private Action<PointerEventData> _endDragAction;

        public void Init(Action<PointerEventData> beginDragAction, Action<PointerEventData> dragAction, Action<PointerEventData> endDragAction)
        {
            _beginDragAction = beginDragAction;
            _dragAction = dragAction;
            _endDragAction = endDragAction;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _beginDragAction?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragAction?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _endDragAction?.Invoke(eventData);
        }
    }
}