using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField,Header("待機時間")]
    public float waitTime = 2f;
    [SerializeField,Header("待機時回転方向")]
    public Vector3 waitRotation;
    [SerializeField,Header("待機するかどうか")]
    public bool isDoNotStop = false;
}
