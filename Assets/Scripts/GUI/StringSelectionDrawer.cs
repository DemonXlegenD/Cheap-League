using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StringSelectionAttribute))]
public class StringSelectionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            StringSelectionAttribute stringSelection = attribute as StringSelectionAttribute;
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(stringSelection.options, property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, stringSelection.options);
            property.stringValue = stringSelection.options[selectedIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}