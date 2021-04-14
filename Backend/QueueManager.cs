using System;
using System.Collections.Generic;
using System.Threading;

namespace FinalYearProject.Backend
{
    /// <summary>
    /// Queue Management
    /// </summary>
    public class QueueManager
    {
        /// <summary>
        /// Stores the current tasks of the workflow
        /// </summary>
        private Queue<string> CurrentTasks { get; set; }

        /// <summary>
        /// Stores the Cancel token for the work thread
        /// </summary>
        private CancellationTokenSource CancelTokenSource { get; set; }

        /// <summary>
        /// True if it has queue, otherwise false
        /// </summary>
        public bool HasQueue => CurrentTasks.Count > 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public QueueManager()
        {
            // Initialises the queue
            CurrentTasks = new Queue<string>();
        }

        /// <summary>
        /// Loads tasks into queue
        /// </summary>
        /// <param name="tasks">The tasks of the workflow</param>
        public void LoadQueue(List<string> tasks)
        {
            foreach (string task in tasks)
            {
                CurrentTasks.Enqueue(task);
            }
        }

        /// <summary>
        /// Starts the queue
        /// </summary>
        public void StartQueue()
        {
            // Just unpause if we've already paused
            if (PauseToken.Pause)
            {
                PauseToken.Pause = false;
                return;
            }

            // TODO: Do checks to make sure queue has been sorted


            // Add/"Reset" cancel token
            CancelTokenSource = new CancellationTokenSource();

            // Start thread to run tasks
            Thread thread = new(() => Run(CancelTokenSource.Token));
            thread.Start();
        }

        /// <summary>
        /// Thread that will run the tasks
        /// </summary>
        /// <param name="cancelToken">Cancel token</param>
        private void Run(CancellationToken cancelToken)
        {
            // Loop through each queue element
            while (CurrentTasks.Count > 0)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    Console.WriteLine("Cancelled");
                    break;
                }

                if (PauseToken.Pause)
                {
                    Console.WriteLine($"Paused: {PauseToken.Pause}");
                    SpinWait.SpinUntil(() => !PauseToken.Pause);
                }

                string currentTask = CurrentTasks.Dequeue();
                Console.WriteLine($"Output from thread: {currentTask}");
            }
        }

        /// <summary>
        /// Pause the current queue
        /// </summary>
        public void PauseQueue()
        {
            PauseToken.Pause = true;
        }

        /// <summary>
        /// Stop the current queue
        /// </summary>
        public void StopQueue()
        {
            CancelTokenSource.Cancel();
        }
    }

    /// <summary>
    /// Pause Token
    /// </summary>
    class PauseToken
    {
        /// <summary>
        /// Pause the queue
        /// </summary>
        public static bool Pause { get; set; } = false;
    }
}