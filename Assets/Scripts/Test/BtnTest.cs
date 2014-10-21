using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BtnTest : MonoBehaviour 
{
	#region Variables (private)

    [SerializeField]
    private TextAsset _XML;

	#endregion

	#region Properties (public)

	#endregion

	#region Unity event functions

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () 
	{
        var parser = new LevelXMLParser(_XML);
        var levelInfo = parser.ToLevelInfo();
        var levelMemento = parser.ToGridMemento();

        Debug.Log("Leido :\"" + levelInfo.LevelName + "\"");
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () 
	{	
	}

    void OnMouseDown()
    {
    }

    void OnMouseUp()
    {
    }
    
	#endregion

	#region Methods

	#endregion
}
