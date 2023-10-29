using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{


    [SerializeField] private List<Transform> wayPoints;
    public int nextIdx = 0; //배열의 인덱스

    //public Transform[] Points;
    //public Transform parent;

    [SerializeField] private NavMeshAgent agent;
    Transform enemyTr;




    //2023-0912
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;

    private bool _patrolling; //순찰여부를 판단하는 변수
    public bool patrolling
    {
        get { return _patrolling; }
        set 
        {
            _patrolling = value;

            //2023_0912_18:51 패트롤상태의 회전계수
            damping = 1.0f;

            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                MoveWayPoint();
            }
        }
    }

    private Vector3 _traceTarget;
    public Vector3 traceTrarget //원본보호용 프로퍼티
    {
        get { return _traceTarget; }
        set 
        { 
            _traceTarget = value;
            agent.speed = traceSpeed;


            //2023_0912_18:51 추적상태의 회전계수
            damping = 7.0f;


            TraceTarget(_traceTarget);
        }
    }

    public float speed //독자적 프로퍼티 (원본보호용x)
    {
        get { return agent.velocity.magnitude; } //속도의 크기
    }


    //2023_0912_16:49
    private float damping = 1.0f;
    [SerializeField] private EnemyAI enemyAI;







    void Start()
    {
        #region 예전버전에서 트랜스폼을 담는 방법
        //parent = GameObject.Find("PatrolPathLines").transform;
        //Points = parent.GetComponentsInChildren<Transform>();
        ////Points = GameObject.Find("PatrolPathLines").GetComponentsInChildren<Transform>();

        //for (int i = 0; i < Points.Length; i++)
        //{
        //    //if (Points[i] != transform) //자기자신빼고 담겠다.
        //    if (Points[i] != parent) //부모빼고 담겠다.
        //    {
        //        wayPoints[i] = Points[i]; //리스트에 담는다.
        //    }
        //} 
        #endregion

        //var group = GameObject.Find("PatrolPathLines");
        Transform group = GameObject.Find("PatrolPathLines").transform;
        if (group != null) //유효성검사
        {
            group.GetComponentsInChildren<Transform>(wayPoints); //리스트 주소를 넣었다. (리스트에 담는다.)
            wayPoints.RemoveAt(0); // 부모까지 포함되는것 방지
        }

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;              //목적지 도착하면 속도줄일 필요가 없기 때문.(true:속도줄인다)

        enemyTr = transform;


        //2023_0912_18:51 자동회전기능을 비활성화
        agent.updateRotation = false;




        MoveWayPoint();
    }
    //Find는 업데이트에서 실시간으로 쓰면 느리다. Start에서만 써야한다. (하이러키의 오브젝트를 찾는함수)




    void Update()
    {
        //2023_0912
        if (!_patrolling) return;



        //2023_0912_16:53 //추적중일때만 회전하기위해
        if(agent.isStopped == false)
        {           //agent가 가야할 방향벡터를 쿼터니언 타입의 각도로 반환
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }                                           //A->B각도까지 점진적으로 부드럽게 반환된다.






        //float remainDist = Vector3.Distance(enemyTr.position, wayPoints[nextIdx].position);
        //if(remainDist <= 0.25f)

        if (agent.remainingDistance <= 0.25f)        
        {
            //2023_0915
            //nextIdx = ++nextIdx % wayPoints.Count;
            nextIdx = Random.Range(0, wayPoints.Count);

            MoveWayPoint();
        }

    }

    void MoveWayPoint()
    {
        //2023-0912
        //최단경로가 안잡히거나 계산이 안되면 빠져나감 (파묻혀있거나, patrol path를 땅밑에 넣었거나)
        if (agent.isPathStale) return;

        agent.destination = wayPoints[nextIdx].position;    // 포지션값 대입
        agent.isStopped = false;                            // 추적 시작
    }



    //2023_0912
    private void TraceTarget(Vector3 pos)
    {
        if(agent.isPathStale)
            return;



        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        //바로 정지하기 위해 속도를 0으로 설정
        _patrolling = false;
    }

}

