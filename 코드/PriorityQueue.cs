using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : new()
{    
    List<T> _list;
    CompareMethod<T> _compare;

    public delegate bool CompareMethod<T1>(T1 a, T1 b);    

    public PriorityQueue(CompareMethod<T> method)
    {
        _list = new List<T>();
        _list.Add(default(T));
        _compare = method;
    }

    public void Push(T value)
    {
        _list.Add(value);

        heapify(_list.Count - 1);
    }

    public T Top()
    {
        if (Empty())
            return _list[0];

        return _list[1];
    }

    public bool Empty()
    {
        if (_list.Count == 1) return true;

        return false;
    }

    public void Pop()
    {
        if (_list.Count == 1) return;

        swap(1, _list.Count - 1);
        _list.RemoveAt(_list.Count - 1);

        heapifyReverse(1);
    }

    public void Clear()
    {
        while(_list.Count > 1) Pop();
    }


    void heapify(int i)
    {
        int idx = i;
        int parentIdx = idx / 2;

        while (parentIdx > 0)
        {
            if (_compare(_list[parentIdx], _list[idx]))
            {
                swap(parentIdx, idx);                
            }

            idx = parentIdx;
            parentIdx = idx / 2;
        }
    }

    void heapifyReverse(int i)
    {
        int idx = i;
        int left = idx * 2;
        int right = idx * 2 + 1;

        while (left < _list.Count || right < _list.Count)
        {
            if (left < _list.Count && !_compare(_list[left], _list[idx]) &&
                right < _list.Count && !_compare(_list[right], _list[idx]))
            {
                if (!_compare(_list[right], _list[left])) { swap(idx, right); idx = right; }
                else { swap(idx, left); idx = left; }
            }
            else if (left < _list.Count && !_compare(_list[left], _list[idx]))
            {
                swap(idx, left);
                idx = left;
            }
            else if (right < _list.Count && !_compare(_list[right], _list[idx]))
            {
                swap(idx, right);
                idx = right;
            }
            else break;

            left = idx * 2;
            right = idx * 2 + 1;
        }
    }

    public void swap(int idx1, int idx2)
    {
        T temp = _list[idx1];
        _list[idx1] = _list[idx2];
        _list[idx2] = temp;
    }

    public void print()
    {
        foreach (var tr in _list)
        {
            Debug.Log(tr);
        }
    }
}