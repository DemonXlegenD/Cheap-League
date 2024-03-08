using UnityEngine;

public class OpenPanelButton : MonoBehaviour
{


    [SerializeField] private PanelType type;

    private MenuController menuController;

    public void Start()
    {
        menuController = FindObjectOfType<MenuController>();    
    }

    public void OnClick()
    {
        menuController.OpenPanel(type);
    }
}
