using UnityEngine;
using System;
using System.Collections;

public class CharacterState
{
	#region Variables (private)

    private int _indexType;    
    private Vector3 _position;
    private Quaternion _rotation;
    private Character.ActionState _actionState;
    private float _life;
    private float _energy;
    private float _runSpeed;
    private float _generalSpeed;
    private float _fallingSpeed;

	#endregion

	#region Propiedades (public)

    public int IndexType
    {
        get { return _indexType; }
    }

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public float FallingSpeed
    {
        get { return _fallingSpeed; }
        set { _fallingSpeed = value; }
    }

    public float GeneralSpeed
    {
        get { return _generalSpeed; }
        set { _generalSpeed = value; }
    }

    public float RunSpeed
    {
        get { return _runSpeed; }
        set { _runSpeed = value; }
    }

    public float Energy
    {
        get { return _energy; }
        set { _energy = value; }
    }

    public float Life
    {
        get { return _life; }
        set { _life = value; }
    }

    public Character.ActionState ActionState
    {
        get { return _actionState; }
        set { _actionState = value; }
    }

    public Quaternion Rotation
    {
        get { return _rotation; }
        set { _rotation = value; }
    }

	#endregion
    
	#region Metodos
    
    public CharacterState(Type characterType)
    {
        this._indexType = Index.IndexOfCharacter(characterType);
    }

	#endregion
}
