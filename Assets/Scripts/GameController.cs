using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        Preload,
        InGame,
        LevelUpScreen,
        ToGameOver
    }

    public static GameController g;

    [HideInInspector]
    public bool IsInitialized;

    public bool PlayerHasDied;
  
    public CinemachineVirtualCamera MainView;
    [HideInInspector]
    public Camera PrimaryCamera;

    public StateInGame stateInGame;

    [HideInInspector]
    public HeroController Hero;

    [HideInInspector]
    public WeaponManager Weapons;
    [HideInInspector]
    public MonsterManager Monsters;
    [HideInInspector]
    public ItemManager Items;

    public float _startTime;
    public ParticleSystem explosionsSystems;
    public ParticleSystem.EmitParams standardEmissionParams;

    public float elapsedTime;
    public float deltaTime;

    public float CameraPad = 16*4;
    public float GameClearTime = -1;

    [HideInInspector]
    public Vector3 CameraBounds;

    public void Setup()
    {
        IsInitialized = true;
        g = this;
        UpdateXpBar(0);
        UpdateHpBar(1);

        PrimaryCamera = Camera.main;
        explosionsSystems = Instantiate(Resources.Load<ParticleSystem>("Prefabs/FX/ExplosionSystem"), transform.position, Quaternion.identity, transform);
        standardEmissionParams = new ParticleSystem.EmitParams();

        Hero = Instantiate(Resources.Load<HeroController>("Prefabs/Hero"), transform.position, Quaternion.identity);
        Hero.gameController = this;
        Weapons = Instantiate(Resources.Load<WeaponManager>("Managers/WeaponManager"), transform.position, Quaternion.identity, transform);
        Monsters = Instantiate(Resources.Load<MonsterManager>("Managers/MonsterManager"), transform.position, Quaternion.identity, transform);
        Items = Instantiate(Resources.Load<ItemManager>("Managers/ItemManager"), transform.position, Quaternion.identity, transform);

        MainView.Follow = Hero.transform;
        _startTime = Time.time;

        CameraBounds = PrimaryCamera.ViewportToScreenPoint(new Vector3(1, 1));
        Debug.Log(CameraBounds);
    }

    public void UpdateXpBar(float amount)
    {
        stateInGame.inGameUI.UpdateXpBar(amount);
    }

    public void UpdateHpBar(float amount)
    {
        stateInGame.inGameUI.UpdateHpBar(amount);
    }

    public void UpdateLevelText(int level)
    {
        stateInGame.inGameUI.UpdateLabel($"Level : {level}");
    }

    public void OnUpdate()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            if(Monsters != null)
            {
                if(Monsters.currentLevelSequence != null)
                {
                    Monsters.currentLevelSequence.IncrementWave();
                }
            }
        }

        deltaTime = Time.deltaTime;
        elapsedTime += Time.deltaTime;
        Hero.OnUpdate(deltaTime);

        Weapons.OnUpdate(deltaTime);
        Monsters.OnUpdate(deltaTime);
        Items.OnUpdate(deltaTime);

        

        if(Hero.defineSheet.HP <= 0)
        {
            PlayerHasDied = true;
        }

        

    }

    public void PlayExplosionAt(Vector3 position)
    {
        standardEmissionParams.position = position;
        explosionsSystems.Emit(standardEmissionParams, 1);
    }

    public Vector3 ViewPortToWorld(Vector3 v_position)
    {
        return PrimaryCamera.ViewportToWorldPoint(v_position);
    }

    public Vector3 WorldToViewport(Vector3 w_position)
    {
        return PrimaryCamera.WorldToViewportPoint(w_position);
    }

    public Vector3 WorldToScreen(Vector3 w_position)
    {
        return PrimaryCamera.WorldToScreenPoint(w_position);
    }

    public Vector3 ScreenToWorld(Vector3 s_position)
    {
        return PrimaryCamera.ScreenToWorldPoint(s_position);
    }

    public bool OutsideCamera(Vector3 worldPosition)
    {
        Vector3 screenPos = WorldToScreen(worldPosition);

        return (screenPos.x < -CameraPad || screenPos.x > GameController.g.CameraBounds.x + CameraPad) || (screenPos.y < -CameraPad || screenPos.y > GameController.g.CameraBounds.y + CameraPad);
    }

    public Vector3 GetPIPositionOutsideCamera(float scaleX = 1, float scaleY = 1, float fuzzyX = 1, float fuzzyY = 1, float RangeMin = 0, float RangeMax = 1)
    {
        Vector3 position = new Vector3();
        Vector3 center = CameraBounds * 0.5f;
        float randomAngle = Random.Range(RangeMin, RangeMax) * Mathf.PI * 2f;
        position.x = center.x + Mathf.Cos(randomAngle) * CameraBounds.x * 0.5f * scaleX * Random.Range(1,fuzzyX);
        position.y = center.y + Mathf.Sin(randomAngle) * CameraBounds.y * 0.5f * scaleY * Random.Range(1,fuzzyY);

        return ScreenToWorld(position);
    }

    public void WorldPause()
    {
        Hero.Pause();
    }

    public void WorldResume()
    {
        Hero.Resume();
    }
}
