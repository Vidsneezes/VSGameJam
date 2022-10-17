using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Poolable
{
    public WeaponData weaponData;

    public float HP;
    public HeroController Hero;
    public bool collisionOff;
    public float Delay = -1;

    public IWeaponLogic WeaponLogic;
    public SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D circleCollider2D;
    public Vector2 velocity;
    public Vector2 _acceleration;
    public float kineticSpeed = 0;
    public float _timer;
    public Vector2 origin;
    public Vector2 destination;
    private float _delayTimer;

    public override void SetUp()
    {
        base.SetUp();
        _timer = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        _spriteRenderer.material.SetFloat("_BouncingHeight", 0);
    }

    void UpdateFacing(float x)
    {
        if (Mathf.Abs(x) < 0.1f)
        {
            return;
        }

        if (x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    public void Launch(Vector3 direction)
    {
        _acceleration = direction * weaponData.speed;
        SetAim(direction);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        _localPosition = position;
        _rigidbody.position = position;
    }

    public void MeleeLaunch(Vector3 direction)
    {
        velocity = direction * weaponData.speed;
        SetAim(direction);
    }

    public void LaunchWithFacing(Vector3 direction)
    {
        velocity = direction * weaponData.speed;
        UpdateFacing(direction.x);
    }

    void SetAim(Vector3 direction)
    {
        if (weaponData.aimDirection)
        {
            float atan = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, atan - 90);
        }
    }

    public void SetDestination(Vector2 position)
    {
        destination = position;
    }

    public void UpdatePositionViaVelocity()
    {
        _rigidbody.position = _rigidbody.position + velocity * GameController.g.deltaTime;
    }

    public override void OnUpdate(float deltaTime)
    {
        if(Delay > 0.01f && GameController.g.elapsedTime - _delayTimer < Delay)
        {
            return;
        }

        if (weaponData != null && WeaponLogic != null)
        {
            WeaponLogic.OnUpdateWeapon(this);

            if (weaponData.spinSpeed > 0.01)
            {
                transform.RotateAround(transform.position, Vector3.forward, GameController.g.elapsedTime * weaponData.spinSpeed);
            }

            if (GameController.g.elapsedTime - _timer > weaponData.AliveTime)
            {
                Kill();
            }

            if(_alive && (int)HP <= 0 && !weaponData.invincible)
            {
                Kill();
            }
        }
    }

    public override void Revive()
    {
        _alive = true;
        gameObject.SetActive(true);
        _timer = GameController.g.elapsedTime;
        kineticSpeed = 0;
        origin = transform.position;
        velocity = Vector2.zero;
        _acceleration = Vector2.zero;
        _localPosition = Vector2.zero;
        kineticSpeed = 0;
        collisionOff = false;
        Delay = 0;
        _delayTimer = GameController.g.elapsedTime;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if (weaponData != null)
        {
            HP = weaponData.HP;
            circleCollider2D.radius = weaponData.radius;
            _spriteRenderer.sprite = weaponData.displayImage;
            if(weaponData.Style == WeaponData.WeaponStyles.Calvary)
            {
                _spriteRenderer.material.SetFloat("_BouncingHeight", 0.1f);
            }
            else
            {
                _spriteRenderer.material.SetFloat("_BouncingHeight", 0);
            }

        }
    }

    public void TakeDamage(float amount)
    {
        HP -= 1;
    }

    public override void Kill()
    {
        base.Kill();
    }

    public void MakeTransparent()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0.45f);
    }

    public void MakeSolid()
    {
        _spriteRenderer.color = Color.white;
    }
}
