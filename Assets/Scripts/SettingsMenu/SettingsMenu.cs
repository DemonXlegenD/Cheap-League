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
        /// Le tab index courrant (au d�but � -1 puis modifi� � 0 pour effectuer la premi�re modification au lancement).
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

            //Les modifications apport�es aux donn�es sont automatiquement refl�t�es dans l'interface utilisateur
            TabBar.AddDataBinder<SettingsCollection, TabButtonVisuals>(BindSettingsTab);

            // Gestion des �v�nements sur la tab
            TabBar.AddGestureHandler<Gesture.OnClick, TabButtonVisuals>(HandleSettingsTabClicked);
            TabBar.AddGestureHandler<Gesture.OnHover, TabButtonVisuals>(TabButtonVisuals.HandleHover);
            TabBar.AddGestureHandler<Gesture.OnPress, TabButtonVisuals>(TabButtonVisuals.HandlePress);
            TabBar.AddGestureHandler<Gesture.OnRelease, TabButtonVisuals>(TabButtonVisuals.HandleRelease);
            TabBar.AddGestureHandler<Gesture.OnUnhover, TabButtonVisuals>(TabButtonVisuals.HandleUnhover);
            TabBar.AddGestureHandler<Gesture.OnCancel, TabButtonVisuals>(TabButtonVisuals.HandleCancel);

            //Les modifications apport�es aux donn�es sont automatiquement refl�t�es dans l'interface utilisateur
            SettingsList.AddDataBinder<FloatSetting, SliderVisuals>(BindSlider);
            SettingsList.AddDataBinder<BoolSetting, ToggleVisuals>(BindToggle);
            SettingsList.AddDataBinder<MultiOptionSetting, DropdownVisuals>(BindDropDown);
            SettingsList.AddDataUnbinder<MultiOptionSetting, DropdownVisuals>(UnbindDropDown);

            //Gestion des �v�nements pour le dropdown
            SettingsList.AddGestureHandler<Gesture.OnClick, DropdownVisuals>(HandleDropdownClicked);
            SettingsList.AddGestureHandler<Gesture.OnCancel, DropdownVisuals>(DropdownVisuals.HandlePressCanceled);
            SettingsList.AddGestureHandler<Gesture.OnPress, DropdownVisuals>(DropdownVisuals.HandlePressed);
            SettingsList.AddGestureHandler<Gesture.OnRelease, DropdownVisuals>(DropdownVisuals.HandleReleased);

            //Gestion des �v�nements pour la checkbox
            SettingsList.AddGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClicked);
            SettingsList.AddGestureHandler<Gesture.OnCancel, ToggleVisuals>(ToggleVisuals.HandlePressCanceled);
            SettingsList.AddGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePressed);
            SettingsList.AddGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleReleased);

            //Gestion des �v�nements pour le slider
            SettingsList.AddGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);

            //Applique les donn�es � la vue
            TabBar.SetDataSource(SettingsTabs);

            //On cherche le premier ItemView pour afficher les data
            if (TabBar.TryGetItemView(0, out ItemView tabView))
            {
                //Par d�faut on affiche le premier item des settings (ici cam�ra)
                SelectTab(tabView.Visuals as TabButtonVisuals, 0);
            }

            //Gestion des �v�nements pour le bouton retour (ce bouton renverra � la sc�ne pr�c�dente => TODO: options devrait �tre un panel)
            BackButton.AddGestureHandler<Gesture.OnClick, ButtonVisuals>(HandleBackButtonClicked);
            BackButton.AddGestureHandler<Gesture.OnPress, ButtonVisuals>(ButtonVisuals.HandlePressed);
            BackButton.AddGestureHandler<Gesture.OnRelease, ButtonVisuals>(ButtonVisuals.HandleReleased);
            BackButton.AddGestureHandler<Gesture.OnCancel, ButtonVisuals>(ButtonVisuals.HandlePressCanceled);
        }

        private void OnDisable()
        {
            HandleFocusChanged(focusReceiver: null);

            //Retire les �venements de scroll et press
            UIRoot.RemoveGestureHandler<Gesture.OnPress>(HandleSomethingPressed);
            UIRoot.RemoveGestureHandler<Gesture.OnScroll>(HandleSomethingScrolled);

            //Retire la tab affichant les donn�es
            TabBar.RemoveDataBinder<SettingsCollection, TabButtonVisuals>(BindSettingsTab);

            //Retire les �v�nements de la tab
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

            //Retire les �v�nements de chaque type de controles
            SettingsList.RemoveDataBinder<FloatSetting, SliderVisuals>(BindSlider);
            SettingsList.RemoveDataBinder<BoolSetting, ToggleVisuals>(BindToggle);
            SettingsList.RemoveDataBinder<MultiOptionSetting, DropdownVisuals>(BindDropDown);
            SettingsList.RemoveDataUnbinder<MultiOptionSetting, DropdownVisuals>(UnbindDropDown);

            //Retire les �v�nements du dropdown
            SettingsList.RemoveGestureHandler<Gesture.OnClick, DropdownVisuals>(HandleDropdownClicked);
            SettingsList.RemoveGestureHandler<Gesture.OnCancel, DropdownVisuals>(DropdownVisuals.HandlePressCanceled);
            SettingsList.RemoveGestureHandler<Gesture.OnPress, DropdownVisuals>(DropdownVisuals.HandlePressed);
            SettingsList.RemoveGestureHandler<Gesture.OnRelease, DropdownVisuals>(DropdownVisuals.HandleReleased);

            //Retire les �v�nements de la checkbow
            SettingsList.RemoveGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClicked);
            SettingsList.RemoveGestureHandler<Gesture.OnCancel, ToggleVisuals>(ToggleVisuals.HandlePressCanceled);
            SettingsList.RemoveGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePressed);
            SettingsList.RemoveGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleReleased);

            //Retire les �v�nements du slider
            SettingsList.RemoveGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);
        }

        /// <summary>
        /// G�re tous les �v�nements de pression pour suivre les changements de "focus". Les �v�nements de pression couvrent tous les �v�nements de "pointer down".
        /// </summary>
        /// <param name="evt">Les donn�es associ�es � l'�v�nement de pression.</param>
        private void HandleSomethingPressed(Gesture.OnPress evt)
        {
            // Indique au ControlsPanel de g�rer le fait qu'un nouvel �l�ment est press�
            HandleFocusChanged(evt.Receiver);
        }

        /// <summary>
        /// G�re tous les �v�nements de d�filement pour suivre les changements de "focus". Les �v�nements de d�filement g�rent tous les �v�nements de molette de la souris (et les �v�nements de d�filement du pointeur).
        /// </summary>
        /// <param name="evt">Les donn�es associ�es � l'�v�nement de d�filement.</param>
        private void HandleSomethingScrolled(Gesture.OnScroll evt)
        {
            if (evt.ScrollType == ScrollType.Inertial)
            {
                // Pas un �v�nement de d�filement manuel, donc nous pouvons l'ignorer.
                return;
            }

            // Indique au ControlsPanel de g�rer le fait qu'un nouvel �l�ment est d�fil�
            HandleFocusChanged(evt.Receiver);
        }

        /// <summary>
        /// Indique � ce panneau de contr�le de g�rer un �v�nement de changement de focus, qui peut impliquer
        /// de retirer son concept de "focus" de l'objet "focus" actuellement.
        /// 
        /// Par exemple,
        /// Cela va r�duire tout menu d�roulant �tendu si <paramref name="focusReceiver"/>
        /// est en dehors de la hi�rarchie enfant du menu d�roulant �tendu.
        /// </summary>
        /// <param name="focusReceiver">Le nouvel objet "focus". Peut �tre null.</param>
        public void HandleFocusChanged(UIBlock focusReceiver)
        {
            if (currentlyExpandedDropdown == null)
            {
                // Ne suit pas actuellement un menu d�roulant en focus,
                // donc nous n'avons rien � faire ici.
                return;
            }

            // null peut �tre fourni si rien de nouveau n'est "focus�"
            if (focusReceiver != null)
            {
                if (focusReceiver.transform.IsChildOf(currentlyExpandedDropdown.View.transform))
                {
                    // Le nouvel objet en focus est un enfant (inclusif) du
                    // menu d�roulant actuellement �tendu, donc nous voulons laisser
                    // le menu d�roulant �tendu.
                    return;
                }
            }

            // Quelque chose en dehors de la hi�rarchie du menu d�roulant actuellement �tendu
            // a �t� "focus�", donc r�duisez le menu d�roulant et effacez-le.
            currentlyExpandedDropdown.Collapse();
            currentlyExpandedDropdown = null;
        }

        /// <summary>
        /// Lie le visuel <paramref name="button"/> � son objet de donn�es correspondant.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de liaison.</param>
        /// <param name="button">L'objet <see cref="TabButtonVisuals"/> repr�sentant les donn�es � lier � la vue.</param>
        /// <param name="index">L'index dans <see cref="SettingsTabs"/> de l'objet de donn�es � lier � la vue.</param>
        private void BindSettingsTab(Data.OnBind<SettingsCollection> evt, TabButtonVisuals button, int index)
        {
            // Les UserData sur cet �v�nement de liaison sont la m�me valeur stock�e
            // � l'index donn� `index` dans la liste de SettingsTabs.
            //
            // Autrement dit,
            // evt.UserData == SettingsTabs[index]
            SettingsCollection settings = evt.UserData;

            // Met � jour le texte du label pour refl�ter la cat�gorie de param�tres
            // que le bouton repr�sente
            button.Label.Text = settings.Category;
        }


        /// <summary>
        /// Lorsqu'un onglet est cliqu�, met � jour le <see cref="ControlsPanel"/> pour afficher la liste des param�tres associ�s � la cat�gorie d'onglet s�lectionn�e.
        /// </summary>
        /// <param name="evt">L'�v�nement de pression.</param>
        /// <param name="button">L'objet <see cref="TabButtonVisuals"/> qui a �t� cliqu�.</param>
        /// <param name="index">L'index dans <see cref="SettingsTabs"/> de l'objet repr�sent� par <paramref name="button"/>.</param>
        private void HandleSettingsTabClicked(Gesture.OnClick evt, TabButtonVisuals button, int index)
        {
            SelectTab(button, index);
        }

        /// <summary>
        /// D�finit l'onglet actuellement s�lectionn�, et remplit <see cref="ControlsPanel"/> avec une liste de contr�les UI pour configurer les param�tres utilisateur sous-jacents.
        /// </summary>
        /// <param name="button">L'objet visuel de l'onglet s�lectionn�.</param>
        /// <param name="index">L'index dans <see cref="SettingsTabs"/> de l'objet repr�sent� par <paramref name="button"/>.</param>
        private void SelectTab(TabButtonVisuals button, int index)
        {
            if (index == selectedIndex)
            {
                // Tente de s�lectionner l'index d'onglet d�j� s�lectionn�.
                // Pas besoin de changer quoi que ce soit.
                return;
            }

            // Si l'onglet pr�c�demment s�lectionn� �tait valide (ce qui peut ne pas �tre le cas lors de la premi�re initialisation).
            if (selectedIndex >= 0)
            {
                if (TabBar.TryGetItemView(selectedIndex, out ItemView selectedTab))
                {
                    // Met � jour les visuels de l'onglet pr�c�demment s�lectionn� pour indiquer
                    // qu'il n'est plus s�lectionn�.
                    TabButtonVisuals selected = selectedTab.Visuals as TabButtonVisuals;
                    selected.IsSelected = false;
                }
            }

            // Met � jour notre index de suivi selectedIndex
            selectedIndex = index;

            // Met � jour les visuels du nouvel onglet s�lectionn� pour indiquer qu'il est maintenant s�lectionn�.
            button.IsSelected = true;

            // Remplit le ControlsPanel avec la liste des param�tres
            // soutenant la cat�gorie d'onglets s�lectionn�e.
            SettingsScroller.CancelScroll();
            SettingsList.SetDataSource(SettingsTabs[index].Settings);
        }

        /// <summary>
        /// Lie un objet de donn�es <see cref="FloatSetting"/> � un contr�le UI de type <see cref="SliderVisuals"/>.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de liaison.</param>
        /// <param name="sliderControl">L'objet interactif <see cref="ItemVisuals"/> pour afficher les donn�es du curseur pertinent.</param>
        /// <param name="index">L'index dans <see cref="DataSource"/> du <see cref="FloatSetting"/> � lier � la vue.</param>
        private void BindSlider(Data.OnBind<FloatSetting> evt, SliderVisuals sliderControl, int index)
        {
            // Les UserData sur cet �v�nement de liaison sont la m�me valeur stock�e
            // � l'index donn� `index` dans la DataSource.
            //
            // Autrement dit,
            // evt.UserData == DataSource[index]
            FloatSetting slider = evt.UserData;

            // Met � jour le texte du label du contr�le
            sliderControl.Label.Text = slider.Label;

            // Met � jour la barre de remplissage pour refl�ter la valeur du curseur entre sa plage min/max
            sliderControl.FillBar.Size.X.Percent = (slider.Value - slider.Min) / (slider.Max - slider.Min);

            // Met � jour les unit�s affich�es
            sliderControl.Units.Text = slider.ValueLabel;
        }

        /// <summary>
        /// Lie un objet de donn�es <see cref="BoolSetting"/> � un contr�le UI de type <see cref="ToggleVisuals"/>.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de liaison.</param>
        /// <param name="toggleControl">L'objet interactif <see cref="ItemVisuals"/> pour afficher les donn�es de bascule pertinentes.</param>
        /// <param name="index">L'index dans <see cref="DataSource"/> du <see cref="BoolSetting"/> � lier � la vue.</param>
        private void BindToggle(Data.OnBind<BoolSetting> evt, ToggleVisuals toggleControl, int index)
        {
            // Les UserData sur cet �v�nement de liaison sont la m�me valeur stock�e
            // � l'index donn� `index` dans la DataSource.
            //
            // Autrement dit,
            // evt.UserData == DataSource[index]
            BoolSetting toggle = evt.UserData;

            // Met � jour le texte du label du contr�le
            toggleControl.Label.Text = toggle.Label;

            // Met � jour l'indicateur de bascule du contr�le
            toggleControl.IsOnIndicator.gameObject.SetActive(toggle.IsOn);
        }

        /// <summary>
        /// Lie un objet de donn�es <see cref="MultiOptionSetting"/> � un contr�le UI de type <see cref="DropdownVisuals"/>.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de liaison.</param>
        /// <param name="dropdownControl">L'objet interactif <see cref="ItemVisuals"/> pour afficher les donn�es de menu d�roulant pertinentes.</param>
        /// <param name="index">L'index dans <see cref="DataSource"/> du <see cref="MultiOptionSetting"/> � lier � la vue.</param>
        private void BindDropDown(Data.OnBind<MultiOptionSetting> evt, DropdownVisuals dropdownControl, int index)
        {
            // Assure que le contr�le d�marre dans un �tat r�duit
            // lorsqu'il est nouvellement li� � la vue.
            dropdownControl.Collapse();

            // Les UserData sur cet �v�nement de liaison sont la m�me valeur stock�e
            // � l'index donn� `index` dans la DataSource.
            //
            // Autrement dit,
            // evt.UserData == DataSource[index]
            MultiOptionSetting dropdown = evt.UserData;

            // Met � jour le texte du label du contr�le
            dropdownControl.DropdownLabel.Text = dropdown.Label;

            // Met � jour le champ de s�lection pour indiquer l'option actuellement s�lectionn�e dans le menu d�roulant
            dropdownControl.SelectionLabel.Text = dropdown.SelectionName;
        }

        /// <summary>
        /// D�tache un objet de donn�es <see cref="MultiOptionSetting"/> d'un contr�le UI de type <see cref="DropdownVisuals"/>.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de d�tachement.</param>
        /// <param name="dropdownControl">L'objet interactif <see cref="ItemVisuals"/> affichant les donn�es de menu d�roulant pertinentes.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="MultiOptionSetting"/> qui est d�tach� de la vue.</param>
        private void UnbindDropDown(Data.OnUnbind<MultiOptionSetting> evt, DropdownVisuals dropdownControl, int index)
        {
            if (dropdownControl == currentlyExpandedDropdown)
            {
                // si ce menu d�roulant est suivi en tant qu'objet actuellement �tendu
                // effacer ce champ pour indiquer qu'aucun menu d�roulant n'est actuellement �tendu
                currentlyExpandedDropdown = null;
            }

            // Assure que le contr�le de menu d�roulant est r�duit
            dropdownControl.Collapse();
        }

        /// <summary>
        /// G�re un objet <see cref="DropdownVisuals"/> dans la <see cref="SettingsList"/> �tant cliqu�, et l'�tend ou le r�duit en cons�quence.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de clic.</param>
        /// <param name="dropdownControl">L'objet <see cref="ItemVisuals"/> qui a �t� cliqu�.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="MultiOptionSetting"/> repr�sent� par <paramref name="dropdownControl"/>.</param>
        private void HandleDropdownClicked(Gesture.OnClick evt, DropdownVisuals dropdownControl, int index)
        {
            if (evt.Receiver.transform.IsChildOf(dropdownControl.OptionsView.transform))
            {
                // L'objet cliqu� n'�tait pas le menu d�roulant lui-m�me mais plut�t un �l�ment de liste � l'int�rieur du menu d�roulant.
                // Le dropdownControl lui-m�me g�rera cet �v�nement, donc nous n'avons rien � faire ici.
                return;
            }

            if (dropdownControl.IsExpanded)
            {
                // R�duit le menu d�roulant et arr�te de le suivre
                // en tant qu'objet �tendu en focus.
                dropdownControl.Collapse();

                // Comme cet �v�nement sera toujours appel� chaque fois qu'un menu d�roulant est
                // cliqu�, nous pouvons s�rement supposer que currentlyExpandedDropdown == dropdownControl,
                // puisque nous l'assignons ci-dessous et nulle part ailleurs.
                currentlyExpandedDropdown = null;
            }
            else
            {
                // Obtient la source de donn�es sous-jacente du menu d�roulant
                // pour pouvoir �tendre le dropdownControl avec
                // son ensemble d'options s�lectionnables.
                MultiOptionSetting dropdown = CurrentSettings[index] as MultiOptionSetting;

                // Indique au menu d�roulant de s'�tendre, montrant une liste de
                // options s�lectionnables.
                dropdownControl.Expand(dropdown);

                // Commence � suivre ce dropdownControl comme le
                // dropdown actuellement �tendu.
                currentlyExpandedDropdown = dropdownControl;
            }
        }

        /// <summary>
        /// G�re un objet <see cref="ToggleVisuals"/> dans la <see cref="SettingsList"/> �tant cliqu�, et bascule son �tat visuel et son �tat de donn�es en cons�quence.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de clic.</param>
        /// <param name="toggleControl">L'objet <see cref="ItemVisuals"/> qui a �t� cliqu�.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="BoolSetting"/> repr�sent� par <paramref name="toggleControl"/>.</param>
        private void HandleToggleClicked(Gesture.OnClick evt, ToggleVisuals toggleControl, int index)
        {
            // Obtient l'objet de donn�es de bascule sous-jacent repr�sent� par toggleControl
            BoolSetting toggle = CurrentSettings[index] as BoolSetting;

            // Bascule l'�tat de l'objet de donn�es
            toggle.IsOn = !toggle.IsOn;

            // Met � jour l'�tat visuel de la bascule pour refl�ter le nouveau �tat IsOn
            toggleControl.IsOnIndicator.gameObject.SetActive(toggle.IsOn);
        }

        /// <summary>
        /// G�re un objet <see cref="SliderVisuals"/> dans la <see cref="SettingsList"/> �tant gliss�, et ajuste son �tat visuel et son �tat de donn�es en cons�quence.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de glissement.</param>
        /// <param name="sliderControl">L'objet <see cref="ItemVisuals"/> qui a �t� gliss�.</param>
        /// <param name="index">L'index dans la <see cref="DataSource"/> du <see cref="FloatSetting"/> repr�sent� par <paramref name="sliderControl"/>.</param>
        private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals sliderControl, int index)
        {
            // Obtient l'objet de donn�es de curseur sous-jacent repr�sent� par sliderControl
            FloatSetting slider = CurrentSettings[index] as FloatSetting;

            // Convertit la position de glissement actuelle en espace local du curseur
            Vector3 pointerLocalPosition = sliderControl.SliderBounds.transform.InverseTransformPoint(evt.PointerPositions.Current);

            // Obtient la distance du curseur depuis le bord gauche du contr�le
            float sliderPositionFromLeft = pointerLocalPosition.x + 0.5f * sliderControl.SliderBounds.CalculatedSize.X.Value;

            // Convertit la distance depuis le bord gauche en un pourcentage depuis le bord gauche
            float sliderPercent = Mathf.Clamp01(sliderPositionFromLeft / sliderControl.SliderBounds.CalculatedSize.X.Value);

            // Met � jour le contr�le de curseur � la nouvelle valeur dans sa plage min/max
            slider.Value = Mathf.Lerp(slider.Min, slider.Max, sliderPercent);

            // Met � jour la barre de remplissage du contr�le pour refl�ter sa nouvelle valeur de curseur
            sliderControl.FillBar.Size.X.Percent = sliderPercent;

            // Met � jour le texte d'unit� du contr�le pour afficher la valeur num�rique
            // associ�e au contr�le de curseur
            sliderControl.Units.Text = slider.ValueLabel;
        }

        /// <summary>
        /// Enregistre dans la console chaque fois que le bouton de retour est cliqu�. Pr�sent ici comme un exemple dans cet exemple.
        /// </summary>
        /// <param name="evt">Les donn�es d'�v�nement de clic.</param>
        /// <param name="button">Le bouton recevant l'�v�nement.</param>
        private void HandleBackButtonClicked(Gesture.OnClick evt, ButtonVisuals button)
        {
            gameManager.LoadPreviousScene();
        }
    }
}
