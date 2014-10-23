using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ExpandableMatrix<T>
{
    #region Variables (private)

    private T[, ,] _positives;
    private T[, ,] _negativesX;
    private T[, ,] _negativesY;
    private T[, ,] _negativesZ;
    private T[, ,] _negativesXY;
    private T[, ,] _negativesXZ;
    private T[, ,] _negativesYZ;
    private T[, ,] _negativesXYZ;

    private int _growExtraSpaces;

    #endregion

    #region Propiedades (public)

    public T this[int x, int y, int z]
    {
        get
        {
            T result = default(T);

            var octant = Octant.None;
            octant |= x < 0 ? Octant.NegativeX : Octant.PositiveX;
            octant |= y < 0 ? Octant.NegativeY : Octant.PositiveY;
            octant |= z < 0 ? Octant.NegativeZ : Octant.PositiveZ;

            try
            {
                switch (octant)
                {
                    case Octant.PositiveX | Octant.PositiveY | Octant.PositiveZ: result = _positives[x, y, z]; break;
                    case Octant.NegativeX | Octant.PositiveY | Octant.PositiveZ: result = _negativesX[-1 - x, y, z]; break;
                    case Octant.PositiveX | Octant.NegativeY | Octant.PositiveZ: result = _negativesY[x, -1 - y, z]; break;
                    case Octant.PositiveX | Octant.PositiveY | Octant.NegativeZ: result = _negativesZ[x, y, -1 - z]; break;
                    case Octant.NegativeX | Octant.NegativeY | Octant.PositiveZ: result = _negativesXY[-1 - x, -1 - y, z]; break;
                    case Octant.NegativeX | Octant.PositiveY | Octant.NegativeZ: result = _negativesXZ[-1 - x, y, -1 - z]; break;
                    case Octant.PositiveX | Octant.NegativeY | Octant.NegativeZ: result = _negativesYZ[x, -1 - y, -1 - z]; break;
                    case Octant.NegativeX | Octant.NegativeY | Octant.NegativeZ: result = _negativesXYZ[-1 - x, -1 - y, -1 - z]; break;
                }
            }
            catch { };

            return result;
        }
        set
        {
            var octant = Octant.None;
            octant |= x < 0 ? Octant.NegativeX : Octant.PositiveX;
            octant |= y < 0 ? Octant.NegativeY : Octant.PositiveY;
            octant |= z < 0 ? Octant.NegativeZ : Octant.PositiveZ;

            switch (octant)
            {
                case Octant.PositiveX | Octant.PositiveY | Octant.PositiveZ: SetValue(ref _positives, x, y, z, value); break;
                case Octant.NegativeX | Octant.PositiveY | Octant.PositiveZ: SetValue(ref _negativesX, -1 - x, y, z, value); break;
                case Octant.PositiveX | Octant.NegativeY | Octant.PositiveZ: SetValue(ref _negativesY, x, -1 - y, z, value); break;
                case Octant.PositiveX | Octant.PositiveY | Octant.NegativeZ: SetValue(ref _negativesZ, x, y, -1 - z, value); break;
                case Octant.NegativeX | Octant.NegativeY | Octant.PositiveZ: SetValue(ref _negativesXY, -1 - x, -1 - y, z, value); break;
                case Octant.NegativeX | Octant.PositiveY | Octant.NegativeZ: SetValue(ref _negativesXZ, -1 - x, y, -1 - z, value); break;
                case Octant.PositiveX | Octant.NegativeY | Octant.NegativeZ: SetValue(ref _negativesYZ, x, -1 - y, -1 - z, value); break;
                case Octant.NegativeX | Octant.NegativeY | Octant.NegativeZ: SetValue(ref _negativesXYZ, -1 - x, -1 - y, -1 - z, value); break;
            }
        }
    }

    #endregion

    #region Metodos (publicos)

    public ExpandableMatrix()
    {
        this._growExtraSpaces = 1;
        this._positives = new T[0, 0, 0];
        this._negativesX = new T[0, 0, 0];
        this._negativesY = new T[0, 0, 0];
        this._negativesZ = new T[0, 0, 0];
        this._negativesXY = new T[0, 0, 0];
        this._negativesXZ = new T[0, 0, 0];
        this._negativesYZ = new T[0, 0, 0];
        this._negativesXYZ = new T[0, 0, 0];
    }

    public ExpandableMatrix(IntVector3 size, int growExtraSpaces = 0)
    {
        this._growExtraSpaces = growExtraSpaces;

        size = new IntVector3(
            Mathf.Max(0, size.x),
            Mathf.Max(0, size.y),
            Mathf.Max(0, size.z)
        );

        this._positives = new T[size.x, size.y, size.z];
        this._negativesX = new T[0, 0, 0];
        this._negativesY = new T[0, 0, 0];
        this._negativesZ = new T[0, 0, 0];
        this._negativesXY = new T[0, 0, 0];
        this._negativesXZ = new T[0, 0, 0];
        this._negativesYZ = new T[0, 0, 0];
        this._negativesXYZ = new T[0, 0, 0];
    }

    public ExpandableMatrix(IntVector3 minPoint, IntVector3 maxPoint, int growExtraSpaces = 0)
    {
        this._growExtraSpaces = growExtraSpaces;

        minPoint = new IntVector3(
            Mathf.Min(0, minPoint.x),
            Mathf.Min(0, minPoint.y),
            Mathf.Min(0, minPoint.z)
        );

        maxPoint = new IntVector3(
            Mathf.Max(0, maxPoint.x),
            Mathf.Max(0, maxPoint.y),
            Mathf.Max(0, maxPoint.z)
        );

        this._positives = new T[maxPoint.x + 1, maxPoint.y + 1, maxPoint.z + 1];
        this._negativesX = new T[-minPoint.x, maxPoint.y + 1, maxPoint.z + 1];
        this._negativesY = new T[maxPoint.x + 1, -minPoint.y, maxPoint.z + 1];
        this._negativesZ = new T[maxPoint.x + 1, maxPoint.y + 1, -minPoint.z];
        this._negativesXY = new T[-minPoint.x, -minPoint.y, maxPoint.z + 1];
        this._negativesXZ = new T[-minPoint.x, maxPoint.y + 1, -minPoint.z];
        this._negativesYZ = new T[maxPoint.x + 1, -minPoint.y, -minPoint.z];
        this._negativesXYZ = new T[-minPoint.x, -minPoint.y, -minPoint.z];
    }

    /// <summary>
    /// Funcion que devuelve la longitud en el espacio de una de las tres dimensiones.
    /// </summary>
    /// <param name="dimension">Dimension a obtener la longitud de valores que poseen una imagen</param>
    /// <returns>La longitud devuelta en un vector2 cuyo X es la longitud de los negtivos e Y es la longitud de los positivos</returns>
    public Vector2 GetLength(int dimension)
    {
        int negLength, posLength;
        if (dimension == 0)
        {
            negLength = Mathf.Max(_negativesX.GetLength(0), _negativesXY.GetLength(0), _negativesXZ.GetLength(0), _negativesXYZ.GetLength(0));
            posLength = Mathf.Max(_positives.GetLength(0), _negativesY.GetLength(0), _negativesZ.GetLength(0), _negativesYZ.GetLength(0));
        }
        else if (dimension == 1)
        {
            negLength = Mathf.Max(_negativesY.GetLength(1), _negativesXY.GetLength(1), _negativesYZ.GetLength(1), _negativesXYZ.GetLength(1));
            posLength = Mathf.Max(_positives.GetLength(1), _negativesX.GetLength(1), _negativesZ.GetLength(1), _negativesXZ.GetLength(1));
        }
        else
        {
            negLength = Mathf.Max(_negativesZ.GetLength(2), _negativesXZ.GetLength(2), _negativesYZ.GetLength(2), _negativesXYZ.GetLength(2));
            posLength = Mathf.Max(_positives.GetLength(2), _negativesY.GetLength(2), _negativesX.GetLength(2), _negativesXY.GetLength(2));
        }

        return new Vector3(negLength, posLength);
    }

    #endregion

    #region Metodos (privados)

    private void SetValue(ref T[, ,] matrix, int x, int y, int z, T value)
    {
        if (OutOfMatrix(matrix, x, y, z))
        {
            //Debug.Log("Agrandando Grilla");
            ResizeToInclude(ref matrix, x + _growExtraSpaces, y + _growExtraSpaces, z + _growExtraSpaces);
        }

        matrix[x, y, z] = value;
    }

    private void ResizeToInclude(ref T[, ,] matrix, int x, int y, int z)
    {
        var xLength = Mathf.Max(matrix.GetLength(0), x + 1);
        var yLength = Mathf.Max(matrix.GetLength(1), y + 1);
        var zLength = Mathf.Max(matrix.GetLength(2), z + 1);
        ResizeArray(ref matrix, xLength, yLength, zLength);
    }

    private void ResizeArray(ref T[, ,] array, int xLength, int yLength, int zLength)
    {
        var newArray = new T[xLength, yLength, zLength];
        int minX = Math.Min(xLength, array.GetLength(0));
        int minY = Math.Min(yLength, array.GetLength(1));
        int minZ = Math.Min(zLength, array.GetLength(2));

        for (int i = 0; i < minX; i++)
            for (int j = 0; j < minY; j++)
                for (int k = 0; k < minZ; k++)
                    newArray[i, j, k] = array[i, j, k];

        array = newArray;
    }

    private bool OutOfMatrix(T[, ,] matrix, int x, int y, int z)
    {
        return (x + 1 > matrix.GetLength(0) || y + 1 > matrix.GetLength(1) || z + 1 > matrix.GetLength(2));
    }

    #endregion

    [Flags]
    enum Octant
    {
        None = 0x000,
        PositiveX = 0x001,
        NegativeX = 0x002,
        PositiveY = 0x010,
        NegativeY = 0x020,
        PositiveZ = 0x100,
        NegativeZ = 0x200,
    }
}
