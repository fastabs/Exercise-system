using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ExerciseSystem
{
    public class ButtonWidget : Widget, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
    {
        [field: SerializeField] public LeanButton Button { get; private set; }
        [field: SerializeField] public LeanBox Outline { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Title { get; private set; }
        [field: SerializeField] public LoaderPlaceholder LoaderPlaceholder { get; private set; }

        private Coroutine _coroutine;

        public bool IsPressed { get; private set; }

        public virtual event UnityAction OnClick
        {
            add => Button.OnClick.AddListener(value);
            remove => Button.OnClick.RemoveListener(value);
        }

        public virtual void RemoveAllListeners()
        {
            Button.OnClick.RemoveAllListeners();
        }

        public void SetAsDefault()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            LoaderPlaceholder.gameObject.SetActive(false);
            Title.gameObject.SetActive(true);
            Button.interactable = true;
        }

        public void SetAsLoading()
        {
            _coroutine = this.WaitForSeconds(() =>
            {
                LoaderPlaceholder.gameObject.SetActive(true);
                Title.gameObject.SetActive(false);
            }, .5f);
            
            Button.interactable = false;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsPressed = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            IsPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsPressed = false;
        }
    }
}