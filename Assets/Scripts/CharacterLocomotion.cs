using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class CharacterLocomotion : MonoBehaviour
{
    #region Variables (private)

    [SerializeField]
    private float _runSpeed = 3f;

    [SerializeField]
    private float _generalSpeed = 1f;

    [SerializeField]
    private List<ActionData> _actionsData;
    private Dictionary<string, ActionData> _actionsHash;

    private State _state;
    private CharacterInput _input;
    private CharacterMecanimController _mecanim;
    
    #endregion

    #region Properties (public)

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

    public IntVector3 Position
    {
        get
        {
            return CustomMathf.RoundToIntVector(new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z - .5f));
        }
    }

    public IntVector3 Direction
    {
        get
        {
            return CustomMathf.RoundToIntVector(transform.forward);
        }
    }

    #endregion

    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        _mecanim = GetComponent<CharacterMecanimController>();
        _input = new CharacterInput();
        GenerateHashes();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        // En caso de que este inactivo, permite el comienzo de una nueva accion
        if (IsInactive())
            StartAction();
    }

    #endregion

    #region Metodos privados

    /// <summary>
    /// Genera la hashtable (diccionario) de las acciones
    /// </summary>
    private void GenerateHashes()
    {
        if (_actionsHash == null)
        {
            _actionsHash = new Dictionary<string, ActionData>();
        }

        _actionsHash.Clear();

        for (int i = 0; i < _actionsData.Count; i++)
        {
            _actionsHash.Add(_actionsData[i].Name, _actionsData[i]);
        }
    }

    /// <summary>
    /// Determina la acción a realizar. 
    /// Segun el input y las posibilidades a partir de los cubos que lo rodean.
    /// </summary>
    private void StartAction()
    {
        // Esta colgado
        if (_state == State.Hanging)
        {
            // Si no hay input
            if (!_input.HasInput())
            {
                Hang();
            }

            // Hay input
            else
            {
                HangingManagement();
            }
        }

        // Esta agarrando un bloque
        else if (_state == State.Gripping)
        {
            // Si suelta el boton de agarre
            if (!_input.BtnA)
            {
                Debug.Log("Accion: Soltando bloque (no hay input)");
                ReleaseGrip();
            }

            // Hay input de boton de agarre (al menos)
            else
            {
                GrippingManagement();
            }
        }

        // Esta parado
        else if (_state == State.Standing)
        {
            // Si no hay input
            if (!_input.HasInput())
            {
                Stand();
            }

            // Hay input
            else
            {
                // Hay input de boton de agarre
                if (_input.BtnA)
                {
                    GrippingManagement();
                }

                // Sigue estando parado
                if (_state == State.Standing)
                {
                    // Hay input de direccion
                    if (_input.Direction != null)
                    {
                        LocomotionManagement();
                    }

                    // No hay input de direccion
                    else
                    {
                        // Se queda parado
                        Stand();
                    }
                }
            }
        }

        // Borra el input almacenado
        _input.Empty();
    }

    private void GrippingManagement()
    {
        // Determina la posicion objetivo
        var frontPos = Position + Direction;

        // Obtiene el cubo objetivo
        var grippedBlock = Level.Grid.ExistsStillAt(frontPos) ? Level.Grid[frontPos] : null;

        // Si no estaba agarrando un bloque (estaba parado)
        if (_state != State.Gripping)
        {
            // Hay bloque => Agarra
            if (grippedBlock != null)
            {
                Debug.Log("Accion: Agarrando bloque");
                Grip();
            }
        }

        // Estaba agarrando un bloque y hay que bloque que agarrar
        else if (grippedBlock != null)
        {
            // Hay input de movimiento
            if (_input.Direction != null)
            {
                
                // Empuja hacia el bloque
                if (_input.Direction == Direction)
                {
                    // Empuja
                    Debug.Log("Accion: Empujando");
                    Push(grippedBlock);
                }

                // Tira del bloque
                else if (_input.Direction == Direction * -1f)
                {
                    // Tira
                    Debug.Log("Accion: Tirando");
                    Pull(grippedBlock);
                }
            }

            // No hay input de movimiento
            else
            {
                // Se queda agarrando el bloque
                Grip();
            }
        }

        // Estaba agarrando un bloque pero NO hay bloque que agarrar
        else
        {
            Debug.Log("Accion: Soltando bloque (sin bloque que agarrar)");
            ReleaseGrip();
        }
    }

    private void HangingManagement()
    {
        // Hay input del boton de agarre
        if (_input.BtnA)
        {
            Hang();
        }
        // Hay input de direccion unicamente
        else
        {
            // Transforma el input (da prioridad al input vertical)    
            var newInput = new Vector3(0, _input.Direction.z, 0);
            if (newInput.y == 0 && _input.Direction.x != 0)
            {
                if (_input.Direction.x > 0)
                    newInput += transform.right;
                else
                    newInput += (transform.right * -1f);
            }

            // Determina la posicion objetivo
            var targetPos = Position + newInput;
            //Debug.Log(Position);
            //Debug.Log(targetPos);

            // Desplazamiento horizontal
            if (newInput.y == 0 && newInput.x != 0)
            {
                // Hay un cubo en el lugar destino? -> Dobla (Acute)
                if (Level.Grid.ExistsStillAt(targetPos))
                {
                    // Direccion de strafe
                    if (_input.Direction.x > 0)
                    {
                        Debug.Log("Accion: Colgando -> Giro Derecha Concavo");
                        HangRightConcave();
                    }
                    else
                    {
                        Debug.Log("Accion: Colgando -> Giro Izquierda Concavo");
                        HangLeftConcave();
                    }
                }

                // Hay un cubo delante del lugar destino?
                else if (Level.Grid.ExistsStillAt(targetPos + Direction))
                {
                    // Direccion de strafe
                    if (_input.Direction.x > 0)
                    {
                        Debug.Log("Accion: Colgando -> Derecha");
                        HangRight();
                    }
                    else
                    {
                        Debug.Log("Accion: Colgando -> Izquierda");
                        HangLeft();
                    }
                }

                // No hay cubo en el lugar destino y no hay cubo que impida doblar -> Dobla (Obtuse)
                else
                {
                    if (_input.Direction.x > 0 &&
                        !Level.Grid.ExistsStillAt(targetPos + Direction + Vector3.up))
                    {
                        Debug.Log("Accion: Colgando -> Giro Derecha Convexo");
                        HangRightConvexe();
                    }
                    else if (!Level.Grid.ExistsStillAt(targetPos + Direction + Vector3.up))
                    {
                        Debug.Log("Accion: Colgando -> Giro Izquierda Convexo");
                        HangLeftConvexe();
                    }

                }
            }

            // Desplazamiento vertical
            else if (newInput.y == 1)
            {
                if (!Level.Grid.ExistsStillAt(targetPos + Direction))
                {
                    Debug.Log("Accion: Colgando -> Subir");
                    HangUp();
                }
                else
                {
                    Hang();
                }
            }
        }
    }

    private void LocomotionManagement()
    {
        // Si no esta orientado, gira
        if (Direction != _input.Direction)
        {
            Debug.Log("Accion: Girando (" + _input.Direction + ")");
            Turn(_input.Direction);
            return;
        }

        // Determina la posicion objetivo
        var targetPos = Position + Direction;
        
        // Hay un cubo en el lugar destino?
        if (Level.Grid.ExistsAt(targetPos))
        {
            // El cubo a trepar no esta quieto o hay un cubos que impidan trepar? (arriba nuestro o arriba del cubo a trepar)
            if (Level.Grid.ExistsStillAt(targetPos) &&
                !Level.Grid.ExistsStillAt(targetPos + Vector3.up) &&
                !Level.Grid.ExistsStillAt(Position + Vector3.up))
            {
                // Trepa
                Debug.Log("Accion: Trepando");
                Climb();
            }
        }

        // Hay un cubo en el piso del lugar destino?
        else if (Level.Grid.ExistsStillAt(targetPos - Vector3.up))
        {
            // Avanza
            Debug.Log("Accion: Avanzando");
            Run();
        }

        // Hay un cubo 2 niveles mas abajo?
        else if (Level.Grid.ExistsStillAt(targetPos - Vector3.up * 2f))
        {
            // Salta abajo
            Debug.Log("Accion: Saltando abajo");
            JumpDown();
        }

        else
        {
            // Se cuelga
            Debug.Log("Accion: Colgandonse");
            HangDown();
        }
    }
    
    /// <summary>
    /// Se encarga las transformaciones y cambios en el flag de estado
    /// </summary>
    /// <param name="action">Contenedor con los datos de las transformaciones</param>
    /// <param name="newState">Estado en el cual se encontrara mientras realiza la accion</param>
    /// <param name="endState">Estado en el que quedara luego de terminada la accion</param>
    /// <param name="speedFactor">Velocidad a la que realiza la accion. Siendo '1' la velocidad normal.</param>
    /// <param name="afterAction">Delegado de intrucciones a realizar finalizada la tarea</param>
    /// <returns></returns>
    private IEnumerator DoAction(ActionData action, State newState, State endState, float speedFactor, Action afterAction = null)
    {
        // Cambio al nuevo estado y establece el root motion
        _state = newState;
        _mecanim.ApplyRootMotion(action.RootMotion);

        // Almacena valores iniciales
        var elapsedTime = -Time.deltaTime;
        var startPosition = transform.position;
        var startRotation = transform.rotation;
        var actionDuration = action.duration / speedFactor;

        // Flags de presencia de curvas
        var hasDeltaX = action.deltaX.keys.Length > 0;
        var hasDeltaY = action.deltaY.keys.Length > 0;
        var hasDeltaZ = action.deltaZ.keys.Length > 0;
        var hasDeltaR = action.deltaR.keys.Length > 0;

        // A lo largo de la duracion de la accion aplica las tranformaciones
        var deltaRotation = 0f;
        var deltaPosition = Vector3.zero;
        while (elapsedTime < actionDuration)
        {
            // Incrementa el tiempo
            elapsedTime += Time.deltaTime;

            // Porcentaje [0,1] de progreso de la accion
            var fraction = Mathf.Clamp01(elapsedTime / actionDuration);
            //Debug.Log(fraction);

            #region Posicion

            // Obtiene el nuevo delta
            var currDeltaPosition = new Vector3(
                hasDeltaX ? action.deltaX.Evaluate(fraction) : 0f,
                hasDeltaY ? action.deltaY.Evaluate(fraction) : 0f,
                hasDeltaZ ? action.deltaZ.Evaluate(fraction) : 0f
            );

            // Lo orienta
            currDeltaPosition = startRotation * currDeltaPosition;

            // Si es el ultimo frame de accion el posicionamiento es absoluto, sino relativo
            if (fraction == 1f)
            {
                // Modifica la posicion
                transform.position = startPosition + currDeltaPosition;
            }
            else
            {
                // Modifica la posicion
                transform.position += currDeltaPosition - deltaPosition;

                // Guarda el delta actual
                deltaPosition = currDeltaPosition;
            }

            #endregion

            #region Rotacion

            if (hasDeltaR)
            {
                // Obtiene el nuevo delta
                var currDeltaRotation = action.deltaR.Evaluate(fraction);

                // Si es el ultimo frame de accion la rotacion es absoluta, sino relativa
                if (fraction == 1f)
                {
                    transform.rotation = startRotation * Quaternion.Euler(0f, currDeltaRotation, 0f);
                }
                else
                {
                    transform.rotation = transform.rotation * Quaternion.Euler(0f, currDeltaRotation - deltaRotation, 0f);
                    deltaRotation = currDeltaRotation;
                }
            }

            #endregion

            // Espera al siguiente frame, solo si no termino la accion
            if (fraction != 1f) yield return 0;
        }

        // Ejecuta la accion si la hay
        if (afterAction != null)
            afterAction();

        // Cambia al estado en que queda luego de la accion y desactiva el rootMotion
        _state = endState;
        _mecanim.ApplyRootMotion(false);
    }

    #region Acciones

    private void Stand()
    {
        // Nuevo estado
        _state = State.Standing;

        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);
    }

    private void Hang()
    {
        // Nuevo estado
        _state = State.Hanging;

        // Confugiracion de mecanim
        _mecanim.SetFlag("Hanging");
        _mecanim.SetSpeed(_generalSpeed);
    }

    private void Grip()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Gripping");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(new ActionData("Gripping", .15f), State.Moving, State.Gripping, _generalSpeed));
    }

    private void ReleaseGrip()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(new ActionData("ReleaseGripping", .15f), State.Moving, State.Standing, _generalSpeed));
    }

    private void Turn(Vector3 forward)
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);

        // Determina el tipo de giro (izq, derecha, atras)
        ActionData actionData;
        var angle = Mathf.Round(Vector3.Angle(Direction, forward));
        if (angle == 180f)
            actionData = _actionsHash["TurnBack"];
        else if (Vector3.Cross(forward, Direction).y > 0)
            actionData = _actionsHash["TurnLeft"];
        else
            actionData = _actionsHash["TurnRight"];

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(actionData, State.Moving, State.Standing, _generalSpeed));
    }

    private void Run()
    {
        // Confugiracion de mecanim
        _mecanim.SetSpeed(1f);
        _mecanim.SetFlag("Run");
        _mecanim.SetFloat("RunSpeed", _runSpeed / transform.localScale.x);

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(DoAction(_actionsHash["Run"], State.Moving, State.Standing, _runSpeed, delegate { _mecanim.SetSpeed(_generalSpeed); }));
    }

    private void Climb()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Climb");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["Climb"], State.Moving, State.Standing, _generalSpeed));
    }

    private void JumpDown()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("JumpDown");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["JumpDown"], State.Moving, State.Standing, _generalSpeed));
    }

    private void Push(Block block)
    {
        // Comunica al bloque la instruccion de "Push" y le devuelve el tiempo que demora la accion
        var duration = block.Pushed(Position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(_Push(1 / duration));
    }

    private IEnumerator _Push(float actionSpeed)
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Push");
        _mecanim.SetSpeed(actionSpeed);

        // Realiza la accion
        yield return StartCoroutine(DoAction(_actionsHash["Push"], State.Moving, State.Moving, actionSpeed));

        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);

        // Espera a que termine la transicion antes de setear el estado final
        yield return new WaitForSeconds(0.2f);
        _state = State.Standing;
    }

    private void Pull(Block block)
    {
        // Comunica al bloque la instruccion de "Pull" y le devuelve el tiempo que demora la accion
        var duration = block.Pulled(Position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;

        // Confugiracion de mecanim
        var actionSpeed = 1 / duration;
        _mecanim.SetFlag("Pull");
        _mecanim.SetSpeed(actionSpeed);

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(DoAction(_actionsHash["Pull"], State.Moving, State.Gripping, actionSpeed, delegate { _mecanim.SetSpeed(_generalSpeed); }));
    }

    private void HangUp()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangUp");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangUp"], State.Moving, State.Standing, _generalSpeed));
    }

    private void HangDown()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangDown");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangDown"], State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangLeft()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangLeft");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangLeft"], State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangRight()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangRight");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangRight"], State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangLeftConcave()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangLeftConcave");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangLeftConcave"], State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangRightConcave()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangRightConcave");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangRightConcave"], State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangLeftConvexe()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangLeftConvexe");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangLeftConvexe"], State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangRightConvexe()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangRightConvexe");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangRightConvexe"], State.Moving, State.Hanging, _generalSpeed));
    }

    #endregion

    #endregion

    #region Metodos publicos

    /// <summary>
    /// Determina si el character esta inactivo y podra realizar acciones
    /// </summary>
    /// <returns></returns>
    public bool IsInactive()
    {
        return _state == State.Standing || _state == State.Hanging || _state == State.Gripping;
    }

    /// <summary>
    /// Establece el input que determinara el accionar del character
    /// </summary>
    /// <param name="input">Input</param>
    public void SetInput(CharacterInput input)
    {
        this._input = input;
    }

    #endregion

    enum State
    {
        Standing,
        Gripping,
        Hanging,
        Moving,
        Falling
    }
}
