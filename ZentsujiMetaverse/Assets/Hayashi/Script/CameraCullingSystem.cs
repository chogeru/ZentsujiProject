using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCullingSystem : MonoBehaviour
{
    public LayerMask playerLayerMask; // �v���C���[���C���[�̃}�X�N

    private Camera mainCamera;
    private int defaultCullingMask; // �����̃J�����O�}�X�N

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        defaultCullingMask = mainCamera.cullingMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // �v���C���[�I�u�W�F�N�g���R���C�_�[�ɓ��������̏���
            mainCamera.cullingMask &= ~playerLayerMask; // �v���C���[���C���[���J�����̃J�����O�}�X�N���珜�O
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // �v���C���[�I�u�W�F�N�g���R���C�_�[����o�����̏���
            mainCamera.cullingMask = defaultCullingMask; // �J�����O�}�X�N�������l�ɖ߂�
        }
    }
}
