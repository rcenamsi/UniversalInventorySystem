﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryUI))]
public class InventoryUIInspector : Editor
{

    SerializedProperty autoGenerateUI;
    SerializedProperty generatedUIParentProp;
    SerializedProperty slotPrefabProp;
    SerializedProperty canvasProp;
    SerializedProperty DontDropItemRectProp;
    SerializedProperty slotsProp;
    SerializedProperty dragObjProp;
    SerializedProperty outlineColorProp;
    SerializedProperty outlineSizeProp;
    SerializedProperty hideInventoryProp;
    SerializedProperty toggleKeyProp;
    SerializedProperty togglableObjectProp;
    SerializedProperty invProp;

    bool autoGenUI;

    bool slotsHeader;

    bool shaderFold;
    bool slotsFold;
    bool toggleFold;
    bool invFold;

    private void OnEnable()
    {
        autoGenerateUI = serializedObject.FindProperty("generateUIFromSlotPrefab");
        generatedUIParentProp = serializedObject.FindProperty("generatedUIParent");
        slotPrefabProp = serializedObject.FindProperty("slotPrefab");
        canvasProp = serializedObject.FindProperty("canvas");
        DontDropItemRectProp = serializedObject.FindProperty("DontDropItemRect");
        slotsProp = serializedObject.FindProperty("slots");
        dragObjProp = serializedObject.FindProperty("dragObj");
        outlineColorProp = serializedObject.FindProperty("outlineColor");
        outlineSizeProp = serializedObject.FindProperty("outlineSize");
        hideInventoryProp = serializedObject.FindProperty("hideInventory");
        toggleKeyProp = serializedObject.FindProperty("toggleKey");
        togglableObjectProp = serializedObject.FindProperty("togglableObject");
        invProp = serializedObject.FindProperty("inv");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if(!slotsHeader && !shaderFold && !toggleFold && !invFold) EditorGUILayout.HelpBox("Click on a header to open the configurations!", MessageType.Info);
        //EditorGUILayout.Knob(new Vector2(50, 50), 1, 0, 3, "m", Color.red, Color.green, true);
        slotsHeader = EditorGUILayout.Foldout(slotsHeader, "Slots Configuration", true, EditorStyles.boldLabel);

        if (slotsHeader)
        {
            EditorGUILayout.Separator();

            autoGenerateUI.boolValue = EditorGUILayout.Toggle(new GUIContent("Automatically generate slots"), autoGenerateUI.boolValue);

            if (autoGenerateUI.boolValue)
            {
                EditorGUILayout.HelpBox(new GUIContent("Parent transform for automaticaly genarated slots"));
                EditorGUILayout.ObjectField(generatedUIParentProp, new GUIContent("Parent transform"));
                EditorGUILayout.HelpBox(new GUIContent("Prefab of the slot to be instatiated"));
                EditorGUILayout.ObjectField(slotPrefabProp, new GUIContent("Slot prefab"));
            }

            EditorGUILayout.Separator();

            EditorGUILayout.ObjectField(canvasProp, new GUIContent("Canvas where the inventory is"));

            EditorGUILayout.HelpBox(new GUIContent("The Rect in witch the item wont be dropped when stop draging the item"));
            EditorGUILayout.ObjectField(DontDropItemRectProp, new GUIContent("InventoryRect"));

            EditorGUILayout.Separator();

            slotsFold = EditorGUILayout.Foldout(slotsFold, new GUIContent("Slots GameObjects"), true);  
            
            if(slotsFold)
            {
                EditorGUI.indentLevel++;
                slotsProp.arraySize = EditorGUILayout.IntField("Size", slotsProp.arraySize);

                if(GUILayout.Button("Add seleted GameObgects to the list"))
                {
                    Transform[] go = Selection.transforms;

                    for (int i = 0; i < go.Length; i++)
                    {
                        if (slotsProp.GetArrayElementAtIndex(i).objectReferenceValue != null) continue;
                        slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = go[i].gameObject;
                    }
                }

                for(int i = 0; i < slotsProp.arraySize;i++)
                {
                    EditorGUILayout.ObjectField(slotsProp.GetArrayElementAtIndex(i), new GUIContent("Slot " + i.ToString()));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(new GUIContent("The GameObject that represents a dragged item. We recomend using the default one"));
            EditorGUILayout.ObjectField(dragObjProp, new GUIContent("Drag Object"));
        }

        EditorGUILayout.Separator();

        shaderFold = EditorGUILayout.Foldout(shaderFold, "Shader Configuration", true, EditorStyles.boldLabel);

        if (shaderFold)
        {
            EditorGUILayout.HelpBox(new GUIContent("This is the color of the outline generated when draging a object (if the object provided has the inventory material)"));
            outlineColorProp.colorValue = EditorGUILayout.ColorField(new GUIContent("Outline color"), outlineColorProp.colorValue);

            outlineSizeProp.floatValue = EditorGUILayout.Slider(new GUIContent("Outline Size") ,outlineSizeProp.floatValue, 0, 10);
        }

        EditorGUILayout.Separator();

        toggleFold = EditorGUILayout.Foldout(toggleFold, "Toggle Inventory Configuration", true, EditorStyles.boldLabel);

        if (toggleFold)
        {
            hideInventoryProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Toggle inventory by Keyboard"), hideInventoryProp.boolValue);

            if(hideInventoryProp.boolValue)
            {

                InventoryUI invUI = (InventoryUI)target;

                invUI.toggleKey = (KeyCode)EditorGUILayout.EnumPopup("Toggle key", invUI.toggleKey);

                EditorGUILayout.ObjectField(togglableObjectProp, new GUIContent("Togglable Object"));
            }
        }
        
        EditorGUILayout.Separator();

        invFold = EditorGUILayout.Foldout(invFold, "Inventory Configuration", true, EditorStyles.boldLabel);


        if(invFold)
        {
            EditorGUILayout.PropertyField(invProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}