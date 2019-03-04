using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBlocks;
using SerializableCollections;

public class WorldPath : WorldBlock
{
    static public WorldPath CreatePath()
    {
        var go = new GameObject("WorldPath");
        var wp = go.AddComponent<WorldPath>();
        wp.AddSurface();
        return wp;
    }

    public float m_pathWidth = 2.0f;
    public SerializableDictionary<int, float> m_pointWidths = new SerializableDictionary<int, float>();

    protected override EdgeLoop GetLoop(EdgeLoopOptions options = null)
    {
        var src = base.GetLoop(options);
        int srcPointCount = src.m_points.Count;
        if (srcPointCount < 2)
            return src;

        var dest = new EdgeLoop();
        dest.CopyLoop(src);
        dest.m_points.Clear();

        int destPointCount = srcPointCount * 2;
        var points = new Vector2[destPointCount];

        for (int i=0; i<srcPointCount; i++)
        {
            var prev = src.m_points[Mathf.Max(0, i - 1)];
            var curr = src.m_points[i];
            var next = src.m_points[Mathf.Min(i + 1, srcPointCount - 1)];

            var difP = curr - prev;
            var difN = next - prev;

            var angP = Mathf.Atan2(difP.y, difP.x);
            var angN = Mathf.Atan2(difN.y, difN.x);

            var ang = angP;
            if (i == 0)
            {
                ang = angN;
            }
            else if (i == srcPointCount - 1)
            {
                ang = angP;
            }
            else
            {
                ang = (angN + angP) * 0.5f;
                ang = Mathf.Repeat(ang + Mathf.PI, Mathf.PI * 2.0f) - Mathf.PI;
            }

            float pointWidth = m_pathWidth;
            if (m_pointWidths.ContainsKey(i))
            {
                pointWidth *= m_pointWidths[i];
            }

            ang += Mathf.PI * 0.5f;
            var off = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * pointWidth * 0.5f;

            int upI = i;
            points[upI] = curr - off;

            int downI = destPointCount - i - 1;
            points[downI] = curr + off;
        }

        dest.m_points.AddRange(points);

        return dest;
    }

    public override void CopyStyle(WorldBlock other)
    {
        if (other is WorldPath)
        {
            var wp = (WorldPath)other;
            m_pathWidth = wp.m_pathWidth;
        }
        base.CopyStyle(other);
    }
}
