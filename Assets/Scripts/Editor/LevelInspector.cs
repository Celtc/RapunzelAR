using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Level))]
public class LevelInspector : Editor
{
	#region Variables (private)

    private bool _foldoutGrid;
    private ExpandableMatrix<bool> _foldoutBools = new ExpandableMatrix<bool>();

	#endregion

	#region Funciones de evento de Unity

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        EditorGUILayout.Space();
        _foldoutGrid = EditorGUILayout.Foldout(_foldoutGrid, "Grid");

        var grid = (target as Level).Grid;
        if (_foldoutGrid && grid != null)
        {
            var lengthX = grid.GetLength(0);
            var lengthY = grid.GetLength(1);
            var lengthZ = grid.GetLength(2);
            for (int y = (int)-lengthY.x; y < lengthY.y; y++)
            {
                EditorGUI.indentLevel = 1;
                _foldoutBools[0, y, 0] = EditorGUILayout.Foldout(_foldoutBools[0, y, 0], "Y " + y.ToString());
                if (_foldoutBools[0, y, 0])
                {
                    for (int z = (int)-lengthZ.x; z < lengthZ.y; z++)
                    {
                        EditorGUI.indentLevel = 2;
                        _foldoutBools[1, y, z] = EditorGUILayout.Foldout(_foldoutBools[1, y, z], "Z " + z.ToString());
                        if (_foldoutBools[1, y, z])
                        {
                            for (int x = (int)-lengthX.x; x < lengthX.y; x++)
                            {
                                EditorGUI.indentLevel = 3;
                                EditorGUILayout.ObjectField("X " + x.ToString(), grid[x, y, z], typeof(Block), true);
                            }
                        }
                    }
                }
            }
        }
    } 

	#endregion

	#region Metodos

	#endregion
}
