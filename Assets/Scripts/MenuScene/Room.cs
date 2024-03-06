using NovaSamples.UIControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private MenuController menuController;

    private string currentSelection;
    // Start is called before the first frame update
    void Start()
    {
        menuController = FindAnyObjectByType<MenuController>();
        currentSelection = dropdown.DropdownOptions.CurrentSelection;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnChange()
    {
        currentSelection = dropdown.DropdownOptions.CurrentSelection;
    }

    public void OnStart()
    {
        if(currentSelection == "TRAINING")
        {
            menuController.ChangeScene("FieldScene");
        }
        else if (currentSelection == "1 VS 1")
        {
            menuController.ChangeScene("FieldScene");
        }
        else if (currentSelection == "GOAL")
        {
            menuController.ChangeScene("GameScene");
        }
    }
}
