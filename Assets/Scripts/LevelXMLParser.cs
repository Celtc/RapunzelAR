using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

public class LevelXMLParser
{
	#region Variables (private)

    private TextAsset _XMLsource;

	#endregion
    
	#region Metodos (Publicos)
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="level">TextAsset a parsear</param>
    public LevelXMLParser(TextAsset level)
    {
        this._XMLsource = level;
    }

    /// <summary>
    /// Extrae el memento inicial del nivel
    /// </summary>
    /// <returns>GridMemento resultante</returns>
    public GridMemento ToGridMemento()
    {
        // Parsea el body
        return ParseBody();
    }

    /// <summary>
    /// Extrae la info del nivel
    /// </summary>
    /// <returns></returns>
    public LevelInfo ToLevelInfo()
    {
        // Parse el header
        return ParseHeader();
    }


    public static implicit operator GridMemento(LevelXMLParser p)
    {
        return p.ToGridMemento();
    }

    public static implicit operator LevelInfo(LevelXMLParser p)
    {
        return p.ToLevelInfo();
    }

    #endregion

    #region Metdodos (Privados)

    /// <summary>
    /// Parsea el textAsset a un memento
    /// </summary>
    /// <returns>GridMemento resultante</returns>
    private GridMemento ParseBody()
    {
        // Variables
        var blocksState = new List<BlockState>();
        var charactersState = new List<CharacterState>();

        // Parsea el XML
        using (var stringReader = new System.IO.StringReader(_XMLsource.text))
        {
            // Lee el archivo
            var xml = XDocument.Parse(stringReader.ReadToEnd());

            // Extrae el tamaño de la grilla
            var size = new Vector3(
                int.Parse(xml.Root.Element("Data").Element("Size").Element("X").Value),
                int.Parse(xml.Root.Element("Data").Element("Size").Element("Y").Value),
                int.Parse(xml.Root.Element("Data").Element("Size").Element("Z").Value)
            );

            // Itera por niveles
            for (int y = 0; y < size.y; y++)
            {
                // Grilla del Y actual
                var heightStruct = xml.Root.Element("Grid").Element("Y" + y.ToString()).Value;
                using (var strReader = new StringReader(heightStruct))
                {
                    // Itera por profundidad y ancho
                    for (int z = 0; z < size.z; z++)
                    {
                        var line = strReader.ReadLine();
                        for (int x = 0; x < size.x; x++)
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
                                    newBlockState.Position = new Vector3(x, y, z);
                                    newBlockState.IsBasement = baseFlag;

                                    // Agrea el state
                                    blocksState.Add(newBlockState);
                                }
                            }
                        }
                    }
                }
            }
        }        

        // Devuelve el grid
        return new GridMemento(blocksState, charactersState);
    }
    
    /// <summary>
    /// Parsea el Header del XML para obtener la info
    /// </summary>
    /// <returns>Estructura con la info del nivel</returns>
    private LevelInfo ParseHeader()
    {
        // Variable contenedora de la info
        var levelInfo = new LevelInfo();

        // Parsea el XML
        using (var stringReader = new System.IO.StringReader(_XMLsource.text))
        {
            var xml = XDocument.Parse(stringReader.ReadToEnd());
            levelInfo.Version = xml.Root.Element("Version").Value;
            levelInfo.LevelName = xml.Root.Element("Data").Element("Name").Value;
            levelInfo.Difficulty = int.Parse(xml.Root.Element("Data").Element("Difficulty").Value);
            levelInfo.GoldScore = float.Parse(xml.Root.Element("Data").Element("GoldScore").Value);
            levelInfo.SilverScore = float.Parse(xml.Root.Element("Data").Element("SilverScore").Value);
            levelInfo.BronzeScore = float.Parse(xml.Root.Element("Data").Element("BronzeScore").Value);
            levelInfo.Rewinds = int.Parse(xml.Root.Element("Data").Element("Rewinds").Value);
            levelInfo.Size = new IntVector3(
                int.Parse(xml.Root.Element("Data").Element("Size").Element("X").Value),
                int.Parse(xml.Root.Element("Data").Element("Size").Element("Y").Value),
                int.Parse(xml.Root.Element("Data").Element("Size").Element("Z").Value)
            );
        }

        return levelInfo;
    }

    #endregion
}