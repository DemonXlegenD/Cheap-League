using Nova;
using UnityEngine;

[RequireComponent(typeof(UIBlock))]
public class MenuPanel : MonoBehaviour
{

    [SerializeField] private PanelType type;
    private bool state = false;


    private UIBlock uiBlock;
    public void Awake()
    {
        uiBlock = GetComponent<UIBlock>();
    }

    #region State

    private void UpdateState()
    {
        //Mettre une animation
        uiBlock.enabled = state;
        uiBlock.gameObject.SetActive(state);
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
