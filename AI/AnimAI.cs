using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAI : MonoBehaviour
{
    const string MACHINE_NAME = "BehaviourMachine";

    public RuntimeAnimatorController controller;
    public Actor actor { get; protected set; }
    public Animator animator { get; protected set; }

    void Awake()
    {
        Require();
    }

    public void Require()
    {
        actor = GetComponent<Actor>();
        if (animator == null)
        {
            Transform controllerT = transform.Find(MACHINE_NAME);
            if (controllerT == null)
            {
                controllerT = new GameObject(MACHINE_NAME).transform;
            }
            animator = controllerT.GetComponent<Animator>();
        }
    }
}
