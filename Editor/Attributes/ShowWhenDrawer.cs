using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GiveUsComponents {

	[CustomPropertyDrawer(typeof(ShowWhenAttribute))]
	public class ShowWhenDrawer : PropertyDrawer {

	    public override void OnGUI(
	    	Rect position, SerializedProperty property, GUIContent label
	    ) {
	        var field = attribute as ShowWhenAttribute;
	        var path = property.propertyPath;
	        var obj = property.serializedObject;
	        var pos = path.LastIndexOf(".");
	        var newPath = path.Substring(0, pos + 1) + field.name;
	        var master = obj.FindProperty(newPath);
	        if (master == null) return;
	        if (field.flags && (master.ulongValue & field.value) > 0
	        || field.flags && !field.bools  && master.ulongValue == field.value
	        || field.bools && master.boolValue == (field.value > 0)) {
	        	EditorGUI.PropertyField(position, property, true);
	        }
	    }

	    // public override float GetPropertyHeight(
	    // 	SerializedProperty property, GUIContent label
	    // ) => -10f;

	    public override float GetPropertyHeight(
	    	SerializedProperty property, GUIContent label
	    ) {
	        var field = attribute as ShowWhenAttribute;
	        var path = property.propertyPath;
	        var obj = property.serializedObject;
	        var pos = path.LastIndexOf(".");
	        var newPath = path.Substring(0, pos + 1) + field.name;
	        var master = obj.FindProperty(newPath);
	        if (master == null) 
	        	return -EditorGUIUtility.standardVerticalSpacing;

	        if (field.flags && (master.ulongValue & field.value) > 0
	        || field.flags && !field.bools  && master.ulongValue == field.value
	        || field.bools && master.boolValue == (field.value > 0))
	        	return EditorGUI.GetPropertyHeight(property);
	        else return -EditorGUIUtility.standardVerticalSpacing;
	    }
	}

}