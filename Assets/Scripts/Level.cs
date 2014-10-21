using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	#region Variables (private)

    private static GridOriginator _gridOriginator;
    private static GridCaretaker _gridCaretaker;
    private static TextAsset _currentLevel;
    
	#endregion

	#region Propiedades (public)

    public static Grid Grid
    {
        get
        {
            return _gridOriginator.Grid;
        }
    }

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

	}

	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{
	
	}

    /// <summary>
    /// Llamado al cagar una escena.
    /// </summary>
    private void OnLevelWasLoaded(int level)
    {
        if (level == 1 && _currentLevel != null)
            Level.LoadLevel(new LevelParser(_currentLevel));
    }

	#endregion

    #region Metodos privados
    
    #endregion

    #region Metodos publicos
            
    /// <summary>
    /// Establece como el nivel candidato a cargar el textAsset deseado
    /// </summary>
    /// <param name="levelName">Nombre del nivel (textAsset) a cargar</param>
    public static void SetLevel(string levelName)
    {
        var level = Index.Levels.Find(x => x.name == levelName);
        if (level)
        {
            Debug.Log("Cargando el nivel \"" + levelName + "\"");
            _currentLevel = level;
        }
        else
        {
            Debug.Log("No se encontro el nivel \"" + levelName + "\"");
        }
    }
    
    /// <summary>
    /// Carga un nivel, instanciando un memento del mismo
    /// </summary>
    /// <param name="gridMemento"></param>
    public static void LoadLevel(GridMemento gridMemento)
    {
        // Instancia las clases de memento si no estaban instanciadas
        if (_gridOriginator == null) _gridOriginator = new GridOriginator();
        if (_gridCaretaker == null) _gridCaretaker = new GridCaretaker(10);

        // Spawnea los bloques
        foreach (var blockState in gridMemento.BlocksState)
        {
            Level.SpawnBlock(blockState);
        }

        // Spawnea los characters
        foreach (var charState in gridMemento.CharactersState)
        {
            Level.SpawnCharacter(charState, true);
        }
    }

    /// <summary>
    /// Descarga el nivel, destruyendo todos los bloques y characters y limpiando la grilla
    /// </summary>
    public static void Unload()
    {
        // Destruye todos los bloques
        foreach(var block in _gridOriginator.Grid.AllBlocks)
        {
            Destroy(block.gameObject);
        }

        // Destruye todos los characters
        foreach (var character in _gridOriginator.Grid.AllCharacters)
        {
            Destroy(character.gameObject);
        }

        // Destruye la grilla
        _gridOriginator.Clear();
    }
    
    /// <summary>
    /// Spawnea un bloque en el mundo
    /// </summary>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="blockID">ID del tipo de bloque a spawnear (Index)</param>
    /// <param name="basement">Flag que determina si el cubo es inamovible</param>
    public static void SpawnBlockAt(int blockID, IntVector3 pos, bool basement = false)
    {
        SpawnBlockAt(Index.Blocks[blockID], pos, basement);
    }

    /// <summary>
    /// Spawnea un bloque en el mundo
    /// </summary>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="block">Bloque a spawnear, se creara una instancia del mismo</param>
    /// <param name="basement">Flag que determina si el cubo es inamovible</param>
    public static void SpawnBlockAt(Block block, IntVector3 pos, bool basement = false)
    {
        // Instancia el bloque
        var instancedBlock = ((GameObject)Instantiate(block.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Block>();

        // Lo ubica y asigna flag de basement
        instancedBlock.SetPosition(pos);
        instancedBlock.isBasement = basement;

        // Lo agrega al grid
        _gridOriginator.Grid.AddBlock(instancedBlock);
    }    

    /// <summary>
    /// Spawnea un bloque con un determinado estado
    /// </summary>
    /// <param name="blockState">Estado del bloque</param>
    public static void SpawnBlock(BlockState blockState)
    {
        // Instancia el bloque
        var instancedBlock = ((GameObject)Instantiate(Index.Blocks[blockState.IndexType].gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Block>();

        // Asigna el estado
        instancedBlock.SetState(blockState);

        // Lo agrea al grid
        _gridOriginator.Grid.AddBlock(instancedBlock);
    }

    /// <summary>
    /// Spawnea un character en el mundo. Si es un jugador lo asigna a las camaras.
    /// </summary>
    /// <param name="character">Character a spawner, se creara una instancia del mismo</param>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="player">Jugador que sera seguido por las camaras</param>
    public static void SpawnCharacterAt(int characterID, IntVector3 pos, bool player = false)
    {
        SpawnCharacterAt(Index.Characters[characterID], pos, player);
    }

    /// <summary>
    /// Spawnea un character en el mundo. Si es un jugador lo asigna a las camaras.
    /// </summary>
    /// <param name="character">Character a spawner, se creara una instancia del mismo</param>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="player">Jugador que sera seguido por las camaras</param>
    public static void SpawnCharacterAt(Character character, IntVector3 pos, bool player = false)
    {
        // Instancia el character
        var instancedCharacter = ((GameObject)Instantiate(character.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Character>();

        // Lo ubica
        instancedCharacter.SetPosition(pos);

        // Si es un character jugable lo targetea con la camara
        if (instancedCharacter.tag == "Player")
        {
            Camera.main.GetComponent<SmoothFollowAdvance>().SetTarget(instancedCharacter.transform);
        }
        
        // Lo agrega al grid
        _gridOriginator.Grid.AddCharacter(instancedCharacter.GetComponent<Character>());
    }

    /// <summary>
    /// Spawnea un character con un determinado estado
    /// </summary>
    /// <param name="characterState">Estado del character</param>
    /// <param name="player">Jugador que sera seguido por las camaras</param>
    public static void SpawnCharacter(CharacterState characterState, bool player = false)
    {

        // Instancia el character
        var instancedCharacter = ((GameObject)Instantiate(Index.Characters[characterState.IndexType].gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Character>();

        // Asigna el estado
        instancedCharacter.SetState(characterState);

        // Si es un character jugable lo targetea con la camara
        if (instancedCharacter.tag == "Player")
        {
            Camera.main.GetComponent<SmoothFollowAdvance>().SetTarget(instancedCharacter.transform);
        }

        // Lo agrega al grid
        _gridOriginator.Grid.AddCharacter(instancedCharacter.GetComponent<Character>());
    }

    /// <summary>
    /// Guarda el estado de la grilla
    /// </summary>
    public static void SaveState()
    {
        _gridCaretaker.Push(_gridOriginator.CreateMemento());
    }

    /// <summary>
    /// Carga un estado previo
    /// </summary>
    public static void LoadPreviousState()
    {
        //TODO: Esto deberia hacerse con NGUI
        var gridMemento = _gridCaretaker.Pop();
        if (gridMemento == null) return;

        var sFader = Camera.main.GetComponent<ScreenFader>();
        sFader.Action = () => _gridOriginator.SetMemento(gridMemento);
        sFader.FadeInOut();
    }

	#endregion
}