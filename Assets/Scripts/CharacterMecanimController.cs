using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class CharacterMecanimController : MonoBehaviour
{
	#region Variables (private)
    
    [SerializeField]
    private bool _sendingFlags = false;

    [SerializeField]
    private bool _avgSpeedOnRootedTransition = true;

    [SerializeField]
    private MecanimMode _mode = MecanimMode.SingleActive;

    private bool _dirtyFlags;
    private bool _applyRootMotion;
    private float _animationSpeed;

    private List<MecanimFlag> _flagsList;
    private Dictionary<string, MecanimFlag> _flagsHash;

    private Animator _animator;
    private Character _charLocomotion;

	#endregion

	#region Propiedades (public)

    public MecanimMode Mode
    {
        get { return _mode; }
        set { _mode = value; }
    }

    public bool SendingFlags
    {
        get { return _sendingFlags; }
        set { _sendingFlags = value; }
    }

	#endregion

	#region Funciones de evento de Unity

	/// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
	/// </summary>
	void Start ()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
            _animator.applyRootMotion = this._applyRootMotion;
        }

        if (_charLocomotion == null)
        {
            _charLocomotion = GetComponent<Character>();
        }

        _flagsList = new List<MecanimFlag>();
        _flagsHash = new Dictionary<string, MecanimFlag>();
	}

	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{
        _dirtyFlags = false;

        if (_sendingFlags)
            SendValues();

        if (_mode == MecanimMode.ResetingFlags)
            ResetFlags();
	}

    void LateUpdate()
    {
        if (_avgSpeedOnRootedTransition && _applyRootMotion)
            ApplyAvgSpeed();
    }

	#endregion

    #region Metodos
    
    /// <summary>
    /// Genera la hashtable (diccionario) de los flags
    /// </summary>
    private void GenerateHashes()
    {
        _flagsHash.Clear();

        for (int i = 0; i < _flagsList.Count; i++)
        {
            _flagsHash.Add(_flagsList[i].Name, _flagsList[i]);
        }
    }

    /// <summary>
    /// Resetea los flags a su valor por defecto (false)
    /// </summary>
    private void ResetFlags()
    {
        foreach(var flag in _flagsList)
        {
            flag.Value = false;
        }
    }
    
    /// <summary>
    /// Transmite los flags a mecanim
    /// </summary>
    private void SendValues()
    {
        foreach (var flag in _flagsList)
        {
            _animator.SetBool(flag.Name, flag.Value);   
        }
    }

    /// <summary>
    /// Aplica al objeto poseedor del componente mecanim un mov. promedio mientras dure la transicion.
    /// </summary>
    private void ApplyAvgSpeed()
    {
        if (!_animator.IsInTransition(0) && !_dirtyFlags)
        {
            _animator.applyRootMotion = true;
        }
        else
        {
            _animator.applyRootMotion = false;
            //var avgSpeedZ = _animator.GetFloat("AvgSpeedZ");            
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.8f * Time.deltaTime);
            //Debug.Log("Aplicando avgSpeedZ: " + (avgSpeedZ * Time.deltaTime).ToString());
        }        
    }
    
    /// <summary>
    /// Establece el valor de verdad del flag. Si no existe lo agrega.
    /// </summary>
    /// <param name="flagName">Nombre del flag</param>
    /// <param name="value">Valor a tomar</param>
    public void SetFlag(string flagName, bool value = true)
    {
        _dirtyFlags = true;

        if (_mode == MecanimMode.SingleActive && value == true)
            ResetFlags();

        if (_flagsHash.ContainsKey(flagName))
        {

            _flagsHash[flagName].Value = value;
        }
        else
        {
            _flagsList.Add(new MecanimFlag(flagName, value));
            _flagsHash.Add(flagName, _flagsList[_flagsList.Count - 1]);
        }
    }

    /// <summary>
    /// Activa un trigger
    /// </summary>
    /// <param name="triggerName">Nombre del trigger</param>
    public void SetTrigger(string triggerName)
    {
        _dirtyFlags = true;

        if (_mode == MecanimMode.SingleActive)
            ResetFlags();

        _animator.SetTrigger(triggerName);
    }

    /// <summary>
    /// Activa un trigger si no se encuentra ya en el estado deseado
    /// </summary>
    /// <param name="triggerName">Nombre del trigger</param>
    /// <param name="wantedState">Nombre del estado</param>
    public void SetTrigger(string triggerName, string wantedState)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(wantedState)) return;

        _dirtyFlags = true;

        if (_mode == MecanimMode.SingleActive)
            ResetFlags();

        _animator.SetTrigger(triggerName);
    }

    /// <summary>
    /// Establece el valor de una variable float
    /// </summary>
    /// <param name="name">Nombre de la variable</param>
    /// <param name="value">Valor</param>
    public void SetFloat(string name, float value)
    {
        _animator.SetFloat(name, value);
    }

    /// <summary>
    /// Establece la velocidad de reproduccion de las animaciones
    /// </summary>
    /// <param name="speed">Velocidad de reproduccion</param>
    public void SetSpeed(float speed)
    {
        if (_animationSpeed != speed)
        {
            _animationSpeed = speed;
            _animator.speed = speed;
        }
    }

    /// <summary>
    /// Activa o desactiva el root motion
    /// </summary>
    /// <param name="value">Valor de verdad</param>
    public void ApplyRootMotion(bool value)
    {
        if (_applyRootMotion != value)
        {
            _applyRootMotion = value;

            if (!value || (value && !_avgSpeedOnRootedTransition))
                _animator.applyRootMotion = value;
        }
    }
    
	#endregion

    public enum MecanimMode
    {
        ResetingFlags,
        SingleActive,
        Free
    }
}