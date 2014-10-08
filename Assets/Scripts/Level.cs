using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	#region Variables (private)

    private static Grid _grid;
    
	#endregion

	#region Propiedades (public)

    public static Grid Grid
    {
        get
        {
            return _grid;
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

	#endregion

    #region Metodos privados
    
    /// <summary>
    /// Instancia un nivel
    /// </summary>
    /// <param name="level">Asset del nivel a cargar</param>
    private static void InstantiateLevel(TextAsset level)
    {
        // Crea la grilla
        _grid = new Grid();

        // Parsea el nivel
        var strReader = new StringReader(level.text);
        for (int y = 0; y < 40; y++)
        {
            for (int z = 0; z < 20; z++)
            {
                var line = strReader.ReadLine();
                for (int x = 0; x < 20; x++)
                {
                    // Lee el caracter en la pos X (omite las comas)
                    List<string> entries = line.Split(',').ToList<string>();
                    var currEntry = entries[x];

                    // Si no es 0 hay algo
                    if (currEntry != "0")
                    {
                        // Si es una M -> Meta a llegar
                        if (currEntry == "M")
                        {

                        }
                        // Si inicia con una P -> Spawn de jugador
                        else if (currEntry.StartsWith("P"))
                        {
                            // ID
                            currEntry = currEntry.Substring(1);
                            var charID = int.Parse(currEntry) - 1;

                            // Instancia el jugador
                            SpawnCharacterAt(Index.Characters[charID], new IntVector3(x, y, z));
                        }
                        // Sino -> Posible Spawn de bloque
                        else
                        {
                            // Flag de bloque base? (no aplica gravedad)
                            var baseFlag = currEntry.StartsWith("B");
                            if (baseFlag)
                                currEntry = currEntry.Substring(1);

                            // Indice
                            var blockIndex = int.Parse(currEntry) - 1;

                            // Spawnea el bloque
                            Level.SpawnBlockAt(new IntVector3(x, y, z), Index.Blocks[blockIndex], baseFlag);
                        }
                    }
                }
            }

            // Descarta la linea vacia entre niveles de Y
            strReader.ReadLine();
        }
    }

    #endregion

    #region Metodos publicos

    /// <summary>
    /// Carga un nivel, cargando los elementos del mismo y generando su grilla
    /// </summary>
    /// <param name="levelName">Nombre del nivel a cargar</param>
    public static void Load(string levelName)
    {
        var level = Index.Levels.Find(x => x.name == levelName);
        if (level)
        {
            Debug.Log("Cargando el nivel \"" + levelName + "\"");
            InstantiateLevel(level);
        }
        else
        {
            Debug.Log("No se encontro el nivel \"" + levelName + "\"");
        }
    }

    /// <summary>
    /// Carga un nivel, cargando los elementos del mismo y generando su grilla
    /// </summary>
    /// <param name="levelID">ID del nivel a cargar</param>
    public static void Load(int levelID)
    {
        InstantiateLevel(Index.Levels[levelID]);
    }

    /// <summary>
    /// Spawnea un bloque en el mundo
    /// </summary>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="blockID">ID del tipo de bloque a spawnear (Index)</param>
    /// <param name="basement">Flag que determina si el cubo es inamovible</param>
    public static void SpawnBlockAt(IntVector3 pos, int blockID, bool basement = false)
    {
        SpawnBlockAt(pos, Index.Blocks[blockID], basement);
    }

    /// <summary>
    /// Spawnea un bloque en el mundo
    /// </summary>
    /// <param name="pos">Posicion en donde hara spawn</param>
    /// <param name="block">Bloque a spawnear, se creara una instancia del mismo</param>
    /// <param name="basement">Flag que determina si el cubo es inamovible</param>
    public static void SpawnBlockAt(IntVector3 pos, Block block, bool basement = false)
    {
        // Instancia el bloque
        var instancedBlock = ((GameObject)Instantiate(block.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<Block>();

        // Lo ubica y asigna flag de basement
        instancedBlock.position = pos;
        instancedBlock.isBasement = basement;

        // Lo agrea al grid
        _grid.AddBlock(instancedBlock);
    }    

    /// <summary>
    /// Spawnea un character en el mundo
    /// </summary>
    /// <param name="character">Character a spawner, se creara una instancia del mismo</param>
    /// <param name="pos">Posicion en donde hara spawn</param>
    public static void SpawnCharacterAt(GameObject character, IntVector3 pos)
    {
        Instantiate(character, new Vector3(pos.x + .5f, pos.y, pos.z + .5f), Quaternion.identity);
    }

	#endregion
}