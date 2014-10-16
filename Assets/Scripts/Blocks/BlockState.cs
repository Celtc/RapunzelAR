using UnityEngine;
using System;
using System.Collections;

public class BlockState
{
	#region Variables (private)

    private int _indexType;
    private float _moveTime;
    private float _shakingTime;
    private float _shakingSpeed;
    private float _fallingSpeed;
    private Vector3 _position;
    private bool _isBasement;

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

    public float MoveTime
    {
        get { return _moveTime; }
        set { _moveTime = value; }
    }

    public float ShakingTime
    {
        get { return _shakingTime; }
        set { _shakingTime = value; }
    }

    public float ShakingSpeed
    {
        get { return _shakingSpeed; }
        set { _shakingSpeed = value; }
    }

    public float FallingSpeed
    {
        get { return _fallingSpeed; }
        set { _fallingSpeed = value; }
    }

    public bool IsBasement
    {
        get { return _isBasement; }
        set { _isBasement = value; }
    }

	#endregion
    
	#region Metodos

    public BlockState(Type blockType)
    {
        this._indexType = Index.IndexOfBlock(blockType);
    }

	#endregion
}
