using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Setting
{
    public string Name;
}

[System.Serializable]
public class BoolSetting : Setting
{
    public bool State;
}

[System.Serializable]
public class FloatSetting : Setting
{
    [SerializeField] private float value;
    [SerializeField] private float Min;
    [SerializeField] private float Max;
    [SerializeField] private string ValueFormat = "{0:0.0}";

    public float Value
    {
        get => Mathf.Clamp(value, Min, Max);
        set => this.value = Mathf.Clamp(value, Min, Max);
    }

    public string DisplayValue => string.Format(ValueFormat, Value);    
}