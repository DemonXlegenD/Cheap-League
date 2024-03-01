using Nova;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace NovaSamples.UIControls
{
    public class TabButton : UIControl<TabButtonVisuals>
    {
        [SerializeField] private string label;
        [SerializeField] private List<SettingsCollection> SettingsCollections = null;
        [SerializeField] private ListView TabButtons = null;
        [SerializeField] private int selectedIndex = 0;

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public void Start()
        {
        }

        private void OnEnable()
        {
            TabButtons.AddDataBinder<SettingsCollection, TabButtonVisuals>(BindTab);
            TabButtons.AddGestureHandler<Gesture.OnHover, TabButtonVisuals>(TabButtonVisuals.HandleHover);
            TabButtons.AddGestureHandler<Gesture.OnPress, TabButtonVisuals>(TabButtonVisuals.HandlePress);
            TabButtons.AddGestureHandler<Gesture.OnRelease, TabButtonVisuals>(TabButtonVisuals.HandleRelease);
            TabButtons.AddGestureHandler<Gesture.OnUnhover, TabButtonVisuals>(TabButtonVisuals.HandleUnhover);
            TabButtons.AddGestureHandler<Gesture.OnClick, TabButtonVisuals>(HandleTabClicked);

            TabButtons.SetDataSource<SettingsCollection>(SettingsCollections);

            if(TabButtons.TryGetItemView(0, out ItemView firsTab))
            {
                SelectTab(firsTab.Visuals as TabButtonVisuals, 0);
            }
        }


        private void HandleTabClicked(Gesture.OnClick evt, TabButtonVisuals target, int index)
        {
            SelectTab(target, index);
        }

        private void SelectTab(TabButtonVisuals visuals, int index)
        {
            if(index == selectedIndex)
            {
                return;
            }
            if(selectedIndex >= 0 && TabButtons.TryGetItemView(selectedIndex, out ItemView currentItemView))
            {
                (currentItemView.Visuals as TabButtonVisuals).IsSelected = false;
            }

            selectedIndex = index;
            visuals.IsSelected = true;  
        }


        private void BindTab(Data.OnBind<SettingsCollection> evt, TabButtonVisuals target, int index)
        {
            target.Label.Text = evt.UserData.Category;
            target.IsSelected = false;
        }

        private void OnDisable()
        {

        }

    }

}
