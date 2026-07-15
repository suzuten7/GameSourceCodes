using System;

public class CircularBuffer<T>
{
    private T[] buffer;
    private int head;
    private int tail;
    private int count;

    public int Capacity => buffer.Length;
    public int Count => count;

    public CircularBuffer(int capacity)
    {
        buffer = new T[capacity];
    }

    public void Enqueue(T item)
    {
        buffer[tail] = item;
        tail = (tail + 1) % Capacity;
        if (count == Capacity)
        {
            head = (head + 1) % Capacity; // overwrite
        }
        else
        {
            count++;
        }
    }

    public T Dequeue()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Buffer is empty");
        }
        var item = buffer[head];
        head = (head + 1) % Capacity;
        count--;
        return item;
    }
}
