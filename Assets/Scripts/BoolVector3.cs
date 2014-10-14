using UnityEngine;
using System.Collections;

[System.Serializable]
public class BoolVector3
{
	#region Variables (private)

    [SerializeField]
    private bool _x;
    [SerializeField]
    private bool _y;
    [SerializeField]
    private bool _z;

	#endregion

	#region Propiedades (public)

    public bool x { get { return _x; } set { _x = value; } }
    public bool y { get { return _y; } set { _y = value; } }
    public bool z { get { return _z; } set { _z = value; } }

	#endregion

	#region Metodos
    
    public BoolVector3(bool x, bool y, bool z)
    {
        this._x = x;
        this._y = y;
        this._z = z;
    }

	#endregion
}
