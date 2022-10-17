using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    protected bool _alive;
    
    public Vector2 LocalPosition
    {
        get
        {
            return _localPosition;
        }
        set
        {
            _localPosition = value;
        }
    }
    [HideInInspector]
    public bool OutOfCameraSpace;
    [HideInInspector]
    public float TimeOutsideCamera;

    protected Vector2 _localPosition;
    public Vector2 offset; 
    public float radius;
    public Vector2 ColliderPosition
    {
        get
        {
            return _localPosition + offset;
        }
    }

    public virtual bool IsAlive
    {
        get
        {
            return _alive;
        }
    }

    public virtual void SetPosition(Vector2 position)
    {
        _localPosition = position;
        transform.position = position;
    }


    public virtual void SetUp()
    {
        _alive = true;
    }

    public virtual void Revive()
    {
        _alive = true;
        gameObject.SetActive(true);
    }

    public virtual void PreUpdate()
    {

    }

    public virtual void OnUpdate(float delatTime)
    {

    }

    public virtual void Kill()
    {
        _alive = false;
    }

    public bool CalculateOutOfCameraView()
    {
        OutOfCameraSpace = GameController.g.OutsideCamera(_localPosition);
        return OutOfCameraSpace;
    }

}
