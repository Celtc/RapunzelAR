using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LevelParser
{
	#region Variables (private)

    private TextAsset _source;

	#endregion
    
	#region Metodos (Publicos)
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="level">TextAsset a parsear</param>
    public LevelParser(TextAsset level)
    {
        this._source = level;
    }

    /// <summary>
    /// Parsea el asset
    /// </summary>
    /// <returns>GridMemento resultante</returns>
    public GridMemento ToGridMemento()
    {
        // Parsea el nivel
        var memento = Parse();

        // Crea el memento
        return memento;
    }

    public static implicit operator GridMemento(LevelParser p)
    {
        return p.ToGridMemento();
    }

    #endregion

    #region Metdodos (Privados)

    /// <summary>
    /// Parsea el textAsset a un memento
    /// </summary>
    /// <returns>GridMemento resultante</returns>
    private GridMemento Parse()
    {
        // Variables
        var blocksState = new List<BlockState>();
        var charactersState = new List<CharacterState>();

        // Itera por niveles
        var strReader = new StringReader(_source.text);
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
                        // Si inicia con una C -> Spawn de Character
                        else if (currEntry.StartsWith("C"))
                        {
                            // ID
                            currEntry = currEntry.Substring(1);
                            var charID = int.Parse(currEntry) - 1;

                            // Crea el nuevo state
                            var newCharState = Index.Characters[charID].GetState();
                            newCharState.Position = new Vector3(x + .5f, y, z + .5f);

                            // Agrea el state
                            charactersState.Add(newCharState);
                        }
                        // Sino -> Posible Spawn de bloque
                        else
                        {
                            // Flag de bloque base? (no aplica gravedad)
                            var baseFlag = currEntry.StartsWith("B");
                            if (baseFlag)
                                currEntry = currEntry.Substring(1);

                            // Indice
                            var blockID = int.Parse(currEntry) - 1;

                            // Crea el nuevo state
                            var newBlockState = Index.Blocks[blockID].GetState();
                            newBlockState.Position = new Vector3(x + .5f, y + .5f, z + .5f);
                            newBlockState.IsBasement = baseFlag;

                            // Agrea el state
                            blocksState.Add(newBlockState);
                        }
                    }
                }
            }

            // Descarta la linea vacia entre niveles de Y
            strReader.ReadLine();
        }

        // Devuelve el grid
        return new GridMemento(blocksState, charactersState);
    }

    #endregion
}
