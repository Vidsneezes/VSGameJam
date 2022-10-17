using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Poolable
{
    public ItemData itemData;

    public float AttractSpeed = 20;
    public float Gravity = 2;
    public AudioSource audioSource;

    [HideInInspector]
    public HeroController Hero;

    private SpriteRenderer _spriteRenderer;
    private Vector2 velocity;
    private float _timer;
    private bool _isBeingCollected;
    private float speedGravity;

    public override void SetUp()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (_isBeingCollected && Hero != null)
        {
            speedGravity += Mathf.Clamp(speedGravity + deltaTime * Gravity, 0f, 1f);
            Vector3 position = transform.position;

            position = Vector2.MoveTowards(position, Hero.transform.position, deltaTime * AttractSpeed * speedGravity);
            if((Hero.transform.position - position).sqrMagnitude < 0.1f)
            {
                Hero.AddXpToPool((int)itemData.AmountXP);
                Hero.CollectHealth(itemData.AmountHp);
                StateManager.g.globalAudioSource.PlayOneShot(audioSource.clip);
                switch(itemData.ability)
                {
                    case ItemData.SpecialAbility.Eradicate:
                        GameController.g.Monsters.EradicateAbility();
                        break;
                }
                _alive = false;
            }
            transform.position = position;
        }
    }

    public override void Revive()
    {
        base.Revive();
        Hero = null;
        _isBeingCollected = false;
        _spriteRenderer.sprite = itemData.displayImage;
    }

    public override void Kill()
    {
        _isBeingCollected = true;
    }

}