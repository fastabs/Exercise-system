namespace ExerciseSystem
{
    public struct ExercisePresenters<TData, TResult>
        where TData : IExerciseData
        where TResult : IExerciseResult
    {
        public IExercisePresenter<TResult> InfoPresenter;
        public IExercisePresenter<TData> InProgressPresenter;
        public IExercisePresenter<TResult> ResultPresenter;

        public readonly void CreateInfo(TResult result)
        {
            InfoPresenter.CreateView(result);    
        }
        
        public readonly void CreateInProgress(TData data)
        {   
            InProgressPresenter.CreateView(data);
        }

        public readonly void CreateResult(TResult result)
        {
            ResultPresenter.CreateView(result);
        }

        public readonly void DestroyInfo()
        {
            InfoPresenter.DestroyView();
        }
        
        public readonly void DestroyInProgress()
        {
            InProgressPresenter.DestroyView();
        }

        public readonly void DestroyResult()
        {
            ResultPresenter.DestroyView();
        }
    }
}