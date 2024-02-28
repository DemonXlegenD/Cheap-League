using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Information")]

    [SerializeField] private string p_Name;
    [SerializeField] private string p_Description;
    [SerializeField] private Team p_Team;



    #region Getter
    public string GetName() { return p_Name; }
    public string GetDescription() { return p_Description; }
    public Team GetTeam() { return p_Team; }
    #endregion
}
