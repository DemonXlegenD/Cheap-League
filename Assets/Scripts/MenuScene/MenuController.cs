using System.Collections.Generic;
using UnityEngine;


public enum PanelType
{
    None,
    Main,
    Room,
    Options,
    Credits,
}

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private List<MenuPanel> panelList = new List<MenuPanel>();
    private Dictionary<PanelType, MenuPanel> panelDict = new Dictionary<PanelType, MenuPanel>();

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        foreach (var _panel in panelList)
        {
            if (_panel) panelDict.Add(_panel.GetPanelType(), _panel);
        }

        OpenOnePanel(PanelType.Main);
    }

    private void OpenOnePanel(PanelType _type)
    {
        foreach (var _panel in panelList)
        {
            _panel.ChangeState(false);
        }

        if (_type != PanelType.None)
        {
            panelDict[_type].ChangeState(true);
        }
    }
    public void ChangeScene(string _sceneName)
    {
        gameManager.ChangeScene(_sceneName);
    }

    public void OpenPanel(PanelType _type)
    {
        OpenOnePanel(_type);
    }

    public void Quit()
    {
        gameManager.Quit();
    }
}
