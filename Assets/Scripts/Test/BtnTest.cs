using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BtnTest : MonoBehaviour 
{
	#region Variables (private)
        
	#endregion

	#region Properties (public)

	#endregion

	#region Unity event functions

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () 
	{
        LoadLevel();
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
        //Debug.Log(LevelGrid.GetBlock(8, 0, 9));
        //Debug.Log(LevelGrid.GetBlocks());
    }

    void LoadLevel()
    {
        Level.Load("test");
    }
    
	#endregion

	#region Methods

	#endregion
}
