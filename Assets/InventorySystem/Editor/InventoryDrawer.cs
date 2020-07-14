﻿/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 *  
 *  This is an Editor Script, it is responsible for drawing the inpector or drawer of the class in the attribute before the class
 */

using UnityEngine;
using UnityEditor;
using UniversalInventorySystem;
using System.Runtime.InteropServices;

[CustomPropertyDrawer(typeof(Inventory))]
public class InventoryDrawer : PropertyDrawer
{
    float baseAmount = 3f;
    float amountOfFilds = 3f;
    bool useObjValues;
    float total = 0;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * amountOfFilds;
    }
    bool unfold = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {   
        amountOfFilds = baseAmount + total;
        total = 0;
        SerializedObject serializedObject = null;
        if (property.objectReferenceValue != null) serializedObject = new SerializedObject(property.objectReferenceValue);
        if (serializedObject != null)
        {
            serializedObject.Update();
            var id = serializedObject.FindProperty("id");
            var areItemsUsable = serializedObject.FindProperty("areItemsUsable");
            var areItemsDroppable = serializedObject.FindProperty("areItemsDroppable");
            var slotAmounts = serializedObject.FindProperty("slotAmounts");
            var slots = serializedObject.FindProperty("slots");
            var inte = serializedObject.FindProperty("interactiable");

            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 100;
            position.height /= amountOfFilds;

            //position.y += position.height;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Inventory (id: " + id.intValue.ToString() + ")"));
            var tmpRect = position;
            position.x += 110;
            position.width = 140;
            if (GUI.Button(position, useObjValues ? "Edit Values" : "Use Object Value")) useObjValues = !useObjValues;
            position = tmpRect;
            position.y += 18;

            if (!useObjValues)
            {
                baseAmount = 3f;
                var usableRect = new Rect(position.x, position.y, 120, position.height);
                areItemsUsable.boolValue = EditorGUI.Toggle(usableRect, "Can use items", areItemsUsable.boolValue);
                var dropItemRect = new Rect(position.x + 150, position.y, 120, position.height);
                areItemsDroppable.boolValue = EditorGUI.Toggle(dropItemRect, "Can drop items", areItemsDroppable.boolValue);

                var inteRect = new Rect(position.x + 300, position.y, position.width - 300, position.height);
                EditorGUI.PropertyField(inteRect, inte);

                position.y += position.height;

                var foldRect = new Rect(position.x, position.y, 50, position.height);
                unfold = EditorGUI.Foldout(foldRect, unfold, new GUIContent("Slots"), true);

                if (slotAmounts.intValue != slots.arraySize) slotAmounts.intValue = slots.arraySize;
                var slotAmountsRect = new Rect(position.x + 50, position.y, position.width - 150, position.height);
                slotAmounts.intValue = EditorGUI.IntField(slotAmountsRect, new GUIContent("Amount of slots"), slotAmounts.intValue);
                if (slotAmounts.intValue < 0) slotAmounts.intValue = 0;

                position.y += position.height;

                var tmp = slots.arraySize;
                slots.arraySize = slotAmounts.intValue >= 0 ? slotAmounts.intValue : slots.arraySize;
                if (unfold)
                {
                    EditorGUI.indentLevel++;
                    //amountOfFilds = baseAmount + 1;
                    //position.y += position.height;
                    if (slots != null)
                    {
                        if (slots.arraySize > 0)
                        {
                            float childHeight = 0;
                            for (int i = 0; i < slots.arraySize; i++)
                            {
                                childHeight += EditorGUI.GetPropertyHeight(slots.GetArrayElementAtIndex(i)) / 18;
                                EditorGUI.PropertyField(position, slots.GetArrayElementAtIndex(i));
                                if (i > tmp - 1)
                                {
                                    slots.GetArrayElementAtIndex(i).FindPropertyRelative("interative").intValue = -1;
                                }
                                position.y += EditorGUI.GetPropertyHeight(slots.GetArrayElementAtIndex(i));
                            }
                            total += childHeight;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    amountOfFilds = baseAmount;
                }
            } else
            {
                baseAmount = 2f;
                position.width -= 20;
                EditorGUI.ObjectField(position, property);
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
        else
        {
            amountOfFilds = 1;
            baseAmount = 1;
            EditorGUI.ObjectField(position, property);
            if (property.objectReferenceValue != null)
            {
                serializedObject = new SerializedObject(property.objectReferenceValue);
                serializedObject.ApplyModifiedProperties();
                useObjValues = true;
            }
        }
    }
}
