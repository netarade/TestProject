using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    public float hp = 100f;
    //public float Hp { get; }
    [SerializeField] private GameObject BloodEffect; // 혈흔 효과 파티클

    SwatAI swatAiScrt;

    //public ImageConversion hpBarImage;


    void Start()
    {
        BloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == bulletTag)
        {
            //2023-0919 오브젝트 사라지는것이 아니라 비활성화
            col.gameObject.SetActive(false);
            //Destroy(col.gameObject);
            ShowBloodEffect(col);

            hp -= col.gameObject.GetComponent<BulletCtrl>().damage;
            hp = Mathf.Clamp(hp, 0f, 100f);
            Debug.Log($"HP: {hp}");

            if (hp <= 0f)
            {
                //GetComponent<SwatAction>().Die();
                //swatAiScrt.PrevState = swatAiScrt.State;
                //swatAiScrt.State = SwatAI.EnumState.DIE;
            }
        }
    }

    private void ShowBloodEffect(Collision col)
    {
        Vector3 pos = col.GetContact(0).point;     
        Vector3 _normal = col.GetContact(0).normal; 

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        GameObject blood = Instantiate(BloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }

    //private void OnEnable() //오브젝트가 활성화 되었을 때
    //{
    //    SetHP();
    //}

    //private void SetHP()
    //{
    //    uiCanvas = GameObject.Find("UI-Canvas").GetComponent<Canvas>();
    //    GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform); //UI Canvas 하위에 EnemyHpBar 생성된다. 
    //    hpBarImage = hpBar.GetComponentsInChildren<Image>()[1]; //[1]는 자식부모 상관없고 몇번째 있는지만 따진다. HpBarImage는 EnemyBar하위에 있다.
        

    //    var _hpBar = hpBar.GetComponent<EnemyHpBar>();
    //    _hpBar.targetTr = this.gameObject.transform;
    //    _hpBar.offset = Vector3.zero; //디테일한 위치.
    //}

    //IEnumerator PushPool()
    //{
    //    yield return new WaitForSeconds(3f);
    //    this.gameObject.SetActive(false);
    //    this.gameObject.tag ="SWAT";
    //    IsDie =false;
    //    moveAgent.Patrolling = true;
    //    swatDamage.hp = 100;
    //    GetComponent

    //}


}
