using UnityEngine;
using System.Collections;

[System.Serializable]
public class CustomUISpriteAnimationData
{
	#region Variables (private)

    [SerializeField]
    private string _spriteName;

    [SerializeField]
    private float _showTime;

	#endregion

	#region Propiedades (public)

    public string SpriteName
    {
        get { return _spriteName; }
        set { _spriteName = value; }
    }

    public float ShowTime
    {
        get { return _showTime; }
        set { _showTime = value; }
    }

	#endregion    
}
