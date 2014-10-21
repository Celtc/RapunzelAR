using UnityEngine;
using System.Collections;

public class LevelInfo
{
	#region Variables (private)

    private string levelName;
    private float goldScore;
    private float silverScore;
    private float bronzeScore;
    private int rewinds;

	#endregion

	#region Propiedades (public)


    public string LevelName
    {
        get { return levelName; }
        set { levelName = value; }
    }

    public int Rewinds
    {
        get { return rewinds; }
        set { rewinds = value; }
    }

    public float BronzeScore
    {
        get { return bronzeScore; }
        set { bronzeScore = value; }
    }

    public float SilverScore
    {
        get { return silverScore; }
        set { silverScore = value; }
    }

    public float GoldScore
    {
        get { return goldScore; }
        set { goldScore = value; }
    }
	#endregion

	#region Metodos

	#endregion
}
