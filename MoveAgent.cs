using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{


    [SerializeField] private List<Transform> wayPoints;
    public int nextIdx = 0; //�迭�� �ε���

    //public Transform[] Points;
    //public Transform parent;

    [SerializeField] private NavMeshAgent agent;
    Transform enemyTr;




    //2023-0912
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;

    private bool _patrolling; //�������θ� �Ǵ��ϴ� ����
    public bool patrolling
    {
        get { return _patrolling; }
        set 
        {
            _patrolling = value;

            //2023_0912_18:51 ��Ʈ�ѻ����� ȸ�����
            damping = 1.0f;

            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                MoveWayPoint();
            }
        }
    }

    private Vector3 _traceTarget;
    public Vector3 traceTrarget //������ȣ�� ������Ƽ
    {
        get { return _traceTarget; }
        set 
        { 
            _traceTarget = value;
            agent.speed = traceSpeed;


            //2023_0912_18:51 ���������� ȸ�����
            damping = 7.0f;


            TraceTarget(_traceTarget);
        }
    }

    public float speed //������ ������Ƽ (������ȣ��x)
    {
        get { return agent.velocity.magnitude; } //�ӵ��� ũ��
    }


    //2023_0912_16:49
    private float damping = 1.0f;
    [SerializeField] private EnemyAI enemyAI;







    void Start()
    {
        #region ������������ Ʈ�������� ��� ���
        //parent = GameObject.Find("PatrolPathLines").transform;
        //Points = parent.GetComponentsInChildren<Transform>();
        ////Points = GameObject.Find("PatrolPathLines").GetComponentsInChildren<Transform>();

        //for (int i = 0; i < Points.Length; i++)
        //{
        //    //if (Points[i] != transform) //�ڱ��ڽŻ��� ��ڴ�.
        //    if (Points[i] != parent) //�θ𻩰� ��ڴ�.
        //    {
        //        wayPoints[i] = Points[i]; //����Ʈ�� ��´�.
        //    }
        //} 
        #endregion

        //var group = GameObject.Find("PatrolPathLines");
        Transform group = GameObject.Find("PatrolPathLines").transform;
        if (group != null) //��ȿ���˻�
        {
            group.GetComponentsInChildren<Transform>(wayPoints); //����Ʈ �ּҸ� �־���. (����Ʈ�� ��´�.)
            wayPoints.RemoveAt(0); // �θ���� ���ԵǴ°� ����
        }

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;              //������ �����ϸ� �ӵ����� �ʿ䰡 ���� ����.(true:�ӵ����δ�)

        enemyTr = transform;


        //2023_0912_18:51 �ڵ�ȸ������� ��Ȱ��ȭ
        agent.updateRotation = false;




        MoveWayPoint();
    }
    //Find�� ������Ʈ���� �ǽð����� ���� ������. Start������ ����Ѵ�. (���̷�Ű�� ������Ʈ�� ã���Լ�)




    void Update()
    {
        //2023_0912
        if (!_patrolling) return;



        //2023_0912_16:53 //�������϶��� ȸ���ϱ�����
        if(agent.isStopped == false)
        {           //agent�� ������ ���⺤�͸� ���ʹϾ� Ÿ���� ������ ��ȯ
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }                                           //A->B�������� ���������� �ε巴�� ��ȯ�ȴ�.






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
        //�ִܰ�ΰ� �������ų� ����� �ȵǸ� �������� (�Ĺ����ְų�, patrol path�� ���ؿ� �־��ų�)
        if (agent.isPathStale) return;

        agent.destination = wayPoints[nextIdx].position;    // �����ǰ� ����
        agent.isStopped = false;                            // ���� ����
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
        //�ٷ� �����ϱ� ���� �ӵ��� 0���� ����
        _patrolling = false;
    }

}

