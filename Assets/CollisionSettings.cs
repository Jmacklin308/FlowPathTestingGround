using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSettings : MonoBehaviour
{
    [SerializeField] private bool reuseCollisionCallbacks;

    private void Start()
    {
        Physics.reuseCollisionCallbacks = reuseCollisionCallbacks;
    }
}
