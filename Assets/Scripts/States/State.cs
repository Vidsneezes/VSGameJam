using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public StateUI stateUi;

    public virtual void OnCreated()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void PoppedFromStateMachine()
    {
        GameObject.Destroy(stateUi.gameObject);
    }

    public virtual void Enabled()
    {

    }

    public void PopState()
    {
        StateManager.g.PopState();
    }

}
