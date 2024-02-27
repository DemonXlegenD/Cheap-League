using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MenuPanel : MonoBehaviour
{

    [SerializeField] private PanelType type;
    private bool state = false;

    private Canvas canvas;
    public void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    #region State

    private void UpdateState()
    {
        //Mettre une animation
        canvas.enabled = state;
    }

    public void ChangeState()
    {
        state = !state;
        UpdateState();
    }

    public void ChangeState(bool _state)
    {
        state = _state;
        UpdateState();
    }

    #endregion

    #region Getter

    public PanelType GetPanelType() { return type; }

    #endregion
}
