using UnityEngine;
using System.Collections;

public class PreserveOnSceneLoad : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private bool _preserve;
    
	#endregion

	#region Propiedades (public)
    
    public bool Indestructible
    {
        get { return _preserve; }
        set { _preserve = value; }
    }

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Llamado al cagar una escena.
    /// </summary>
    private void OnLevelWasLoaded()
    {
        if (!_preserve)
            Destroy(gameObject);
    }

	#endregion
}
