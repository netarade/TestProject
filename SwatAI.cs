using System.Collections;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

public class SwatAI : MonoBehaviour
{
    private Transform playerTr;
    private Transform enemyTr;

    private float dist;
    private readonly float attackDist = 3.5f;
    private readonly float TraceDist = 10.0f;

    SwatAction swatActionScrt;
    SwatDamage swatDmgScrt;


    private int yieldCnt = 0;           // 코루틴 호출 횟수
    private int patrolRanPeriod;        // 아이들->패트롤 상태 활성화 최소 요구 시간
    private int idleRanPeriod;          // 패트롤->아이들 상태 활성화 최소 요구 시간

    public enum EnumState { NULL, IDLE, ATTACK, TRACE, PATROL, DIE };

    private EnumState state;
    public EnumState State { get { return state; } set { state = value; Debug.Log($"현재state : {state}");  } }

    private EnumState prevState;  // 이전 상태를 기록하기 위한 변수
    public EnumState PrevState { get { return prevState; } set { prevState = value; Debug.Log($"이전state: {prevState}"); } }

    private float wsTime; //코루틴 활성화 시간
    public float WsTime { get; }

    private bool isStateChanged = false;
    public bool IsStateChanged { get { return isStateChanged; } set { isStateChanged = value; Debug.Log("바꿨습니다."); } }

    Coroutine crtChkState;
    Coroutine crtActCall;




    void Awake()
    {
        enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;
        state = EnumState.IDLE;
        prevState = EnumState.NULL; //초기값을 달리줘야 로직 수행이 가능하다.
        swatActionScrt = GetComponent<SwatAction>();
        swatDmgScrt = GetComponent<SwatDamage>();

        patrolRanPeriod = Random.Range(20, 60);
        idleRanPeriod = Random.Range(50, 100);
        wsTime = 0.3f;

        Debug.Log($"아이들->패트롤, 수행까지 {patrolRanPeriod * wsTime}초 남았습니다.");


        StartCoroutine(Co_CheckState());
        StartCoroutine(Co_ActionCall());
    }









    IEnumerator Co_CheckState()
    {
        while (true)
        {
            dist = Vector3.Distance(enemyTr.position, playerTr.position);

            if (dist <= attackDist)
            {
                state = EnumState.ATTACK;
            }
            else if (dist <= TraceDist)
            {
                state = EnumState.TRACE;
            }
            else if (dist > TraceDist)
            {
                //선수실행: 아이들, 계속해서 거리가 먼상태라고 가정할때
                //아이들->패트롤 혹은 패트롤->아이들간의 전환이 이루어져야함. 

                if (state == EnumState.IDLE && yieldCnt >= patrolRanPeriod) //20~60번 양보했다면 (아이들을 어느정도 했다면)
                {
                    Debug.Log($"아이들->패트롤, 시행된 양보횟수:{yieldCnt}");

                    yieldCnt = 0;
                    patrolRanPeriod = Random.Range(20, 60);
                    state = EnumState.PATROL;

                    Debug.Log($"아이들->패트롤, 수행까지 {patrolRanPeriod * wsTime}초 남았습니다.");

                }
                else if (state == EnumState.PATROL && yieldCnt >= idleRanPeriod) //50~100번 양보했다면 (패트롤을 어느정도 했다면)
                {
                    Debug.Log($"패트롤->아이들, 시행된 양보횟수:{yieldCnt}");

                    yieldCnt = 0;
                    idleRanPeriod = Random.Range(50, 100);
                    state = EnumState.IDLE;

                    Debug.Log($"패트롤->아이들, 수행까지 {idleRanPeriod * wsTime}초 남았습니다.");

                }
                else if (state != EnumState.PATROL && state != EnumState.IDLE) //최초실행상태는 아이들이다. yieldCnt를 0으로 만들며,
                {                                                                        //그 이후는 실행되지 말아야 한다.(패트롤을 아이들로 바로 만들지 말아야 한다.)
                    Debug.Log($"공격,추적->아이들, 시행된 양보횟수:{yieldCnt}");

                    yieldCnt = 0;
                    state = EnumState.IDLE;
                }
            }

            if(swatDmgScrt.hp<=0f)
            {
                state = EnumState.DIE;
            }



            yield return new WaitForSeconds(wsTime);
            yieldCnt++;
        }
    }

    IEnumerator Co_ActionCall()
    {
        while (true)
        {
            yield return new WaitForSeconds(wsTime);


            if (state != prevState ) // state갱신이 이뤄졌을 때 한번만 수행한다.
            {
                isStateChanged = true;

                switch (state)
                {
                    case EnumState.ATTACK:
                        Debug.Log("attack");
                        swatActionScrt.Attack();
                        break;

                    case EnumState.TRACE:
                        Debug.Log("trace");
                        swatActionScrt.Trace();
                        break;

                    case EnumState.PATROL:
                        Debug.Log("patrol");
                        swatActionScrt.Patrol();
                        break;

                    case EnumState.IDLE:
                        Debug.Log("idle");
                        swatActionScrt.Idle();
                        break;
                    case EnumState.DIE:
                        Debug.Log("die");
                        swatActionScrt.Die();
                        break;
                }
                prevState = state;
            }            
            
                isStateChanged = false;

        }
    }







}