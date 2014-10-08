using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Index : MonoBehaviour
{
	#region Variables (private)

    // Singleton pattern
    private static Index _instance;

    // Variables de instancia
    [SerializeField]
    private List<TextAsset> _levels = new List<TextAsset>();
    [SerializeField]
    private List<Block> _blocks = new List<Block>();
    [SerializeField]
    private List<GameObject> _characters = new List<GameObject>();

	#endregion

	#region Propiedades (public)

    public static Index instance
    {
        get
        {
            return _instance;
        }
    }

    //TODO: Revisar el uso de memoria, tal ves sea mejor utilizar un encapsulamiento al cual se acceda a los elementos especificando el indice
    public static List<TextAsset> Levels
    {
        get
        {
            return _instance._levels;
        }
    }

    public static List<Block> Blocks
    {
        get
        {
            return _instance._blocks;
        }
    }

    public static List<GameObject> Characters
    {
        get
        {
            return _instance._characters;
        }
    }

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {
        if (Index._instance == null)
            Index._instance = this;
    }

	#endregion

	#region Metodos

	#endregion
}
