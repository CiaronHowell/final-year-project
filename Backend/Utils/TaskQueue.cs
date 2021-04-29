using System;
using System.Collections.Generic;

namespace FinalYearProject.Backend.Utils
{
    /// <summary>
    /// Encapsulation of System.Collections.Generic.Queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskQueue<T>
    {
        /// <summary>
        /// The queue
        /// </summary>
        private Queue<T> _queue = new();

        /// <summary>
        /// Event for dequeuing
        /// </summary>
        public event EventHandler<DequeuedEventArgs<T>> Dequeued;

        /// <summary>
        /// On dequeued
        /// </summary>
        /// <param name="dequeuedElement"></param>
        private void OnDequeued(T dequeuedElement)
        {
            // Invoke the event if it isn't null
            Dequeued?.Invoke(this, new(dequeuedElement));
        }

        /// <summary>
        /// Gets the number of elements in the queue
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// Add element to queue
        /// </summary>
        /// <param name="element"></param>
        public void Enqueue(T element)
        {
            _queue.Enqueue(element);
        }

        /// <summary>
        /// Remove element from queue
        /// </summary>
        /// <returns>Element that was removed from queue</returns>
        public T Dequeue()
        {
            T element = _queue.Dequeue();
            // Get the element that is dequeued and invoke the event 
            OnDequeued(element);

            return element;
        }

        /// <summary>
        /// Clear the queue
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }
    }

    /// <summary>
    /// Dequeued event args
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DequeuedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Dequeued element
        /// </summary>
        public T DequeuedElement { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dequeuedElement">The dequeued element</param>
        public DequeuedEventArgs (T dequeuedElement)
        {
            DequeuedElement = dequeuedElement;
        }
    }
}
