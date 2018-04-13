using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBlocks;

public class WorldBlockInverted : WorldBlock
{
    static public WorldBlockInverted CreateInverted()
    {
        var go = new GameObject("WorldBlockInverted");
        var wp = go.AddComponent<WorldBlockInverted>();
        wp.AddSurface();
        return wp;
    }

    public float m_borderSize = 10.0f;
    public SerializableDictionary<int, float> m_pointWidths = new SerializableDictionary<int, float>();

    protected override EdgeLoop GetLoop()
    {
        return base.GetLoop();
    }

    public override void CopyStyle(WorldBlock other)
    {
        if (other is WorldBlockInverted)
        {
            var wbi = (WorldBlockInverted)other;
            m_borderSize = wbi.m_borderSize;
        }
        base.CopyStyle(other);
    }
}
