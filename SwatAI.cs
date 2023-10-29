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


    private int yieldCnt = 0;           // �ڷ�ƾ ȣ�� Ƚ��
    private int patrolRanPeriod;        // ���̵�->��Ʈ�� ���� Ȱ��ȭ �ּ� �䱸 �ð�
    private int idleRanPeriod;          // ��Ʈ��->���̵� ���� Ȱ��ȭ �ּ� �䱸 �ð�

    public enum EnumState { NULL, IDLE, ATTACK, TRACE, PATROL, DIE };

    private EnumState state;
    public EnumState State { get { return state; } set { state = value; Debug.Log($"����state : {state}");  } }

    private EnumState prevState;  // ���� ���¸� ����ϱ� ���� ����
    public EnumState PrevState { get { return prevState; } set { prevState = value; Debug.Log($"����state: {prevState}"); } }

    private float wsTime; //�ڷ�ƾ Ȱ��ȭ �ð�
    public float WsTime { get; }

    private bool isStateChanged = false;
    public bool IsStateChanged { get { return isStateChanged; } set { isStateChanged = value; Debug.Log("�ٲ���ϴ�."); } }

    Coroutine crtChkState;
    Coroutine crtActCall;




    void Awake()
    {
        enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;
        state = EnumState.IDLE;
        prevState = EnumState.NULL; //�ʱⰪ�� �޸���� ���� ������ �����ϴ�.
        swatActionScrt = GetComponent<SwatAction>();
        swatDmgScrt = GetComponent<SwatDamage>();

        patrolRanPeriod = Random.Range(20, 60);
        idleRanPeriod = Random.Range(50, 100);
        wsTime = 0.3f;

        Debug.Log($"���̵�->��Ʈ��, ������� {patrolRanPeriod * wsTime}�� ���ҽ��ϴ�.");


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
                //��������: ���̵�, ����ؼ� �Ÿ��� �ջ��¶�� �����Ҷ�
                //���̵�->��Ʈ�� Ȥ�� ��Ʈ��->���̵鰣�� ��ȯ�� �̷��������. 

                if (state == EnumState.IDLE && yieldCnt >= patrolRanPeriod) //20~60�� �纸�ߴٸ� (���̵��� ������� �ߴٸ�)
                {
                    Debug.Log($"���̵�->��Ʈ��, ����� �纸Ƚ��:{yieldCnt}");

                    yieldCnt = 0;
                    patrolRanPeriod = Random.Range(20, 60);
                    state = EnumState.PATROL;

                    Debug.Log($"���̵�->��Ʈ��, ������� {patrolRanPeriod * wsTime}�� ���ҽ��ϴ�.");

                }
                else if (state == EnumState.PATROL && yieldCnt >= idleRanPeriod) //50~100�� �纸�ߴٸ� (��Ʈ���� ������� �ߴٸ�)
                {
                    Debug.Log($"��Ʈ��->���̵�, ����� �纸Ƚ��:{yieldCnt}");

                    yieldCnt = 0;
                    idleRanPeriod = Random.Range(50, 100);
                    state = EnumState.IDLE;

                    Debug.Log($"��Ʈ��->���̵�, ������� {idleRanPeriod * wsTime}�� ���ҽ��ϴ�.");

                }
                else if (state != EnumState.PATROL && state != EnumState.IDLE) //���ʽ�����´� ���̵��̴�. yieldCnt�� 0���� �����,
                {                                                                        //�� ���Ĵ� ������� ���ƾ� �Ѵ�.(��Ʈ���� ���̵�� �ٷ� ������ ���ƾ� �Ѵ�.)
                    Debug.Log($"����,����->���̵�, ����� �纸Ƚ��:{yieldCnt}");

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


            if (state != prevState ) // state������ �̷����� �� �ѹ��� �����Ѵ�.
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