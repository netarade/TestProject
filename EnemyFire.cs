using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


[RequireComponent(typeof(AudioSource))] //오디오소스 삭제하면 경고띄운다.
public class EnemyFire : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip fireSfx;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Position")] 
    [SerializeField] private Transform enemyTr;
    [SerializeField] private Transform playerTr;

    private readonly int hashFire = Animator.StringToHash("Fire");
    public bool IsFire = false;
    float nextFire = 0.0f;              //다음 발사 할 시간 계산용 변수
    readonly float fireRate = 0.1f;     //발사 간격
    readonly float damping = 10.0f;     //회전 속도

    [SerializeField] private GameObject e_bullet;
    [SerializeField] private Transform e_firePos;


    //2023_0913
    private readonly int hashReload = Animator.StringToHash("Reload");
    [Header("Reaload")]
    [SerializeField] private readonly float reloadTime = 2.0f;      // 재장전시간
    [SerializeField] private int maxBullet = 10;                    // 탄환수
    [SerializeField] private int currentBullet = 10;                // 탄환수
    [SerializeField] private bool IsReload = false;                 // 재장전여부
    [SerializeField] private WaitForSeconds wsReload;               // 재장전시간 동안 기다릴 변수
    [SerializeField] private AudioClip reloadSfx;                   // 재장전 사운드


    //2023_0915
    private readonly int hashOffset = Animator.StringToHash("Offset");


    void Start()
    {
        source = GetComponent<AudioSource>();
        fireSfx = Resources.Load("Sounds/p_ak_1") as AudioClip ;

        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        e_bullet = Resources.Load<GameObject>("E_Bullet");
        e_firePos = transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Transform>();

        //2023_0913
        reloadSfx = Resources.Load<AudioClip>("Sounds/p_reload");
        wsReload = new WaitForSeconds(reloadTime);


    }

    void Update()
    {
        if(!IsReload && IsFire) //2023_0913 !IsReload 추가
        {
            if(Time.time > nextFire)
            {

                //2023_0915
                animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));

                Fire();
                nextFire = Time.time + Random.Range(fireRate, 0.25f);
            }

            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position); //타겟위치에서-자신의위치빼면 거리가 나온다.: 벡터값을 각도로 바꿔줌.  
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping); //자신위치에서 타겟 각까지 부드럽게 회전
        }
    }






    public void Fire()
    {
        //2023_0913
        //불변수도 넘버와 연산자로 조건을 만들어 줘서 true false로 만들 수 있다.
        IsReload = (--currentBullet%maxBullet)==0; 
        if(IsReload)//IsReload가 true가 된시점에
        {
            StartCoroutine(Reloading());
        }



        animator.SetTrigger(hashFire);
        source.PlayOneShot(fireSfx, 1.0f);

        //2023-0919 오브젝트 풀링 미리 생성안함.
        //Instantiate(e_bullet, e_firePos.position, e_firePos.rotation);
        var bulletObj = ObjectPoolingManager.poolmanager.GetEnemyBullet();
        if( bulletObj != null )
        {
            bulletObj.transform.position = e_firePos.position;
            bulletObj.transform.rotation = e_firePos.rotation;
            bulletObj.SetActive(true);
            StartCoroutine(BulletDestroy(bulletObj));
        }
    }


    //2023_0913
    IEnumerator Reloading()
    {
        animator.SetTrigger(hashReload);
        source.PlayOneShot(reloadSfx, 1.0f);
        yield return new WaitForSeconds(reloadTime);
        currentBullet = maxBullet;
        IsReload = false;
        
    }


    IEnumerator BulletDestroy(GameObject _bullet)
    {
        yield return new WaitForSeconds(3.0f);
        _bullet.SetActive(false);
    }

}
