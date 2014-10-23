using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class UIMovesDisplay : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private UILabel _movesLabel;

    [SerializeField]
    private CharacterStadistics _charStadistics;

	#endregion
    
	#region Funciones de evento de Unity
    
	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{
	    if (_movesLabel != null && _charStadistics != null)
        {
            _movesLabel.text = _charStadistics.Moves.ToString();
        }
	}

	#endregion

    #region Metodos (publicos)

    public void SetTarget(Character character)
    {
        var charStadistics = character.GetComponent<CharacterStadistics>();
        if (charStadistics != null)
            this._charStadistics = charStadistics;
    }
    
	#endregion
}
