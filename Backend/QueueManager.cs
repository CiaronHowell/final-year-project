using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // TODO: We will load the queue with the names of the methods

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
                Debug.WriteLine("Resume workflow");

                return;
            }

            // TODO: Do checks to make sure queue has been sorted

            // Add/"Reset" cancel token
            CancelTokenSource = new CancellationTokenSource();
            // Register callback method that will run post cancellation
            CancelTokenSource.Token.Register(CleanUp);

            // Start thread to run tasks
            Thread thread = new(() => RunWorkflow(CancelTokenSource.Token));
            thread.Start();
        }

        /// <summary>
        /// Thread that will run the tasks
        /// </summary>
        /// <param name="cancelToken">Cancel token</param>
        private void RunWorkflow(CancellationToken cancelToken)
        {
            // Loop through each queue element
            while (CurrentTasks.Count > 0)
            {
                if (PauseToken.Pause)
                {
                    Debug.WriteLine($"Paused: {PauseToken.Pause}");
                    // Wait until unpaused or a cancellation has occurred
                    SpinWait.SpinUntil(() => !PauseToken.Pause || cancelToken.IsCancellationRequested);
                }

                if (cancelToken.IsCancellationRequested)
                {
                    Debug.WriteLine("Cancelled");
                    break;
                }

                string currentTask = CurrentTasks.Dequeue();
                Debug.WriteLine($"Output from thread: {currentTask}");

                // TODO: Called run method in ModuleManager with current task as the string
                // TODO: Add try catch

                DateTime waitTill = DateTime.Now.AddSeconds(5);
                SpinWait.SpinUntil(() => DateTime.Now > waitTill);
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

        /// <summary>
        /// Performs clean up after cancellation
        /// </summary>
        private void CleanUp()
        {
            // Clear queue ready for next workflow
            CurrentTasks.Clear();
            Debug.WriteLine("Clearing Current Tasks");

            // Make sure we're unpaused
            PauseToken.Pause = false;
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