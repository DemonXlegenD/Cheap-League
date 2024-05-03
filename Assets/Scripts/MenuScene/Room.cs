using NovaSamples.UIControls;
using UnityEngine;

public class Room : MonoBehaviour
{

    GameManager gameManager;
    [SerializeField] private Dropdown dropdownMod;
    [SerializeField] private Dropdown dropdownPlayers;
    [SerializeField] private MenuController menuController;

    private string currentSelectionMod;
    private string currentSelectionPlayers;
    void Start()
    {
        gameManager = GameManager.Instance;
        menuController = FindAnyObjectByType<MenuController>();
        currentSelectionMod = dropdownMod.DropdownOptions.CurrentSelection;
        currentSelectionPlayers = dropdownPlayers.DropdownOptions.CurrentSelection;
    }
    public void OnChange()
    {
        currentSelectionMod = dropdownMod.DropdownOptions.CurrentSelection;
    }

    public void OnStart()
    {
        if (currentSelectionPlayers == "1")
        {
            gameManager.SetNumberPlayers(1);
        }
        else if (currentSelectionMod == "2")
        {
            gameManager.SetNumberPlayers(2);
        }
       
        if (currentSelectionMod == "TRAINING")
        {
            menuController.ChangeScene("FieldScene");
        }
        else if (currentSelectionMod == "1 VS 1")
        {
            menuController.ChangeScene("FieldScene");
        }
        else if (currentSelectionMod == "GOAL")
        {
            menuController.ChangeScene("GameScene");
        }
    }
}
