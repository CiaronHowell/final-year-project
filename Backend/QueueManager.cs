using FinalYearProject.Backend.Utils;
using FinalYearProject.Backend.Utils.Structs;
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
        /// Stores the ModuleManager
        /// </summary>
        private ModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Stores the current tasks of the workflow
        /// </summary>
        private TaskQueue<string> CurrentTasks { get; set; }

        /// <summary>
        /// Task info
        /// </summary>
        private List<WorkflowMethod> TaskInfo { get; set; }

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
        public QueueManager(ModuleManager moduleManager)
        {
            // Initialises the queue
            CurrentTasks = new TaskQueue<string>();
            CurrentTasks.Dequeued += CurrentTasks_Dequeued;

            ModuleManager = moduleManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">TaskQueue</param>
        /// <param name="e">The dequeued item</param>
        private void CurrentTasks_Dequeued(object sender, DequeuedEventArgs<string> e)
        {
            Debug.WriteLine($"{e.DequeuedElement} has been dequeued");

            // Send current dequeued item back to GUI
            Window.SendMessage("currentTask", e.DequeuedElement);
        }

        /// <summary>
        /// Loads tasks into queue
        /// </summary>
        /// <param name="tasks">The tasks of the workflow</param>
        public void LoadQueue(List<WorkflowMethod> tasks)
        {
            // Load up queue with the method names
            foreach (WorkflowMethod task in tasks)
            {
                CurrentTasks.Enqueue(task.MethodId);
            }

            // Store the actual info (such parameters)
            TaskInfo = tasks;
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

                // Get the current task id
                string currentTaskId = CurrentTasks.Dequeue();
                Debug.WriteLine($"Output from thread: {currentTaskId}");

                // Find the task info using the task id
                WorkflowMethod currentTask = TaskInfo.Find(workflowMethod => workflowMethod.MethodId == currentTaskId);
                // Give the name of the method and the parameters
                ModuleManager.Run(currentTask.MethodName, currentTask.Parameters);

                // TODO: Add try catch

                DateTime waitTill = DateTime.Now.AddSeconds(5);
                SpinWait.SpinUntil(() => DateTime.Now > waitTill);
            }

            Window.CurrentWindow.OpenAlertWindow("Executing Workflow", "Workflow has finished");
            // Send back nothing to tell the GUI to clear the coloured element
            Window.SendMessage("currentTask", "");
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