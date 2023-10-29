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
    
    // 다른 클래스에서 접근허용을 위해 public 선언을 하더라도, 인스펙터 상으로 표시하고 싶지 않을 때
    [HideInInspector] public Vector3 offset = Vector3.zero;
    [HideInInspector] public Transform targetTr;

    void Start()
    {
        //부모 오브젝트의 컴포넌트를 가져온다.
        canvas = GetComponentInParent<Canvas>();

        //캔버스안에 Canvas컴포넌트의 Render Camera속성에 UICamaera를 드래그해서 등록하였다.
        uiCamera = canvas.worldCamera;

        //rectParent = GetComponentInParent<RectTransform>();
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = GetComponent<RectTransform>();
    }

    //UI가 에너미 생성되었을 때부터, 에너미 움직임을 따라다니기 위해
    void LateUpdate()
    {   
        //월드 좌표를 스크린 좌표로 변환한다. 피봇위치가 발쪽으로 잡히는데 올리기 위해 offset설정. 일단 zero로 해놓음.
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        if( screenPos.z < 0.0f )
        {
            screenPos *= -1.0f; //음수를 양수로 만든다.
        }
        // 스크린 좌표는 2D 좌표계이기 때문에 카메라(주인공)가 180도 회전해
        // 적 캐릭터와 등지고 있다고 하더라도 화면에 표시된다.
        // 버그는 아니지만, 원치 않은 결과이다. 카메라가 180도 회전 했는 지를
        // z값이 음수가 되면 180도 이상 회전한 것으로 판단 -1을 곱해서 양수가 되도록 한다.
        // 스크린좌표는 좌측 하단이 (0,0)이다. (0,0)에서 우측 상단까지가 픽셀 폭이 된다.
        
        // RectTransform 좌표값을 전달 받을 변수
        var localPos = Vector2.zero;

        //스크린 좌표를 RectTransform 기준의 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectParent, screenPos, uiCamera, out localPos);
        // 캔버스의 RectTransform, 스크린좌표, UI랜더링 카메라 오브젝트, 변환된 좌표

        //부모와 같이 움직이지 않는다. hpbar만 움직여야 한다.
        rectHp.localPosition = localPos; 


    }
}
