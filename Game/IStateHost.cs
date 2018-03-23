using UnityEngine;
using System.Collections;

public interface IStateHost
{
    void PickState();
    Coroutine StartCoroutine(IEnumerator ienumerator);
    void StopAllCoroutines();
}
