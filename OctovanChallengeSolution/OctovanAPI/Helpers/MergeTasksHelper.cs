using OctovanAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Helpers
{
    public class MergeTasksHelper
    {
        List<List<TaskModel>> _listOfListOfTasks;
        public MergeTasksHelper(List<List<TaskModel>> listOfListOfTasks)
        {
            _listOfListOfTasks = listOfListOfTasks;
        }

        public List<TaskModel> MergeWithLinq()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<TaskModel> output = new List<TaskModel>();
            output = _listOfListOfTasks.SelectMany(x => x).OrderByDescending(x=>x.CreatedAt).ThenByDescending(n=>n.Id).ToList();
            watch.Stop();
            string x = $"Execution Time: {watch.ElapsedTicks} ms";
            return output;
        }

        // herbir listeyi bir kere dolaş - stack
        // 3 * O( n^2)
        // Bubble sort
        public List<TaskModel> MergeManuallyBubbleSort()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<TaskModel> output = new List<TaskModel>();
            int countOfListOfList = _listOfListOfTasks.Count();

            // List<List<TaskModel>> to List<TaskModel>
            for (int i = 0; i < countOfListOfList; i++)
            {
                List<TaskModel> listTask = _listOfListOfTasks[i];
                int countOfList = listTask.Count();
                for (int j = 0; j < countOfList; j++)
                {
                    TaskModel task = listTask[j];
                    output.Add(task);
                }
            }
            // List<TaskModel> Order by CreatedAt
            for (int i = (output.Count - 1); i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    if (output[j - 1].CreatedAt < output[j].CreatedAt)
                    {
                        var temp = output[j - 1];
                        output[j - 1] = output[j];
                        output[j] = temp;
                    }
                }
            }
            // List<TaskModel> Order by Id if CreatedAt values equals
            for (int i = (output.Count - 1); i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    if (output[j - 1].CreatedAt == output[j].CreatedAt && output[j - 1].Id < output[j].Id)
                    {
                        var temp = output[j - 1];
                        output[j - 1] = output[j];
                        output[j] = temp;
                    }
                }
            }
            watch.Stop();
            string x = $"Execution Time: {watch.ElapsedTicks} ms"; // 3998 372
            return output;
        }

    }
}