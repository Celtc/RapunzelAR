using UnityEngine;
using System.Collections;

public class DeathTimer : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private float _lifeTime = 5f;
    [SerializeField]
    private bool _playOnAwake = true;

    private float _timeLeft;

	#endregion

	#region Propiedades (public)
    
    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }

    public bool PlayOnAwake
    {
        get { return _playOnAwake; }
        set { _playOnAwake = value; }
    }

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
    /// </summary>
    void Start()
    {
        this.enabled = false;
        if (_playOnAwake)
            StartTimer();
    }

	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0) Destroy(this.gameObject);
	}

	#endregion

	#region Metodos

    public void StartTimer()
    {
        _timeLeft = _lifeTime;
        this.enabled = true;
    }

    public void StopTimer()
    {
        this.enabled = false;
    }

	#endregion
}
