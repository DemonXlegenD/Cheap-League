using Nova;
using NovaSamples.UIControls;
using System;
using UnityEngine;


    public class TabButtonVisuals : UIControlVisuals
    {
        public TextBlock Label = null;
        public UIBlock2D Background = null;
        public UIBlock2D SelectedIndicator = null;

        public Color DefaultColor1;
        public Color SelectedColor;

        public Color DefaultGradientColor;
        public Color HoveredGradientColor;
        public Color PressedGradientColor;

        public bool IsSelected
        {
            get => SelectedIndicator.gameObject.activeSelf;
            set
            {
                SelectedIndicator.gameObject.SetActive(value);
                Background.Color = value ? SelectedColor : DefaultColor1;
            }
        }

    internal static void HandleHover(Gesture.OnHover evt, TabButtonVisuals target, int index)
        {
            target.Background.Gradient.Color = target.HoveredGradientColor;
        }

        internal static void HandlePress(Gesture.OnPress evt, TabButtonVisuals target, int index)
        {
            target.Background.Gradient.Color = target.PressedGradientColor;
        }

        internal static void HandleRelease(Gesture.OnRelease evt, TabButtonVisuals target, int index)
        {
            target.Background.Gradient.Color = target.HoveredGradientColor;
        }

        internal static void HandleUnhover(Gesture.OnUnhover evt, TabButtonVisuals target, int index)
        {
            target.Background.Gradient.Color = target.DefaultGradientColor;
        }
    }

