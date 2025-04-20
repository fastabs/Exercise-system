namespace ExerciseSystem
{
    public abstract class Exercise<TData, TResult> : IExercise
        where TData : IExerciseData
        where TResult : IExerciseResult
    {
        protected readonly ExercisePresenters<TData, TResult> Presenters;
        protected readonly TData ExerciseData;
        protected readonly IExerciseRepository<TData, TResult> Repository;

        protected Exercise(ExercisePresenters<TData, TResult> presenters, TData exerciseData, IExerciseRepository<TData, TResult> repository)
        {
            Presenters = presenters;
            ExerciseData = exerciseData;
            Repository = repository;
        }

        public abstract void ShowInfo();
        public abstract void Start();
        public abstract void Finish();
        public abstract void Close();
        public abstract TResult GetResult();
    }
}