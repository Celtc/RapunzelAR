using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCaretaker
{
	#region Variables (private)

    private MaxStack<GridMemento> _stack;

	#endregion
    
	#region Metodos

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="stackSize">Tamaño del stack</param>
    public GridCaretaker(int stackSize)
    {
        this._stack = new MaxStack<GridMemento>(stackSize);
    }

    /// <summary>
    /// Extrae el ultimo memento
    /// </summary>
    /// <returns>El ultimo memento o null si no hay ninguno</returns>
    public GridMemento Pop()
    {
        if (_stack.Count == 0) return null;

        return _stack.Pop();
    }

    /// <summary>
    /// Agrega un nuevo memento
    /// </summary>
    /// <param name="gridMemento">Memento a agregar</param>
    public void Push(GridMemento gridMemento)
    {
        _stack.Push(gridMemento);
    }

	#endregion
}
