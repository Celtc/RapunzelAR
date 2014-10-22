using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    #region Variables (privadas)

    [SerializeField]
    protected float _life = 100f;
    [SerializeField]
    protected float _energy = 100f;

    [SerializeField]
    protected float _runSpeed = 3f;
    [SerializeField]
    protected float _generalSpeed = 1f;
    [SerializeField]
    protected float _fallingSpeed = 3f;
    
    [SerializeField]
    protected List<ActionData> _actionsData;
    protected Dictionary<string, ActionData> _actionsHash;

    protected ActionState _actionState;
    protected CharacterInput _input;
    protected CharacterMecanimController _mecanim;

    public delegate void GripEventDelegate();
    protected List<GripEventDelegate> _pushDelegates = new List<GripEventDelegate>();
    protected List<GripEventDelegate> _pullDelegates = new List<GripEventDelegate>();

    #endregion

    #region Properties (public)

    public float Life
    {
        get { return _life; }
        set { _life = value; }
    }

    public float Energy
    {
        get { return _energy; }
        set { _energy = value; }
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

    public float FallingSpeed
    {
        get { return _fallingSpeed; }
        set { _fallingSpeed = value; }
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
        {
            if (!GravityCheck())
                StartAction();
        }

        // Esta cayendo
        else if (IsFalling())
        {
            FallingManagement();
        }
    }
    
    #endregion

    #region Metodos privados

    /// <summary>
    /// Genera la hashtable (diccionario) de las acciones
    /// </summary>
    protected void GenerateHashes()
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
    /// Determina si el character debe caer o no, y si corresponde comienza la caida
    /// </summary>
    /// <returns>Verdadero en caso de comenzar a caer</returns>
    protected bool GravityCheck()
    {
        var falling = false;

        // Si en estados donde se apoya sobre sus pies
        if (_actionState == ActionState.Standing || _actionState == ActionState.Gripping)
        {
            if (!Level.Grid.ExistsAt(Position - Vector3.up))
            {
                Debug.Log("Accion: Cayendo");
                falling = true;
                Fall();
            }
        }

        // Si esta sostenido de un bloque
        else if (_actionState == ActionState.Hanging)
        {
            if (!Level.Grid.ExistsAt(Position + Direction))
            {
                Debug.Log("Accion: Cayendo");
                falling = true;
                HangToFall();
            }
        }

        return falling;
    }

    /// <summary>
    /// Determina la acción a realizar. 
    /// Segun el input y las posibilidades a partir de los cubos que lo rodean.
    /// </summary>
    protected void StartAction()
    {
        // Esta colgado
        if (_actionState == ActionState.Hanging)
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
        else if (_actionState == ActionState.Gripping)
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
        else if (_actionState == ActionState.Standing)
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
                if (_actionState == ActionState.Standing)
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

    /// <summary>
    /// Determina la acciones automaticas (no dependen del input) que ocurren en plena caida
    /// </summary>
    protected void FallingManagement()
    {
        // Determina la posicion objetivo
        var underPos = Position - Vector3.up;
        
        // Si no hay bloques debajo
        if (!Level.Grid.ExistsStillAt(underPos))
        {
            // Hay un hueco enfrente donde se puede agarrar
            if (!Level.Grid.ExistsStillAt(Position + Direction) &&
                Level.Grid.ExistsStillAt(underPos + Direction))
            {
                Debug.Log("Cayendo -> Agarrandose");
                FallToHang();
            }

            // No hay bloques de donde agarrarse
            else
            {
                Fall();
            }
        }

        // Hay un bloque debajo
        else
        {
            Debug.Log("Cayendo -> Aterrizando");
            Land();
        }
    }

    /// <summary>
    /// Administra las acciones de agarrar un bloque
    /// </summary>
    protected void GrippingManagement()
    {
        // Determina la posicion objetivo
        var frontPos = Position + Direction;

        // Obtiene el cubo objetivo
        var grippedBlock = Level.Grid.ExistsStillAt(frontPos) ? Level.Grid[frontPos] : null;

        // Si no estaba agarrando un bloque (estaba parado)
        if (_actionState != ActionState.Gripping)
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

                // Tira del bloque, si no hay bloques atras
                else if (_input.Direction == Direction * -1f &&
                    !Level.Grid.ExistsAt(Position - Direction))
                {
                    // Hay un bloque atras abajo donde pararse
                    if (Level.Grid.ExistsStillAt(Position - Direction - Vector3.up))
                    {
                        // Tira
                        Debug.Log("Accion: Tirando");
                        Pull(grippedBlock);
                    }

                    // No hay bloque donde quedarse parado =>  Tira pero cae
                    else
                    {
                        // Tira
                        Debug.Log("Accion: Tirando hacia borde");
                        PullToEdge(grippedBlock);
                    }
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

    /// <summary>
    /// Administra las acciones estando colgado
    /// </summary>
    protected void HangingManagement()
    {
        // Boton de agarre
        if (_input.BtnA)
        {
            // Hay bloque debajo
            if (Level.Grid.ExistsStillAt(Position - Vector3.up))
            {
                Debug.Log("Accion: Descolgando");
                HangDrop();
            }
            // No hay bloque debajo
            else
            {
                Debug.Log("Accion: Colgando -> Caer");
                HangToFall();
            }
        }
        // Desplazamiento
        else if (_input.Direction != null)
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
            var targetPos = Position + CustomMathf.RoundToIntVector(newInput);

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
                    else if (_input.Direction.x < 0 && 
                        !Level.Grid.ExistsStillAt(targetPos + Direction + Vector3.up))
                    {
                        Debug.Log("Accion: Colgando -> Giro Izquierda Convexo");
                        HangLeftConvexe();
                    }
                    else
                    {
                        Hang();
                    }
                }
            }

            // Desplazamiento vertical
            else if (newInput.y != 0)
            {
                // Input: arriba, y no hay bloque que lo impida -> sube
                if (newInput.y == 1 && !Level.Grid.ExistsStillAt(targetPos + Direction))
                {
                    Debug.Log("Accion: Colgando -> Subir");
                    HangUp();
                }
                // Input: abajo, y hay un bloque debajo -> descuelga
                else if (newInput.y == -1 && Level.Grid.ExistsStillAt(targetPos))
                {
                    Debug.Log("Accion: Descolgando");
                    HangDrop();
                }
                // Queda colgado
                else
                {
                    Hang();
                }
            }
        }
    }

    /// <summary>
    /// Administra las acciones de desplazamiento, horizontal y/o vertical
    /// </summary>
    protected void LocomotionManagement()
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

            // Sino puede trepar
            else
            {
                // Se queda parado
                Stand();
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
    protected IEnumerator DoFixedAction(ActionData action, ActionState newState, ActionState endState, float speedFactor, Action afterAction = null)
    {
        // Cambio al nuevo estado y establece el root motion
        _actionState = newState;
        _mecanim.ApplyRootMotion(action.RootMotion);

        // Almacena valores iniciales
        var startPosition = transform.position;
        var startRotation = transform.rotation;
        var actionDuration = action.duration / speedFactor;

        // Calcula cantidad de frames a emplear
        var totalFrames = (int)(actionDuration / Time.fixedDeltaTime);
        var framesCounter = 0;

        // Flags de presencia de curvas
        var hasDeltaX = action.deltaX.keys.Length > 0;
        var hasDeltaY = action.deltaY.keys.Length > 0;
        var hasDeltaZ = action.deltaZ.keys.Length > 0;
        var hasDeltaR = action.deltaR.keys.Length > 0;

        // A lo largo de la duracion de la accion aplica las tranformaciones
        var deltaRotation = 0f;
        var deltaPosition = Vector3.zero;
        while (framesCounter < totalFrames)
        {
            // Porcentaje [0,1] de progreso de la accion
            var fraction = Mathf.Clamp01((float)framesCounter++ / (totalFrames - 1));

            #region Posicion

            // Obtiene el nuevo delta
            var currDeltaPosition = new Vector3(
                hasDeltaX ? action.deltaX.Evaluate(fraction) : 0f,
                hasDeltaY ? action.deltaY.Evaluate(fraction) : 0f,
                hasDeltaZ ? action.deltaZ.Evaluate(fraction) : 0f
            );

            //Debug.Log("Fraction: " + fraction.ToString() + "\n DeltaZ: " + (currDeltaPosition - deltaPosition).z.ToString());

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

            // Espera al siguiente frame
            yield return new WaitForFixedUpdate();
        }

        // Ejecuta la accion si la hay
        if (afterAction != null)
            afterAction();

        // Cambia al estado en que queda luego de la accion y desactiva el rootMotion
        _actionState = endState;
        _mecanim.ApplyRootMotion(false);
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
    protected IEnumerator DoAction(ActionData action, ActionState newState, ActionState endState, float speedFactor, Action afterAction = null)
    {
        // Cambio al nuevo estado y establece el root motion
        _actionState = newState;
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
            //Debug.Log("Fraction: " + fraction.ToString() + "\n DeltaZ: " + (currDeltaPosition - deltaPosition).x.ToString());

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
        _actionState = endState;
        _mecanim.ApplyRootMotion(false);
    }

    #region Acciones

    protected void Stand()
    {
        // Nuevo estado
        _actionState = ActionState.Standing;

        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);
    }

    protected void Hang()
    {
        // Nuevo estado
        _actionState = ActionState.Hanging;

        // Confugiracion de mecanim
        _mecanim.SetFlag("Hanging");
        _mecanim.SetSpeed(_generalSpeed);
    }

    protected void Grip()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Gripping");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(new ActionData("Gripping", .15f), ActionState.Moving, ActionState.Gripping, _generalSpeed));
    }

    protected void ReleaseGrip()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(new ActionData("ReleaseGripping", .15f), ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    protected void Turn(Vector3 forward)
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
        StartCoroutine(DoAction(actionData, ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    protected void Run()
    {
        // Confugiracion de mecanim
        _mecanim.SetSpeed(1f);
        _mecanim.SetFlag("Run");
        _mecanim.SetFloat("RunSpeed", _runSpeed / transform.localScale.x);

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(DoAction(_actionsHash["Run"], ActionState.Moving, ActionState.Standing, _runSpeed, delegate { _mecanim.SetSpeed(_generalSpeed); }));
    }

    protected void Climb()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Climb");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["Climb"], ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    protected void JumpDown()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("JumpDown");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["JumpDown"], ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    protected void Push(Block block)
    {
        // Dispara los delegados
        foreach (var del in _pushDelegates)
            del();

        // Comunica al bloque la instruccion de "Push" y le devuelve el tiempo que demora la accion
        var duration = block.Pushed(Position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;

        // Inicia la corutina
        StartCoroutine(_Push(1 / duration));
    }

    protected IEnumerator _Push(float actionSpeed)
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Push");
        _mecanim.SetSpeed(actionSpeed);

        // Realiza la accion
        yield return StartCoroutine(DoAction(_actionsHash["Push"], ActionState.Moving, ActionState.Moving, actionSpeed));

        // Confugiracion de mecanim
        _mecanim.SetFlag("Standing");
        _mecanim.SetSpeed(_generalSpeed);

        // Espera a que termine la transicion antes de setear el estado final
        yield return new WaitForSeconds(0.2f);
        _actionState = ActionState.Standing;
    }

    protected void Pull(Block block)
    {
        // Dispara los delegados
        foreach (var del in _pullDelegates)
            del();

        // Comunica al bloque la instruccion de "Pull" y le devuelve el tiempo que demora la accion
        var duration = block.Pulled(Position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;

        // Confugiracion de mecanim
        var actionSpeed = 1 / duration;
        _mecanim.SetFlag("Pull");
        _mecanim.SetSpeed(actionSpeed);

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(DoAction(_actionsHash["Pull"], ActionState.Moving, ActionState.Gripping, actionSpeed, delegate { _mecanim.SetSpeed(_generalSpeed); }));
    }

    protected void PullToEdge(Block block)
    {
        // Dispara los delegados
        foreach (var del in _pullDelegates)
            del();

        // Comunica al bloque la instruccion de "Pull" y le devuelve el tiempo que demora la accion
        var duration = block.Pulled(Position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;

        // Inicia la corutina
        StartCoroutine(_PullToEdge(1 / duration));
    }

    protected IEnumerator _PullToEdge(float actionSpeed)
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("PullToEdge");
        _mecanim.SetSpeed(_generalSpeed);

        // Realiza la primera parte de la accion
        yield return StartCoroutine(DoAction(_actionsHash["PullToEdge"], ActionState.Moving, ActionState.Moving, actionSpeed));

        // Confugiracion de mecanim
        _mecanim.SetFlag("EdgeToHang");
        _mecanim.SetSpeed(_generalSpeed);

        // Realiza la segunda parte de la accion
        yield return StartCoroutine(DoAction(_actionsHash["EdgeToHang"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangUp()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangUp");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangUp"], ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    protected void HangDown()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangDown");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangDown"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangLeft()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangLeft");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangLeft"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangRight()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangRight");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangRight"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangLeftConcave()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangLeftConcave");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangLeftConcave"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangRightConcave()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangRightConcave");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangRightConcave"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangLeftConvexe()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangLeftConvexe");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangLeftConvexe"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangRightConvexe()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangRightConvexe");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangRightConvexe"], ActionState.Moving, ActionState.Hanging, _generalSpeed));
    }

    protected void HangDrop()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("HangDrop");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangDrop"], ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    protected void HangToFall()
    {
        // Confugiracion de mecanim
        _mecanim.SetTrigger("Fall");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["HangToFall"], ActionState.Moving, ActionState.Falling, _fallingSpeed));
    }

    protected void Fall()
    {
        // Confugiracion de mecanim
        _mecanim.SetTrigger("Fall", "Fall");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["Fall"], ActionState.Moving, ActionState.Falling, _fallingSpeed));
    }

    protected void FallToHang()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Hanging");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["FallToHang"], ActionState.Moving, ActionState.Hanging, _fallingSpeed));
    }

    protected void Land()
    {
        // Confugiracion de mecanim
        _mecanim.SetFlag("Land");
        _mecanim.SetSpeed(_generalSpeed);

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_actionsHash["Land"], ActionState.Moving, ActionState.Standing, _generalSpeed));
    }

    #endregion

    #endregion

    #region Metodos publicos

    public void RegisterPushDel(GripEventDelegate del)
    {
        _pushDelegates.Add(del);
    }

    public void RegisterPullDel(GripEventDelegate del)
    {
        _pullDelegates.Add(del);
    }

    /// <summary>
    /// Determina si el character esta inactivo y podra realizar acciones
    /// </summary>
    /// <returns></returns>
    public bool IsInactive()
    {
        return _actionState == ActionState.Standing || _actionState == ActionState.Hanging || _actionState == ActionState.Gripping;
    }

    /// <summary>
    /// Determina si el character esta en accion de caida
    /// </summary>
    /// <returns></returns>
    public bool IsFalling()
    {
        return _actionState == ActionState.Falling;
    }

    /// <summary>
    /// Destruye el character e instancia el efecto sangriento
    /// </summary>
    public void Smash()
    {
        Debug.Log("Character: \"" + this.name + "\" siendo aplastado");
        Instantiate(Resources.Load<GameObject>("Prefabs/Particles/BloodSplatter"), transform.position, Quaternion.identity);
        Destroy(this.gameObject, .1f);
    }

    /// <summary>
    /// Asigna una nueva posicion al character
    /// </summary>
    /// <param name="pos">Posicion deseada</param>
    public void SetPosition(IntVector3 pos)
    {
        transform.position = new Vector3(pos.x + .5f, pos.y, pos.z + .5f);
    }

    /// <summary>
    /// Establece el input que determinara el accionar del character
    /// </summary>
    /// <param name="input">Input</param>
    public void SetInput(CharacterInput input)
    {
        //input.Direction = Vector3.right;
        this._input = input;
    }

    /// <summary>
    /// Asigna un nuevo estado al character
    /// </summary>
    /// <param name="state">Estado deseado</param>
    public void SetState(CharacterState state)
    {
        this._actionState = state.ActionState;
        this._energy = state.Energy;
        this._fallingSpeed = state.FallingSpeed;
        this._generalSpeed = state.GeneralSpeed;
        this._life = state.Life;
        this.transform.position = state.Position;
        this.transform.rotation = state.Rotation;
        this._runSpeed = state.RunSpeed;
    }

    /// <summary>
    /// Obtiene el estado interno del character
    /// </summary>
    /// <returns>Estado actual</returns>
    public CharacterState GetState()
    {
        var state = new CharacterState(this.GetType());
        state.ActionState = this._actionState;
        state.Energy = this._energy;
        state.FallingSpeed = this._fallingSpeed;
        state.GeneralSpeed = this._generalSpeed;
        state.Life = this._life;
        state.Position = this.transform.position;
        state.Rotation = this.transform.rotation;
        state.RunSpeed = this._runSpeed;

        return state;
    }
    
    #endregion
    
    public enum ActionState
    {
        Standing,
        Gripping,
        Hanging,
        Moving,
        Falling
    }
}
