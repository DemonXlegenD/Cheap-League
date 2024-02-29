using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private Collection<GameObject> teamsCollection = new Collection<GameObject>();

    #region Getter

    public GameObject GetTeamByIndex(int index)
    {
        return teamsCollection.GetItemByIndex(index);
    }

    public GameObject GetTeamByName(string name)
    {
        return teamsCollection.GetItemBykey(name);
    }

    public GameObject GetTeamByColor(ColorNameTeam colorName)
    {
        Debug.Log(colorName.ToString());
        return teamsCollection.FindItemsByProperty((value) => { return value.GetComponent<Team>().GetTeamColor(); }, colorName)[0];
    }

    #endregion

    #region Add
    public void AddTeam(GameObject team)
    {
        teamsCollection.AddItem(team.GetComponent<Team>().GetTeamNameString(), team);
    }
    #endregion



}