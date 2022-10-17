using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Poolable
{
    public MonsterData monsterData;

    public float HP;

    public int state;
    public AudioClip onHitSound;
    public AudioClip onDeadSound;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    private Vector2 velocity;
    private Vector2 avoidance;
    private float _speedGravity;
    private float _internalTimer;
    private float _holdInterval;
    private float _hurtTime;
    private float _ricochetForce = 1;
    private bool justTookDamage;
    private float _facing;

    public override void SetUp()
    {
        base.SetUp();

        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        state = 0;
        if(Random.Range(0f,1f) < 0.3f)
        {
            state = 1;
        }
        _internalTimer = Random.Range(GameController.g.elapsedTime- 1, GameController.g.elapsedTime + 1);
        _facing = 1;
        _spriteRenderer.material.SetFloat("_BouncingHeight", -0.04f);
        radius = _circleCollider.radius;
        offset = _circleCollider.offset;
    }

    public void SetAvoidance(Vector2 _avoid)
    {
        avoidance = (GetVelocity() - _avoid).normalized * 0.2f;
    }

    public void ResetAvoidance()
    {
        avoidance = Vector2.zero;
    }

    public override void SetPosition(Vector2 position)
    {
        _localPosition = position;
        _rigidbody.position = position;
        transform.position = position;
    }

    void UpdateFacing(float x)
    {
        if (Mathf.Abs(x) < 0.1f)
        {
            return;
        }

        if (x < 0)
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

    public void SetHoldInterval()
    {
        _holdInterval = Random.Range(monsterData.holdTime * 0.78f, monsterData.holdTime * 1.2f);
    }

    public override void PreUpdate()
    {
        _rigidbody.position = _localPosition;
        transform.position = _localPosition;
    }
    
    public override void OnUpdate(float deltaTime)
    {
        if (monsterData != null)
        {
            switch (state)
            {
                case 0:
                    //if (GameController.g.elapsedTime - _internalTimer > _holdInterval)
                    //{
                    //    state = 1;
                    //    _internalTimer = GameController.g.elapsedTime;
                    //    _spriteRenderer.material.SetFloat("_BounceTime", 1);
                    //}
                    //break;
                case 1:
                    //velocity = GetVelocity() + avoidance;

                    //_localPosition = _localPosition + velocity * deltaTime;
                    //_spriteRenderer.material.SetFloat("_BounceTime", 1f-(GameController.g.elapsedTime - _internalTimer) / monsterData.moveTime);
                    //if (GameController.g.elapsedTime - _internalTimer > monsterData.moveTime)
                    //{
                    //    state = 0;
                    //    _internalTimer = GameController.g.elapsedTime;
                    //    SetHoldInterval();
                    //    _spriteRenderer.material.SetFloat("_BounceTime", 0);

                    //}
                    velocity = GetVelocity() + avoidance;

                    _localPosition = _localPosition + velocity * deltaTime;
                    _spriteRenderer.material.SetFloat("_BounceTime", 1);
                    break;
                case 3:

                    velocity = GetVelocity() + avoidance;

                    _localPosition = _localPosition + -velocity * deltaTime * _ricochetForce;

                    break;
            }

            if(velocity.x > 0.1f)
            {
                UpdateFacing(1);
            }
            else if(velocity.x < -0.1f)
            {
                UpdateFacing(-1);
            }

            if (justTookDamage && GameController.g.elapsedTime - _hurtTime > 0.1f)
            {
                state = 0;
                justTookDamage = false;
                _spriteRenderer.material.SetFloat("_Hurt", 0.0f);
            }

            if ((int)HP <= 0)
            {
                Kill();
            }

            _rigidbody.position = _localPosition;
        }
    }

    public Vector2 GetVelocity()
    {
        return (GameController.g.Hero.transform.position - transform.position).normalized * monsterData.speed;
    }

    public override void Kill()
    {
        base.Kill();
        _spriteRenderer.material.SetFloat("_Hurt", 0.0f);
        _spriteRenderer.material.SetFloat("_BounceTime", 1);
        GameController.g.Monsters.PlaySound(onDeadSound);
        //StateManager.g.globalAudioSource.PlayOneShot(onDeadSound, 0.65f);
        GameController.g.PlayExplosionAt(_localPosition + _circleCollider.offset);
        if (monsterData != null && monsterData.itemLootTable != null)
        {
            GameController.g.Items.DropItemFromLootTable(monsterData.itemLootTable, transform.position);
        }
    }

    public void Retire()
    {
        base.Kill();
        _spriteRenderer.material.SetFloat("_Hurt", 0.0f);
        _spriteRenderer.material.SetFloat("_BounceTime", 1);
    }

    public void TakeDamageByWeaponData(WeaponData weaponData)
    {
        float finalDamage = GameController.g.Hero.defineSheet.BaseDamage + weaponData.damage;

        if(Random.Range(0f,1f) < 0.05f)
        {
            finalDamage *= 1.15f;
        }

        TakeDamage(finalDamage);
    }

    public void TakeDamage(float amount)
    {
        if (justTookDamage == false)
        {
            justTookDamage = true;
            state = 3;

            _hurtTime = GameController.g.elapsedTime;

            float crit = 1;
            if (Random.Range(0f, 1f) < 0.09f)
            {
                crit = 1.8f;
            }

            HP -= amount * crit;
            _ricochetForce = 1;
            if (HP <= 7)
            {
                _ricochetForce = 10;
            }
            _ricochetForce *= crit;
            _spriteRenderer.material.SetFloat("_Hurt", 1);
            _spriteRenderer.material.SetFloat("_BounceTime", 0);
            GameController.g.Monsters.PlaySound(onHitSound);

            //StateManager.g.globalAudioSource.PlayOneShot(onHitSound);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            Weapon weapon = collision.GetComponent<Weapon>();
            if (weapon.IsAlive && !weapon.collisionOff)
            {
                if (weapon.HP > 0)
                {
                    TakeDamageByWeaponData(weapon.weaponData);
                }
                weapon.TakeDamage(HP);

            }
        }
    }

    public override void Revive()
    {
        if (monsterData != null)
        {
            HP = monsterData.HP;
            _spriteRenderer.sprite = monsterData.displayImage;
            _circleCollider.offset = monsterData.colliderOffset;
            _circleCollider.radius = monsterData.radius;
        }
        _spriteRenderer.material.SetFloat("_Hurt", 0);
        _spriteRenderer.material.SetFloat("_BounceTime", 0);

        _alive = true;
        _hurtTime = GameController.g.elapsedTime;
        justTookDamage = false;
        gameObject.SetActive(true);
        state = 0;
        velocity = Vector2.zero;
        SetHoldInterval();
        _internalTimer = Random.Range(GameController.g.elapsedTime - 1, GameController.g.elapsedTime + 1);
        radius = _circleCollider.radius;
        offset = _circleCollider.offset;
    }

}
