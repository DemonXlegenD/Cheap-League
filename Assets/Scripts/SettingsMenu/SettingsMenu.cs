using Nova;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public UIBlock Root = null;

    [Header("Temporary")]
    public BoolSetting BoolSetting = new BoolSetting();
    public ItemView ToggleItemView = null;
    public FloatSetting FloatSetting = new FloatSetting();
    public ItemView SliderItemView = null;

    private void Start()
    {
        // Visual Only
        Root.AddGestureHandler<Gesture.OnHover, ToggleVisuals>(ToggleVisuals.HandleHover);
        Root.AddGestureHandler<Gesture.OnUnhover, ToggleVisuals>(ToggleVisuals.HandleUnhover);
        Root.AddGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePress);
        Root.AddGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleRealease);

        // State changing
        Root.AddGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClicked);

        //Temporary
        BindToggle(BoolSetting, ToggleItemView.Visuals as ToggleVisuals);
    }

    private void HandleToggleClicked(Gesture.OnClick evt, ToggleVisuals target)
    {
        BoolSetting.State = !BoolSetting.State;
        target.IsChecked = BoolSetting.State;
    }

    private void BindSlider(FloatSetting floatSetting, SliderVisuals visuals)
    {
    }


    private void BindToggle(BoolSetting boolSetting, ToggleVisuals visuals)
    {
        visuals.Label.Text = boolSetting.Name;
        visuals.IsChecked = boolSetting.State;
    }
}
