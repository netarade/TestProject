using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    public float hp = 100f;
    //public float Hp { get; }
    [SerializeField] private GameObject BloodEffect; // ���� ȿ�� ��ƼŬ

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
            //2023-0919 ������Ʈ ������°��� �ƴ϶� ��Ȱ��ȭ
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

    //private void OnEnable() //������Ʈ�� Ȱ��ȭ �Ǿ��� ��
    //{
    //    SetHP();
    //}

    //private void SetHP()
    //{
    //    uiCanvas = GameObject.Find("UI-Canvas").GetComponent<Canvas>();
    //    GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform); //UI Canvas ������ EnemyHpBar �����ȴ�. 
    //    hpBarImage = hpBar.GetComponentsInChildren<Image>()[1]; //[1]�� �ڽĺθ� ������� ���° �ִ����� ������. HpBarImage�� EnemyBar������ �ִ�.
        

    //    var _hpBar = hpBar.GetComponent<EnemyHpBar>();
    //    _hpBar.targetTr = this.gameObject.transform;
    //    _hpBar.offset = Vector3.zero; //�������� ��ġ.
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
