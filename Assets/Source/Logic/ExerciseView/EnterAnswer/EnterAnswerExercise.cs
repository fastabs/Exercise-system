﻿using System;

namespace ExerciseSystem
{
    public sealed class EnterAnswerExercise : Exercise<EnterAnswerExerciseData, CountRightExerciseResult>
    {
        public EnterAnswerExercise(ExercisePresenters<EnterAnswerExerciseData, CountRightExerciseResult> presenters, EnterAnswerExerciseData exerciseData)
            : base(presenters, exerciseData,
                new ResourcesExerciseRepository<EnterAnswerExerciseData, CountRightExerciseResult>(exerciseData.ExerciseId))
        {
        }

        public override void ShowInfo()
        {
            Presenters.CreateInfo(Repository.GetResult());
        }
        
        public override void Start()
        {
            Presenters.DestroyInfo();
            Presenters.CreateInProgress(ExerciseData);
        }

        public override void Finish()
        {
            Presenters.CreateResult(GetResult());
            ShowResultQuestions();
            Presenters.DestroyInProgress();
            Repository.SaveResult(GetResult());
        }

        public override void Close()
        {
            var observerSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox;
            var exerciseState = observerSandbox.ExerciseViewSandbox.ExerciseState;
            if (exerciseState == ExerciseState.Info)
            {
                Presenters.DestroyInfo();
                return;
            }
            if (exerciseState == ExerciseState.Result)
            {
                Presenters.DestroyResult();
                return;
            }
            
            Presenters.DestroyInProgress();
        }
        
        public override CountRightExerciseResult GetResult()
        {
            var presenter = (EnterAnswerExerciseWidget) Presenters.InProgressPresenter;

            var questions = presenter.Questions;

            var questionsCount = questions.Count;
            var rightAnswersCount = 0;
            
            foreach (var questionWidget in questions)
            {
                if (questionWidget.IsAnswerRight())
                    rightAnswersCount++;
            }

            var resultData = Repository.GetResult();
            var bestResult = resultData.BestResult;

            if (rightAnswersCount > bestResult)
            {
                bestResult = rightAnswersCount;
                var newBestResult = bestResult / (float)questionsCount;
                App.RuntimeData.UserInterfaceModel.Observer.UpdatedExerciseResult = newBestResult;
            }
            
            var currentTime = App.StaticData.Scene.MainSandbox.ExerciseTimer.CurrentTime;
            var bestTime = resultData.BestTime;
            
            if (rightAnswersCount > bestTime)
                bestTime = currentTime;
            else if (rightAnswersCount == bestResult && currentTime < bestTime)
                bestTime = currentTime;
            
            if (bestTime == 0)
                bestTime = currentTime;

            var triesCount = resultData.TriesCount;
            triesCount++;
            
            return new CountRightExerciseResult(ExerciseData.ExerciseId, questionsCount, rightAnswersCount, bestResult, triesCount, bestTime);
        }

        private void ShowResultQuestions()
        {
            var presenter = (EnterAnswerExerciseWidget) Presenters.InProgressPresenter;
            
            var questions = presenter.Questions;
            
            var exerciseViewSandbox = App.StaticData.Scene.MainSandbox.ObserverSandbox.ExerciseViewSandbox;
            var resultWidget = exerciseViewSandbox.CountRightResultWidget;
            
            resultWidget.ShowLatinTranslationQuestions(questions);
        }
    }
}