using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridOriginator
{
	#region Variables (private)

    private Grid _grid;
    
	#endregion

	#region Propiedades (public)
    
    public Grid Grid
    {
        get { return _grid; }
    }

	#endregion
    
	#region Metodos

    /// <summary>
    /// Contructor
    /// </summary>
    public GridOriginator()
    {
        this._grid = new Grid();
    }

    /// <summary>
    /// Limpia la grilla
    /// </summary>
    public void Clear()
    {
        this._grid = new Grid();
    }

    /// <summary>
    /// Crea un momento del estado actual de la grilla
    /// </summary>
    /// <returns>Estado actual</returns>
    public GridMemento CreateMemento()
    {
        // Lista de estados de characters
        var charactersState = new List<CharacterState>();
        foreach(var character in Level.Grid.AllCharacters)
        {
            charactersState.Add(character.GetState());
        }

        // Lista de estados de bloques
        var blocksState = new List<BlockState>();
        foreach (var block in Level.Grid.AllBlocks)
        {
            blocksState.Add(block.GetState());
        }

        // Crea el memento
        return new GridMemento(blocksState, charactersState);
    }

    /// <summary>
    /// Establece un nuevo estado a la grilla
    /// </summary>
    /// <param name="memento">Estado deseado</param>
    public void SetMemento(GridMemento memento)
    {
        // Destruye todos los objetos
        Level.Unload();

        // Carga el memento
        Level.Load(memento);
    }

	#endregion
}
