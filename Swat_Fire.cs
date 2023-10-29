using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;



public class Swat_Fire : MonoBehaviour
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

    [SerializeField] private GameObject s_bullet;
    [SerializeField] private Transform s_firePos;


    //2023_0913
    private readonly int hashReload = Animator.StringToHash("Reload");
    [Header("Reaload")]
    [SerializeField] private readonly float reloadTime = 2.0f;      // �������ð�
    [SerializeField] private int maxBullet = 10;                    // źȯ��
    [SerializeField] private int currentBullet = 10;                // źȯ��
    [SerializeField] private bool IsReload = false;                 // ����������
    [SerializeField] private WaitForSeconds wsReload;               // �������ð� ���� ��ٸ� ����
    [SerializeField] private AudioClip reloadSfx;                   // ������ ����




    void Start()
    {
        source = GetComponent<AudioSource>();
        fireSfx = Resources.Load("Sounds/p_ak_1") as AudioClip;

        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        s_bullet = Resources.Load<GameObject>("S_Bullet");
        s_firePos = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Transform>();

        //2023_0913
        reloadSfx = Resources.Load<AudioClip>("Sounds/p_reload");
        wsReload = new WaitForSeconds(reloadTime);


    }

    void Update()
    {
        if (!IsReload && IsFire) //2023_0913 !IsReload �߰�
        {
            if (Time.time > nextFire)
            {
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
        IsReload = (--currentBullet % maxBullet) == 0;
        if (IsReload)//IsReload�� true�� �Ƚ�����
        {
            StartCoroutine(Reloading());
        }



        animator.SetTrigger(hashFire);
        source.PlayOneShot(fireSfx, 1.0f);

        //Instantiate(e_bullet, e_firePos.position, e_firePos.rotation);
        var bulletObj = ObjectPoolingManager.poolmanager.GetSwatBullet();
        if( bulletObj != null )
        {
            bulletObj.transform.position = s_firePos.position;
            bulletObj.transform.rotation = s_firePos.rotation;
            bulletObj.SetActive(true);


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
}
