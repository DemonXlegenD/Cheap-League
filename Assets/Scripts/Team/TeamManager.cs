using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private List<Team> teamsList = new List<Team>();
    [SerializeField] private Dictionary<string, Team> teamsDict = new Dictionary<string, Team>();

}
