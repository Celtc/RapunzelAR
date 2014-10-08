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

    private State _state;
    private Animator _animator;
    private CharacterInput _input;

    #region Flags de animaciones

    private bool standing = false;
    private bool gripStanding = false;
    private bool run = false;
    private bool climb = false;
    private bool jumpDown = false;
    private bool push = false;
    private bool pull = false;
    private bool hangUp = false;
    private bool hangDown = false;
    private bool hangLeft = false;
    private bool hangRight = false;
    private bool hangAcuteLeft = false;
    private bool hangAcuteRight = false;
    private bool hangObtuseLeft = false;
    private bool hangObtuseRight = false;

    #endregion

    #region Data de Acciones

    [SerializeField]
    private ActionData _turnLeft;
    [SerializeField]
    private ActionData _turnRight;
    [SerializeField]
    private ActionData _turnBack;
    [SerializeField]
    private ActionData _run;
    [SerializeField]
    private ActionData _climb;
    [SerializeField]
    private ActionData _jumpDown;
    [SerializeField]
    private ActionData _push;
    [SerializeField]
    private ActionData _pull;
    [SerializeField]
    private ActionData _hangUp;
    [SerializeField]
    private ActionData _hangDown;
    [SerializeField]
    private ActionData _hangLeft;
    [SerializeField]
    private ActionData _hangRight;
    [SerializeField]
    private ActionData _hangAcuteLeft;
    [SerializeField]
    private ActionData _hangAcuteRight;
    [SerializeField]
    private ActionData _hangObtuseLeft;
    [SerializeField]
    private ActionData _hangObtuseRight;

    #endregion

    #endregion

    #region Properties (public)

    public float generalSpeed
    {
        get { return _generalSpeed; }
        set { _generalSpeed = value; }
    }

    public float runSpeed
    {
        get { return _runSpeed; }
        set { _runSpeed = value; }
    }

    public IntVector3 position
    {
        get { return transform.position; }
        set { transform.position = new Vector3(value.x + .5f, value.y + .5f, value.z + .5f); }
    }

    public Vector3 exactPosition
    {
        get { return new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z - .5f); }
        private set { transform.position = new Vector3(value.x + .5f, value.y, value.z + .5f); }
    }
    
    #endregion

    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        _animator = GetComponent<Animator>();
        _input = new CharacterInput(null, false, false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void LateUpdate()
    {
        // En caso de que este inactivo, permite el comienzo de una nueva accion
        if (IsInactive())
            StartAction();

        // Transmite a mecanim los flags de animacion
        SetMecanimVars();
    }

    #endregion

    #region Metodos privados

    /// <summary>
    /// Reinicia los flags de animacion
    /// </summary>
    private void ResetFlags()
    {        
        standing = false;
        gripStanding = false;
        run = false;
        climb = false;
        jumpDown = false;
        push = false;
        pull = false;
        hangUp = false;
        hangDown = false;
        hangLeft = false;
        hangRight = false;
        hangAcuteLeft = false;
        hangAcuteRight = false;
        hangObtuseLeft = false;
        hangObtuseRight = false;
    }

    /// <summary>
    /// Transmite al animator los flags de animacion para moverse entre estados
    /// </summary>
    private void SetMecanimVars()
    {
        _animator.SetBool("Standing", standing);
        _animator.SetBool("GripStanding", gripStanding);
        _animator.SetBool("MoveFwd", run);
        _animator.SetBool("Climb", climb);
        _animator.SetBool("JumpDown", jumpDown);
        _animator.SetBool("Push", push);
        _animator.SetBool("Pull", pull);
        _animator.SetBool("HangUp", hangUp);
        _animator.SetBool("HangDown", hangDown);
        _animator.SetBool("HangLeft", hangLeft);
        _animator.SetBool("HangRight", hangRight);
        _animator.SetBool("HangAcuteLeft", hangAcuteLeft);
        _animator.SetBool("HangAcuteRight", hangAcuteRight);
        _animator.SetBool("HangObtuseLeft", hangObtuseLeft);
        _animator.SetBool("HangObtuseRight", hangObtuseRight);
    }

    /// <summary>
    /// Determina la acción a realizar. 
    /// Segun el input y las posibilidades a partir de los cubos que lo rodean.
    /// </summary>
    private void StartAction()
    {
        // Seteo inicial de variables
        ResetFlags();

        // Esta colgado?
        if (_state == State.Hanging)
        {
            HangingManagement();
        }
        else
        {
            // Si no hay input
            if (!_input.HasInput())
            {
                Stand();
            }
            // Hay input del boton de agarre
            else if (_input.BtnA)
            {
                GripManagement();
            }
            // Hay input de direccion unicamente
            else
            {
                LocomotionManagement();
            }
        }

        // Borra el input almacenado
        _input.Empty();
    }
    
    private void GripManagement()
    {
        // Determina la posicion objetivo
        var frontPos = position + transform.forward;

        // Obtiene el cubo objetivo
        var grippedBlock = Level.Grid[frontPos];
        if (grippedBlock != null && !grippedBlock.isMoving)
        {
            // Esta agarrando un bloque?
            if (_state != State.GripStanding)
            {
                Debug.Log("Accion: Agarrando Bloque");
                Grip();
            }
            else if (_input.Direction != null)
            {
                // Empuja hacia el bloque
                if (_input.Direction == transform.forward)
                {
                    // Empuja
                    Debug.Log("Accion: Empujando");
                    Push(grippedBlock);
                }

                // Tira del bloque
                else if (_input.Direction == transform.forward * -1f)
                {
                    // Tira
                    Debug.Log("Accion: Tirando");
                    Pull(grippedBlock);
                }                
            }
            // No hace nada
            else
            {
                // Vuelve a la animacion de gripStand
                gripStanding = true;
            }
        }
        else
        {
            // Si no hay bloque que agarrar se queda parado
            Stand();
        }
    }
    
    private void HangingManagement()
    {
        // Si no hay input
        if (!_input.HasInput())
        {
            Hang();
        }
        // Hay input del boton de agarre
        else if (_input.BtnA)
        {
        }
        // Hay input de direccion unicamente
        else
        {
            // Transforma el input (da prioridad al input vertical)           
            var newInput = new Vector3(0, _input.Direction.z > 0 ? 1 : 0, 0);
            if (newInput.y == 0)
                newInput += transform.rotation * new Vector3(_input.Direction.x, 0, 0);

            // Determina la posicion objetivo
            var targetPos = position + newInput;

            // Desplazamiento horizontal
            if (newInput.y == 0)
            {
                // Hay un cubo en el lugar destino? -> Dobla (Acute)
                if (Level.Grid.ExistsStillAt(targetPos))
                {
                    // Direccion de strafe
                    if (_input.Direction.x > 0)
                    {
                        Debug.Log("Accion: Colgando -> Dobla Agudo Derecha");
                        HangAcuteRight();
                    }
                    else
                    {
                        Debug.Log("Accion: Colgando -> Dobla Agudo Izquierda");
                        HangAcuteLeft();
                    }
                }

                // Hay un cubo delante del lugar destino?
                else if (Level.Grid.ExistsStillAt(targetPos + transform.forward))
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
                        !Level.Grid.ExistsStillAt(targetPos + transform.forward + Vector3.up))
                    {
                        Debug.Log("Accion: Colgando -> Dobla Grave Derecha");
                        HangObtuseRight();
                    }
                    else if (!Level.Grid.ExistsStillAt(targetPos + transform.forward + Vector3.up))
                    {
                        Debug.Log("Accion: Colgando -> Dobla Grave Izquierda");
                        HangObtuseLeft();
                    }

                }
            }

            // Desplazamiento vertical
            else if (newInput.y > 0)
            {
                if (!Level.Grid.ExistsStillAt(targetPos + transform.forward))
                {
                    Debug.Log("Accion: Colgando -> Subir");
                    HangUp();
                }
            }
        }
    }

    private void LocomotionManagement()
    {
        // Si no esta orientado, gira
        if (transform.forward != _input.Direction)
        {
            Debug.Log("Accion: Girando");
            Turn(_input.Direction);
            return;
        }

        // Determina la posicion objetivo
        var targetPos = position + transform.forward;

        // Hay un cubo en el lugar destino?
        if (Level.Grid.ExistsAt(targetPos))
        {
            // El cubo a trepar no esta quieto o hay un cubos que impidan trepar? (arriba nuestro o arriba del cubo a trepar)
            if (Level.Grid.ExistsStillAt(targetPos) &&
                !Level.Grid.ExistsStillAt(targetPos + Vector3.up) &&
                !Level.Grid.ExistsStillAt(position + Vector3.up))
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
        // Cambio al nuevo estado
        _state = newState;

        // Almacena valores iniciales
        var elapsedTime = -Time.deltaTime;
        var startPosition = exactPosition;
        var startRotation = transform.rotation;
        var actionDuration = action.baseDuration / speedFactor;

        // Flags de presencia de curvas
        var hasDeltaX = action.deltaX.keys.Length > 0;
        var hasDeltaY = action.deltaY.keys.Length > 0;
        var hasDeltaZ = action.deltaZ.keys.Length > 0;
        var hasDeltaR = action.deltaR.keys.Length > 0;
        
        // A lo largo de la duracion de la accion aplica las tranformaciones
        while (elapsedTime < actionDuration)
        {
            // Incrementa el tiempo
            elapsedTime += Time.deltaTime;

            // Porcentaje [0,1] de progreso de la accion
            var fraction = Mathf.Clamp01(elapsedTime / actionDuration);
            Debug.Log(fraction);

            #region Posicion

            // Crea el vector delta
            var deltaPosition = new Vector3(
                hasDeltaX ? action.deltaX.Evaluate(fraction) : 0f,
                hasDeltaY ? action.deltaY.Evaluate(fraction) : 0f,
                hasDeltaZ ? action.deltaZ.Evaluate(fraction) : 0f
            );
            
            // Lo orienta
            deltaPosition = startRotation * deltaPosition;

            // Modifica la posicion
            exactPosition = startPosition + deltaPosition;

            #endregion

            #region Rotacion

            if (hasDeltaR)
            {
                var deltaRotation = action.deltaR.Evaluate(fraction);
                transform.rotation = startRotation * Quaternion.Euler(0f, deltaRotation, 0f);
            }

            #endregion
            
            // Espera al siguiente frame
            yield return 0;
        }

        // Ejecuta la accion si la hay
        if (afterAction != null)
            afterAction();

        // Cambia al estado en que queda luego de la accion
        _state = endState;
    }

    /// <summary>
    /// Determina si el character esta inactivo y podra realizar acciones
    /// </summary>
    /// <returns></returns>
    private bool IsInactive()
    {
        return _state == State.Standing || _state == State.Hanging || _state == State.GripStanding;
    }

    #region Acciones

    private void Stand()
    {
        standing = true;
        _state = State.Standing;
    }

    private void Hang()
    {
        _state = State.Hanging;
    }

    private void Turn(Vector3 forward)
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "Standing" (no hay animación para turning)
        standing = true;

        // Determina el tipo de giro (izq, derecha, 180)
        ActionData actionData;
        if (forward == transform.forward * -1)
            actionData = _turnBack;
        else
            actionData = (Vector3.Cross(forward, transform.forward).y > 0) ? _turnLeft : _turnRight;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(actionData, State.Moving, State.Standing, _generalSpeed));
    }

    private void Run()
    {
        // La velocidad deseada se divide por el scale, para compensar la diferencia de tamaño del modelo
        var blendFactor = _runSpeed / transform.localScale.x;

        // Como la velocidad varia utilizando diferentes animaciones la velocidad 
        // de Mecanim se mantiene en la original: 1, y cambia el float de blend de animaciones
        _animator.speed = 1f;
        _animator.SetFloat("RunSpeed", blendFactor);

        // Establece el flag de estado run
        run = true;      
        
        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(DoAction(_run, State.Moving, State.Standing, _runSpeed, delegate { _animator.speed = _generalSpeed; }));
    }

    private void Climb()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "Climb"
        climb = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_climb, State.Moving, State.Standing, _generalSpeed));
    }

    private void JumpDown()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        jumpDown = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_jumpDown, State.Moving, State.Standing, _generalSpeed));
    }

    private void Grip()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "GripStance"
        gripStanding = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(new ActionData(.25f), State.Moving, State.GripStanding, _generalSpeed));
    }

    private void Push(Block block)
    {
        // Comunica al bloque la instruccion de "Push" y le devuelve el tiempo que demora la accion
        var duration = block.Pushed(position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;
        
        // Establece la velocidad de mecanim
        var actionSpeed = 1 / duration;
        _animator.speed = actionSpeed;

        // Activa el flag de animacion "Push"
        push = true;

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(PushSubRoutine(actionSpeed));
    }

    private IEnumerator PushSubRoutine(float actionSpeed)
    {
        // Realiza la accion
        yield return StartCoroutine(DoAction(_push, State.Moving, State.Moving, actionSpeed, delegate { _animator.speed = _generalSpeed; }));

        // Establece el flag de animacion en standing
        standing = true;

        // Espera a que termine la transicion antes de setear el estado en standing
        yield return new WaitForSeconds(0.2f);

        // Establece el estado en standing para habilitar nuevas acciones
        _state = State.Standing;
    }

    private void Pull(Block block)
    {
        // Comunica al bloque la instruccion de "Pull" y le devuelve el tiempo que demora la accion
        var duration = block.Pulled(position);

        // Si es NaN el bloque es inamovible
        if (float.IsNaN(duration)) return;
        
        // Establece la velocidad de mecanim
        var actionSpeed = 1 / duration;
        _animator.speed = actionSpeed;

        // Activa el flag de animacion "Pull"
        pull = true;

        // Inicia la rutina de la accion. Se le indica que una vez finalizada la tarea vuelva a la velocidad general
        StartCoroutine(DoAction(_pull, State.Moving, State.GripStanding, actionSpeed, delegate { _animator.speed = _generalSpeed; }));
    }

    private void HangUp()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangUp = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangUp, State.Moving, State.Standing, _generalSpeed));
    }

    private void HangDown()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangDown = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangDown, State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangLeft()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangLeft = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangLeft, State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangRight()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangRight = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangRight, State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangAcuteLeft()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangAcuteLeft = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangAcuteLeft, State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangAcuteRight()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangAcuteRight = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangAcuteRight, State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangObtuseLeft()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangObtuseLeft = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangObtuseLeft, State.Moving, State.Hanging, _generalSpeed));
    }

    private void HangObtuseRight()
    {
        // Establece la velocidad de mecanim
        _animator.speed = _generalSpeed;

        // Activa el flag de animacion "JumpDown"
        hangObtuseRight = true;

        // Inicia la rutina de la accion
        StartCoroutine(DoAction(_hangObtuseRight, State.Moving, State.Hanging, _generalSpeed));
    }

    #endregion

    #endregion

    #region Metodos publicos

    public void SetInput(CharacterInput input)
    {
        this._input = input;
    }

    #endregion

    enum State
    {
        Standing,
        GripStanding,
        Hanging,
        Moving,
        Falling
    }
}
