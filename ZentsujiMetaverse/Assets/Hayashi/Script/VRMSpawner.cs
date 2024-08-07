using UnityEngine;
using VRM;
using UniGLTF;
using System.IO;
using VRMShaders;
using Unity.VisualScripting;
using UnityEditor;
using Cysharp.Threading.Tasks;
using System;

public class VRMSpawner : MonoBehaviour
{
    [SerializeField,Header("VRMのパス")]
    private string m_VrmFilePath = "test.vrm";
    [SerializeField,Header("読み込んだVRMのスポーン地点")]
    private Transform m_SpawnPoint;
    [SerializeField,Header("セットするAnimator")]
    private RuntimeAnimatorController m_AnimatorController;
    [SerializeField, Header("生成したNPCの移動ポイント")]
    private Transform[] m_Transfrom;
    [SerializeField,Header("vrmサイズ")]
    private Vector3 m_VrmSize = Vector3.one;

    private const float ColliderHeight = 2f;
    private const float ColliderRadius = 0.2f;
    private static readonly Vector3 ColliderCenter = new Vector3(0, 1, 0);

    private const float NPCMoveSpeed = 1f;
    private const float NPCRotationSpeed = 7f;
    private const string WalkParameterName = "Walk";
    private const string URPShaderName = "lilToon";

    void Start()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, m_VrmFilePath);
        fullPath = fullPath.Replace("\\", "/");
        Debug.Log("フルパス: " + fullPath);

        if (File.Exists(fullPath))
        {
            Debug.Log("VRMファイルが見つかりました: " + fullPath);
            LoadVRM(fullPath);
        }
        else
        {
            Debug.LogError("VRMファイルが見つかりません: " + fullPath);
        }
    }
    private async UniTask LoadVRM(string path)
    {
        try
        {
            //VRMファイルをバイト配列として読み込む
            byte[] vrmData = File.ReadAllBytes(path);

            //VRMファイルのインポート設定
            var gltfData = new GlbLowLevelParser(path, vrmData).Parse();
            var vrmDataInstance = new VRMData(gltfData);
            var vrmImporter = new VRMImporterContext(vrmDataInstance);

            //VRMのメタデータを取得
            var meta = vrmImporter.ReadMeta();
            DebugUtility.Log("Meta Title: " + meta.Title);

            //VRMを読み込んで生成
            var awaitCaller = new ImmediateCaller();
            var vrmModel = await vrmImporter.LoadAsync(awaitCaller);

            //ネイティブ配列の解放
            gltfData.Dispose();

            //読み込んだVRMをGameObjectとしてシーンに生成
            vrmModel.ShowMeshes();
            vrmModel.transform.SetPositionAndRotation(m_SpawnPoint.position, m_SpawnPoint.rotation);
            vrmModel.transform.localScale = m_VrmSize;

            SetVRMComponent(vrmModel);
            // シェーダーをURP用に変換
            ConvertShadersToURP(vrmModel);
        }
        catch (Exception ex)
        {
            DebugUtility.Log("VRM読み込み中にエラー発生");
        }
    }
    private void ConvertShadersToURP(RuntimeGltfInstance vrmModelInstance)
    {
        // VRMモデルインスタンス内のすべてのレンダラーを取得
        var renderers = vrmModelInstance.Root.GetComponentsInChildren<Renderer>();

        // 各レンダラーをループ
        foreach (var renderer in renderers)
        {
            // 各レンダラーのマテリアルをループ
            foreach (var material in renderer.materials)
            {
                // 元のメインテクスチャを取得
                var mainTexture = material.GetTexture("_MainTex");

                // 新しいシェーダーを割り当て
                    material.shader = Shader.Find(URPShaderName);

                // 新しいシェーダーにメインテクスチャを再設定
                if (mainTexture != null)
                {
                    material.SetTexture("_MainTex", mainTexture);
                }

                // 透明マテリアルのプロパティを設定
                if (material.name.Contains("Brow") || material.name.Contains("Eyelash") || material.name.Contains("Eyeline"))
                {
                    material.SetFloat("_Surface", 1);
                    material.SetFloat("_Blend", 0); 
                    material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_ZWrite", 0);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.SetFloat("_Cutoff", 0.5f);
                }

            }
        }
    }

    public void SetVRMComponent(RuntimeGltfInstance vrm)
    {
        if (vrm != null)
        {
            vrm.gameObject.tag = "NPC";
            SetAnimator(vrm);
            SetCollider(vrm);
            SetNPCMove(vrm);
        }
        else
        {
            DebugUtility.Log("vrmが無いよ");
        }
    }
    private void SetAnimator(RuntimeGltfInstance vrm)
    {
        vrm.AddComponent<Animator>();
        Animator animator = vrm.GetComponent<Animator>();
        animator.runtimeAnimatorController = m_AnimatorController;
        animator.applyRootMotion = false;
    }
    private void SetCollider(RuntimeGltfInstance vrm)
    {
        //コライダーの追加と各コライダーの設定
        vrm.AddComponent<CapsuleCollider>();
        CapsuleCollider capsuleCollider = vrm.GetComponent<CapsuleCollider>();
        capsuleCollider.height = ColliderHeight;
        capsuleCollider.radius = ColliderRadius;
        capsuleCollider.center = ColliderCenter;
    }

    private void SetNPCMove(RuntimeGltfInstance vrm)
    {
        //NPCMoveをセット
        vrm.AddComponent<NPCMove>();
        
        NPCMove npcMove = vrm.GetComponent<NPCMove>();
        npcMove.m_WayPoints = m_Transfrom;
        npcMove.m_MoveSpeed = NPCMoveSpeed;
        npcMove.m_RotationSpeed = NPCRotationSpeed;
        npcMove.m_Animator = vrm.GetComponent<Animator>();
        npcMove.m_WalkParameterName = WalkParameterName;
    }
}
