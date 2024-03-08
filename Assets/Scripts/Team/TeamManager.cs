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

    public void ShowInformation(string TeamName, Color color)
    {
        foreach (GameObject team in teamsCollection.GetItemsList())
        {
            team.GetComponent<Team>().ShowInformation(TeamName, color);
        }
    }
    public void ShowWin(string TeamName, Color color)
    {
        foreach (GameObject team in teamsCollection.GetItemsList())
        {
            team.GetComponent<Team>().ShowWin(TeamName, color);
        }
    }

    #endregion

    #region Add
    public void AddTeam(GameObject team)
    {
        teamsCollection.AddItem(team.GetComponent<Team>().GetTeamNameString(), team);
    }
    #endregion


    public Team TeamWon()
    {
        foreach (GameObject team in teamsCollection.GetItemsList())
        {
            if (team.GetComponent<Score>().IsScoreAtteint())
            {
                return team.GetComponent<Team>();
            }
        }
        return null;
    }

    public Team TeamEndTime()
    {
        GameObject team1 = teamsCollection.GetItemByIndex(0);
        GameObject team2 = teamsCollection.GetItemByIndex(1);

        if(team1.GetComponent<Score>().GetCurrentScore() == team2.GetComponent<Score>().GetCurrentScore())
        {
            return null;
        }
        else if (team1.GetComponent<Score>().GetCurrentScore() > team2.GetComponent<Score>().GetCurrentScore())
        {
            return team1.GetComponent<Team>();
        }
        else if (team2.GetComponent<Score>().GetCurrentScore() > team1.GetComponent<Score>().GetCurrentScore())
        {
            return team2.GetComponent<Team>();
        }
        return null;
    }
}