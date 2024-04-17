using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class MenuSelector : MonoBehaviour
{
    public List<GameObject> menuItems; // メニュー項目のリスト
    public List<GameObject> menuPanels; // 各メニュー項目に対応するパネルのリスト
    private int selectedItemIndex = 0;
    public UnityEvent[] actions;

    void Start()
    {
        UpdateMenuSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteSelectedItem();
        }
    }

    private void MoveSelection(int direction)
    {
        selectedItemIndex += direction;

        if (selectedItemIndex >= menuItems.Count)
        {
            selectedItemIndex = 0;
        }
        else if (selectedItemIndex < 0)
        {
            selectedItemIndex = menuItems.Count - 1;
        }

        UpdateMenuSelection();
    }

    private void UpdateMenuSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            SEManager.instance.PlaySound("MenuSelectSE");
            menuPanels[i].SetActive(i == selectedItemIndex);
        }
    }

    private void ExecuteSelectedItem()
    {
        if (selectedItemIndex < actions.Length && actions[selectedItemIndex] != null)
        {
            actions[selectedItemIndex].Invoke();
            SEManager.instance.PlaySound("GameStartSE");
            DisableMenuInput();
        }
        else
        {
            Debug.LogError("No action bound to this menu item index");
        }
    }

    private void DisableMenuInput()
    {
        enabled = false; // Disable Update method to stop menu interaction
    }
}
