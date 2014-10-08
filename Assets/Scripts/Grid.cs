﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Grid
{
    #region Constantes

    const int _GRIDSIZE_X = 20;
    const int _GRIDSIZE_Y = 40;
    const int _GRIDSIZE_Z = 20;

    #endregion

	#region Variables (private)

    // Grilla de bloques
    private Block[, ,] _grid = new Block[_GRIDSIZE_X, _GRIDSIZE_Y, _GRIDSIZE_Z];

    // Diccionario para facilitar el acceso utilizando indices (independientemente de la posicion de los bloques)
    private Dictionary<Block, IntVector3> _hashTable = new Dictionary<Block, IntVector3>(_GRIDSIZE_X * _GRIDSIZE_Y * _GRIDSIZE_Z);

	#endregion

	#region Propiedades (public)

    public Block this[int x, int y, int z]
    {
        get
        {
            return OutOfGrid(new IntVector3(x, y, z)) ? null : _grid[x, y, z];
        }
    }

    public Block this[IntVector3 pos]
    {
        get
        {
            return OutOfGrid(pos) ? null : _grid[pos.x, pos.y, pos.z];
        }
    }

    public Block[, ,] AllBlocks
    {
        get
        {
            return _grid.Clone() as Block[, ,];
        }
    }

	#endregion
    
    #region Metodos privados

    /// <summary>
    /// Recupera un bloque de una posicion determinada. NO incluye validaciones de indices.
    /// </summary>
    /// <param name="pos">Posicion a recuperar</param>
    /// <returns></returns>
    private Block GetAt(IntVector3 pos)
    {
        return _grid[pos.x, pos.y, pos.z];
    }

    /// <summary>
    /// Asigna un bloque a una posicion determinada. NO incluye validaciones de indices.
    /// </summary>
    /// <param name="pos">Posicion a asignar</param>
    /// <param name="block">Bloque a asignar</param>
    /// <returns></returns>
    private void SetAt(IntVector3 pos, Block block)
    {
        _grid[pos.x, pos.y, pos.z] = block;
    }
    
    #endregion

    #region Metodos publicos

    /// <summary>
    /// Agrega un bloque a la grilla
    /// </summary>
    /// <param name="block"></param>
    public void AddBlock(Block block)
    {  
        // Guarda el bloque en la matriz
        SetAt(block.position, block);

        // Lo almacena en la hashTable
        _hashTable.Add(block, block.position);
    }

    /// <summary>
    /// Remueve un bloque de la grilla
    /// </summary>
    /// <param name="block">Bloque a destruir</param>
    public void RemoveBlock(Block block)
    {
        // Vacia las entradas
        SetAt(block.position, null);
        _hashTable.Remove(block);
    }

    /// <summary>
    /// Remueve un bloque de la grilla
    /// </summary>
    /// <param name="pos">Posicion del bloque a remover</param>
    public void RemoveBlock(IntVector3 pos)
    {
        // Obtiene el bloque a destruir
        var block = GetAt(pos);
        if (block == null) return;

        // Destruye el bloque
        RemoveBlock(block);

        return;
    }

    /// <summary>
    /// Actualiza la grilla a partir de un cambio en los bloques
    /// </summary>
    /// <param name="block">Bloque que sufrio alteraciones</param>
    public void Update(Block block)
    {
        // Si el bloque no existe sale
        if (!_hashTable.ContainsKey(block)) throw new KeyNotFoundException();

        // Obtiene la antigua y nueva posicion
        var from = _hashTable[block];
        var to = block.position;

        // Si no hay un cambio de posicion, omite mover indices
        if (from != to)
        {
            // Si habia un bloque en la nueva posicion lo destruye
            if (GetAt(to) != null)
                RemoveBlock(GetAt(to));

            // Actualiza la hashtable
            _hashTable[block] = to;
        }

        // Actualiza la grilla
        SetAt(from, null);
        SetAt(to, block);
    }

    /// <summary>
    /// Actualiza la grilla a partir de un cambio SOLO de posicion.
    /// </summary>
    /// <param name="from">Posicion original en la que se encontraba el bloque</param>
    /// <param name="to">Posicion final del bloque</param>
    public void Update(IntVector3 from, IntVector3 to)
    {
        // Obtiene el bloque a actualizar, sino existe sale
        var block = GetAt(from);
        if (block == null) return;

        // Si habia un bloque en la nueva posicion lo destruye
        if (GetAt(to) != null)
            RemoveBlock(GetAt(to));

        // Actualiza la hashtable
        _hashTable[block] = to;

        // Actualiza la grilla
        SetAt(from, null);
        SetAt(to, block);
    }

    /// <summary>
    /// Posicion en la que se encuentra un bloque
    /// </summary>
    /// <param name="block">Bloque del cual se desea conocer la posicion</param>
    /// <returns></returns>
    public IntVector3 IndexOf(Block block)
    {
        return _hashTable[block];
    }
    
    /// <summary>
    /// Indica si existe un bloque en una posicion
    /// </summary>
    /// <param name="pos">Posicion a verificar</param>
    /// <returns></returns>
    public bool ExistsAt(IntVector3 pos)
    {
        return this[pos] != null;
    }

    /// <summary>
    /// Indica si existe un bloque estacionario en una posicion
    /// </summary>
    /// <param name="pos">Posicion a verificar</param>
    /// <returns></returns>
    public bool ExistsStillAt(IntVector3 pos)
    {
        var result = false;

        var block = this[pos];
        if (block && !block.isMoving)
            result = true;

        return result;
    }

    /// <summary>
    /// Determina si una posicion esta dentro de los rangos de la grilla
    /// </summary>
    /// <param name="pos">Posicion a verificar</param>
    /// <returns></returns>
    public bool OutOfGrid(IntVector3 pos)
    {
        var result = false;

        if (pos.x < 0 || pos.x >= _GRIDSIZE_X ||
            pos.z < 0 || pos.z >= _GRIDSIZE_Y ||
            pos.y < 0 || pos.y >= _GRIDSIZE_Z)
            result = true;

        return result;
    }

    #endregion
}