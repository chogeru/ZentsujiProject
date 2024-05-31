using UnityEngine;
using VRM;
using UniGLTF;
using System.IO;
using VRMShaders;
using Unity.VisualScripting;
using Mirror;

public class VRMSpawner : MonoBehaviour
{
    public string vrmFilePath = "test.vrm"; // VRMファイルの相対パス
    public Transform spawnPoint; // スポーンポイント
    public RuntimeAnimatorController animatorController; // アニメーターコントローラー

    void Start()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, vrmFilePath);
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

        // VRMファイルをバイト配列として読み込む
        byte[] vrmData = File.ReadAllBytes(path);

        // VRMファイルのインポート設定
        var gltfData = new GlbLowLevelParser(path, vrmData).Parse();
        var vrmDataInstance = new VRMData(gltfData);
        var vrmImporter = new VRMImporterContext(vrmDataInstance);

        // VRMのメタデータを取得
        var meta = vrmImporter.ReadMeta();
        Debug.Log("Meta Title: " + meta.Title);

        // VRMを読み込んで生成
        var awaitCaller = new ImmediateCaller();
        var vrmModel = await vrmImporter.LoadAsync(awaitCaller);

        // 読み込んだVRMをGameObjectとしてシーンに生成
        vrmModel.ShowMeshes();
      

        // スポーンポイントに移動
        vrmModel.transform.position = spawnPoint.position;
        vrmModel.transform.rotation = spawnPoint.rotation;

        // アニメーターを追加してアニメーションコントローラーを設定
        var animator = vrmModel.GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        SetVRM(vrmModel);
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
                // シェーダーが置き換え対象であるか確認
                if (material.shader.name.Contains("UniGLTF/UniUnlit"))
                {
                    // 元のメインテクスチャを取得
                    var mainTexture = material.GetTexture("_MainTex");

                    // 新しいシェーダーを割り当て
                //    material.shader = Shader.Find("Universal Render Pipeline/RealToon/Version 5/Default/Default");

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
    }

    public void SetVRM(RuntimeGltfInstance vrm)
    {
        vrm.AddComponent<NetworkIdentity>();
        vrm.AddComponent<NetworkTransformUnreliable>();
        vrm.AddComponent<Rigidbody>();
        vrm.AddComponent<CapsuleCollider>();
        vrm.AddComponent<PlayerController>();


        Rigidbody rigidbody = vrm.GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        CapsuleCollider capsuleCollider = vrm.GetComponent<CapsuleCollider>();
        capsuleCollider.height = 2;
        capsuleCollider.radius = 0.2f;
        capsuleCollider.center = new Vector3(0, 1, 0);
    }
}
