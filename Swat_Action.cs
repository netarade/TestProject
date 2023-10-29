using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Swat_Action : MonoBehaviour
{
    private NavMeshAgent navi;
    private Transform[] trArr;
    private int nextIdx = 1;

    private Transform playerTr;
    private Transform enemyTr;
    Animator animator;
    Swat_Fire swatFireScrt;

    void Start()
    {
        enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;
        trArr = GameObject.Find("PatrolPathLines").GetComponentsInChildren<Transform>();
        navi = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        swatFireScrt = GetComponent<Swat_Fire>();

        navi.isStopped = true;
        navi.speed = 2.5f;
    }

    public void Patrol()
    {
        Debug.Log("Patrol");
        navi.isStopped = false;
        navi.destination = trArr[nextIdx].transform.position;
        navi.speed = 2.5f;
        animator.SetFloat("speed", 0.5f);

        if (navi.remainingDistance >= 0.25f)
            return;
        else if (navi.remainingDistance < 0.25f && nextIdx == trArr.Length - 1)
            nextIdx = 1;

        nextIdx++;
        navi.destination = trArr[nextIdx].transform.position;
    }

    public void Trace()
    {
        Debug.Log("Trace");
        navi.isStopped = false;
        navi.destination = playerTr.position;
        navi.speed = 5f;
        animator.SetFloat("speed", 1f);
    }
    public void Attack()
    {
        Debug.Log("Attack");
        navi.isStopped = true;
        navi.speed = 2.5f;
        animator.SetFloat("speed", 0.5f);

        swatFireScrt.IsFire = true;
    }
    public void Idle()
    {
        Debug.Log("Idle");
        navi.isStopped = true;
        navi.speed = 5f;
        animator.SetFloat("speed", 0f);
    }



}
