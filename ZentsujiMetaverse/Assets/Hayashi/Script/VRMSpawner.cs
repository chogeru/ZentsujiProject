using UnityEngine;
using VRM;
using UniGLTF;
using System;
using System.IO;
using System.Threading.Tasks;
using VRMShaders;
using Unity.VisualScripting;
using static UnityEditor.Rendering.CameraUI;
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
        // シェーダーをURP用に変換
        ConvertShadersToURP(vrmModel);

        // スポーンポイントに移動
        vrmModel.transform.position = spawnPoint.position;
        vrmModel.transform.rotation = spawnPoint.rotation;

        // アニメーターを追加してアニメーションコントローラーを設定
        var animator = vrmModel.GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        SetVRM(vrmModel);
    }
    private void ConvertShadersToURP(RuntimeGltfInstance vrmModelInstance)
    {
        // すべてのメッシュレンダラーを取得
        var renderers = vrmModelInstance.Root.GetComponentsInChildren<Renderer>();

        // すべてのマテリアルをURPシェーダーに変更
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {                                //VRM
                if (material.shader.name.Contains("UniGLTF/UniUnlit"))
                {
                    // 元のテクスチャを取得
                    var mainTexture = material.GetTexture("_MainTex");

                    // 新しいURPシェーダーを設定
                    material.shader = Shader.Find("Universal Render Pipeline/RealToon/Version 5/Default/Default");

                    // テクスチャを再設定
                    if (mainTexture != null)
                    {
                        material.SetTexture("Texture", mainTexture);
                    }
                    // マテリアル名が「Eyebrow」を含む場合、透明に設定
                    if (material.name.Contains("Brow")|| material.name.Contains("Eyelash")|| material.name.Contains("Eyeline")|| material.name.Contains("Glasses"))
                    {
                        material.SetFloat("TransparentMode", 1.0f);
                        material.SetFloat("Width", 0.15f);
                        /*
                        // マテリアルのレンダリングモードを透明に設定
                        material.SetFloat("_Surface", 1); // 1は透明
                        material.SetFloat("_Blend", 0); // 0はアルファブレンド

                        // アルファ値を設定（必要に応じて調整）
                        material.SetFloat("_AlphaClip", 0.5f); // クリップのアルファ値

                        // その他の透明用プロパティを設定
                        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetFloat("_ZWrite", 0);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        */
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


        Rigidbody rigidbody=vrm.GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        CapsuleCollider capsuleCollider = vrm.GetComponent<CapsuleCollider>();
        capsuleCollider.height = 2;
        capsuleCollider.radius = 0.2f;
        capsuleCollider.center = new Vector3(0, 1, 0);
    }
}
