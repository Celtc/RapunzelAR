using UnityEngine;
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

    protected float _shakingTimer = 0f;
    protected bool _shakingRoutine = false;
    protected bool _isShaking = false;
    protected bool _isFalling = false;
    protected bool _isMoving = false;

    #endregion

    #region Propiedades (public)
    
    public IntVector3 position
    {
        get { return transform.position; }
        set { transform.position = new Vector3(value.x + .5f, value.y + .5f, value.z + .5f); }
    }

    public Vector3 exactPosition
    {
        get { return new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z - .5f); }
        private set { transform.position = new Vector3(value.x + .5f, value.y + .5f, value.z + .5f); }
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
        else if (SupportIsShaking(underBlocks))
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
        var chainCube = Level.Grid[position + direction];
        if (chainCube != null)
            moveTime = chainCube.MoveBlock(direction, moveTime);

        // Si el moveTime  se vuelve NaN significa que hay algun bloque encadenado que no se puede mover
        if (moveTime == float.NaN) return float.NaN;

        // Mueve el bloque
        var from = transform.position;
        var to = transform.position + direction;
        Translate(from, to, moveTime);

        // Devuelve el tiempo que demoro mover el bloque y sus encadenados
        return moveTime;
    }
        
    protected void Translate(IntVector3 from, IntVector3 to, float duration, Action postAction = null)
    {
        StartCoroutine(TranslateRoutine(from, to, duration, postAction));
    }

    protected IEnumerator TranslateRoutine(IntVector3 from, IntVector3 to, float duration, Action postAction)
    {
        this._isMoving = true;

        // Variables temporales
        var fraction = 0f;
        var elapsedTime = 0f;

        // Comienza al sig. frame
        yield return 0;

        // Mientras que el tiempo sea menor a la duracion
        while (elapsedTime < duration)
        {
            // Incrementa el tiempo
            elapsedTime += Time.deltaTime;

            // Realiza el lerp
            fraction = Mathf.Clamp01(elapsedTime / duration);
            exactPosition = Vector3.Lerp(from, to, fraction);

            // Espera al siguiente frame
            yield return 0;
        }

        // Ejecuta la accion si la hay
        if (postAction != null)
            postAction();

        // Actualiza la grilla
        Level.Grid.Update(this);

        this._isMoving = false;
    }
    
    protected void Fall()
    {
        if (!isMoving)
        {
            var from = position;
            var to = position - Vector3.up;
            Translate(from, to, 1f / _fallingSpeed,
                (to.y == 0) ? (Action)(() => { _isBasement = true; }) : null
            );
        }
    }

    protected void Shake()
    {
        if (!_shakingRoutine)
        {
            StartCoroutine(ShakeRoutine());
        }
    }

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
    
    protected List<Block> SupportingBlocks()
    {
        var blocks = new List<Block>();

        var myPos = (IntVector3)transform.position;
        var underBlocksPos = new IntVector3[] { 
            new IntVector3(myPos.x, myPos.y - 1, myPos.z-1),
            new IntVector3(myPos.x-1, myPos.y - 1, myPos.z),
            new IntVector3(myPos.x, myPos.y - 1, myPos.z),
            new IntVector3(myPos.x+1, myPos.y - 1, myPos.z),
            new IntVector3(myPos.x, myPos.y - 1, myPos.z+1),
        };

        foreach (var pos in underBlocksPos)
        {
            var underBlock = Level.Grid[pos];
            if (underBlock && !underBlock.isFalling)
            {
                blocks.Add(underBlock);
            }
        }

        return blocks;
    }

    protected bool SupportIsShaking(List<Block> blocks)
    {
        return !blocks.Exists(x => !x._isShaking);
    }
    
    #endregion

    #region Metodos publicos
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromPos"></param>
    /// <returns></returns>
    public float Pushed(IntVector3 fromPos)
    {
        // Calcula la direccion y mueve el bloque
        var direction = position - fromPos;
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
        var direction = fromPos - position;
        return MoveBlock(direction, _moveTime);
    }
    
    #endregion
}
