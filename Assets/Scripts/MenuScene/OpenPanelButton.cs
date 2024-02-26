using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanelButton : MonoBehaviour
{


    [SerializeField] private PanelType type;

    private MenuController menuController;

    // Start is called before the first frame update
    public void Start()
    {
        menuController = FindObjectOfType<MenuController>();    
    }

    public void OnClick()
    {
        menuController.OpenPanel(type);
    }
}
