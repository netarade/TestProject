using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //2023-0926

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private float hp = 100f;
    [SerializeField] private GameObject BloodEffect; // 혈흔 효과 파티클
    
    //2023-0926
    private float initHp = 100f;
    [Header("UI")]
    [SerializeField] private GameObject hpBarPrefab; //hpbar 프리팹
    [SerializeField] Vector3 hpBarOffset = new Vector3(0f, 2.2f, 0f); //hpbar 위치
    //2023-0926
    [SerializeField] public Canvas uiCanvas; //부모가 될 canvas
    [SerializeField] Image hpBarImage; //hpbar이미지

    void Start()
    {
        BloodEffect=Resources.Load<GameObject>( "BloodSprayEffect" );

        //2023-0927       
        SetHP();
    }

    //2023-0927
    private void OnEnable()
    {
        //SetHP();
    }

    private void SetHP()
    {
        //2023-0926
        uiCanvas=GameObject.Find( "UI-Canvas" ).GetComponent<Canvas>();
        GameObject hpBar = Instantiate( hpBarPrefab, uiCanvas.transform ); //태어날오브젝트, 위치
        hpBarImage=hpBar.GetComponentsInChildren<Image>()[1]; //2번째 자식


        


        //시작하자마자 hpBar는 할당되어 있을 것이다.
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr=this.gameObject.transform;
        _hpBar.offset=hpBarOffset;


        
        //2023-0927
        if(GetComponent<EnemyAI>().IsDie)
            HpBarDestroy();
            //Invoke( "HpBarDestroy", 0.2f);
            //InvokeRepeating( "HpBarDestroy",0.3f,0.5f);
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.tag == bulletTag)
        {
            //2023-0919 오브젝트 사라지는것이 아니라 비활성화
            col.gameObject.SetActive(false);
            //Destroy(col.gameObject);

            ShowBloodEffect(col);

            hp -= col.gameObject.GetComponent<BulletCtrl>().damage;
            hp = Mathf.Clamp(hp, 0f, 100f);
            Debug.Log($"HP: {hp}");

            //2023-0926
            hpBarImage.fillAmount = hp/initHp;



            if(hp<=0f)
            {
                //2023-0921
                GetComponent<EnemyAI>().Die();
                //GetComponent<EnemyAI>().state = EnemyAI.State.DIE; //열거형이 정의되어있음.

                //2023-0926
                //죽었을 때 생명게이지 투명처리
                hpBarImage.GetComponentsInChildren<Image>()[1].color = Color.clear;
                

            }
        }
    }

    

    private void ShowBloodEffect(Collision col)
    {
        //Vector3 pos = col.contacts[0].point;     //맞은 위치의 지점
        //Vector3 _normal = col.contacts[0].normal; //맞은 위치의 방향(법선: 평면에서 수직)
        Vector3 pos = col.GetContact(0).point;     //맞은 위치의 지점
        Vector3 _normal = col.GetContact(0).normal; //맞은 위치의 방향(법선: 평면에서 수직)

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
        //Quaternion rot = Quaternion.LookRotation(pos);

        GameObject blood = Instantiate(BloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }




    //2023-0927
    private void HpBarDestroy()
    {        
        Destroy(hpBarPrefab, 0.1f);
    }
}
