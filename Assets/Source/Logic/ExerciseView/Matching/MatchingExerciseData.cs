using System;

namespace ExerciseSystem
{
    [Serializable]
    public struct MatchingExerciseData : IExerciseData
    {
        public string ExerciseId { get; set; }
        
        public QuestionAnswer[] QuestionsAnswers { get; set; }

        [Serializable]
        public struct QuestionAnswer
        {
            public string ContextId;
            public string Answer;
        }
    }
}