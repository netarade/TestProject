using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


[RequireComponent(typeof(AudioSource))] //������ҽ� �����ϸ� ������.
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
    float nextFire = 0.0f;              //���� �߻� �� �ð� ���� ����
    readonly float fireRate = 0.1f;     //�߻� ����
    readonly float damping = 10.0f;     //ȸ�� �ӵ�

    [SerializeField] private GameObject e_bullet;
    [SerializeField] private Transform e_firePos;


    //2023_0913
    private readonly int hashReload = Animator.StringToHash("Reload");
    [Header("Reaload")]
    [SerializeField] private readonly float reloadTime = 2.0f;      // �������ð�
    [SerializeField] private int maxBullet = 10;                    // źȯ��
    [SerializeField] private int currentBullet = 10;                // źȯ��
    [SerializeField] private bool IsReload = false;                 // ����������
    [SerializeField] private WaitForSeconds wsReload;               // �������ð� ���� ��ٸ� ����
    [SerializeField] private AudioClip reloadSfx;                   // ������ ����


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
        if(!IsReload && IsFire) //2023_0913 !IsReload �߰�
        {
            if(Time.time > nextFire)
            {

                //2023_0915
                animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));

                Fire();
                nextFire = Time.time + Random.Range(fireRate, 0.25f);
            }

            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position); //Ÿ����ġ����-�ڽ�����ġ���� �Ÿ��� ���´�.: ���Ͱ��� ������ �ٲ���.  
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping); //�ڽ���ġ���� Ÿ�� ������ �ε巴�� ȸ��
        }
    }






    public void Fire()
    {
        //2023_0913
        //�Һ����� �ѹ��� �����ڷ� ������ ����� �༭ true false�� ���� �� �ִ�.
        IsReload = (--currentBullet%maxBullet)==0; 
        if(IsReload)//IsReload�� true�� �Ƚ�����
        {
            StartCoroutine(Reloading());
        }



        animator.SetTrigger(hashFire);
        source.PlayOneShot(fireSfx, 1.0f);

        //2023-0919 ������Ʈ Ǯ�� �̸� ��������.
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
