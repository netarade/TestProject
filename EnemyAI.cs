using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Animator))]

public class EnemyAI : MonoBehaviour
{
    public enum State //열거형 상수 가독성을 위해 사용
    {
        PATROL = 1, TRACE, ATTACK, DIE
    }
    public State state = State.PATROL;
    //열거형상수 변수 선언 및 기본값 초기화
    //어떤상태인지 알아내기 위해서

    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform enemyTr;
    [SerializeField] MoveAgent moveAgent;
    [SerializeField] public bool IsDie = false;
    [SerializeField] private float attackDist = 5.0f;
    [SerializeField] private float traceDist = 10.0f;

    private WaitForSeconds ws;


    [SerializeField] Animator animator;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    //스트링 값을 정수로 변환해서 속도를 빠르게
    //애니메이터 파라미터 해시 값을 미리 추출한다.

    private readonly int hashSpeed = Animator.StringToHash("ForwardSpeed");


    //================================================
    //2023_0912_17:38
    [SerializeField] private EnemyFire enemyFire; //EnemyAi와 정보를 주고받기 위해

    //2023-0913
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");


    //2023_0915
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashOffset = Animator.StringToHash("Offset");

    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");




    void Awake()
    {
        

        playerTr = GameObject.FindWithTag("Player").transform;
        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        ws = new WaitForSeconds(0.3f);

        animator = GetComponent<Animator>();

        //2023_0912_17:38
        enemyFire = GetComponent<EnemyFire>();

        
        //2023_0915 
        //2023_0920 Awake에서 hash트리의 값이 0으로 고정되는 문제 발견
        //animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f)); //Cycle Offset값을 불규칙하게 줌, 애니메이션 재생간격
        //animator.SetFloat(hashWalkSpeed, Random.Range(0.0f, 1.0f)); // 애니메이션 스피드
    }



    // Awake와 Start
    // 공통점 : 게임시작전 한 번만 출되어 초기화되는 공간이다.
    // Awake와 Start의 차이점 Awake가 Start보다 먼저 호출된다.

    // AWake는 스크립트 비활성화되어도 무조건 실행한다.
    // 단 게임오브젝트의 Awake()랜덤순서로 실행되기 때문에 스크립트 간의 참조를 설정하기 위해 Awake를 사용하고
    // 정보를 받는 경우에는 Start()함수를 사용한다,
    // 체크해제해도 잡아버린다.



    private void OnEnable() //Start함수보다 먼저호출되며, 오브젝트가 활성화되었을때 호출된다.
    {
        //스타트처럼 한번만호출되는 공간
        //오브젝트나 스크립트간 활성화될때 자동으로 호출되는 콜백함수
        StartCoroutine(CheckState());
        StartCoroutine(StateAction());

        //2023-0918
        Damage.OnPlayerDie += this.OnPlayerDie; //이벤트 연결
        //BarrelCtrl.OnExpDie += this.Die;
        //
        
        //2023_0920
        //OnEanble로 옮기는 이유:Start보다 먼저 호출되므로, Awake에서는 값이 0으로 고정되는 문제
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f)); //Cycle Offset값을 불규칙하게 줌, 애니메이션 재생간격
        animator.SetFloat(hashWalkSpeed, Random.Range(0.0f, 1.0f)); // 애니메이션 스피드   
    }

    //2023-0918
    private void OnDisable() //오브젝트가 비활성화 될 때 호출
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
        //BarrelCtrl.OnExpDie -= this.Die;
    }



    IEnumerator CheckState()
    {
        while (!IsDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(enemyTr.position, playerTr.position);
            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (dist < traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            yield return ws; //돌리고 0.3초 양보한다.
        }
    }

    IEnumerator StateAction()
    {
        while (!IsDie)
        {
            yield return ws; //먼저 0.3초 양보한다

            //2023-0918
            if(GameManager.instance.IsGameOver) yield break;


            switch (state)
            {
                case State.PATROL:
                    enemyFire.IsFire = false; //2023_0912_17:38
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    enemyFire.IsFire = false; //2023_0912_17:38
                    moveAgent.traceTrarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    enemyFire.IsFire = true; //2023_0912_17:38
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;

                case State.DIE:
                    Die();//2023_0913
                    break;
            }


        }

    }

    public void Die()
    {
        StopAllCoroutines(); //2023-0919
        enemyFire.IsFire = false; //2023_0912_17:38
        moveAgent.Stop();

        IsDie = true; //2023_0913
        animator.SetInteger(hashDieIdx, Random.Range(0, 3));
        animator.SetTrigger(hashDie);
        GetComponent<CapsuleCollider>().enabled = false;

        //2023_09_15
        this.gameObject.tag = "Untagged"; //5초후 destroy한다면 tag 갯수를 차지하고 있을 것이다.
                
        //2023_0918
        GetComponent<Rigidbody>().isKinematic = true; //남은 물리력 제거                
        

        //2023-0920 오브젝트 풀링(Destroy 해서는 안된다)
        //Destroy(gameObject, 5.0f); //주석처리
        StartCoroutine(PoolPush());
        
        //2023-1006
        GameManager.instance.InckillCount();

    }

    IEnumerator PoolPush()//죽고나서 3초후에 한 번만 호출됨.
    {
        yield return new WaitForSeconds( 3f );


        this.gameObject.SetActive(false); //바로 꺼주는게 아니라 3초후 꺼준다.
        this.gameObject.tag = "ENEMY"; //태그 다시 넣는다.  
        IsDie = false;
        state = State.TRACE;           // 죽는 애니메이션 반복하는 문제: 활성화 되었을 때 초기 state가 Trace상태가 되어야 한다.
        
        GetComponent<CapsuleCollider>().enabled = true;                     
        GetComponent<Rigidbody>().isKinematic = false; // 물리력 복원
    }



    private void Update()
    {
        //(프로퍼티)speed 파라미터에 이동속도 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    //2023_0915
    public void OnPlayerDie()
    {
        //2023-0919 죽었는데 다시 일어나 춤을 출 필요가 없다. 안하면 다시 일어나 춤춘다.
        if(IsDie) return;

        moveAgent.Stop();
        enemyFire.IsFire = false;
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);

        //2023-0918
        GameManager.instance.IsGameOver = true;
    }

}