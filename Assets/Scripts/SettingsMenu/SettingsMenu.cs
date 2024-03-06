using Nova;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NovaSamples.SettingsMenu
{
    /// <summary>
    /// The root-level UI controller responsible for displaying a list of <see cref="SettingsCollection"/>s, where each collection
    /// is represented by a tab button in a tab bar. When one of the tabs in the tab bar is selected, its corresponding 
    /// list of <see cref="SettingsCollection.Settings"/> will be used to populate another list with a set of UI controls.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {

        [SerializeField] private GameManager gameManager;

        [Header("Root")]
        [Tooltip("The root UIBlock for the entire settings menu.")]
        public UIBlock UIRoot = null;

        [Header("Tabs")]
        [Tooltip("The ListView to populate with a set of tab buttons.")]
        public ListView TabBar = null;
        [Tooltip("The datasource for the TabBar.")]
        public List<SettingsCollection> SettingsTabs = null;

        [Header("Controls")]
        [FormerlySerializedAs("ListView")]
        public ListView SettingsList = null;
        [Tooltip("The Scroller responsible for scrolling the settings ListView.")]
        [FormerlySerializedAs("Scroller")]
        public Scroller SettingsScroller = null;

        [Header("Navigation")]
        [Tooltip("The UIBlock of a \"Back\" button to simulate a navigable page. Just logs to the console in this example.")]
        public UIBlock BackButton = null;

        /// <summary>
        /// Le tab index courrant (au début à -1 puis modifié à 0 pour effectuer la première modification au lancement).
        /// </summary>
        [NonSerialized]
        private int selectedIndex = -1;
        /// <summary>
        /// Le dropdown.
        /// </summary>
        [NonSerialized, HideInInspector]
        private DropdownVisuals currentlyExpandedDropdown = null;

        private List<Setting> CurrentSettings => SettingsTabs[selectedIndex].Settings;

        private void Awake()
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
        private void OnEnable()
        {
            // Permet la gestion du scroll et du press dans l'uiblock 
            UIRoot.AddGestureHandler<Gesture.OnPress>(HandleSomethingPressed);
            UIRoot.AddGestureHandler<Gesture.OnScroll>(HandleSomethingScrolled);

            //Les modifications apportées aux données sont automatiquement reflétées dans l'interface utilisateur
            TabBar.AddDataBinder<SettingsCollection, TabButtonVisuals>(BindSettingsTab);

            // Gestion des événements sur la tab
            TabBar.AddGestureHandler<Gesture.OnClick, TabButtonVisuals>(HandleSettingsTabClicked);
            TabBar.AddGestureHandler<Gesture.OnHover, TabButtonVisuals>(TabButtonVisuals.HandleHover);
            TabBar.AddGestureHandler<Gesture.OnPress, TabButtonVisuals>(TabButtonVisuals.HandlePress);
            TabBar.AddGestureHandler<Gesture.OnRelease, TabButtonVisuals>(TabButtonVisuals.HandleRelease);
            TabBar.AddGestureHandler<Gesture.OnUnhover, TabButtonVisuals>(TabButtonVisuals.HandleUnhover);
            TabBar.AddGestureHandler<Gesture.OnCancel, TabButtonVisuals>(TabButtonVisuals.HandleCancel);

            //Les modifications apportées aux données sont automatiquement reflétées dans l'interface utilisateur
            SettingsList.AddDataBinder<FloatSetting, SliderVisuals>(BindSlider);
            SettingsList.AddDataBinder<BoolSetting, ToggleVisuals>(BindToggle);
            SettingsList.AddDataBinder<MultiOptionSetting, DropdownVisuals>(BindDropDown);
            SettingsList.AddDataUnbinder<MultiOptionSetting, DropdownVisuals>(UnbindDropDown);

            //Gestion des événements pour le dropdown
            SettingsList.AddGestureHandler<Gesture.OnClick, DropdownVisuals>(HandleDropdownClicked);
            SettingsList.AddGestureHandler<Gesture.OnCancel, DropdownVisuals>(DropdownVisuals.HandlePressCanceled);
            SettingsList.AddGestureHandler<Gesture.OnPress, DropdownVisuals>(DropdownVisuals.HandlePressed);
            SettingsList.AddGestureHandler<Gesture.OnRelease, DropdownVisuals>(DropdownVisuals.HandleReleased);

            //Gestion des événements pour la checkbox
            SettingsList.AddGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClicked);
            SettingsList.AddGestureHandler<Gesture.OnCancel, ToggleVisuals>(ToggleVisuals.HandlePressCanceled);
            SettingsList.AddGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePressed);
            SettingsList.AddGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleReleased);

            //Gestion des événements pour le slider
            SettingsList.AddGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);

            //Applique les données à la vue
            TabBar.SetDataSource(SettingsTabs);

            //On cherche le premier ItemView pour afficher les data
            if (TabBar.TryGetItemView(0, out ItemView tabView))
            {
                //Par défaut on affiche le premier item des settings (ici caméra)
                SelectTab(tabView.Visuals as TabButtonVisuals, 0);
            }

            //Gestion des événements pour le bouton retour (ce bouton renverra à la scène précédente => TODO: options devrait être un panel)
            BackButton.AddGestureHandler<Gesture.OnClick, ButtonVisuals>(HandleBackButtonClicked);
            BackButton.AddGestureHandler<Gesture.OnPress, ButtonVisuals>(ButtonVisuals.HandlePressed);
            BackButton.AddGestureHandler<Gesture.OnRelease, ButtonVisuals>(ButtonVisuals.HandleReleased);
            BackButton.AddGestureHandler<Gesture.OnCancel, ButtonVisuals>(ButtonVisuals.HandlePressCanceled);
        }

        private void OnDisable()
        {
            HandleFocusChanged(focusReceiver: null);

            //Retire les évenements de scroll et press
            UIRoot.RemoveGestureHandler<Gesture.OnPress>(HandleSomethingPressed);
            UIRoot.RemoveGestureHandler<Gesture.OnScroll>(HandleSomethingScrolled);

            //Retire la tab affichant les données
            TabBar.RemoveDataBinder<SettingsCollection, TabButtonVisuals>(BindSettingsTab);

            //Retire les événements de la tab
            TabBar.RemoveGestureHandler<Gesture.OnClick, TabButtonVisuals>(HandleSettingsTabClicked);
            TabBar.RemoveGestureHandler<Gesture.OnHover, TabButtonVisuals>(TabButtonVisuals.HandleHover);
            TabBar.RemoveGestureHandler<Gesture.OnPress, TabButtonVisuals>(TabButtonVisuals.HandlePress);
            TabBar.RemoveGestureHandler<Gesture.OnRelease, TabButtonVisuals>(TabButtonVisuals.HandleRelease);
            TabBar.RemoveGestureHandler<Gesture.OnUnhover, TabButtonVisuals>(TabButtonVisuals.HandleUnhover);
            TabBar.RemoveGestureHandler<Gesture.OnCancel, TabButtonVisuals>(TabButtonVisuals.HandleCancel);

            BackButton.RemoveGestureHandler<Gesture.OnClick, ButtonVisuals>(HandleBackButtonClicked);
            BackButton.RemoveGestureHandler<Gesture.OnPress, ButtonVisuals>(ButtonVisuals.HandlePressed);
            BackButton.RemoveGestureHandler<Gesture.OnRelease, ButtonVisuals>(ButtonVisuals.HandleReleased);
            BackButton.RemoveGestureHandler<Gesture.OnCancel, ButtonVisuals>(ButtonVisuals.HandlePressCanceled);

            //Retire les événements de chaque type de controles
            SettingsList.RemoveDataBinder<FloatSetting, SliderVisuals>(BindSlider);
            SettingsList.RemoveDataBinder<BoolSetting, ToggleVisuals>(BindToggle);
            SettingsList.RemoveDataBinder<MultiOptionSetting, DropdownVisuals>(BindDropDown);
            SettingsList.RemoveDataUnbinder<MultiOptionSetting, DropdownVisuals>(UnbindDropDown);

            //Retire les événements du dropdown
            SettingsList.RemoveGestureHandler<Gesture.OnClick, DropdownVisuals>(HandleDropdownClicked);
            SettingsList.RemoveGestureHandler<Gesture.OnCancel, DropdownVisuals>(DropdownVisuals.HandlePressCanceled);
            SettingsList.RemoveGestureHandler<Gesture.OnPress, DropdownVisuals>(DropdownVisuals.HandlePressed);
            SettingsList.RemoveGestureHandler<Gesture.OnRelease, DropdownVisuals>(DropdownVisuals.HandleReleased);

            //Retire les événements de la checkbow
            SettingsList.RemoveGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClicked);
            SettingsList.RemoveGestureHandler<Gesture.OnCancel, ToggleVisuals>(ToggleVisuals.HandlePressCanceled);
            SettingsList.RemoveGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePressed);
            SettingsList.RemoveGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleReleased);

            //Retire les événements du slider
            SettingsList.RemoveGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);
        }

        /// <summary>
        /// Gère tous les événements de pression pour suivre les changements de "focus". Les événements de pression couvrent tous les événements de "pointer down".
        /// </summary>
        /// <param name="evt">Les données associées à l'événement de pression.</param>
        private void HandleSomethingPressed(Gesture.OnPress evt)
        {
            // Indique au ControlsPanel de gérer le fait qu'un nouvel élément est pressé
            HandleFocusChanged(evt.Receiver);
        }

        /// <summary>
        /// Gère tous les événements de défilement pour suivre les changements de "focus". Les événements de défilement gèrent tous les événements de molette de la souris (et les événements de défilement du pointeur).
        /// </summary>
        /// <param name="evt">Les données associées à l'événement de défilement.</param>
        private void HandleSomethingScrolled(Gesture.OnScroll evt)
        {
            if (evt.ScrollType == ScrollType.Inertial)
            {
                // Pas un événement de défilement manuel, donc nous pouvons l'ignorer.
                return;
            }

            // Indique au ControlsPanel de gérer le fait qu'un nouvel élément est défilé
            HandleFocusChanged(evt.Receiver);
        }

        /// <summary>
        /// Indique à ce panneau de contrôle de gérer un événement de changement de focus, qui peut impliquer
        /// de retirer son concept de "focus" de l'objet "focus" actuellement.
        /// 
        /// Par exemple,
        /// Cela va réduire tout menu déroulant étendu si <paramref name="focusReceiver"/>
        /// est en dehors de la hiérarchie enfant du menu déroulant étendu.
        /// </summary>
        /// <param name="focusReceiver">Le nouvel objet "focus". Peut être null.</param>
        public void HandleFocusChanged(UIBlock focusReceiver)
        {
            if (currentlyExpandedDropdown == null)
            {
                // Ne suit pas actuellement un menu déroulant en focus,
                // donc nous n'avons rien à faire ici.
                return;
            }

            // null peut être fourni si rien de nouveau n'est "focusé"
            if (focusReceiver != null)
            {
                if (focusReceiver.transform.IsChildOf(currentlyExpandedDropdown.View.transform))
                {
                    // Le nouvel objet en focus est un enfant (inclusif) du
                    // menu déroulant actuellement étendu, donc nous voulons laisser
                    // le menu déroulant étendu.
                    return;
                }
            }

            // Quelque chose en dehors de la hiérarchie du menu déroulant actuellement étendu
            // a été "focusé", donc réduisez le menu déroulant et effacez-le.
            currentlyExpandedDropdown.Collapse();
            currentlyExpandedDropdown = null;
        }

        /// <summary>
        /// Lie le visuel <paramref name="button"/> à son objet de données correspondant.
        /// </summary>
        /// <param name="evt">Les données d'événement de liaison.</param>
        /// <param name="button">L'objet <see cref="TabButtonVisuals"/> représentant les données à lier à la vue.</param>
        /// <param name="index">L'index dans <see cref="SettingsTabs"/> de l'objet de données à lier à la vue.</param>
        private void BindSettingsTab(Data.OnBind<SettingsCollection> evt, TabButtonVisuals button, int index)
        {
            // Les UserData sur cet événement de liaison sont la même valeur stockée
            // à l'index donné `index` dans la liste de SettingsTabs.
            //
            // Autrement dit,
            // evt.UserData == SettingsTabs[index]
            SettingsCollection settings = evt.UserData;

            // Met à jour le texte du label pour refléter la catégorie de paramètres
            // que le bouton représente
            button.Label.Text = settings.Category;
        }


        /// <summary>
        /// Lorsqu'un onglet est cliqué, met à jour le <see cref="ControlsPanel"/> pour afficher la liste des paramètres associés à la catégorie d'onglet sélectionnée.
        /// </summary>
        /// <param name="evt">L'événement de pression.</param>
        /// <param name="button">L'objet <see cref="TabButtonVisuals"/> qui a été cliqué.</param>
        /// <param name="index">L'index dans <see cref="SettingsTabs"/> de l'objet représenté par <paramref name="button"/>.</param>
        private void HandleSettingsTabClicked(Gesture.OnClick evt, TabButtonVisuals button, int index)
        {
            SelectTab(button, index);
        }

        /// <summary>
        /// Définit l'onglet actuellement sélectionné, et remplit <see cref="ControlsPanel"/> avec une liste de contrôles UI pour configurer les paramètres utilisateur sous-jacents.
        /// </summary>
        /// <param name="button">L'objet visuel de l'onglet sélectionné.</param>
        /// <param name="index">L'index dans <see cref="SettingsTabs"/> de l'objet représenté par <paramref name="button"/>.</param>
        private void SelectTab(TabButtonVisuals button, int index)
        {
            if (index == selectedIndex)
            {
                // Tente de sélectionner l'index d'onglet déjà sélectionné.
                // Pas besoin de changer quoi que ce soit.
                return;
            }

            // Si l'onglet précédemment sélectionné était valide (ce qui peut ne pas être le cas lors de la première initialisation).
            if (selectedIndex >= 0)
            {
                if (TabBar.TryGetItemView(selectedIndex, out ItemView selectedTab))
                {
                    // Met à jour les visuels de l'onglet précédemment sélectionné pour indiquer
                    // qu'il n'est plus sélectionné.
                    TabButtonVisuals selected = selectedTab.Visuals as TabButtonVisuals;
                    selected.IsSelected = false;
                }
            }

            // Met à jour notre index de suivi selectedIndex
            selectedIndex = index;

            // Met à jour les visuels du nouvel onglet sélectionné pour indiquer qu'il est maintenant sélectionné.
            button.IsSelected = true;

            // Remplit le ControlsPanel avec la liste des paramètres
            // soutenant la catégorie d'onglets sélectionnée.
            SettingsScroller.CancelScroll();
            SettingsList.SetDataSource(SettingsTabs[index].Settings);
        }

        /// <summary>
        /// Lie un objet de données <see cref="FloatSetting"/> à un contrôle UI de type <see cref="SliderVisuals"/>.
        /// </summary>
        /// <param name="evt">Les données d'événement de liaison.</param>
        /// <param name="sliderControl">L'objet interactif <see cref="ItemVisuals"/> pour afficher les données du curseur pertinent.</param>
        /// <param name="index">L'index dans <see cref="DataSource"/> du <see cref="FloatSetting"/> à lier à la vue.</param>
        private void BindSlider(Data.OnBind<FloatSetting> evt, SliderVisuals sliderControl, int index)
        {
            // Les UserData sur cet événement de liaison sont la même valeur stockée
            // à l'index donné `index` dans la DataSource.
            //
            // Autrement dit,
            // evt.UserData == DataSource[index]
            FloatSetting slider = evt.UserData;

            // Met à jour le texte du label du contrôle
            sliderControl.Label.Text = slider.Label;

            // Met à jour la barre de remplissage pour refléter la valeur du curseur entre sa plage min/max
            sliderControl.FillBar.Size.X.Percent = (slider.Value - slider.Min) / (slider.Max - slider.Min);

            // Met à jour les unités affichées
            sliderControl.Units.Text = slider.ValueLabel;
        }

        /// <summary>
        /// Lie un objet de données <see cref="BoolSetting"/> à un contrôle UI de type <see cref="ToggleVisuals"/>.
        /// </summary>
        /// <param name="evt">Les données d'événement de liaison.</param>
        /// <param name="toggleControl">L'objet interactif <see cref="ItemVisuals"/> pour afficher les données de bascule pertinentes.</param>
        /// <param name="index">L'index dans <see cref="DataSource"/> du <see cref="BoolSetting"/> à lier à la vue.</param>
        private void BindToggle(Data.OnBind<BoolSetting> evt, ToggleVisuals toggleControl, int index)
        {
            // Les UserData sur cet événement de liaison sont la même valeur stockée
            // à l'index donné `index` dans la DataSource.
            //
            // Autrement dit,
            // evt.UserData == DataSource[index]
            BoolSetting toggle = evt.UserData;

            // Met à jour le texte du label du contrôle
            toggleControl.Label.Text = toggle.Label;

            // Met à jour l'indicateur de bascule du contrôle
            toggleControl.IsOnIndicator.gameObject.SetActive(toggle.IsOn);
        }

        /// <summary>
        /// Lie un objet de données <see cref="MultiOptionSetting"/> à un contrôle UI de type <see cref="DropdownVisuals"/>.
        /// </summary>
        /// <param name="evt">Les données d'événement de liaison.</param>
        /// <param name="dropdownControl">L'objet interactif <see cref="ItemVisuals"/> pour afficher les données de menu déroulant pertinentes.</param>
        /// <param name="index">L'index dans <see cref="DataSource"/> du <see cref="MultiOptionSetting"/> à lier à la vue.</param>
        private void BindDropDown(Data.OnBind<MultiOptionSetting> evt, DropdownVisuals dropdownControl, int index)
        {
            // Assure que le contrôle démarre dans un état réduit
            // lorsqu'il est nouvellement lié à la vue.
            dropdownControl.Collapse();

            // Les UserData sur cet événement de liaison sont la même valeur stockée
            // à l'index donné `index` dans la DataSource.
            //
            // Autrement dit,
            // evt.UserData == DataSource[index]
            MultiOptionSetting dropdown = evt.UserData;

            // Met à jour le texte du label du contrôle
            dropdownControl.DropdownLabel.Text = dropdown.Label;

            // Met à jour le champ de sélection pour indiquer l'option actuellement sélectionnée dans le menu déroulant
            dropdownControl.SelectionLabel.Text = dropdown.SelectionName;
        }

        /// <summary>
        /// Détache un objet de données <see cref="MultiOptionSetting"/> d'un contrôle UI de type <see cref="DropdownVisuals"/>.
        /// </summary>
        /// <param name="evt">Les données d'événement de détachement.</param>
        /// <param name="dropdownControl">L'objet interactif <see cref="ItemVisuals"/> affichant les données de menu déroulant pertinentes.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="MultiOptionSetting"/> qui est détaché de la vue.</param>
        private void UnbindDropDown(Data.OnUnbind<MultiOptionSetting> evt, DropdownVisuals dropdownControl, int index)
        {
            if (dropdownControl == currentlyExpandedDropdown)
            {
                // si ce menu déroulant est suivi en tant qu'objet actuellement étendu
                // effacer ce champ pour indiquer qu'aucun menu déroulant n'est actuellement étendu
                currentlyExpandedDropdown = null;
            }

            // Assure que le contrôle de menu déroulant est réduit
            dropdownControl.Collapse();
        }

        /// <summary>
        /// Gère un objet <see cref="DropdownVisuals"/> dans la <see cref="SettingsList"/> étant cliqué, et l'étend ou le réduit en conséquence.
        /// </summary>
        /// <param name="evt">Les données d'événement de clic.</param>
        /// <param name="dropdownControl">L'objet <see cref="ItemVisuals"/> qui a été cliqué.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="MultiOptionSetting"/> représenté par <paramref name="dropdownControl"/>.</param>
        private void HandleDropdownClicked(Gesture.OnClick evt, DropdownVisuals dropdownControl, int index)
        {
            if (evt.Receiver.transform.IsChildOf(dropdownControl.OptionsView.transform))
            {
                // L'objet cliqué n'était pas le menu déroulant lui-même mais plutôt un élément de liste à l'intérieur du menu déroulant.
                // Le dropdownControl lui-même gérera cet événement, donc nous n'avons rien à faire ici.
                return;
            }

            if (dropdownControl.IsExpanded)
            {
                // Réduit le menu déroulant et arrête de le suivre
                // en tant qu'objet étendu en focus.
                dropdownControl.Collapse();

                // Comme cet événement sera toujours appelé chaque fois qu'un menu déroulant est
                // cliqué, nous pouvons sûrement supposer que currentlyExpandedDropdown == dropdownControl,
                // puisque nous l'assignons ci-dessous et nulle part ailleurs.
                currentlyExpandedDropdown = null;
            }
            else
            {
                // Obtient la source de données sous-jacente du menu déroulant
                // pour pouvoir étendre le dropdownControl avec
                // son ensemble d'options sélectionnables.
                MultiOptionSetting dropdown = CurrentSettings[index] as MultiOptionSetting;

                // Indique au menu déroulant de s'étendre, montrant une liste de
                // options sélectionnables.
                dropdownControl.Expand(dropdown);

                // Commence à suivre ce dropdownControl comme le
                // dropdown actuellement étendu.
                currentlyExpandedDropdown = dropdownControl;
            }
        }

        /// <summary>
        /// Gère un objet <see cref="ToggleVisuals"/> dans la <see cref="SettingsList"/> étant cliqué, et bascule son état visuel et son état de données en conséquence.
        /// </summary>
        /// <param name="evt">Les données d'événement de clic.</param>
        /// <param name="toggleControl">L'objet <see cref="ItemVisuals"/> qui a été cliqué.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="BoolSetting"/> représenté par <paramref name="toggleControl"/>.</param>
        private void HandleToggleClicked(Gesture.OnClick evt, ToggleVisuals toggleControl, int index)
        {
            // Obtient l'objet de données de bascule sous-jacent représenté par toggleControl
            BoolSetting toggle = CurrentSettings[index] as BoolSetting;

            // Bascule l'état de l'objet de données
            toggle.IsOn = !toggle.IsOn;

            // Met à jour l'état visuel de la bascule pour refléter le nouveau état IsOn
            toggleControl.IsOnIndicator.gameObject.SetActive(toggle.IsOn);
        }

        /// <summary>
        /// Gère un objet <see cref="SliderVisuals"/> dans la <see cref="SettingsList"/> étant glissé, et ajuste son état visuel et son état de données en conséquence.
        /// </summary>
        /// <param name="evt">Les données d'événement de glissement.</param>
        /// <param name="sliderControl">L'objet <see cref="ItemVisuals"/> qui a été glissé.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="FloatSetting"/> représenté par <paramref name="sliderControl"/>.</param>
        private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals sliderControl, int index)
        {
            // Obtient l'objet de données de curseur sous-jacent représenté par sliderControl
            FloatSetting slider = CurrentSettings[index] as FloatSetting;

            // Convertit la position de glissement actuelle en espace local du curseur
            Vector3 pointerLocalPosition = sliderControl.SliderBounds.transform.InverseTransformPoint(evt.PointerPositions.Current);

            // Obtient la distance du curseur depuis le bord gauche du contrôle
            float sliderPositionFromLeft = pointerLocalPosition.x + 0.5f * sliderControl.SliderBounds.CalculatedSize.X.Value;

            // Convertit la distance depuis le bord gauche en un pourcentage depuis le bord gauche
            float sliderPercent = Mathf.Clamp01(sliderPositionFromLeft / sliderControl.SliderBounds.CalculatedSize.X.Value);

            // Met à jour le contrôle de curseur à la nouvelle valeur dans sa plage min/max
            slider.Value = Mathf.Lerp(slider.Min, slider.Max, sliderPercent);

            // Met à jour la barre de remplissage du contrôle pour refléter sa nouvelle valeur de curseur
            sliderControl.FillBar.Size.X.Percent = sliderPercent;

            // Met à jour le texte d'unité du contrôle pour afficher la valeur numérique
            // associée au contrôle de curseur
            sliderControl.Units.Text = slider.ValueLabel;
        }

        /// <summary>
        /// Enregistre dans la console chaque fois que le bouton de retour est cliqué. Présent ici comme un exemple dans cet exemple.
        /// </summary>
        /// <param name="evt">Les données d'événement de clic.</param>
        /// <param name="button">Le bouton recevant l'événement.</param>
        private void HandleBackButtonClicked(Gesture.OnClick evt, ButtonVisuals button)
        {
            gameManager.LoadPreviousScene();
        }
    }
}
