using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class UITimerDisplay : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private UILabel _timerLabel;

    [SerializeField]
    private CharacterStadistics _charStadistics;

	#endregion
    
	#region Funciones de evento de Unity
    
	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{
	    if (_timerLabel != null && _charStadistics != null)
        {
            var currTime =  TimeSpan.FromSeconds(_charStadistics.Timer);
            _timerLabel.text = currTime.ToString().Substring(0, 8);
        }
	}

	#endregion

    #region Metodos (privados)
    
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
