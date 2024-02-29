using System;
using System.Collections;
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
    [SerializeField] private ColorNameTeam teamColor;
    [SerializeField] private string teamName;
    [SerializeField] private string teamDescription;

    [Header("Team Players")]
    [SerializeField] private Collection<GameObject> playersCollection;

    [SerializeField] private Score scoreTeam;

    public UnityEvent OnGoalTeamScored;

    public void Start()
    {
        OnGoalTeamScored.AddListener(TeamHasScored);
    }
    #region Getter

    public ColorNameTeam GetTeamColor() { return teamColor; }
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
    }
    #endregion


    public void TeamHasScored()
    {
        scoreTeam.IncreaseScore();
    }
    public void Update()
    {
    }

    public void OnDestroy()
    {
        playersCollection.ClearItems();
    }
}