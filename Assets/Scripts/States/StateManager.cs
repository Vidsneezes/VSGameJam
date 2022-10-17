using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager g;

    public enum States
    {
        Preload,
        InGame,
        LevelUp,
        Pause,
        GameOver,
        GameOverReward,
        TitleScreen
    }

    public List<State> stateStack;
    public State CurrentState;
    public Canvas MainCanvas;
    public bool PlayerWon;
    public AudioSource globalAudioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        stateStack = new List<State>();
        g = this;
        PlayerWon = false;
        Screen.SetResolution(1280, 720, true);
    }

    private void Start()
    {
        PushState(States.TitleScreen);
    }

    private void Update()
    {
        if(CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }

    public void PushState(States state)
    {
        string newStateName = "State" + state.ToString();
        Type stateType = Type.GetType(newStateName);
        if(CurrentState != null && CurrentState.name == newStateName)
        {
            return;
        }

        GameObject inGameState = new GameObject(newStateName);
        inGameState.transform.SetParent(transform);
        State m = (State)inGameState.AddComponent(stateType);
        stateStack.Add(m);
        CurrentState = m;
        CurrentState.OnCreated();
        CurrentState.Enabled();
    }

    public void PopState()
    {
        if(stateStack.Count >= 1)
        {
            State poppedState = stateStack[stateStack.Count - 1];

            stateStack.RemoveAt(stateStack.Count - 1);
            poppedState.PoppedFromStateMachine();

            if (stateStack.Count == 0)
            {
                CurrentState = null;
            }
            else
            {
                CurrentState = stateStack[stateStack.Count - 1];
                CurrentState.Enabled();
            }
            GameObject.Destroy(poppedState.gameObject);
        }
    }

    public StateUI SpawnUI(string path)
    {
        return Instantiate(Resources.Load<StateUI>(path), MainCanvas.transform);
    }
}
