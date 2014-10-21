//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UISpriteAnimations.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomUISpriteAnimation))]
public class CustomUISpriteAnimationInspector : Editor
{
	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
        base.OnInspectorGUI();

		GUILayout.Space(3f);
		NGUIEditorTools.SetLabelWidth(80f);
		serializedObject.Update();             

		NGUIEditorTools.DrawProperty("Loop", serializedObject, "mLoop");
        NGUIEditorTools.DrawProperty("Pixel Snap", serializedObject, "mSnap");
        NGUIEditorTools.DrawProperty("Keep Size", serializedObject, "_keepSize");

		serializedObject.ApplyModifiedProperties();
	}
}
