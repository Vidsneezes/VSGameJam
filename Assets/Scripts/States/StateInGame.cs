using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateInGame : State
{
    public GameController gameController;
    public InGameUI inGameUI;

    public LevelSequence currentLevelSequence;

    public override void OnCreated()
    {
        if (inGameUI == null)
        {
            inGameUI = (InGameUI)StateManager.g.SpawnUI("Prefabs/UI/UI_InGame");
        }
        StateManager.g.PlayerWon = false;
        inGameUI.UpdateLabel("Level 1");

        StartCoroutine(LoadLevel());
    }

    public override void PoppedFromStateMachine()
    {
        SceneManager.LoadScene("Empty", LoadSceneMode.Single);
        GameObject.Destroy(inGameUI.gameObject);
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Single);
        while(!ao.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        gameController = FindObjectOfType<GameController>();
        gameController.stateInGame = this;


        ao = SceneManager.LoadSceneAsync("AreaForest", LoadSceneMode.Additive);
        while (!ao.isDone)
        {
            yield return null;
        }

        gameController.Setup();

        currentLevelSequence = Instantiate(Resources.Load<LevelSequence>("GameData/LevelSequences/Sequence1"));
        GameController.g.Monsters.currentLevelSequence = currentLevelSequence;
    }

    public override void OnUpdate()
    {
        if(gameController != null && gameController.IsInitialized)
        {
            gameController.OnUpdate();
            if(gameController.GameClearTime > 0.01f && gameController.elapsedTime - gameController.GameClearTime > 0.6f)
            {
                StateManager.g.PlayerWon = true;
            }

            if (gameController.PlayerHasDied || StateManager.g.PlayerWon)
            {
                StateManager.g.PopState();
                StateManager.g.PushState(StateManager.States.GameOver);
                StateManager.g.globalAudioSource.PlayOneShot(Resources.Load<AudioClip>("AudioClips/SFX/PlayerDiedEffect"));
                return;
            }

            inGameUI.TimeLabel.text = Mathf.Round(GameController.g.elapsedTime).ToString("0000") + "";
        
            if(UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                StateManager.g.PushState(StateManager.States.Pause);
            }

        }
    }
}
