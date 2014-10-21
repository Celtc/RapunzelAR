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
    private List<Character> _characters = new List<Character>();

	#endregion

	#region Propiedades (public)

    public static Index instance
    {
        get
        {
            return _instance;
        }
    }

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

    public static List<Character> Characters
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

    public static int IndexOfBlock<T>()
    {
        for (int i = 0; i < _instance._blocks.Count; i++)
        {
            if (_instance._blocks[i].GetType() == typeof(T))
            {
                return i;
            }
        }

        throw new MissingMemberException();
    }

    public static int IndexOfBlock(Type blockType)
    {
        for (int i = 0; i < _instance._blocks.Count; i++)
        {
            if (_instance._blocks[i].GetType() == blockType)
            {
                return i;
            }
        }

        throw new MissingMemberException();
    }

    public static int IndexOfCharacter<T>()
    {
        for (int i = 0; i < _instance._characters.Count; i++)
        {
            if (_instance._characters[i].GetType() == typeof(T))
            {
                return i;
            }
        }

        throw new MissingMemberException();
    }

    public static int IndexOfCharacter(Type characterType)
    {
        for (int i = 0; i < _instance._characters.Count; i++)
        {
            if (_instance._characters[i].GetType() == characterType)
            {
                return i;
            }
        }

        throw new MissingMemberException();
    }
    
	#endregion
}
