using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ExerciseSystem
{
    public sealed class ResourcesExerciseRepository<TData, TResult> : IExerciseRepository<TData, TResult>
        where TData : IExerciseData
        where TResult : IExerciseResult
    {
        private readonly string _resultsDirectory = $"{Application.persistentDataPath}/ExercisesResult/";

        private readonly string _exerciseId;
        private readonly string _pathToData;
        private readonly string _pathToResult;

        public ResourcesExerciseRepository(string exerciseId)
        {
            _exerciseId = exerciseId;
            _pathToData = $"Exercises/{exerciseId}";
            _pathToResult = _resultsDirectory + $"{exerciseId}_result.json";
        }
        
        public void SaveResult(TResult result)
        {
            if (!Directory.Exists(_resultsDirectory))
                Directory.CreateDirectory(_resultsDirectory);
            
            var json = JsonConvert.SerializeObject(result);
            File.WriteAllText(_pathToResult, json);
        }

        public TData GetData()
        {
            if (_exerciseId == App.RuntimeData.UserInterfaceModel.Observer.MyExerciseId)
            {
                var myExerciseJson = File.ReadAllText($"{Application.persistentDataPath}/{_exerciseId}.json");
                return JsonConvert.DeserializeObject<TData>(myExerciseJson);
            }
            
            var json = Resources.Load<TextAsset>(_pathToData).text;
            return JsonConvert.DeserializeObject<TData>(json);
        }

        public TResult GetResult()
        {
            if(!File.Exists(_pathToResult))
                return default;
            
            var json = File.ReadAllText(_pathToResult);
            return JsonConvert.DeserializeObject<TResult>(json);
        }
    }
}