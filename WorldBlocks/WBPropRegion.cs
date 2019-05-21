using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBlocks;

public class WBPropRegion : BaseWorldBlock
{
    public GameObject[] props;
    [Range(0.3f, 20.0f)] public float distance = 3.0f;
    [Range(0, 1)] public float density = 1.0f;
    [Range(0, 1)] public float evenness = 0.5f;
    public bool grounded = true;
    [Range(0, 100)] public int seed = 0;
    Randomiser rand;
    Transform genParent;

    public void Redraw()
    {
        RequireShape();

        if (props == null || props.Length < 1)
            return;

        rand = new Randomiser(seed);

        genParent = Utils.GenerateChild(transform, "Props");

        float variance = distance * 0.5f * (1.0f - evenness);
        RaycastHit hitGround;

        var bounds = m_shape.GetBounds();
        for (float x = bounds.min.x + distance * 0.5f; x <= bounds.max.x; x += distance)
        {
            for (float z = bounds.min.z + distance * 0.5f; z <= bounds.max.z; z += distance)
            {
                var pos = new Vector3(x, 0, z);
                pos.x += rand.Range(-variance, variance);
                pos.z += rand.Range(-variance, variance);

                var rot = Quaternion.AngleAxis(rand.Range(360.0f), Vector3.up);
                var prefab = props[rand.Range(props.Length)];

                if (rand.Chance(density) == false)
                    continue;

                if (m_shape.IsInside(pos) == false)
                    continue;

                if (grounded && Physics.Raycast(pos + Vector3.up * 10.0f, Vector3.down, out hitGround, 20.0f))
                {
                    if (hitGround.transform.IsChildOf(genParent))
                        continue;

                    pos.y = hitGround.point.y;
                }

                var instance = Utils.InstantiateInto(prefab, genParent);
                instance.transform.position = pos;
                instance.transform.rotation = rot;
            }
        }
    }

    public override void SetDirty()
    {
        Redraw();
    }
}
