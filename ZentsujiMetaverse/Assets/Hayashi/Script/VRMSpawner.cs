using UnityEngine;
using VRM;
using UniGLTF;
using System.IO;
using VRMShaders;
using Unity.VisualScripting;
using Mirror;
using UnityEditor;

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
    private async void LoadVRM(string path)
    {

        //VRMファイルをバイト配列として読み込む
        byte[] vrmData = File.ReadAllBytes(path);

        //VRMファイルのインポート設定
        var gltfData = new GlbLowLevelParser(path, vrmData).Parse();
        var vrmDataInstance = new VRMData(gltfData);
        var vrmImporter = new VRMImporterContext(vrmDataInstance);

        //VRMのメタデータを取得
        var meta = vrmImporter.ReadMeta();
        Debug.Log("Meta Title: " + meta.Title);

        //VRMを読み込んで生成
        var awaitCaller = new ImmediateCaller();
        var vrmModel = await vrmImporter.LoadAsync(awaitCaller);

        //読み込んだVRMをGameObjectとしてシーンに生成
        vrmModel.ShowMeshes();


        //スポーンポイントに移動
        vrmModel.transform.position = m_SpawnPoint.position;
        vrmModel.transform.rotation = m_SpawnPoint.rotation;

        SetVRMComponent(vrmModel);
        // シェーダーをURP用に変換
        ConvertShadersToURP(vrmModel);
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
                    material.shader = Shader.Find("lilToon");

                // 新しいシェーダーにメインテクスチャを再設定
                if (mainTexture != null)
                {
                    material.SetTexture("_MainTex", mainTexture);
                }

                // 透明マテリアルのプロパティを設定
                if (material.name.Contains("Brow") || material.name.Contains("Eyelash") || material.name.Contains("Eyeline"))
                {
                    //変更プロパティを設定
                }

            }
        }
    }

    public void SetVRMComponent(RuntimeGltfInstance vrm)
    {
        SetAnimator(vrm);
        SetCollider(vrm);
        SetNPCMove(vrm);
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
        capsuleCollider.height = 2;
        capsuleCollider.radius = 0.2f;
        capsuleCollider.center = new Vector3(0, 1, 0);
    }

    private void SetNPCMove(RuntimeGltfInstance vrm)
    {
        //NPCMoveをセット
        vrm.AddComponent<NPCMove>();
        
        NPCMove npcMove = vrm.GetComponent<NPCMove>();
        npcMove.m_WayPoints = m_Transfrom;
        npcMove.m_MoveSpeed = 1;
        npcMove.m_RotationSpeed = 7;
        npcMove.m_Animator = npcMove.GetComponent<Animator>();
        npcMove.m_WalkParameterName = "Walk";
    }
}
