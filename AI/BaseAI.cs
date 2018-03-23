using UnityEngine;
using System.Collections.Generic;
using Behave;

public abstract class BaseAI : BaseControl
{
    protected MinigameStage gameStage { get; private set; }

    public Vector3 targetPos = Vector3.zero;
    Vector3 lerpedTargetPos = Vector3.zero;

    public override void Reset()
    {
        gameStage = GameObject.FindObjectOfType<MinigameStage>();
        if (gameStage == null)
        {
            Debug.LogError("AI couldn't find game.");
            return;
        }
        targetPos = Vector3.zero;

        Game.WhenReady(OnReady);
        base.Reset();
    }

    void OnEnable()
    {
        Game.WhenReady(OnReady);
    }

    public virtual void OnReady() {}

    protected override void UpdateActions()
    {
        targetPos.y = transform.position.y;

        if (targetPos == Vector3.zero)
        {
            lerpedTargetPos = targetPos;
        }
        else
        {
            lerpedTargetPos = Vector3.Lerp(lerpedTargetPos, targetPos, MathUtils.DeltaEase(0.3f));
        }

        if ((lerpedTargetPos - transform.position).sqrMagnitude < 1.0f)
        {
            actions.move = Vector2.zero;
        }
        else
        {
            var inputDif = lerpedTargetPos - transform.position;
            var input = GetInput(inputDif);
            actions.move = input;
        }

        if (Application.isEditor)
        {
            var color = Color.HSVToRGB(player.index * 0.2f, 1.0f, 1.0f);
            Debug.DrawLine(transform.position + Vector3.up * 0.5f, lerpedTargetPos + Vector3.up * 0.5f, color);
            DebugTools.DrawSphere(lerpedTargetPos + Vector3.up * 0.5f, color, 0.5f);
        }
    }

    protected virtual Vector2 GetInput(Vector3 move)
    {
        move.y = 0.0f;
        move.Normalize();

        var camT = Camera.main.transform;
        move = Quaternion.Inverse(camT.rotation) * move;

        var input = new Vector2(move.x, move.z);
        input.Normalize();
        return input;
    }

    protected Actor[] GetTeammates()
    {
        var result = new List<Actor>();
        var players = gameStage.GetPlayers();
        foreach (var p in players)
        {
            if (p == player)
                continue;

            if (p.faction != player.faction)
                continue;

            result.Add(p);
        }
        return result.ToArray();
    }
}
