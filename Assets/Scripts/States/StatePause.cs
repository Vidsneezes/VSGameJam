using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatePause : State
{
    public StateInGame inGameState;

    public override void OnCreated()
    {
        stateUi = StateManager.g.SpawnUI("Prefabs/UI/UI_Pause");

        UIDocument doc = stateUi.GetComponent<UIDocument>();
        Button continueButton = doc.rootVisualElement.Q<Button>("resume-button");
        Button quitButton = doc.rootVisualElement.Q<Button>("quit-button");

        continueButton.clicked += Button_Continue;
        quitButton.clicked += Button_Quit;
        inGameState = GameObject.FindObjectOfType<StateInGame>();


        //    levelUpUI.PlayButtonClick("AudioClips/SFX/LevelUp");
        GameController.g.explosionsSystems.Pause();
        GameController.g.Hero.hurtSystem.Pause();
        inGameState.inGameUI.inGameMusic.Pause();

    }

    public void Button_Continue()
    {
        PopState();
    }

    public void Button_Quit()
    {
        Application.Quit();
    }

    public override void OnUpdate()
    {
        if(UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PopState();
        }
    }

    public override void PoppedFromStateMachine()
    {
        GameObject.Destroy(stateUi.gameObject);
        GameController.g.explosionsSystems.Play();
        GameController.g.Hero.hurtSystem.Play();
        inGameState.inGameUI.inGameMusic.UnPause();

    }
}
