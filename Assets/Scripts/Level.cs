using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    #region Variables (private)

    // Singleton pattern para poder acceder globalmente a la unica instancia del Level
    private static Level _instance;

    private GridOriginator _gridOriginator;
    private GridCaretaker _gridCaretaker;
    private TextAsset _currentLevel;
    private LevelInfo _currentLevelInfo;

    #endregion

    #region Propiedades (public)

    // Es 10% más optimo acceder hacer public _instance y acceder directamente pero bueno...
    public static Level Instance
    {
        get { return _instance; }
    }
    
    public Grid Grid
    {
        get
        {
            return _gridOriginator.Grid;
        }
    }

    public LevelInfo Info
    {
        get { return _currentLevelInfo; }
    }

    public int HashID
    {
        get { return _currentLevel.GetHashCode(); }
    }

    #endregion

    #region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {
        if (Level._instance == null)
            Level._instance = this;
    }

    /// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Llamado al cagar una escena.
    /// </summary>
    private void OnLevelWasLoaded(int level)
    {
        if (level == 1 && _currentLevel != null)
            Level.Instance.Load(new LevelXMLParser(_currentLevel));
    }

    #endregion

    #region Metodos privados

    #endregion

    #region Metodos publicos

    /// <summary>
    /// Establece el nivel candidato a cargar
    /// </summary>
    /// <param name="levelName">Nombre del nivel (textAsset) a cargar</param>
    public void Set(string levelName)
    {
        Set(Index.Levels.FindIndex(x => x.name == levelName));
    }

    /// <summary>
    /// Establece el nivel candidato a cargar
    /// </summary>
    /// <param name="levelName">Nombre del nivel (textAsset) a cargar</param>
    public void Set(int levelIndex)
    {
        var level = Index.Levels[levelIndex];
        if (level)
        {
            Debug.Log("Establecido el nivel \"" + level.name + "\"");
            _currentLevelInfo = new LevelXMLParser(level);
            _currentLevel = level;
        }
        else
        {
            Debug.Log("No se encontro el nivel \"" + level.name + "\"");
        }
    }

    /// <summary>
    /// Carga un nivel, instanciando un memento del mismo
    /// </summary>
    /// <param name="gridMemento"></param>
    public void Load(GridMemento gridMemento)
    {
        Debug.Log("Cargando memento del nivel \"" + _currentLevelInfo.LevelName + "\"");

        // Instancia las clases de memento si no estaban instanciadas
        if (_gridOriginator == null) _gridOriginator = new GridOriginator(_currentLevelInfo.Size);
        if (_gridCaretaker == null) _gridCaretaker = new GridCaretaker(10);

        // Spawnea los bloques
        foreach (var blockState in gridMemento.BlocksState)
        {
            Level.Instance.SpawnBlock(blockState);
        }

        // Spawnea los characters
        foreach (var charState in gridMemento.CharactersState)
        {
            Level.Instance.SpawnCharacter(charState, true);
        }
    }

    /// <summary>
    /// Descarga el nivel, destruyendo todos los bloques, characters y la grilla
    /// </summary>
    public void Unload()
    {
        // Destruye todos los bloques
        foreach (var block in _gridOriginator.Grid.AllBlocks)
        {
            Destroy(block.gameObject);
        }

        // Destruye todos los characters
        foreach (var character in _gridOriginator.Grid.AllCharacters)
        {
            Destroy(character.gameObject);
        }

        // Destruye la grilla
        _gridOriginator = null;
        _gridCaretaker = null;
    }

    /// <summary>
    /// Destruye todos los gameObjects y limpia la grilla
    /// </summary>
    public void Clear()
    {
        // Destruye todos los bloques
        var blocksCollection = _gridOriginator.Grid.AllBlocks.ToArray();
        foreach (var block in blocksCollection)
        {
            block.Destroy();
        }

        // Destruye todos los characters
        var charCollection = _gridOriginator.Grid.AllCharacters.ToArray();
        foreach (var character in charCollection)
        {
            character.Destroy();
        }
    }

    /// <summary>
    /// Spawnea un bloque en el mundo
    /// </summary>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="blockID">ID del tipo de bloque a spawnear (Index)</param>
    /// <param name="basement">Flag que determina si el cubo es inamovible</param>
    public void SpawnBlockAt(int blockID, IntVector3 pos, bool basement = false)
    {
        SpawnBlockAt(Index.Blocks[blockID], pos, basement);
    }

    /// <summary>
    /// Spawnea un bloque en el mundo
    /// </summary>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="block">Bloque a spawnear, se creara una instancia del mismo</param>
    /// <param name="basement">Flag que determina si el cubo es inamovible</param>
    public void SpawnBlockAt(Block block, IntVector3 pos, bool basement = false)
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
    public void SpawnBlock(BlockState blockState)
    {
        // Instancia el bloque
        var instancedBlock = ((GameObject)Instantiate(Index.Blocks[blockState.IndexType].gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Block>();

        // Asigna el estado
        instancedBlock.SetState(blockState);

        // Lo agrega al grid
        _gridOriginator.Grid.AddBlock(instancedBlock);
    }

    /// <summary>
    /// Spawnea un character en el mundo. Si es un jugador lo asigna a las camaras.
    /// </summary>
    /// <param name="character">Character a spawner, se creara una instancia del mismo</param>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="player">Jugador que sera seguido por las camaras</param>
    public void SpawnCharacterAt(int characterID, IntVector3 pos, bool player = false)
    {
        SpawnCharacterAt(Index.Characters[characterID], pos, player);
    }

    /// <summary>
    /// Spawnea un character en el mundo. Si es un jugador lo asigna a las camaras.
    /// </summary>
    /// <param name="character">Character a spawner, se creara una instancia del mismo</param>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="player">Jugador que sera seguido por las camaras</param>
    public void SpawnCharacterAt(Character character, IntVector3 pos, bool player = false)
    {
        // Instancia el character
        var instancedCharacter = ((GameObject)Instantiate(character.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Character>();

        // Lo ubica
        instancedCharacter.SetPosition(pos);

        // Si es un character jugable
        if (instancedCharacter.tag == "Player")
        {
            // Registra eventos para salvar un memento al mover bloques
            instancedCharacter.RegisterPushDel(() => Level.Instance.SaveState());
            instancedCharacter.RegisterPullDel(() => Level.Instance.SaveState());

            // Lo targetea con la camara
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
    public void SpawnCharacter(CharacterState characterState, bool player = false)
    {

        // Instancia el character
        var instancedCharacter = ((GameObject)Instantiate(Index.Characters[characterState.IndexType].gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Character>();

        // Asigna el estado
        instancedCharacter.SetState(characterState);

        // Si es un character jugable
        if (instancedCharacter.tag == "Player")
        {
            // Registra eventos para salvar un memento al mover bloques
            instancedCharacter.RegisterPushDel(() => Level.Instance.SaveState());
            instancedCharacter.RegisterPullDel(() => Level.Instance.SaveState());

            // Lo targetea con la camara
            Camera.main.GetComponent<SmoothFollowAdvance>().SetTarget(instancedCharacter.transform);
        }

        // Lo agrega al grid
        _gridOriginator.Grid.AddCharacter(instancedCharacter.GetComponent<Character>());
    }

    /// <summary>
    /// Guarda el estado de la grilla
    /// </summary>
    public void SaveState()
    {
        _gridCaretaker.Push(_gridOriginator.CreateMemento());
    }

    /// <summary>
    /// Carga un estado previo
    /// </summary>
    public void LoadPreviousState()
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