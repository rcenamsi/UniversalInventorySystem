﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

//Todo
//Transparent slots when dragging

[System.Serializable]
public class InventoryUI : MonoBehaviour
{ 
    //Slots
    public bool generateUIFromSlotPrefab;
    public GameObject generatedUIParent;

    public GameObject slotPrefab;

    public Canvas canvas;

    public GameObject DontDropItemRect;

    public GameObject[] slots;

    public GameObject dragObj;

    public bool hideDragObj;

    //Sahder
    public Color outlineColor;

    public float outlineSize;

    //Toggle inventory
    public bool hideInventory;

    public KeyCode toggleKey;
    public GameObject togglableObject;

    //Inv
    public Inventory inv;

    //Craft
    public bool isCraftInventory;

    public Vector2Int gridSize;

    public bool allowsPatternCrafting;

    public GameObject[] productSlots;

    [HideInInspector]
    public bool isDraging;
    [HideInInspector]
    public int? dragSlotNumber = null;
    [HideInInspector]
    public bool shouldSwap;
    [HideInInspector]
    public List<Item> pattern = new List<Item>();

    public void SetInventory(Inventory _inv) => inv = _inv;
    public Inventory GetInventory() => inv;
    
    public void Start()
    {
        if (!inv.hasInitializated) inv.InitializeInventory();

        var b = Instantiate(dragObj, canvas.transform);
        b.name = $"DRAGITEMOBJ_{name}_{UnityEngine.Random.Range(int.MinValue, int.MaxValue)}";
        b.AddComponent<DragSlot>();
        b.SetActive(false);
        if(hideDragObj) b.hideFlags = HideFlags.HideInHierarchy;
        dragObj = b;

        InventoryController.inventoriesUI.Add(this);
        if (!generateUIFromSlotPrefab)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].name = i.ToString();
                for(int j = 0; j < slots[i].transform.childCount; j++)
                {
                    Image image;
                    if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                    {
                        ItemDragHandler drag;
                        if(slots[i].transform.GetChild(j).TryGetComponent(out drag))
                        {
                            drag.canvas = canvas;
                            drag.invUI = this;
                        }else 
                        {
                            drag = slots[i].transform.GetChild(j).gameObject.AddComponent<ItemDragHandler>();
                            drag.canvas = canvas;
                            drag.invUI = this;
                        }
                    }
                }               
            }
        }
        ItemDropHandler idh;
        if(!canvas.TryGetComponent(out idh)) canvas.gameObject.AddComponent<ItemDropHandler>();

        if (isCraftInventory)
        {
            for(int i = 0;i < gridSize.x * gridSize.y;i++)
                pattern.Add(null);
        }
    }

    List<GameObject> GenerateUI(int slotAmount)
    {
        List<GameObject> gb = new List<GameObject>();
        for(int i = 0;i < slotAmount; i++)
        {
            var g = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
            g.transform.SetParent(generatedUIParent.transform);
            g.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            var tmp = g.transform.GetComponentInChildren<ItemDragHandler>();
            tmp.canvas = canvas;
            tmp.invUI = this;
            g.name = i.ToString();
            gb.Add(g);
        }
        slots = gb.ToArray();
        return gb;
    }

    bool hasGenerated = false;
    public void Update()
    {
        if (generateUIFromSlotPrefab && !hasGenerated)
        {
            GenerateUI(inv.slotAmounts);
            hasGenerated = true;
        }

        if (hideInventory)
        {
            if (Input.GetKeyDown(toggleKey) && !isDraging)
            {
                togglableObject.SetActive(!togglableObject.activeInHierarchy);
            }
        }

        for (int i = 0;i < inv.slots.Count; i++)
        {
            if(isCraftInventory) pattern[i] = inv.slots[i].item;
           
            Image image;
            TextMeshProUGUI text;
            if (inv.slots[i].item == null)
            {
                for (int j = 0; j < slots[i].transform.childCount; j++)
                {
                    
                    if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                    {
                        image.sprite = null;
                        image.color = new Color(0, 0, 0, 0);
                    }
                    else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                        text.text = "";
                }         
                continue;
            }

            for (int j = 0; j < slots[i].transform.childCount; j++)
            {

                if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                {
                    image.sprite = inv.slots[i].item.sprite;
                    image.color = new Color(1, 1, 1, 1);
                }
                else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                    text.text = inv.slots[i].amount.ToString();
            }

            if (dragObj.GetComponent<DragSlot>().GetSlotNumber() == i && isDraging)
            {
                if (inv.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount() == 0)
                {
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {

                        if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                        {
                            image.sprite = null;
                            image.color = new Color(0, 0, 0, 0);
                        }
                        else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                            text.text = "";
                    }
                }
                else
                {
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {
                        if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                            text.text = (inv.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount()).ToString();
                    }
                }
            }

            if (!isCraftInventory)
            {
                slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                var index = i;
                slots[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log($"Slot {slots[index].name} was clicked");
                    inv.UseItemInSlot(index);
                });
            } 
        }

        if (isCraftInventory)
        {
            Item[] products = InventoryController.CraftItem(pattern.ToArray(), gridSize, false, true);
            
            if(products != null)
            {
                for (int i = 0; i < products.Length; i++)
                {
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {
                        Image image;
                        TextMeshProUGUI text;
                        if (productSlots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                        {
                            image.sprite = products[i].sprite;
                            image.color = new Color(1, 1, 1, 1);
                        }
                        //else if (productSlots[i].transform.GetChild(j).TryGetComponent(out text))
                        //  text.text = products[i].amount.ToString();
                    }
                }
            }else
            {
                for (int i = 0; i < productSlots.Length; i++)
                {
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {
                        Image image;
                        TextMeshProUGUI text;
                        if (productSlots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                        {
                            image.sprite = null;
                            image.color = new Color(0, 0, 0, 0);
                        }
                        //else if (productSlots[i].transform.GetChild(j).TryGetComponent(out text))
                        //  text.text = products[i].amount.ToString();
                    }
                }
            }
        }
    }
}
