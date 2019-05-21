using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomiser 
{
    static public float RandomDecimal(int seed)
    {
        return new Randomiser(seed).Decimal();
    }

    public int seed { get; private set; }
    System.Random rand = null;

    public Randomiser(int seed)
    {
        this.seed = seed >= 0 ? seed : GenSeed();
        Reset();
    }

    public Randomiser()
    {
        seed = GenSeed();
        Reset();
    }

    int GenSeed()
    {
        return Guid.NewGuid().GetHashCode();
    }

    public void Reset()
    {
        rand = new System.Random(seed);
    }

    public int Range(int min, int max)
    {
        return rand.Next(min, max);
    }

    public int Range(int max)
    {
        return rand.Next(max);
    }

    public float Range(float max)
    {
        return Decimal() * max;
    }

    public float Range(float min, float max)
    {
        return (Decimal() * (max - min)) + min;
    }

    public float Decimal()
    {
        return (float)rand.NextDouble();
    }

    public float DecimalNeg()
    {
        return Decimal() * 2.0f - 1.0f;
    }

    public bool Chance(float dec = 0.5f)
    {
        return Decimal() <= dec;
    }

    public int Sort<T>(T a, T b)
    {
        return Chance() ? -1 : 0;
    }

    public Vector3 Direction()
    {
        return new Vector3(DecimalNeg(), DecimalNeg(), DecimalNeg()).normalized;
    }
}
