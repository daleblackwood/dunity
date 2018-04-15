using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Conditions
{
    public enum Operand 
    {
        NUL,
        SET,
        EQ,
        NEQ,
        LT,
        LTE,
        GT,
        GTE,
    }

    [System.Serializable]
    public class Condition
    {
        public string key;
        public Operand operand;
        public string value;
    }

    public Condition[] conditions;
}