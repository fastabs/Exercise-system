using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace ExerciseSystem
{
    public sealed class ExercisesSandbox : ScreenSandbox
    {
        [field: SerializeField] public ExercisesCollapsableContainer ExercisesCollapsableContainer { get; private set; }
        [field: SerializeField] public CreateExerciseButtonWidget CreateExerciseButtonWidget { get; private set;}
        [field: SerializeField] public Transform Container { get; private set; }

        private ObjectPool<ExercisesCollapsableContainer> _containersPool;
        private List<ExercisesCollapsableContainer> _instantiatedContainers;
        
        private int _nextSiblingIndex;

        public void AddCreateButton()
        {
            CreateExerciseButtonWidget.Button.OnClick.RemoveAllListeners();
            
            CreateExerciseButtonWidget.Button.OnClick.AddListener(() =>
            {
                var hubSandbox = App.StaticData.Scene.MainSandbox.HubSandbox;
                hubSandbox.HubCreatingExerciseSandbox.Init();
                hubSandbox.HubCreatingExerciseSandbox.Show();
                hubSandbox.HideBackgroundImage();
            });
            
            CreateExerciseButtonWidget.transform.SetSiblingIndex(0);
        }
        
        public ExercisesCollapsableContainer AddExercisesContainer(string nameKey)
        {
            InitPool();
            _instantiatedContainers ??= new List<ExercisesCollapsableContainer>();
            
            var verticalCollapsableContainer = _containersPool.Get();
            verticalCollapsableContainer.Label.SetTextWithoutLinkTag(nameKey);
            _instantiatedContainers.Add(verticalCollapsableContainer);
            
            verticalCollapsableContainer.transform.SetSiblingIndex(_nextSiblingIndex++);

            return verticalCollapsableContainer;
        }
        
        public bool TryGetWidgetByExerciseId(string exerciseId, out ExerciseButtonWidget widget)
        {
            var widgets = Container.GetComponentsInChildren<ExerciseButtonWidget>();
            widget = widgets.FirstOrDefault(p => p.ExerciseId == exerciseId);

            return widget != null;
        }

        public void Clear()
        {
            if (_instantiatedContainers == null)
                return;
            
            foreach (var verticalCollapsableContainer in _instantiatedContainers)
            {
                verticalCollapsableContainer.Clear();
                _containersPool.Release(verticalCollapsableContainer);
            }
            
            _instantiatedContainers.Clear();
            _nextSiblingIndex = 0;
        }

        private void InitPool()
        {
            if (_containersPool != null)
                return;
            
            _containersPool = new ObjectPool<ExercisesCollapsableContainer>(
                createFunc: () =>
                {
                    var verticalCollapsableContainer = Instantiate(ExercisesCollapsableContainer, Container);
                    verticalCollapsableContainer.Init();
                    
                    return verticalCollapsableContainer;
                },
                actionOnGet: widget =>
                {
                    widget.gameObject.SetActive(true);
                },
                actionOnRelease: widget =>
                {
                    widget.gameObject.SetActive(false);
                    widget.ExpandButton.OnClick.RemoveAllListeners();
                },
                actionOnDestroy: widget =>
                {
                    Destroy(widget.gameObject);
                });
        }
    }
}