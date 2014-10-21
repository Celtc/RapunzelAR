using UnityEngine;
using System.Collections;

[System.Serializable]
public class IntVector3
{
    #region Variables (private)

    [SerializeField]
    private int _x;
    [SerializeField]
    private int _y;
    [SerializeField]
    private int _z;

    #endregion

    #region Properties (public)

    public int x { get { return _x; } set { _x = value; } }
    public int y { get { return _y; } set { _y = value; } }
    public int z { get { return _z; } set { _z = value; } }

    #endregion

    #region Metodos

    public IntVector3(int x, int y)
    {
        _x = x;
        _y = y;
        _z = int.MinValue;
    }

    public IntVector3(int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public IntVector3(Vector3 values)
    {
        _x = (int)values.x;
        _y = (int)values.y;
        _z = (int)values.z;
    }
    
    public override string ToString()
    {
        return "(" + _x.ToString() + ", " + _y.ToString() + ", " + _z.ToString() + ")";
    }
    
    public static IntVector3 operator *(float f, IntVector3 v)
    {
        return new IntVector3((int)(v.x * f), (int)(v.y * f), (int)(v.z * f));
    }

    public static IntVector3 operator *(IntVector3 v, float f)
    {
        return new IntVector3((int)(v.x * f), (int)(v.y * f), (int)(v.z * f));
    }

    public static IntVector3 operator -(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1._x - v2._x, v1._y - v2._y, v1._z - v2._z);
    }

    public static IntVector3 operator -(Vector3 v1, IntVector3 v2)
    {
        return (IntVector3)v1 - v2;
    }

    public static IntVector3 operator -(IntVector3 v1, Vector3 v2)
    {
        return v1 - (IntVector3)v2;
    }

    public static IntVector3 operator +(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1._x + v2._x, v1._y + v2._y, v1._z + v2._z);
    }

    public static IntVector3 operator +(Vector3 v1, IntVector3 v2)
    {
        return (IntVector3)v1 + v2;
    }

    public static IntVector3 operator +(IntVector3 v1, Vector3 v2)
    {
        return v1 + (IntVector3)v2;
    }
    
    public static implicit operator Vector3(IntVector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }
    
    public static implicit operator IntVector3(Vector3 vector)
    {
        return new IntVector3(vector);
    }

    public static implicit operator string(IntVector3 vector)
    {
        return vector.ToString();
    }
    
    public static bool operator ==(IntVector3 v1, IntVector3 v2)
    {
        if (object.ReferenceEquals(v1, null))
        {
            return object.ReferenceEquals(v2, null);
        }

        return v1.Equals(v2);
    }

    public static bool operator !=(IntVector3 v1, IntVector3 v2)
    {
        return !(v1 == v2);
    }
    
    public override bool Equals(System.Object obj)
    {
        if (obj == null) return false;

        var v = obj as IntVector3;
        if ((System.Object)v == null) return false;

        return (this.x == v.x) && (this.y == v.y) && (this.z == v.z);
    }

    public bool Equals(IntVector3 v)
    {
        if ((object)v == null) return false;

        return (this.x == v.x) && (this.y == v.y) && (this.z == v.z);
    }

    public override int GetHashCode()
    {
        return this.x ^ this.y ^ this.z;//base.GetHashCode();
    }

    public static IntVector3 zero { get { return new IntVector3(0, 0, 0); } }

    #endregion
}
