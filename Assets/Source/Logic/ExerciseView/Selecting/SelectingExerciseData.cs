using System;

namespace ExerciseSystem
{
    public struct SelectingExerciseData : IExerciseData
    {
        public string ExerciseId { get; set; }
        public string SceneId { get; set; }
        public QuestionAnswer[] QuestionsAnswers { get; set; }

        [Serializable]
        public struct QuestionAnswer
        {
            public string Question;
            public string Answer;
        }
    }
}