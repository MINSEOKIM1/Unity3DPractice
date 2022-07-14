using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float velocity;

    private void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * (velocity);
        Invoke("DeleteThis", 5f);
    }

    private void DeleteThis()
    {
        Destroy(gameObject);
    }
}
