namespace ExerciseSystem
{
    public interface IExerciseRepository<TData, TResult>
        where TData : IExerciseData
        where TResult : IExerciseResult
    {
        TData GetData();
        void SaveResult(TResult result);
        TResult GetResult();
    }
}