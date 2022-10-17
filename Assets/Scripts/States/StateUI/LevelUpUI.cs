using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpUI : StateUI
{
    [HideInInspector]
    public UIDocument doc;
    [HideInInspector]
    public ProbabilityTable powersTable;

    public void SetUpPowerButtons()
    {
        UIDocument doc = GetComponent<UIDocument>();
        doc.sortingOrder = 1;

        if(powersTable == null)
        {
            powersTable = Resources.Load<ProbabilityTable>("GameData/Tables/GenericPowersTable");
        }

        HashSet<ScriptableObject> bagSelection = powersTable.probabilityBag.PickTwoObjectsUnique();
        VisualElement powerUpPop = doc.rootVisualElement.Q<VisualElement>("power-popup-inner");

        foreach (var power in bagSelection)
        {
            BasePowerUpData basePowerData = ScriptableObject.Instantiate(power) as BasePowerUpData;
            basePowerData.SetUp();

            UIElementsPowerButton TemplateButton = new UIElementsPowerButton();
            TemplateButton.AddToClassList("power-button");
            TemplateButton.name = "template";
            TemplateButton.levelUpUI = this;
            TemplateButton.powerUpSelection = basePowerData;
            TemplateButton.text = basePowerData.description;
            powerUpPop.Add(TemplateButton);
        }
    }


    public void Button_Press(UIElementsPowerButton pb)
    {
        PlayButtonClick("AudioClips/SFX/PowerSelected");
        pb.powerUpSelection.Execute();
        StateInGame inGameState = GameObject.FindObjectOfType<StateInGame>();
        inGameState.inGameUI.inGameMusic.UnPause();

        state.PopState();

    }

    public void PlayButtonClick(string clip)
    {
        StateManager.g.globalAudioSource.PlayOneShot(Resources.Load<AudioClip>(clip));
    }
}

