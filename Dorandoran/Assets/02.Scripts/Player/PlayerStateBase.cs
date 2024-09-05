using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : MonoBehaviour
{
    public float moveSpeed = 7;
    public float rotSpeed = 300;
    public float maxHealth = 100;

    protected float currentHealth = 0;


    void Start()
    {

    }
}
