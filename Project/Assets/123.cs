using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T Item, float Priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((x, y) => x.Priority.CompareTo(y.Priority));
    }

    public T Dequeue()
    {
        if (elements.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty.");
        }

        T item = elements[0].Item;
        elements.RemoveAt(0);
        return item;
    }

    public bool Contains(T item)
    {
        return elements.Exists(x => EqualityComparer<T>.Default.Equals(x.Item, item));
    }
}
