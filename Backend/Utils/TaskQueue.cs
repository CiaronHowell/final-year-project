using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalYearProject.Backend.Utils
{
    public class TaskQueue<T>
    {
        private Queue<T> _queue = new();

        public event EventHandler<DequeuedEventArgs<T>> Dequeued;

        private void OnDequeued(T dequeuedElement)
        {
            Dequeued?.Invoke(this, new(dequeuedElement));
        }

        public int Count => _queue.Count;

        public void Enqueue(T element)
        {
            _queue.Enqueue(element);
        }

        public T Dequeue()
        {
            T element = _queue.Dequeue();
            OnDequeued(element);
            return element;
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }

    //public delegate void DequeuedEventHandler(object sender, DequeuedEventArgs e);

    public class DequeuedEventArgs<T> : EventArgs
    {
        public T DequeuedElement { get; private set; }

        public DequeuedEventArgs (T dequeuedElement)
        {
            DequeuedElement = dequeuedElement;
        }


    }
}
