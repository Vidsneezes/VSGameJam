using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateGameOver : State
{

    public override void OnCreated()
    {
        stateUi = StateManager.g.SpawnUI("Prefabs/UI/UI_GameOver");
        stateUi.OnCreate();
        StartCoroutine(LoadLevel());
    }

    public override void PoppedFromStateMachine()
    {
        base.PoppedFromStateMachine();
        SceneManager.LoadScene("Empty", LoadSceneMode.Single);
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Single);
        while (!ao.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
    }

}
