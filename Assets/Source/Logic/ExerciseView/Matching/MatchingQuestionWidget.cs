using System.Collections.Immutable;
using System.Linq;
using UnityEngine.Pool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ExerciseSystem
{
    public sealed class MatchingQuestionWidget : QuestionWidget
    {
        [field: SerializeField] public Transform QuestionContainer { get; private set; }
        [field: SerializeField] public Transform AnswerContainer { get; private set; }
        
        [field: SerializeField] public MatchingQuestionBlockWidget QuestionBlock { get; private set; }
        [field: SerializeField] public MatchingAnswerWidget AnswerVariant { get; private set; }

        private ObjectPool<MatchingQuestionBlockWidget> _matchingBlockPool;
        private ObjectPool<MatchingAnswerWidget> _answerPool;

        public ImmutableList<MatchingQuestionBlockWidget> QuestionBlocks;
        
        private float _height;
        private Vector2 _lastContentPosition;

        private void Init(MatchingExerciseData.QuestionAnswer questionAnswer)
        {
            var questionBlock = _matchingBlockPool.Get();
            questionBlock.Title.text = $"{QuestionBlocks.Count + 1}";
            questionBlock.RightAnswer = questionAnswer.Answer;

            questionBlock.questionContextId = questionAnswer.ContextId;
            
            var answerVariant = _answerPool.Get();
            answerVariant.Title.SetTextWithoutLinkTag(questionAnswer.Answer);
            answerVariant.Answer = questionAnswer.Answer;
            
            questionBlock.ContextButton.OnClick.AddListener(() =>
            {
                foreach (var blockWidget in QuestionBlocks)
                {
                    blockWidget.LeanToggle.On = false;
                }
                questionBlock.LeanToggle.On = true;
                
                App.ObserverAPI.SetContext(questionAnswer.ContextId);
                var posIContainer = App.StaticData.Scene.MainSandbox.ObserverSandbox.PoIContainer;
                posIContainer.AddPoI(questionAnswer.ContextId);
            });
            
            var position = questionBlock.transform.localPosition;
            position.y = -_height;
            questionBlock.transform.localPosition = position;

            var rectTransform = questionBlock.transform.AsRectTransform();
            _height += rectTransform.rect.height;
            
            answerVariant.Init(
                data => 
                {
                    AnswerVariantBeginDrag(data, answerVariant); 
                },
                data => 
                {
                    AnswerVariantDrag(data, answerVariant); 
                },
                data =>
                {
                    AnswerVariantEndDrag(data, answerVariant);
                });
            
            questionBlock.AnswerArea.CurrentMatchingAnswerWidget = answerVariant;
            
            QuestionBlocks = QuestionBlocks.Add(questionBlock);
            
            if (QuestionBlocks.Count != 1)
                return;
            
            questionBlock.LeanToggle.On = true;
        }

        public void CreateQuestions(MatchingExerciseData exerciseData)
        {
            InitPool();
            
            QuestionBlocks = ImmutableList<MatchingQuestionBlockWidget>.Empty;
            var questionsAnswers = exerciseData.QuestionsAnswers;

            foreach (var questionAnswer in questionsAnswers)
            {
                Init(questionAnswer);
            }
            
            Reshuffle();
        }

        private void Reshuffle()
        {
            var questions = QuestionBlocks.ToArray();

            for (var i = 0; i < questions.Length; i++)
            {
                var temp = questions[i].AnswerArea.CurrentMatchingAnswerWidget;
                var newIndex = Random.Range(i, questions.Length);
                questions[i].AnswerArea.CurrentMatchingAnswerWidget = questions[newIndex].AnswerArea.CurrentMatchingAnswerWidget;
                questions[newIndex].AnswerArea.CurrentMatchingAnswerWidget = temp;
            }
        }
        
        public override bool IsAnswerRight()
        {
            return QuestionBlock.IsRightAnswered;
        }
        
        private void AnswerVariantBeginDrag(PointerEventData eventData, MatchingAnswerWidget answerWidget)
        {
            answerWidget.transform.SetAsLastSibling();
        }
        
        private void AnswerVariantDrag(PointerEventData eventData, MatchingAnswerWidget answerWidget)
        {
            var canvas = App.StaticData.Scene.Canvas;
            var delta = eventData.delta / canvas.scaleFactor;
            var rectTransform = answerWidget.transform.AsRectTransform();
            var anchoredPosition = rectTransform.anchoredPosition;
            
            var exerciseViewSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            var scrollView = exerciseViewSandbox.MatchingExerciseWidget.MatchingScrollView;
            
            var contentDelta = scrollView.content.anchoredPosition - _lastContentPosition;
            anchoredPosition.y += delta.y - contentDelta.y;

            _lastContentPosition = scrollView.content.anchoredPosition;

            if (anchoredPosition.y > 0 || anchoredPosition.y < -_height + rectTransform.rect.height)
                return;
            
            rectTransform.anchoredPosition = anchoredPosition;

            var positionY = answerWidget.transform.position.y;
            positionY -= rectTransform.rect.height / 2;
            
            ScrollingContent(answerWidget);
            var answerAreaWidget = FindAreaForYPosition(positionY);
            
            if (answerAreaWidget == null)
                return;

            foreach (var questionBlock in QuestionBlocks)
            {
                if (questionBlock.AnswerArea != answerAreaWidget)
                    questionBlock.AnswerArea.SetAsDefault();
            }

            var area = FindAreaForAnswer(answerWidget);
            answerAreaWidget.SetAsEntered();
            area.ShowPreview(answerAreaWidget.CurrentMatchingAnswerWidget.Title.text);
        }
        
        private void AnswerVariantEndDrag(PointerEventData eventData, MatchingAnswerWidget answerWidget)
        {
            var positionY = answerWidget.transform.position.y;
            var rectTransform = answerWidget.transform.AsRectTransform();
            positionY -= rectTransform.rect.height / 2;
            var oldArea = FindAreaForAnswer(answerWidget);
            var newArea = FindAreaForYPosition(positionY);

            if (newArea == null)
            {
                oldArea.CurrentMatchingAnswerWidget = answerWidget;
                return;
            }

            var temp = newArea.CurrentMatchingAnswerWidget;
            newArea.CurrentMatchingAnswerWidget = answerWidget;
            oldArea.CurrentMatchingAnswerWidget = temp;
            
            foreach (var matchingBlock in QuestionBlocks)
            {
                matchingBlock.AnswerArea.SetAsDefault();
            }
        }

        private void ScrollingContent(MatchingAnswerWidget answerWidget)
        {
            var exerciseViewSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            var scrollView = exerciseViewSandbox.MatchingExerciseWidget.MatchingScrollView;

            var viewportCorners = new Vector3[4];
            scrollView.viewport.GetWorldCorners(viewportCorners);
            
            var answerWidgetCorners = new Vector3[4];
            answerWidget.Background.rectTransform.GetWorldCorners(answerWidgetCorners);
            
            var topCorner = viewportCorners[0].y + 50f;
            var bottomCorner = viewportCorners[1].y - 50f;
            
            var answerTopCorner = answerWidgetCorners[0].y - 50f;
            var answerBottomCorner = answerWidgetCorners[1].y + 50f;

            if (answerTopCorner < topCorner)
            {
                var currentPosY = scrollView.content.AsRectTransform().anchoredPosition.y;
                scrollView.content.AsRectTransform().anchoredPosition = new Vector2(0f, currentPosY + 30f);
            }
            else if (answerBottomCorner > bottomCorner)
            {
                var currentPosY = scrollView.content.AsRectTransform().anchoredPosition.y;
                scrollView.content.AsRectTransform().anchoredPosition = new Vector2(0f, currentPosY - 30f);
            }
            
            scrollView.verticalNormalizedPosition = Mathf.Clamp(scrollView.verticalNormalizedPosition, 0f, 1f);
        }
        
        private MatchingAnswerAreaWidget FindAreaForYPosition(float yPosition)
        {
            foreach (var questionBlock in QuestionBlocks)
            {
                var answerArea = questionBlock.AnswerArea;
                var rectTransform = answerArea.transform.AsRectTransform();
                var rect = rectTransform.rect;

                var anchoredPositionY = rectTransform.position.y;
                var yMin = anchoredPositionY - rect.height;
                var yMax = anchoredPositionY;

                if (yPosition > yMin && yPosition < yMax)
                    return answerArea;
            }

            return null;
        }
        
        private MatchingAnswerAreaWidget FindAreaForAnswer(MatchingAnswerWidget answerWidget)
        {
            foreach (var matchingBlock in QuestionBlocks)
            {
                var answerArea = matchingBlock.AnswerArea;

                if (answerArea.CurrentMatchingAnswerWidget == answerWidget)
                    return answerArea;
            }

            return null;
        }
        
        public override void SetAsResult()
        {
            base.SetAsResult();
            
            foreach (var matchingBlock in QuestionBlocks)
            {
                matchingBlock.AnswerArea.SetAsResult();
            }
        }
        
        private void InitPool()
        {
            _matchingBlockPool = new ObjectPool<MatchingQuestionBlockWidget>(
                createFunc: () =>
                {
                    var buttonWidget = Instantiate(QuestionBlock, QuestionContainer);
                    return buttonWidget;
                },
                actionOnGet: widget =>
                {
                    widget.gameObject.SetActive(true);
                    widget.LeanToggle.On = false;
                },
                actionOnRelease: widget =>
                {
                    widget.ContextButton.OnClick.RemoveAllListeners();
                    var matchingAnswerWidget = widget.AnswerArea.CurrentMatchingAnswerWidget;
                    
                    widget.AnswerArea.CurrentMatchingAnswerWidget = null;
                    _answerPool.Release(matchingAnswerWidget);
                    
                    widget.gameObject.SetActive(false);
                },
                actionOnDestroy: widget =>
                {
                    Destroy(widget.gameObject);
                }
            );

            _answerPool = new ObjectPool<MatchingAnswerWidget>(
                createFunc: () =>
                {
                    var buttonWidget = Instantiate(AnswerVariant, AnswerContainer);
                    return buttonWidget;
                },
                actionOnGet: widget =>
                {
                    widget.gameObject.SetActive(true);
                },
                actionOnRelease: widget =>
                {
                    widget.Answer = string.Empty;
                    widget.Title.text = string.Empty;
                    widget.gameObject.SetActive(false);
                },
                actionOnDestroy: widget =>
                {
                    Destroy(widget.gameObject);
                }
            );
        }
    }
}