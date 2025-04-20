using UnityEngine;
using UnityEngine.UI;

namespace ExerciseSystem
{
    public sealed class QuestionStateWidget : MonoBehaviour
    {
        [field: SerializeField] public Graphic Default { get; private set; }
        [field: SerializeField] public Image HasAnswer { get; private set; }
        [field: SerializeField] public Sprite RightAnswerIcon { get; private set; }
        [field: SerializeField] public Sprite WrongAnswerIcon { get; private set; }
        [field: SerializeField] public LeanTrigger HasAnswerTrigger { get; private set; }
        [field: SerializeField] public Color SelectedColor { get; private set; } = Color.white;
        [field: SerializeField] public Color DefaultColor { get; private set; } = Color.yellow;
        [field: SerializeField] public Color RightColor { get; private set; } = Color.green;
        [field: SerializeField] public Color WrongColor { get; private set; } = Color.red;
        
        public void SetAsDefault()
        {
           Default.gameObject.SetActive(true);
           Default.color = DefaultColor;
           HasAnswer.gameObject.SetActive(false);
           HasAnswer.sprite = RightAnswerIcon;
        }

        public void SetAsSelected()
        {
            Default.color = SelectedColor;
            HasAnswer.color = SelectedColor;
        }

        public void SetAsHasAnswer()
        {
            HasAnswerTrigger.Trigger();
            
            Default.gameObject.SetActive(false);
            HasAnswer.gameObject.SetActive(true);
            HasAnswer.color = DefaultColor;
        }

        public void SetAsRightAnswer()
        {
            Default.gameObject.SetActive(false);
            HasAnswer.gameObject.SetActive(true);
            HasAnswer.color = RightColor;
            HasAnswer.sprite = RightAnswerIcon;
        }

        public void SetAsWrongAnswer()
        {
            Default.gameObject.SetActive(false);
            HasAnswer.gameObject.SetActive(true);
            HasAnswer.color = WrongColor;
            HasAnswer.sprite = WrongAnswerIcon;
        }

        public void SetAsDisable()
        {
            Default.gameObject.SetActive(false);
            HasAnswer.gameObject.SetActive(false);
        }
    }
}