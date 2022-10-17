using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLevelUp : State
{
    LevelUpUI levelUpUI;

    public override void OnCreated()
    {
        if(levelUpUI == null)
        {
            levelUpUI = (LevelUpUI)StateManager.g.SpawnUI("Prefabs/UI/UI_LevelUp");
        }
        levelUpUI.PlayButtonClick("AudioClips/SFX/LevelUp");
        levelUpUI.state = this;
        levelUpUI.SetUpPowerButtons();
        GameController.g.explosionsSystems.Pause();
        GameController.g.Hero.hurtSystem.Pause();
    }

    public override void OnUpdate()
    {
      
    }

    public override void PoppedFromStateMachine()
    {
        GameController.g.Hero.LevelUp();
        GameObject.Destroy(levelUpUI.gameObject);
        GameController.g.explosionsSystems.Play();
        GameController.g.Hero.hurtSystem.Play();
    }
}
