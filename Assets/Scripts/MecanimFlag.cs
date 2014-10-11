using UnityEngine;
using System.Collections;

[System.Serializable]
public class MecanimFlag
{
	#region Variables (private)

    [SerializeField]
    private string _name;
    [SerializeField]
    private bool _value;

	#endregion

	#region Propiedades (public)

    public string Name
    {
        get { return _name; }
    }

    public bool Value
    {
        get { return this._value; }
        set { this._value = value; }
    }

	#endregion
    
	#region Metodos

    public MecanimFlag()
    {
        this._name = string.Empty;
        this._value = false;
    }

    public MecanimFlag(string name, bool value = false)
    {
        this._name = name;
        this._value = value;
    }

	#endregion
}
