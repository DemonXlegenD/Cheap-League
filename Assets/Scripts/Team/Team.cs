using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField, StringSelection("Orange", "Blue", "White")] private string teamName;
    [SerializeField] private string teamDescription;
    [SerializeField] private List<GameObject> playersList = new List<GameObject>();
    [SerializeField] private Dictionary<string, GameObject> playersDict = new Dictionary<string, GameObject>();

    #region Getter

    public string TeamName() {  return teamName; }
    public string TeamDescription() {  return teamDescription; }
    public GameObject GetPlayerByIndex(int index)
    {
        if (index >= playersList.Count) return null;
        return playersList[index];
    }

    public GameObject GetPLayerByName(string name)
    {
        if (name == null) return null;
        return playersDict[name];
    }

    public List<GameObject> GetPlayers() { return playersList; }

    #endregion

    public void OnDestroy()
    {
        playersList.Clear();
        playersDict.Clear();
    }
}
