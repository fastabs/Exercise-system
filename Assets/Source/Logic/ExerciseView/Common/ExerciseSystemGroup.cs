using Leopotam.Ecs;

namespace ExerciseSystem
{
    public sealed class ExerciseSystemGroup : IEcsSystemGroup
    {
        public IEcsSystem[] GetSystems()
        {
            return new IEcsSystem[]
            {
                new StartExerciseSystem(),
                new ExerciseViewInitSystem(),
                new SelectingExerciseSystem(),
            };
        }
    }
}