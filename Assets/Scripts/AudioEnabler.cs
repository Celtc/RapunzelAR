using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioEnabler : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private float _delayStart = 1f;
    [SerializeField]
    private bool _loop = false;
    [SerializeField]
    private bool _destroyAfterEnable = true;

	#endregion

	#region Propiedades (public)

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {

    }

	/// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
	/// </summary>
	void Start ()	
	{
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        _audioSource.PlayDelayed(_delayStart);
        _audioSource.loop = _loop;

        if(_destroyAfterEnable)
            Destroy(this);
	}

	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{

	}

	#endregion

	#region Metodos

	#endregion
}
