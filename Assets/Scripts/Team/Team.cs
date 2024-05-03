using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ColorNameTeam
{
    Orange,
    Blue,
    White,
}
public class Team : MonoBehaviour
{
    [Header("Team Settings")]
    [SerializeField] private ColorNameTeam teamColorName;
    [SerializeField] private Color teamColor;
    [SerializeField] private string teamName;
    [SerializeField] private string teamDescription;

    [Header("Team Players")]
    [SerializeField] private Collection<GameObject> playersCollection;

    [SerializeField] private Score scoreTeam;

    [SerializeField] private Canvas UITeam;

    [SerializeField] private Information Information;

    public UnityEvent OnGoalTeamScored;

    public void Start()
    {
        OnGoalTeamScored.AddListener(TeamHasScored);
        if(teamColorName == ColorNameTeam.Orange)
        {
            Information.label.color =  new Color(255, 116, 0, 255);
        }
        else if (teamColorName == ColorNameTeam.Blue)
        {
            Information.label.color = new Color(0, 27, 255, 255);
        } 
        else
        {
            Information.label.color = new Color(0, 0, 0, 255);
        }
    }
    #region Getter

    public ColorNameTeam GetTeamColor() { return teamColorName; }
    public Color GetTeamColorR() { return teamColor; }
    public string GetTeamNameString() { return teamName; }
    public string GetTeamDescription() { return teamDescription; }
    public GameObject GetPlayerByIndex(int index)
    {
        return playersCollection.FindItemByIndex(index);
    }

    public GameObject GetPLayerByName(string name)
    {
        return playersCollection.FindItemBykey(name);
    }

    public List<GameObject> GetPlayers() { return playersCollection.GetItemsList(); }

    #endregion

    #region Add

    public void AddPlayer(GameObject player)
    {
        playersCollection.AddItem(player.GetInstanceID().ToString(), player);
        UITeam.renderMode = RenderMode.ScreenSpaceCamera;
        Camera[] cameras = player.GetComponentsInChildren<Camera>();
        foreach(Camera c in cameras)
        {
            if(c.name == "UICamera")
            {
                UITeam.worldCamera = c; 
                break;
            }
        }

    }
    #endregion


    public void TeamHasScored()
    {
        scoreTeam.IncreaseScore();
    }

    public void ShowInformation(string message, Color color)
    {
        Information.OnScored(message, color);
    }
    public void ShowWin(string message, Color color)
    {
        Information.OnWin(message, color);
    }


    public void Update()
    {
    }

    public void OnDestroy()
    {
        playersCollection.ClearItems();
    }
}