using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Gold,
    Silver,
    Heart,
    Bomb,
    Mine,
    Key,
    Time,
    Buck,
    Shovel,
    Sword,
    MAX
}

public class Item : Entity
{
    public ItemType itemType;
    public FXType fxType = FXType.None;
    public int count = 1;
    public bool focus = false;
    public bool saves = false;
    public bool collected { get { return collector != null; } }
    public bool uncollectable = false;
    ItemCollector collector = null;
    float collectSpeed = 0.0f;

    protected override void Awake()
    {
        isTargetable = true;
        base.Awake();
    }

    public void Collect(ItemCollector collector)
    {
        if (timeSinceReady < 1.5f)
            return;

        if (this.collector == collector)
            return;
        
        this.collector = collector;
        collectSpeed = 0.0f;
        OnCollect();
    }

    protected virtual void OnCollect()
    {
    }

    protected virtual void OnFocusComplete()
    {
    }

    protected virtual void Update()
    {
        if (collector != null)
        {
            Vector3 targetPos = collector.transform.position + Vector3.up * 1.0f;
            Vector3 dif = targetPos - transform.position;
            Vector3 move = Vector3.ClampMagnitude(dif, collectSpeed);

            if (move.sqrMagnitude < dif.sqrMagnitude)
            {
                transform.position += move;
                collectSpeed += Time.deltaTime;
            }
            else
            {
                CollectComplete();
            }
        }

        if (itemType == ItemType.Heart && Game.isReady)
        {
            //isTargetable = Game.Instance.player.health < Game.main.player.maxHealth;
        }
    }

    protected virtual void CollectComplete()
    {
        /*
        Game.Instance.saver.saveGame.SetCollected(this);
        if (saves)
        {
            Game.Instance.saver.Save();
        }
        */
        gameObject.SetActive(false);
        SendFX();
    }

    protected virtual void SendFX()
    {
        if (fxType != FXType.None)
        {
            FXer.main.Fire(fxType, transform.position);
        }
    }

    public void Uncollect()
    {
        if (uncollectable == false)
            return;
        
        collector = null;
        OnUncollect();
    }

    protected virtual void OnUncollect()
    {
        gameObject.SetActive(true);
    }
}
