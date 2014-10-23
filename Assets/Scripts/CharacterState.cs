using UnityEngine;
using System;
using System.Collections;

public class CharacterState
{
	#region Variables (private)

    protected int _indexType;
    protected Vector3 _position;
    protected Quaternion _rotation;
    protected Character.ActionState _actionState;
    protected float _life;
    protected float _energy;
    protected float _runSpeed;
    protected float _generalSpeed;
    protected float _fallingSpeed;
    protected int _moves;
    protected float _timer;

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

    public int Moves
    {
        get { return _moves; }
        set { _moves = value; }
    }

    public float Timer
    {
        get { return _timer; }
        set { _timer = value; }
    }

	#endregion
    
	#region Metodos
    
    public CharacterState(Type characterType)
    {
        this._indexType = Index.IndexOfCharacter(characterType);
    }

	#endregion
}
