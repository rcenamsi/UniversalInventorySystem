﻿using UnityEngine;
using UnityEditor;
using UniversalInventorySystem;
using System.Diagnostics;

public class InventoryControllerWindow : EditorWindow
{
    [MenuItem("InventorySystem/InventoryController")]
    public static void Init()
    {
        InventoryControllerWindow window = (InventoryControllerWindow)GetWindow<InventoryControllerWindow>("Controller");
        window.Show();
    }

    bool inventory = true;
    bool inventoryUI = false;
    bool debug = false;
    Vector2 scrollPos;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

        if (GUILayout.Button("Inventories"))
        {
            inventory = true;
            inventoryUI = false;
            debug = false;
        }

        if (GUILayout.Button("InventoriesUI"))
        {
            inventory = false;
            inventoryUI = true;
            debug = false;
        }

        if (GUILayout.Button("Debug"))
        {
            inventory = false;
            inventoryUI = false;
            debug = true;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (inventory)
        {
            EditorGUILayout.LabelField("Inventories");
            for (int i = 0; i < InventoryController.inventories.Count; i++)
            {
                var a = InventoryController.inventories[i];
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{a.key} (id: {a.id})");
                EditorGUILayout.EnumFlagsField(a.interactiable);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField($"Size: {a.SlotAmount}");

                MakeHeader();

                for (int j = 0; j < a.slots.Count; j++)
                {
                    Slot s = a.slots[j];

                    EditorGUILayout.BeginHorizontal();
                    var rect = GUILayoutUtility.GetRect(100, 18);
                    HR(rect);

                    if (s.HasItem) EditorGUI.LabelField(rect, new GUIContent(" Has Item   ", EditorGUIUtility.IconContent("TestPassed").image));
                    else EditorGUI.LabelField(rect, new GUIContent(" Hasn't Item ", EditorGUIUtility.IconContent("TestFailed").image));

                    rect.x += 100;

                    DrawLine(rect);

                    if (s.item != null)
                    {
                        EditorGUI.LabelField(rect, new GUIContent(s.item.itemName, EditorGUIUtility.IconContent("ScriptableObject Icon").image));
                        rect.x += 100;
                        DrawLine(rect);
                        EditorGUI.LabelField(rect, s.amount.ToString());
                        rect.x += 80;
                        DrawLine(rect);
                        EditorGUI.LabelField(rect, $"{s.durability} | {s.item.hasDurability}");
                        rect.x += 110;
                        DrawLine(rect);
                    }
                    else
                    {
                        EditorGUI.LabelField(rect, "None");
                        rect.x += 100;
                        DrawLine(rect);
                        rect.x += 80;
                        DrawLine(rect);
                        rect.x += 110;
                        DrawLine(rect);
                    }
                    EditorGUI.LabelField(rect, s.isProductSlot.ToString());
                    rect.x += 100;
                    DrawLine(rect);
                    EditorGUI.LabelField(rect, s.whitelist == null ? "None" : s.whitelist.ToString());

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }
        else if (inventoryUI)
        {
            EditorGUILayout.LabelField("InventoriesUI");
        }
        else if (debug)
        {
            EditorGUILayout.LabelField("Debugging");
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    Stopwatch stop;
    public void Update()
    {
        if (stop == null)
        {
            stop = new Stopwatch();
            stop.Start();
        }
        if(stop.ElapsedMilliseconds >= 1000)
        {
            stop.Restart();
            Repaint();
        }
    }

    public void DrawLine(Rect rect)
    {
        Handles.color = Color.gray;
        Handles.BeginGUI();
        Handles.DrawLine(
        new Vector3(rect.x - 5, rect.y),
        new Vector3(rect.x - 5, rect.y + 18));
        Handles.EndGUI();
    }

    public void HR(Rect rect)
    {
        Handles.color = Color.gray;
        Handles.BeginGUI();
        Handles.DrawLine(
        new Vector3(rect.x, rect.y),
        new Vector3(rect.x + rect.width, rect.y));
        Handles.EndGUI();
    }

    public void MakeHeader()
    {
        var header = GUILayoutUtility.GetRect(100, 18);
        HR(header);
        EditorGUI.LabelField(header, "Has Item:");
        header.x += 100;
        DrawLine(header);
        EditorGUI.LabelField(header, "Item:");
        header.x += 100;
        DrawLine(header);
        EditorGUI.LabelField(header, "Amount:");
        header.x += 80;
        DrawLine(header);
        EditorGUI.LabelField(header, "Durability | Has");
        header.x += 110;
        DrawLine(header);
        EditorGUI.LabelField(header, "Is Product Slot:");
        header.x += 100;
        DrawLine(header);
        EditorGUI.LabelField(header, "Whitelist:");
    }
}
