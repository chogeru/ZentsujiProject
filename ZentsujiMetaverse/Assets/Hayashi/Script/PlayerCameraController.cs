using System.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_Player;
    [SerializeField]
    private float m_Sensitivity = 2.0f;
    [SerializeField]
    private LayerMask m_ObstacleMask;
    [SerializeField]
    private float m_MinDistance = 1.0f;
    [SerializeField]
    private float m_MaxDistance = 10.0f; 
    [SerializeField] 
    private float m_SphereCastRadius = 0.5f;

    private Camera mainCamera;
    private Vector3 offset;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        offset = transform.position - m_Player.position;

        Observable.EveryUpdate()
            .Where(_ => !MenuUIManager.instance.isOpenUI)
            .Select(_ => {
                Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X") * m_Sensitivity, Input.GetAxis("Mouse Y") * m_Sensitivity);
                Vector2 gamepadInput = Gamepad.current?.rightStick.ReadValue() ?? Vector2.zero;
                float scrollInput = Mouse.current.scroll.y.ReadValue();
                return new Tuple<Vector2, float>(mouseInput + new Vector2(gamepadInput.x * m_Sensitivity, gamepadInput.y * m_Sensitivity), scrollInput);
            })
            .Subscribe(inputs => {
                UpdateCameraPositionAsync(inputs.Item1.x, inputs.Item1.y, inputs.Item2).Forget();
            }).AddTo(this);
    }

    private async UniTaskVoid UpdateCameraPositionAsync(float mouseX, float mouseY, float scrollInput)
    {

        offset = Quaternion.AngleAxis(mouseX, Vector3.up) * Quaternion.AngleAxis(-mouseY, mainCamera.transform.right) * offset;

            float currentDistance = offset.magnitude;
            float newDistance = Mathf.Clamp(currentDistance - scrollInput * m_Sensitivity * Time.deltaTime, m_MinDistance, m_MaxDistance);
            offset = offset.normalized * newDistance;

            Vector3 newPosition = m_Player.position + offset;
            Vector3 direction = newPosition - m_Player.position;

            if (Physics.Raycast(m_Player.position, direction.normalized, out RaycastHit hit, direction.magnitude, m_ObstacleMask))
            {
                newPosition = hit.point - direction.normalized * m_MinDistance;
            }

            mainCamera.transform.position = newPosition;
            mainCamera.transform.LookAt(m_Player.position);

            await UniTask.Yield(PlayerLoopTiming.Update);
        
    }
}
