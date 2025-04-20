namespace ExerciseSystem
{
    public readonly struct TimeExerciseResult: IExerciseResult
    {
        public string ExerciseId { get; }
        
        public readonly string FinalTime;

        public TimeExerciseResult(string exerciseId, string finalTime)
        {
            ExerciseId = exerciseId;
            FinalTime = finalTime;
        }
    }
}