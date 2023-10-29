using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SwatAction : MonoBehaviour
{
    private NavMeshAgent navi;
    private Transform[] trArr;
    private int nextIdx = 1;

    private Transform playerTr;
    //private Transform enemyTr;
    Animator animator;

    SwatAI swaitAiScrt;
    SwatFire swatFireScrt;

    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");

    public delegate void StopYourCoroutine();
    public static event StopYourCoroutine StopFireCoroutine;



    void Start()
    {
        //enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;
        trArr = GameObject.Find("PatrolPathLines").GetComponentsInChildren<Transform>();
        navi = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        swaitAiScrt = GetComponent<SwatAI>();
        swatFireScrt = GetComponent<SwatFire>();



        animator.SetFloat(hashWalkSpeed, Random.Range(0.0f, 1.0f));
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        



        navi.isStopped = true;
        navi.speed = 2.5f;
    }

    //2023-0918
    private void OnEnable()
    {
        Damage.OnPlayerDie+=this.OnPlayerDie;
        //BarrelCtrl.OnExpDie-=this.Die;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie-=this.OnPlayerDie;
        //BarrelCtrl.OnExpDie-=this.Die;
    }





    public void Patrol()
    {
        Debug.Log("Patrol");

        navi.isStopped = false;
        navi.destination = trArr[nextIdx].transform.position;
        navi.speed = 2.5f;
        animator.SetFloat("speed", 0.5f);

        StartCoroutine(Co_PatrolNext());    // 다음 패스파인딩 자체적 반복루틴
    }

    IEnumerator Co_PatrolNext()
    {
        while (true)
        {
            if (swaitAiScrt.IsStateChanged)
                yield break;

            if (navi.remainingDistance<=0.25f)
            {
                if (nextIdx == trArr.Length - 1)
                    nextIdx = 1;

                nextIdx++;
                navi.destination = trArr[nextIdx].transform.position; 
            }

            yield return new WaitForSeconds(0.3f);
        }
    }


    public void Trace()
    {
        navi.isStopped = false;
        navi.destination = playerTr.position;
        navi.speed = 5f;
        animator.SetFloat("speed", 1f);
    }
    public void Attack()
    {
        navi.isStopped = true;
        navi.speed = 2.5f;

        animator.SetBool("IsFire", true);
        StartCoroutine(swatFireScrt.Co_Fire()); //코루틴에서 IsFire을 false로 만든다.
    }
    public void Idle()
    {
        NaviStop();
        animator.SetFloat("speed", 0f);
    }
    public void Die()
    {
        StopAllCoroutines();
        StopFireCoroutine();
        //StopCoroutine(fireCrt);

        int ranInt = Random.Range(0, 3);
        NaviStop();
        animator.SetInteger("IsDieInt", ranInt);
        animator.SetTrigger("IsDieTrig");

        //2023_09_15
        this.gameObject.tag="Untagged"; //5초후 destroy한다면 tag 갯수를 차지하고 있을 것이다.
        Destroy( gameObject, 5.0f );

        //2023_0918
        GetComponent<Rigidbody>().isKinematic=true;
    }


    //2023_09_15
    public void OnPlayerDie()
    {
        NaviStop();
        StopAllCoroutines();
        StopFireCoroutine();
        animator.SetTrigger("PlayerDie");
    }

    public void NaviStop()
    {
        navi.isStopped = true;
        navi.velocity = Vector3.zero;
    }




    
    
    
    
    
    

    
    
    
    
    





}