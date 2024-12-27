using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Model;

public class CircularBuffer<T>
{
    private Queue<T> _queue;
    private int _capacity;

    public CircularBuffer(int capacity)
    {
        _queue = new Queue<T>(capacity);
        _capacity = capacity;
    }

    public void Add(T item)
    {
        if (_queue.Count == _capacity)
        {
            _queue.Dequeue();
        }
        _queue.Enqueue(item);
    }

    public T[] GetItems()
    {
        return _queue.ToArray();
    }

    public int Count => _queue.Count;
    public int Capacity => _capacity;
}
