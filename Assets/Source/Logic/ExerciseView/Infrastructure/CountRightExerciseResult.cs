namespace ExerciseSystem
{
    public readonly struct CountRightExerciseResult : IExerciseResult
    {
        public string ExerciseId { get; }
        
        public readonly int QuestionsCount;
        public readonly int RightAnswersCount;
        public readonly int BestResult;
        public readonly int TriesCount;
        public readonly float BestTime;

        public CountRightExerciseResult(string exerciseId, int questionsCount, int rightAnswersCount, int bestResult, int triesCount, float bestTime)
        {
            ExerciseId = exerciseId;
            QuestionsCount = questionsCount;
            RightAnswersCount = rightAnswersCount;
            BestResult = bestResult;
            TriesCount = triesCount;
            BestTime = bestTime;
        }
    }
}