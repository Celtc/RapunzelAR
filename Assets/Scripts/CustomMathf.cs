using UnityEngine;
using System.Collections;

public class CustomMathf
{
	#region Metodos

    public static Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static IntVector3 RoundToIntVector(Vector3 vector)
    {
        return new IntVector3((int)Mathf.Round(vector.x), (int)Mathf.Round(vector.y), (int)Mathf.Round(vector.z));
    }

	#endregion
}
