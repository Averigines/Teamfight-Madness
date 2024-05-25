using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTeam1;
    [SerializeField] private TextMeshProUGUI textTeam2;

    [SerializeField] private string textTemplateTeam1;
    [SerializeField] private string textTemplateTeam2;

    public void ChangeScore(int scoreTeam1, int scoreTeam2)
    {
        textTeam1.text = textTemplateTeam1 + scoreTeam1;
        textTeam2.text = textTemplateTeam2 + scoreTeam2;
    }
}
