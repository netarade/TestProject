using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EnemyHpBar : MonoBehaviour
{
    private Camera uiCamera;
    private Canvas canvas;
    private RectTransform rectParent;
    private RectTransform rectHp;
    
    // �ٸ� Ŭ�������� ��������� ���� public ������ �ϴ���, �ν����� ������ ǥ���ϰ� ���� ���� ��
    [HideInInspector] public Vector3 offset = Vector3.zero;
    [HideInInspector] public Transform targetTr;

    void Start()
    {
        //�θ� ������Ʈ�� ������Ʈ�� �����´�.
        canvas = GetComponentInParent<Canvas>();

        //ĵ�����ȿ� Canvas������Ʈ�� Render Camera�Ӽ��� UICamaera�� �巡���ؼ� ����Ͽ���.
        uiCamera = canvas.worldCamera;

        //rectParent = GetComponentInParent<RectTransform>();
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = GetComponent<RectTransform>();
    }

    //UI�� ���ʹ� �����Ǿ��� ������, ���ʹ� �������� ����ٴϱ� ����
    void LateUpdate()
    {   
        //���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ�Ѵ�. �Ǻ���ġ�� �������� �����µ� �ø��� ���� offset����. �ϴ� zero�� �س���.
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        if( screenPos.z < 0.0f )
        {
            screenPos *= -1.0f; //������ ����� �����.
        }
        // ��ũ�� ��ǥ�� 2D ��ǥ���̱� ������ ī�޶�(���ΰ�)�� 180�� ȸ����
        // �� ĳ���Ϳ� ������ �ִٰ� �ϴ��� ȭ�鿡 ǥ�õȴ�.
        // ���״� �ƴ�����, ��ġ ���� ����̴�. ī�޶� 180�� ȸ�� �ߴ� ����
        // z���� ������ �Ǹ� 180�� �̻� ȸ���� ������ �Ǵ� -1�� ���ؼ� ����� �ǵ��� �Ѵ�.
        // ��ũ����ǥ�� ���� �ϴ��� (0,0)�̴�. (0,0)���� ���� ��ܱ����� �ȼ� ���� �ȴ�.
        
        // RectTransform ��ǥ���� ���� ���� ����
        var localPos = Vector2.zero;

        //��ũ�� ��ǥ�� RectTransform ������ ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectParent, screenPos, uiCamera, out localPos);
        // ĵ������ RectTransform, ��ũ����ǥ, UI������ ī�޶� ������Ʈ, ��ȯ�� ��ǥ

        //�θ�� ���� �������� �ʴ´�. hpbar�� �������� �Ѵ�.
        rectHp.localPosition = localPos; 


    }
}
