using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBlocks;
using SerializableCollections;

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

    public override EdgeLoop ProcessLoop(EdgeLoop loop, EdgeLoopOptions options = null)
    {
        loop = base.ProcessLoop(loop, options);
        if (options.surface == false)
        {
            return loop;
        }

        var result = new EdgeLoop();
        result.CopyLoop(loop);
        var rect = loop.GetRect();

        rect.xMin -= m_borderSize;
        rect.xMax += m_borderSize;
        rect.yMin -= m_borderSize;
        rect.yMax += m_borderSize;

        var tl = new Vector2(rect.xMin, rect.yMin);
        var tr = new Vector2(rect.xMax, rect.yMin);
        var bl = new Vector2(rect.xMin, rect.yMax);
        var br = new Vector2(rect.xMax, rect.yMax);

        var inPoints = new List<Vector2>(loop.m_points);
        inPoints.Reverse();

        var topI = 0;
        for (var i=0; i<inPoints.Count; i++)
        {
            if (inPoints[i].y < inPoints[topI].y)
            {
                topI = i;
            }
        }

        var highP = inPoints[topI];
        var topP = new Vector2(highP.x, tl.y);

        var points = new List<Vector2>();
        points.Add(tr);
        points.Add(br);
        points.Add(bl);
        points.Add(tl);
        points.Add(topP);

        for (var i = 0; i < inPoints.Count; i++)
        {
            int index = (i + topI) % inPoints.Count;
            points.Add(inPoints[index]);
        }

        points.Add(highP);
        points.Add(topP);

        result.m_points = points;

        return result;
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
