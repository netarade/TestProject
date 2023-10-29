using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Swat_AI : MonoBehaviour
{
    private Transform playerTr;
    private Transform enemyTr;

    private float dist;
    private readonly float attackDist = 3.5f;
    private readonly float TraceDist = 10.0f;
    private bool bCheckPatrol = false;
    Swat_Action swat_action_script;
    private int yieldCnt = 0;
    private int patrolRanPeriod;
    private int idleRanPeriod;


    private enum EnumState { IDLE, ATTACK, TRACE, PATROL };

    private int state;
    public int State
    {
        get { return state; }
        //set {
        //    if ((int)EnumState.IDLE <= value && value <= (int)EnumState.IDLE)
        //        state = value;
        //    else
        //        throw new System.Exception();
        //}
    }


    void Start()
    {
        enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;
        state = (int)EnumState.IDLE;
        swat_action_script = GetComponent<Swat_Action>();


        patrolRanPeriod = Random.Range(20, 60);
        idleRanPeriod = Random.Range(50, 100);

        StartCoroutine(Co_CheckState());
        StartCoroutine(Co_Action());
        Debug.Log($"{bCheckPatrol}");
    }

    //IEnumerator Co_CheckPatrol()
    //{
    //    int ranNum;
    //    while (true)
    //    {
    //        ranNum = Random.Range(0, 4);
    //        bCheckPatrol = (ranNum % 3) == 2;
    //        yield return new WaitForSeconds(3.0f);
    //    }
    //}

    IEnumerator Co_CheckState()
    {
        Debug.Log("Co_CheckState()");
        while (true)
        {
            dist = Vector3.Distance(enemyTr.position, playerTr.position);

            if (dist <= attackDist)
            {
                state = (int)EnumState.ATTACK;
            }
            else if (dist <= TraceDist)
            {
                state = (int)EnumState.TRACE;
            }
            else if (dist > TraceDist)
            {
                if (state == (int)EnumState.IDLE && yieldCnt >= patrolRanPeriod)
                {
                    Debug.Log("아이들->패트롤");
                    state = (int)EnumState.PATROL;
                    yieldCnt = 0;
                    patrolRanPeriod = Random.Range(20, 60);
                }
                else if (state == (int)EnumState.PATROL && yieldCnt >= idleRanPeriod)
                {
                    Debug.Log("패트롤->아이들");
                    state = (int)EnumState.IDLE;
                    yieldCnt = 0;
                    idleRanPeriod = Random.Range(50, 100);
                }
                else if (state != (int)EnumState.PATROL && state != (int)EnumState.IDLE)
                {
                    Debug.Log("공격,추적->아이들");
                    state = (int)EnumState.IDLE;
                    yieldCnt = 0;
                }
            }
            yield return new WaitForSeconds(0.3f);
            yieldCnt++;
            Debug.Log($"{yieldCnt}");
            
        }
    }

    IEnumerator Co_Action()
    {
        Debug.Log("Co_Action()");
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            switch(state)
            {
                case (int)EnumState.ATTACK:
                    Debug.Log("attack");
                    swat_action_script.Attack();
                    break;

                case (int)EnumState.TRACE:
                    Debug.Log("trace");
                    swat_action_script.Trace();
                    break;

                case (int)EnumState.PATROL:
                    Debug.Log("patrol");
                    swat_action_script.Patrol();
                    break;

                case (int)EnumState.IDLE:
                    Debug.Log("idle");
                    swat_action_script.Idle();
                    break;
            }
        }
    }






}