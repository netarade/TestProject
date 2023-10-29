using Codice.Client.BaseCommands.Update.Fast.Transformers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatFire : MonoBehaviour
{
    GameObject bulletPrefab;
    AudioSource srcFire;
    AudioClip clipFire;
    AudioClip clipReload;
    Transform firePos;

    SwatAI swatAiScrt;
    Animator animator;

    int maxBullet = 10;
    int curBullet = 0;

    private readonly int hashOffset = Animator.StringToHash("Offset");


    

    void Start()
    {
        bulletPrefab = Resources.Load("S_Bullet") as GameObject;
 
        srcFire = this.gameObject.GetComponent<AudioSource>();
        clipFire = Resources.Load("Sounds/p_ak_1") as AudioClip;
        clipReload = Resources.Load("Sounds/p_reload") as AudioClip;

        firePos = transform.GetChild(2).GetChild(0).GetChild(0);

        swatAiScrt = this.gameObject.GetComponent<SwatAI>();
        animator = this.gameObject.GetComponent<Animator>();
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
    }

    private void OnEnable()
    {
        SwatAction.StopFireCoroutine += this.StopAllCoroutines;
    }

    private void OnDisable()
    {
        SwatAction.StopFireCoroutine += this.StopAllCoroutines;
    }


    public IEnumerator Co_Fire()
    {
        while (true)
        {
            if ( (swatAiScrt.IsStateChanged && swatAiScrt.State != SwatAI.EnumState.ATTACK) || swatAiScrt.State==SwatAI.EnumState.DIE )
            {
                animator.SetBool("IsFire", false);
                yield break;
            }

            if (curBullet >= maxBullet)
            {
                Reload();
                yield return new WaitForSeconds(1.53f);
            }

            var bulletObj = ObjectPoolingManager.poolmanager.GetSwatBullet();
            if( bulletObj != null )
            {
                bulletObj.transform.position = firePos.position;
                bulletObj.transform.rotation = firePos.rotation;
                bulletObj.SetActive(true);
                StartCoroutine(BulletDestroy(bulletObj));
            }

            //GameObject bulletInstance = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            //Destroy(bulletInstance, 2.0f);



            srcFire.PlayOneShot(clipFire, 1.0f);
            curBullet++;
            yield return new WaitForSeconds(0.3f); 
        }
    }

    private void Reload()
    {
        srcFire.PlayOneShot(clipReload, 1.0f);
        animator.SetTrigger("IsReloadTrig");
        curBullet = 0;
    }

    IEnumerator BulletDestroy(GameObject _bullet)
    {
        yield return new WaitForSeconds(3.0f);
        _bullet.SetActive(false);
    }



}
