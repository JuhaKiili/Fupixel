using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Fupixel))]
public class FupixelEditor : Editor 
{
	Fupixel fupixel { get { return target as Fupixel; } }

	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		if(GUI.changed)
			fupixel.ApplySettings();
	}
}
