using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLMS.Utils
{
    public class WebLMSTasks
    {
        private static int DEFAULT_PERFORM_CAPACITTY = 5;
        private static int DEFAULT_QUEUE_CAPACITTY = 20;

        private static Dictionary<int, Task> PerfomedTasks { get; set; }
        private static Dictionary<int, Task> LazyTasks { get; set; }

        static WebLMSTasks()
        {
            PerfomedTasks = new Dictionary<int, Task>(PerformCapacity);
            LazyTasks = new Dictionary<int, Task>(QueueCapacity);
        }

        public static int PerformCapacity
        {
            get
            {
                return DEFAULT_PERFORM_CAPACITTY;
            }
            set
            {
                DEFAULT_PERFORM_CAPACITTY = value;
            }
        }

        public static int QueueCapacity
        {
            get
            {
                return DEFAULT_QUEUE_CAPACITTY;
            }
            set
            {
                DEFAULT_QUEUE_CAPACITTY = value;
            }
        }

        private static void ContinueWith(Task task)
        {
            task.ContinueWith(t =>
            {
                PerfomedTasks.Remove(t.Id);
                var newKeyValuePair = LazyTasks.FirstOrDefault();
                var newTask = newKeyValuePair.Value;
                if (newTask != null)
                {
                    LazyTasks.Remove(newTask.Id);
                    PerfomedTasks.Add(newTask.Id, newTask);
                    newTask.Start();
                    System.Diagnostics.Debug.WriteLine(string.Format("Start task in ContinueWith: {0}", newTask.Id));
                }
            });
        }


        public static bool IsInPerformQueue(int taskId)
        {
            return PerfomedTasks.ContainsKey(taskId);
        }

        public static bool IsInLazyQueue(int taskId)
        {
            return LazyTasks.ContainsKey(taskId);
        }

        public static bool AddTask(Task task)
        {
            int perfomedTasksCount = PerfomedTasks.Count;
            int queueTasksCount = LazyTasks.Count;

            if (perfomedTasksCount < PerformCapacity)
            {
                PerfomedTasks.Add(task.Id, task);
                ContinueWith(task);
                task.Start();

                System.Diagnostics.Debug.WriteLine(string.Format("Start task in performed: {0}", task.Id));
                return true;
            }
            else if (queueTasksCount < QueueCapacity)
            {
                LazyTasks.Add(task.Id, task);
                ContinueWith(task);
                return true;
            }

            return false;
        }
    }
}