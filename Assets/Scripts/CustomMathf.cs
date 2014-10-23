using UnityEngine;
using System.Collections;

public class CustomMathf
{
	#region Metodos

    public static Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static IntVector3 RoundToIntVector(float x, float y, float z)
    {
        return new IntVector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(z));
    }

    public static IntVector3 RoundToIntVector(Vector3 vector)
    {
        return new IntVector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

	#endregion
}
