using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBlocks;

[RequireComponent(typeof(WBShape))]
public class PropRegion : MonoBehaviour
{
    public GameObject[] props;
    public float density = 3.0f;
}
