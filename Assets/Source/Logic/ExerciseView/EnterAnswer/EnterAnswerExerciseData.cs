using System;
using System.Collections.Immutable;

namespace ExerciseSystem
{
    [Serializable]
    public struct EnterAnswerExerciseData : IExerciseData
    {
        public string ExerciseId { get; set; }
        public QuestionAnswer[] QuestionsAnswers { get; set; }

        [Serializable]
        public struct QuestionAnswer
        {
            public string ContextId;
            public string Question;
            public string[] Answers;
        }
    }
}