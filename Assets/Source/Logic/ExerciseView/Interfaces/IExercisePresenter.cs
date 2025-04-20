namespace ExerciseSystem
{
    public interface IExercisePresenter<TData>
        where TData : IExerciseData
    {
        void CreateView(TData data);
        void DestroyView();
    }
}