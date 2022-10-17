using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIElementsPowerButton : Button
{
    public BasePowerUpData powerUpSelection;
    public LevelUpUI levelUpUI;
    public UIElementsPowerButton() : base()
    {
        clicked += Button_OnPicked;
    }

    public void Button_OnPicked()
    {
        levelUpUI.Button_Press(this);
    }
}
