using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    public float speed;
    public float accelartionWeight;
    public float drag;
    public float invicibleTime;
    public PlayerInput playerInput;
    public InputAction moveAction;
    public ParticleSystem hurtSystem;
    public GameController gameController;

    public Vector3 MidPoint
    {
        get
        {
            return transform.position + Vector3.up * 0.185f;
        }
    }
    public string HeroName;

    public HeroSheet defineSheet;
    public HeroWeaponController WeaponController;
    public AudioClip HurtClip;

    public int Facing
    {
        get
        {
            return _facing;
        }
    }

    public Vector3 WalkDirection
    {
        get
        {
            return velocity.normalized;
        }
    }

    private int _facing;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Vector2 velocity;
    private float _speedGravity;
    private float damageTaken;
    private float _weaponTimer;
    private float _invincibleTime;
    private float _healthDpsTimer;
    private float _xpDpsTimer;
    private StateInGame inGameState;
    private void Awake()
    {
        defineSheet = HeroSheet.GetInstanceHeroSheet(HeroName);

        WeaponController = GetComponent<HeroWeaponController>();
        WeaponController.SetHeroSheet(defineSheet);

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _spriteRenderer.sprite = defineSheet.DisplaySprite;
        Application.targetFrameRate = 121;
        _spriteRenderer.material.SetFloat("_BouncingHeight", 0.001f);
        _spriteRenderer.material.SetColor("_HurtColor", Color.red);
        hurtSystem.Play();

    }

    // Start is called before the first frame update
    void Start()
    {
        moveAction = playerInput.currentActionMap.FindAction("Move");
        _speedGravity = 0;
        _weaponTimer = GameController.g.elapsedTime;

        defineSheet.HP = defineSheet.BaseHP;
        defineSheet.XP = 0;
        defineSheet.Level = 1;
        UpdateFacing(1);
        _healthDpsTimer = 0;
        _xpDpsTimer = 0;

        inGameState = GameObject.FindObjectOfType<StateInGame>();
    }

    // Update is called once per frame
    public void OnUpdate(float deltaTime)
    {
        Vector2 _move = moveAction.ReadValue<Vector2>();

        if(_move.sqrMagnitude > 0.1f)
        {
            _speedGravity += deltaTime * accelartionWeight;
            velocity = Vector2.Lerp(Vector2.zero, _move.normalized * speed * defineSheet.Speed, _speedGravity);
            UpdateFacing(_move.x);
        }
        else
        {
            _speedGravity = 0;
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deltaTime * drag);
        }

        WeaponController.OnUpdate();

        _rigidbody.MovePosition(_rigidbody.position + velocity * deltaTime);

        if(defineSheet.XPPool > 0.01f && GameController.g.elapsedTime - _xpDpsTimer > 0.07f)
        {
            _xpDpsTimer = GameController.g.elapsedTime;
            CollectExperience(1);
            
        }

        if (damageTaken > 0.01f && GameController.g.elapsedTime - _healthDpsTimer > 0.07f)
        {
            defineSheet.HP -= 1;
            damageTaken -= 1;
            _healthDpsTimer = GameController.g.elapsedTime;
            gameController.UpdateHpBar(defineSheet.HP / defineSheet.BaseHP);
            hurtSystem.Emit(5);
            StateManager.g.globalAudioSource.PlayOneShot(HurtClip, 0.4f);
            if (damageTaken < 0.01f)
            {
                _spriteRenderer.material.SetFloat("_Hurt", 0.0f);
            }
        }

        if(_rigidbody.position.x > 15f || _rigidbody.position.x < -15f || _rigidbody.position.y > 13 || _rigidbody.position.y < -14)
        {
            TakeDamage(3);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            item.Hero = this;
            item.Kill();
        }else if (collision.CompareTag("Monster"))
        {
            Monster monster = collision.GetComponent<Monster>();
            TakeDamage(monster.monsterData.damage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (GameController.g.elapsedTime - _invincibleTime > invicibleTime)
        {
            _invincibleTime = GameController.g.elapsedTime;
            damageTaken += amount;
            _spriteRenderer.material.SetFloat("_Hurt", 1f);
        }
    }

    public void CollectHealth(float amount)
    {
        defineSheet.HP += amount;
        gameController.UpdateHpBar(defineSheet.HP / defineSheet.BaseHP);
    }

    public void CollectExperience(int amount)
    {
        defineSheet.XPPool -= amount;
        defineSheet.XP += amount;
        if (defineSheet.ReachedXpRequired())
        {
            if(inGameState == null)
            {
                inGameState = GameObject.FindObjectOfType<StateInGame>();
                inGameState.inGameUI.inGameMusic.Pause();
            }
            else
            {
                inGameState.inGameUI.inGameMusic.Pause();
            }
            StateManager.g.PushState(StateManager.States.LevelUp);
        }
        gameController.UpdateXpBar((float)defineSheet.XP / (float)defineSheet.RequiredXp);
    }

    public void AddXpToPool(int amount)
    {
        defineSheet.XPPool += amount;
    }

    public void UpgradeViaPowerData(HeroPowerUpData powerUpdata)
    {
        defineSheet.UpdateHeroViaPowerData(powerUpdata);
        if(powerUpdata.maxHp > 1.01f)
        {
            CollectHealth(defineSheet.BaseHP * 0.10f);
        }

        float percentage = powerUpdata.hpRecover - 1;
        if(percentage > 0.01f)
        {
            CollectHealth(defineSheet.BaseHP * percentage);
        }
    }

    public void LevelUp()
    {
        int XpLeftOver = defineSheet.XP - defineSheet.RequiredXp;
        defineSheet.XPPool += XpLeftOver;
        defineSheet.XP = 0;
        defineSheet.Level += 1;
        gameController.UpdateXpBar(defineSheet.XP / defineSheet.RequiredXp);
        gameController.UpdateLevelText(defineSheet.Level);
    }

    void UpdateFacing(float x)
    {
        if(Mathf.Abs(x) < 0.1f)
        {
            return;
        }

        if(x < 0)
        {
            _facing = -1;
            _spriteRenderer.flipX = true;
        }
        else
        {
            _facing = 1;
            _spriteRenderer.flipX = false;
        }
    }

    public void Pause()
    {
        GameController.g.explosionsSystems.Pause();
        hurtSystem.Pause();
    }

    public void Resume()
    {
        GameController.g.explosionsSystems.Play();
        hurtSystem.Play();
    }
}
