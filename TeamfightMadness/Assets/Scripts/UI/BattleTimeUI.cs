using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class BattleTimeUI : MonoBehaviour
{
    private TextMeshProUGUI _battleTimer;

    private void Awake()
    {
        _battleTimer = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeTimer(int battleTimeRemaining)
    {
        _battleTimer.text = battleTimeRemaining.ToString();
    }
}
