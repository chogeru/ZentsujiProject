using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class SeasonalTreeChanger : MonoBehaviour
{
    #region 木のインデックス
    [Tab("番号")]
    [SerializeField, Header("通常の木のインデックス")]
    private int m_NormalTreeIndex;
    [SerializeField, Header("桜の木のインデックス")]
    private int m_SakuraTreeIndex;
    [SerializeField, Header("秋の木のインデックス")]
    private int m_AutumnTreeIndex;
    [SerializeField, Header("入れ替える木のインデックス")]
    private int m_TreeIndex;
    [EndTab]
    #endregion

    #region トリガー
    [Tab("トリガー")]
    [SerializeField, Header("草のデバッグログを表示する")]
    private bool m_ShowGrassDebugLog;
    [EndTab]
    #endregion

    #region テレイン
    [Tab("テレイン")]
    [SerializeField, Header("植物を入れ替えるテレイン")]
    private Terrain m_Terrain;
    [EndTab]
    #endregion
    void Start()
    {
        // 開始時に桜や秋の木をすべて通常の木に置き換える
        ResetToNormalTrees();
        // 現在の木のインスタンスとそのプロトタイプインデックスをデバッグログに表示
        Terrain terrain = Terrain.activeTerrain;
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;

        DebugUtility.Log("現在の木のインスタンスとそのプロトタイプインデックス:");

        int currentMonth = System.DateTime.Now.Month;
        int selectedTreeIndex = m_NormalTreeIndex;

        if (currentMonth >= 2 && currentMonth <= 4)
        {
            selectedTreeIndex = m_SakuraTreeIndex;
        }
        else if (currentMonth >= 9 && currentMonth <= 11)
        {
            selectedTreeIndex = m_AutumnTreeIndex;
        }
        UpdateGrassColor(currentMonth);
        ReplaceTrees(selectedTreeIndex);
    }
    [ContextMenu("通常の木に置き換え")]
    void ReplaceWithNormalTrees()
    {
        ResetToNormalTrees();
        ReplaceTrees(m_NormalTreeIndex);
    }

    [ContextMenu("桜の木に置き換え")]
    void ReplaceWithSakuraTrees()
    {
        ResetToNormalTrees();
        ReplaceTrees(m_SakuraTreeIndex);
    }

    [ContextMenu("秋の木に置き換え")]
    void ReplaceWithAutumnTrees()
    {
        ResetToNormalTrees();
        ReplaceTrees(m_AutumnTreeIndex);
    }
    void ReplaceTrees(int newTreeIndex)
    {
        Terrain terrain = m_Terrain;
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;

        for (int i = 0; i < treeInstances.Length; i++)
        {
            if (treeInstances[i].prototypeIndex == m_TreeIndex)
            {
                TreeInstance newTree = treeInstances[i];
                newTree.prototypeIndex = newTreeIndex;
                treeInstances[i] = newTree;
            }
        }
        terrain.terrainData.treeInstances = treeInstances;
    }
    void ResetToNormalTrees()
    {
        Terrain terrain = m_Terrain;
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;

        for (int i = 0; i < treeInstances.Length; i++)
        {
            if (treeInstances[i].prototypeIndex == m_SakuraTreeIndex || treeInstances[i].prototypeIndex == m_AutumnTreeIndex)
            {
                TreeInstance newTree = treeInstances[i];
                newTree.prototypeIndex = m_NormalTreeIndex;
                treeInstances[i] = newTree;
            }
        }

        terrain.terrainData.treeInstances = treeInstances;
    }
    void UpdateGrassColor(int currentMonth)
    {
        Terrain terrain = m_Terrain;
        TerrainData terrainData = terrain.terrainData;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;

        Color dryColor = Color.white;

        if (currentMonth >= 2 && currentMonth <= 4)
        {
            dryColor = Color.black;
        }
        else if (currentMonth >= 9 && currentMonth <= 11)
        {
            dryColor = new Color(1f, 0.5f, 0f); // オレンジ
        }

        foreach (var detailPrototype in detailPrototypes)
        {
            detailPrototype.dryColor = dryColor;
        }

        terrainData.detailPrototypes = detailPrototypes;

        if (m_ShowGrassDebugLog)
        {
            DebugUtility.Log("現在の草のプロトタイプとそのドライカラー:");
            for (int i = 0; i < detailPrototypes.Length; i++)
            {
                DebugUtility.Log($"草 {i}: ドライカラー = {detailPrototypes[i].dryColor}");
            }
        }
    }

    [ContextMenu("通常の草の色に戻す")]
    void ResetGrassColor()
    {
        UpdateGrassColor(1); // 通常の月を1月とする
    }

    [ContextMenu("桜の季節の草の色にする")]
    void SetSakuraSeasonGrassColor()
    {
        UpdateGrassColor(3); // 桜の季節を3月とする
    }

    [ContextMenu("秋の季節の草の色にする")]
    void SetAutumnSeasonGrassColor()
    {
        UpdateGrassColor(10); // 秋の季節を10月とする
    }
}
