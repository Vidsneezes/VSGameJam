using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StateTitleScreen : State
{
    public override void OnCreated()
    {
        stateUi = StateManager.g.SpawnUI("Prefabs/UI/UI_TitleScreen");
        UIDocument doc = stateUi.GetComponent<UIDocument>();
        doc.rootVisualElement.Q<Button>("continue-button").clicked += GoToGame;
        doc.rootVisualElement.Q<Button>("continue-button").clicked += PlayButtonClick;

    }

    public override void OnUpdate()
    {

    }

    public void GoToGame()
    {
        PopState();
        StateManager.g.PushState(StateManager.States.InGame);
    }

    public void PlayButtonClick()
    {
        StateManager.g.globalAudioSource.PlayOneShot(Resources.Load<AudioClip>("AudioClips/SFX/MenuContinue"));
    }
}

