using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : StateUI
{
    public Image HpBar;
    public Image XpBar;
    public TextMeshProUGUI LevelLabel;
    public TextMeshProUGUI TimeLabel;
    public AudioSource inGameMusic;


    public void UpdateLabel(string label)
    {
        LevelLabel.text = label;
    }

    public void UpdateXpBar(float amount)
    {
        XpBar.fillAmount = amount;
    }

    public void UpdateHpBar(float amount)
    {
        HpBar.fillAmount = amount;
    }
}
