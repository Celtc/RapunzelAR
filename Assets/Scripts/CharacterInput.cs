using UnityEngine;
using System.Collections;

public class CharacterInput
{
	#region Variables (private)

    private IntVector3 _direction;
    private bool _btnA;
    private bool _btnB;


	#endregion

	#region Propiedades (public)

    public IntVector3 Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    public bool BtnB
    {
        get { return _btnB; }
        set { _btnB = value; }
    }

    public bool BtnA
    {
        get { return _btnA; }
        set { _btnA = value; }
    }

	#endregion
    
	#region Metodos

    public CharacterInput()
    {
        this._btnA = this._btnB = false;
        this._direction = null;
    }

    public CharacterInput(IntVector3 direction, bool btnA, bool btnB)
    {
        this._btnA = btnA;
        this._btnB = btnB;
        this._direction = direction;
    }

    public bool HasInput()
    {
        return _btnA != false || _btnB != false || _direction != null;
    }

    public void Empty()
    {
        this._direction = null;
        this._btnA = this._btnB = false;
    }

    public static CharacterInput operator +(CharacterInput inA, CharacterInput inB)
    {
        return new CharacterInput(
            inB._direction != null ? inB._direction : inA._direction, 
            inB._btnA, 
            inB._btnB
        );
    }

	#endregion
}