﻿using System;
using System.ComponentModel;
using System.Numerics;
using UnityEngine;

/// <summary>
/// Put this script on a always active GameObject.
/// </summary>
public class InventoryEventHandler : MonoBehaviour
{
    public static InventoryEventHandler current;
 
    public event EventHandler<AddItemEventArgs> OnAddItem;
    public event EventHandler<RemoveItemEventArgs> OnRemoveItem;
    public event EventHandler<SwapItemsEventArgs> OnSwapItem;
    public event EventHandler<SwapItemsTrhuInvEventArgs> OnSwapTrhuInventory;
    public event EventHandler<UseItemEventArgs> OnUseItem;
    public event EventHandler<DropItemEventArgs> OnDropItem;
    public event EventHandler<AddItemEventArgs> OnPickUpItem;
    public event EventHandler<InitializeInventoryEventArgs> OnInitializeInventory;

    public class AddItemEventArgs : EventArgs
    {
        public Inventory inv;
        public bool addedToNewSlot;
        public bool addedToSlot;
        public Item itemAdded;
        public int amount;
        public int? slotNumber;

        public AddItemEventArgs (Inventory _inv, bool _addedToNewSlot, bool _addedToSlot, Item _itemAdded, int _amount, int? _slotNumber)
        {
            inv = _inv;
            addedToNewSlot = _addedToNewSlot;
            addedToSlot = _addedToSlot;
            itemAdded = _itemAdded;
            amount = _amount;
            slotNumber = _slotNumber;
        }
    }
    public class RemoveItemEventArgs
    {
        public Inventory inv;
        public bool removedByUI;
        public int amount;
        public Item item;
        public int? slot;

        public RemoveItemEventArgs(Inventory _inv, bool _removedByUI, int _amount, Item _item, int? _slot)
        {
            inv = _inv;
            removedByUI = _removedByUI;
            amount = _amount;
            item = _item;
            slot = _slot;
        }
    }
    public class SwapItemsEventArgs
    {
        public Inventory inv;
        public int nativeSlot;
        public int targetSlot;
        public Item nativeItem;
        public Item targetItem;
        public int? amount;

        public SwapItemsEventArgs (Inventory _inv, int _nativeSlot, int _targetSlot, Item _nativeItem, Item _targetItem, int? _amount)
        {
            inv = _inv;
            nativeItem = _nativeItem;
            targetItem = _targetItem;
            nativeSlot = _nativeSlot;
            targetSlot = _targetSlot;
            amount = _amount;
        }
    }
    public class SwapItemsTrhuInvEventArgs
    {
        public Inventory nativeInv;
        public Inventory targetInv;
        public int? nativeSlot;
        public int? targetSlot;
        public Item nativeItem;
        public Item targetItem;
        public int? amount;

        public SwapItemsTrhuInvEventArgs(Inventory _nativeInv, Inventory _targetInv, int? _nativeSlot, int? _targetSlot, Item _nativeItem, Item _targetItem, int? _amount)
        {
            nativeInv = _nativeInv;
            targetInv = _targetInv;
            nativeItem = _nativeItem;
            targetItem = _targetItem;
            nativeSlot = _nativeSlot;
            targetSlot = _targetSlot;
            amount = _amount;
        }
    }
    public class UseItemEventArgs
    {
        public Inventory inv;
        public Item item;
        public int slot;

        public UseItemEventArgs(Inventory _inv, Item _item, int _slot)
        {
            inv = _inv;
            item = _item;
            slot = _slot;
        }
    }
    public class DropItemEventArgs 
    {
        public Inventory inv;
        public bool takenFromSpecificSlot;
        public int? slot;
        public Item item;
        public int amount;
        public bool droppedByUI;
        public UnityEngine.Vector3 positionDropped;

        public DropItemEventArgs(Inventory _inv, bool _takenFromSpecificSlot, int? _slot, Item _item, int _amount, bool _droppedByUI, UnityEngine.Vector3 _positionDropped)
        {
            inv = _inv;
            takenFromSpecificSlot = _takenFromSpecificSlot;
            slot = _slot;
            item = _item;
            amount = _amount;
            droppedByUI = _droppedByUI;
            positionDropped = _positionDropped;
        }
    }
    public class InitializeInventoryEventArgs 
    {
        public Inventory inventory;

        public InitializeInventoryEventArgs(Inventory _inv)
        {
            inventory = _inv;
        }
    }

    private void Awake()
    {
        current = this;
    }

    public void Broadcast(BroadcastEventType e,
                          AddItemEventArgs aea = null,
                          RemoveItemEventArgs rea = null,
                          SwapItemsEventArgs sea = null,
                          SwapItemsTrhuInvEventArgs siea = null,
                          UseItemEventArgs uea = null,
                          DropItemEventArgs dea = null,
                          InitializeInventoryEventArgs iea = null)
    {
        Debug.Log($"Broadcasting event {e}");
        switch (e)
        {
            case BroadcastEventType.AddItem:
                OnAddItem?.Invoke(this, aea);
                break;
            case BroadcastEventType.RemoveItem:
                OnRemoveItem?.Invoke(this, rea);
                break;
            case BroadcastEventType.SwapItem:
                OnSwapItem?.Invoke(this, sea);
                break;
            case BroadcastEventType.SwapTrhuInventory:
                OnSwapTrhuInventory?.Invoke(this, siea);
                break;
            case BroadcastEventType.UseItem:
                OnUseItem?.Invoke(this, uea);
                break;
            case BroadcastEventType.DropItem:
                OnDropItem?.Invoke(this, dea);
                break;
            case BroadcastEventType.PickUpItem:
                OnPickUpItem?.Invoke(this, aea);
                break;
            case BroadcastEventType.InitializeInventory:
                OnInitializeInventory?.Invoke(this, iea);
                break;
            default:
                break;
        }
    }
}