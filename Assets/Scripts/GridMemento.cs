using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMemento
{
	#region Variables (private)

    private List<BlockState> _blocksState;
    private List<CharacterState> _charactersState;
    
	#endregion

	#region Propiedades (public)
    
    public List<BlockState> BlocksState
    {
        get { return _blocksState; }
    }

    public List<CharacterState> CharactersState
    {
        get { return _charactersState; }
    }

	#endregion
    
	#region Metodos

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="blocksState">Bloques en el estado almacenado por el memento</param>
    /// <param name="charactersState">Characters en el estado almacenado por el memento</param>
    public GridMemento(List<BlockState> blocksState, List<CharacterState> charactersState)
    {
        this._blocksState = blocksState;
        this._charactersState = charactersState;
    }

	#endregion
}
