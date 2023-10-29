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
    public enum State //������ ��� �������� ���� ���
    {
        PATROL = 1, TRACE, ATTACK, DIE
    }
    public State state = State.PATROL;
    //��������� ���� ���� �� �⺻�� �ʱ�ȭ
    //��������� �˾Ƴ��� ���ؼ�

    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform enemyTr;
    [SerializeField] MoveAgent moveAgent;
    [SerializeField] public bool IsDie = false;
    [SerializeField] private float attackDist = 5.0f;
    [SerializeField] private float traceDist = 10.0f;

    private WaitForSeconds ws;


    [SerializeField] Animator animator;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    //��Ʈ�� ���� ������ ��ȯ�ؼ� �ӵ��� ������
    //�ִϸ����� �Ķ���� �ؽ� ���� �̸� �����Ѵ�.

    private readonly int hashSpeed = Animator.StringToHash("ForwardSpeed");


    //================================================
    //2023_0912_17:38
    [SerializeField] private EnemyFire enemyFire; //EnemyAi�� ������ �ְ�ޱ� ����

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
        //2023_0920 Awake���� hashƮ���� ���� 0���� �����Ǵ� ���� �߰�
        //animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f)); //Cycle Offset���� �ұ�Ģ�ϰ� ��, �ִϸ��̼� �������
        //animator.SetFloat(hashWalkSpeed, Random.Range(0.0f, 1.0f)); // �ִϸ��̼� ���ǵ�
    }



    // Awake�� Start
    // ������ : ���ӽ����� �� ���� ��Ǿ� �ʱ�ȭ�Ǵ� �����̴�.
    // Awake�� Start�� ������ Awake�� Start���� ���� ȣ��ȴ�.

    // AWake�� ��ũ��Ʈ ��Ȱ��ȭ�Ǿ ������ �����Ѵ�.
    // �� ���ӿ�����Ʈ�� Awake()���������� ����Ǳ� ������ ��ũ��Ʈ ���� ������ �����ϱ� ���� Awake�� ����ϰ�
    // ������ �޴� ��쿡�� Start()�Լ��� ����Ѵ�,
    // üũ�����ص� ��ƹ�����.



    private void OnEnable() //Start�Լ����� ����ȣ��Ǹ�, ������Ʈ�� Ȱ��ȭ�Ǿ����� ȣ��ȴ�.
    {
        //��ŸƮó�� �ѹ���ȣ��Ǵ� ����
        //������Ʈ�� ��ũ��Ʈ�� Ȱ��ȭ�ɶ� �ڵ����� ȣ��Ǵ� �ݹ��Լ�
        StartCoroutine(CheckState());
        StartCoroutine(StateAction());

        //2023-0918
        Damage.OnPlayerDie += this.OnPlayerDie; //�̺�Ʈ ����
        //BarrelCtrl.OnExpDie += this.Die;
        //
        
        //2023_0920
        //OnEanble�� �ű�� ����:Start���� ���� ȣ��ǹǷ�, Awake������ ���� 0���� �����Ǵ� ����
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f)); //Cycle Offset���� �ұ�Ģ�ϰ� ��, �ִϸ��̼� �������
        animator.SetFloat(hashWalkSpeed, Random.Range(0.0f, 1.0f)); // �ִϸ��̼� ���ǵ�   
    }

    //2023-0918
    private void OnDisable() //������Ʈ�� ��Ȱ��ȭ �� �� ȣ��
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
            yield return ws; //������ 0.3�� �纸�Ѵ�.
        }
    }

    IEnumerator StateAction()
    {
        while (!IsDie)
        {
            yield return ws; //���� 0.3�� �纸�Ѵ�

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
        this.gameObject.tag = "Untagged"; //5���� destroy�Ѵٸ� tag ������ �����ϰ� ���� ���̴�.
                
        //2023_0918
        GetComponent<Rigidbody>().isKinematic = true; //���� ������ ����                
        

        //2023-0920 ������Ʈ Ǯ��(Destroy �ؼ��� �ȵȴ�)
        //Destroy(gameObject, 5.0f); //�ּ�ó��
        StartCoroutine(PoolPush());
        
        //2023-1006
        GameManager.instance.InckillCount();

    }

    IEnumerator PoolPush()//�װ��� 3���Ŀ� �� ���� ȣ���.
    {
        yield return new WaitForSeconds( 3f );


        this.gameObject.SetActive(false); //�ٷ� ���ִ°� �ƴ϶� 3���� ���ش�.
        this.gameObject.tag = "ENEMY"; //�±� �ٽ� �ִ´�.  
        IsDie = false;
        state = State.TRACE;           // �״� �ִϸ��̼� �ݺ��ϴ� ����: Ȱ��ȭ �Ǿ��� �� �ʱ� state�� Trace���°� �Ǿ�� �Ѵ�.
        
        GetComponent<CapsuleCollider>().enabled = true;                     
        GetComponent<Rigidbody>().isKinematic = false; // ������ ����
    }



    private void Update()
    {
        //(������Ƽ)speed �Ķ���Ϳ� �̵��ӵ� ����
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    //2023_0915
    public void OnPlayerDie()
    {
        //2023-0919 �׾��µ� �ٽ� �Ͼ ���� �� �ʿ䰡 ����. ���ϸ� �ٽ� �Ͼ �����.
        if(IsDie) return;

        moveAgent.Stop();
        enemyFire.IsFire = false;
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);

        //2023-0918
        GameManager.instance.IsGameOver = true;
    }

}