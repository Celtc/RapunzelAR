using UnityEngine;
using System.Collections;

public class RapunzelAPP : MonoBehaviour
{
	#region Variables (private)

    // Singleton pattern
    private static RapunzelAPP _instance;

	#endregion

	#region Propiedades (public)

    public static RapunzelAPP instance
    {
        get
        {
            return _instance;
        }
    }

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {
        if (RapunzelAPP._instance == null)
            RapunzelAPP._instance = this;
    }

	#endregion

	#region Metodos

    /// <summary>
    /// Sale de la aplicacion
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Carga el nivel
    /// </summary>
    public void LoadLevel()
    {
        Level.Set("test");
        Application.LoadLevel(1);
    }

	#endregion
}
