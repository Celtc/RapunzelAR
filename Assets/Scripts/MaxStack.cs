using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implementacion generica de una pila con tamaño maximo,
/// Cuando se agrega un elemento pasada la capacidad maxima, se elimina el elemento mas viejo
/// </summary>
[System.Serializable]
public class MaxStack<T>
{
    #region Variables (private)

    [SerializeField]
    private int _limit;
    [SerializeField]
    private LinkedList<T> _list;

    #endregion

    #region Metodos

    public MaxStack(int maxSize)
    {
        _limit = maxSize;
        _list = new LinkedList<T>();

    }

    public void Push(T value)
    {
        if (_list.Count == _limit)
        {
            _list.RemoveLast();
        }
        _list.AddFirst(value);
    }

    public T Pop()
    {
        if (_list.Count > 0)
        {
            T value = _list.First.Value;
            _list.RemoveFirst();
            return value;
        }
        else
        {
            throw new InvalidOperationException("The Stack is empty");
        }
    }

    public T Peek()
    {
        if (_list.Count > 0)
        {
            T value = _list.First.Value;
            return value;
        }
        else
        {
            throw new InvalidOperationException("The Stack is empty");
        }

    }

    public void Clear()
    {
        _list.Clear();

    }

    public int Count
    {
        get { return _list.Count; }
    }

    /// <summary>
    /// Chequea que el ultimo elemento coincida con el deseado
    /// </summary>
    /// <param name="value">Valor a comparar</param>
    /// <returns></returns>
    public bool IsTop(T value)
    {
        bool result = false;
        if (this.Count > 0)
        {
            result = Peek().Equals(value);
        }
        return result;
    }

    public bool Contains(T value)
    {
        bool result = false;
        if (this.Count > 0)
        {
            result = _list.Contains(value);
        }
        return result;
    }

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    #endregion
}
