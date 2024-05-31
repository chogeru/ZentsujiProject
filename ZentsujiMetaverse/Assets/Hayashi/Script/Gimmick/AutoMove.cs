using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;

public class AutoMove : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float m_Speed;
    [SerializeField, Header("X軸での移動")]
    private bool moveInX = false;

    [SerializeField, Header("Z軸での移動")]
    private bool moveInZ = false;

    private void Update()
    {
        //移動方向の初期化
        Vector3 direction = Vector3.zero;
        //各triggerのtrueに合わせて軸を移動
        if (moveInX) direction.x = 1f;
        if (moveInZ) direction.z = 1f;
        //設定された方向と速度に移動
        transform.Translate(direction * m_Speed * Time.deltaTime);
    }
}
