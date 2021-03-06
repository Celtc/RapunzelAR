﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour
{
    #region Variables (private)

    [SerializeField]
    protected float _moveTime = .5f;
    [SerializeField]
    protected float _shakingTime = 1f; /* http://270c81.medialib.glogster.com/montereygirl14/media/4f/4f6879a9f0470ce7df04084b7dc2566f9c69c2ec/harlem-shake-3.gif */
    [SerializeField]
    protected float _shakingSpeed = 2f; // Cantidad de "Shakes" por seg
    [SerializeField]
    protected float _fallingSpeed = 5f;
    [SerializeField]
    protected bool _isBasement = false;

    [SerializeField]
    protected bool _isShaking = false;
    [SerializeField]
    protected bool _isFalling = false;
    [SerializeField]
    protected bool _isMoving = false;
    protected float _shakingTimer = 0f;
    protected bool _shakingRoutine = false;

    #endregion

    #region Propiedades (public)

    public IntVector3 Position
    {
        get
        {
            return CustomMathf.RoundToIntVector(new Vector3(transform.position.x - .5f, transform.position.y - .5f, transform.position.z - .5f));
        }
    }

    public float moveTime
    {
        get { return _moveTime; }
        set { _moveTime = value; }
    }

    public bool isBasement
    {
        get { return _isBasement; }
        set { _isBasement = value; }
    }

    public bool isMoving
    {
        get
        {
            return _isMoving;
        }
    }

    public bool isShaking
    {
        get
        {
            return _isShaking;
        }
    }

    public bool isFalling
    {
        get
        {
            return _isFalling;
        }
    }

    #endregion

    #region Funciones de evento de Unity

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    protected void Update()
    {
        if (isBasement) return;

        if (!_isMoving)
            GravityCheck();

        if (_isShaking)
            Shake();

        else if (_isFalling)
            Fall();

    }

    #endregion

    #region Metodos privados

    /// <summary>
    /// Reinicia los flas de estados
    /// </summary>
    protected void ResetState()
    {
        this._isFalling = false;
        this._isMoving = false;
    }

    /// <summary>
    /// Determina si el bloque esta temblando o cayendo, y activa los respectivos flags.
    /// </summary>
    protected void GravityCheck()
    {
        _isShaking = false;
        _isFalling = false;

        // Consigue la lista de bloques que estan debajo de este
        var underBlocks = SupportingBlocks();

        // No hay bloques debajo
        if (underBlocks.Count == 0)
        {
            // Si ya temblo el tiempo suficiente, cae
            if (_shakingTimer >= _shakingTime)
            {
                // Establece el estado de Fall
                _isFalling = true;
            }
            else
            {
                // Establece el estado de Shake
                _isShaking = true;
            }
        }

        // Hay bloques debajo pero estan temblando
        else if (!SupportIsFirm(underBlocks))
        {
            _isShaking = true;
        }

        // Hay bloques firmes debajo
        else
        {
            // Reinicia al contador de shaking
            _shakingTimer = 0f;
        }
    }

    protected float MoveBlock(IntVector3 direction, float moveTime)
    {
        // Si es un cubo fijo no se mueve
        if (_isBasement) return float.NaN;

        // El tiempo en moverse puede ser menor o igual al definido
        if (moveTime < _moveTime)
            moveTime = _moveTime;

        // Determina si hay otro cubo implicado en el movimiento
        // El tiempo en moverlos sera el mayor de todos (una cadena es tan fuerte como el mas debil de sus eslabones)
        var chainCube = Level.Instance.Grid[Position + direction];
        if (chainCube != null)
            moveTime = chainCube.MoveBlock(direction, moveTime);

        // Si el moveTime  se vuelve NaN significa que hay algun bloque encadenado que no se puede mover
        if (moveTime == float.NaN) return float.NaN;

        // Mueve el bloque
        var from = transform.position;
        var to = transform.position + (Vector3)direction;
        Translate(from, to, moveTime);

        // Devuelve el tiempo que demoro mover el bloque y sus encadenados
        return moveTime;
    }

    protected void Translate(Vector3 from, Vector3 to, float duration, Action postAction = null)
    {
        StartCoroutine(TranslateRoutine(from, to, duration, postAction));
    }

    /// <summary>
    /// Rutina interna invocada por Translate()
    /// </summary>
    protected IEnumerator TranslateRoutine(Vector3 from, Vector3 to, float duration, Action postAction)
    {
        this._isMoving = true;

        // Variables temporales
        var fraction = 0f;
        var elapsedTime = -Time.deltaTime;

        // Mientras que el tiempo sea menor a la duracion
        while (elapsedTime < duration)
        {
            // Incrementa el tiempo
            elapsedTime += Time.deltaTime;

            // Realiza el lerp
            fraction = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector3.Lerp(from, to, fraction);

            // Espera al siguiente frame (solo si no termino de moverse)
            if (fraction < 1f) yield return 0;
        }

        // Actualiza la grilla
        Level.Instance.Grid.Update(this);

        // Ejecuta la accion si la hay
        if (postAction != null)
            postAction();

        this._isMoving = false;
    }

    /// <summary>
    /// Accion de caer 1 posicion
    /// </summary>
    protected void Fall()
    {
        if (!isMoving)
        {
            // Comienza la traslacion
            var from = transform.position;
            var to = from - Vector3.up;
            Translate(from, to, 1f / _fallingSpeed,
                (to.y < -10f) ? (Action)(() => { this.Destroy(); }) : null
            );

            // Si hay un character abajo, lo aplasta
            var character = Level.Instance.Grid.CharacterAt(Position - Vector3.up);
            if (character != null)
                character.Smash();
        }
    }

    /// <summary>
    /// Accion de temblar durante "shakingTime" segundos
    /// </summary>
    protected void Shake()
    {
        if (!_shakingRoutine)
        {
            StartCoroutine(ShakeRoutine());
        }
    }

    /// <summary>
    /// Rutina interna invocada por Shake()
    /// </summary>
    protected IEnumerator ShakeRoutine()
    {
        _shakingRoutine = true;

        var maxAngle = 5f;
        var tElapsed = 0f;
        var duration = 1 / _shakingSpeed;

        while (tElapsed < duration)
        {
            var currAngle = Mathf.PingPong(tElapsed * 4 / duration, 1) * maxAngle;
            if (tElapsed > duration * 0.5f)
                currAngle *= -1;

            transform.rotation = Quaternion.Euler(currAngle, transform.rotation.eulerAngles.y, currAngle);

            tElapsed += Time.deltaTime;
            if (_isShaking) _shakingTimer += Time.deltaTime;

            yield return 0;
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

        _shakingRoutine = false;
    }

    /// <summary>
    /// Obtiene todos los bloques que sostienen este bloque
    /// </summary>
    /// <returns>Lista de bloques soporte</returns>
    protected List<Block> SupportingBlocks()
    {
        var blocks = new List<Block>();

        var myPos = Position;
        var underBlocksPos = new IntVector3[] {
            CustomMathf.RoundToIntVector(myPos.x, myPos.y - 1, myPos.z-1),
            CustomMathf.RoundToIntVector(myPos.x-1, myPos.y - 1, myPos.z),
            CustomMathf.RoundToIntVector(myPos.x, myPos.y - 1, myPos.z),
            CustomMathf.RoundToIntVector(myPos.x+1, myPos.y - 1, myPos.z),
            CustomMathf.RoundToIntVector(myPos.x, myPos.y - 1, myPos.z+1)
        };

        foreach (var pos in underBlocksPos)
        {
            var underBlock = Level.Instance.Grid[pos];
            if (underBlock != null && !underBlock.isFalling)
            {
                blocks.Add(underBlock);
            }
        }

        return blocks;
    }

    /// <summary>
    /// Dada la lista de bloques que soportan el bloque, determina si este bloque esta firme o temblando
    /// </summary>
    /// <param name="blocks">Lista de bloques soporte</param>
    /// <returns>Verdadero si este bloque esta temblando</returns>
    protected bool SupportIsFirm(List<Block> blocks)
    {
        return blocks.Exists(x => !x.isShaking);
    }

    #endregion

    #region Metodos publicos

    /// <summary>
    /// Destruye el bloque y lo elimina de la grilla
    /// </summary>
    public void Destroy()
    {
        Level.Instance.Grid.RemoveBlock(this);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Establece una nueva posicion para el bloque. Cualquier flag de shake o falling son reseteados.
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(IntVector3 pos)
    {
        transform.position = new Vector3(pos.x + .5f, pos.y + .5f, pos.z + .5f);

        ResetState();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromPos"></param>
    /// <returns></returns>
    public float Pushed(IntVector3 fromPos)
    {
        // Calcula la direccion y mueve el bloque
        var direction = Position - fromPos;
        return MoveBlock(direction, _moveTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromPos"></param>
    /// <returns></returns>
    public float Pulled(IntVector3 fromPos)
    {
        // Calcula la direccion y mueve el bloque
        var direction = fromPos - Position;
        return MoveBlock(direction, _moveTime);
    }

    /// <summary>
    /// Obtiene el estado interno del bloque
    /// </summary>
    /// <returns>Estado actual</returns>
    public BlockState GetState()
    {
        var state = new BlockState(this.GetType());
        state.Position = this.Position;
        state.FallingSpeed = this._fallingSpeed;
        state.ShakingSpeed = this._shakingSpeed;
        state.ShakingTime = this._shakingTime;
        state.IsBasement = this._isBasement;
        state.MoveTime = this._moveTime;

        return state;
    }

    /// <summary>
    /// Asigna un nuevo estado al bloque
    /// </summary>
    /// <param name="state">Estado deseado</param>
    public void SetState(BlockState state)
    {
        SetPosition(state.Position);
        this._fallingSpeed = state.FallingSpeed;
        this._shakingSpeed = state.ShakingSpeed;
        this._shakingTime = state.ShakingTime;
        this._isBasement = state.IsBasement;
        this._moveTime = state.MoveTime;
    }

    #endregion
}
