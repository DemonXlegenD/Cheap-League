using UnityEngine;

public class StringSelectionAttribute : PropertyAttribute
{
    public string[] options;

    public StringSelectionAttribute(params string[] options)
    {
        this.options = options;
    }
}