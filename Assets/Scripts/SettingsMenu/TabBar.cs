using Nova;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabBar : ItemVisuals
{
    [SerializeField] private List<SettingsCollection> SettingsCollections = null;
    [SerializeField] private ListView TabButtons = null;
    [SerializeField] private int selectedIndex = 0;

}
