using System;
using Script.Manager;
using UnityEngine;

public class EnnemyBehaviour : MonoBehaviour
{
    public int ticksBeforeAttack;
    private int ticksSinceSpawn = 0;
    public Transform playerPos;
    private Vector3 moveDistance;
    private bool isDead;

    private void OnEnable()
    {
        TickManager.OnTick += EnnemyAction;
    }

    private void OnDisable()
    {
        TickManager.OnTick -= EnnemyAction;
    }

    private void Start()
    {
        moveDistance = playerPos.position - transform.position;
    }

    private void EnnemyAction()
    {
        if (isDead)
        {
            //la mort
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        transform.position += moveDistance / (ticksBeforeAttack+1);
        ticksSinceSpawn++;
    }
}
