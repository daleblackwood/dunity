using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour 
{
    public Vector2 move;

    new MeshRenderer renderer;
    Material mat;
    Actor actor;

    void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        mat = renderer.sharedMaterial;

        actor = GetComponentInParent<Actor>();
    }

    void LateUpdate()
    {
        if (Game.isReady == false)
            return;

        bool enabled = actor.isInvincible && actor.invincibleMuted == false;
        if (enabled)
        {
            if (actor.invincibleTime < 0.5f)
            {
                enabled = Mathf.Repeat(actor.invincibleTime, 0.2f) < 0.05f;
            }
        }
        renderer.enabled = enabled;

        var moveOffset = move * Time.deltaTime;
        MaterialManager.main.AnimateTextureOffset(mat, moveOffset);
    }
}
