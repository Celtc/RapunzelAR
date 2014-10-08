using UnityEngine;
using System.Collections;

public class BtnTest : MonoBehaviour 
{
	#region Variables (private)

    [SerializeField]
    private GameObject playerModel;

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
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Level.SpawnCharacterAt(Index.Characters[0], new IntVector3(8, 1, 8));
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
