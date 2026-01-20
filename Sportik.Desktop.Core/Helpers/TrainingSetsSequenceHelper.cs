using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Training;

namespace Sportik.Desktop.Core.Helpers
{
    public static class TrainingSetsSequenceHelper
    {
        public static TrainingSet GetNextTrainingSet(IList<TrainingSet> trainingSets, TrainingSet trainingSet)
        {
            Guid trainingSetId = trainingSet.Id;
            int index = -1;

            for (int i = 0; i < trainingSets.Count; i++)
            {
                if (trainingSets[i].Id == trainingSetId)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0 && index < trainingSets.Count - 1)
            {
                return trainingSets[index + 1];
            }

            return null;
        }
    }
}