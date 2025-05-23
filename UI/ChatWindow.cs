using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public partial class ChatWindow : Node {
    [Export] ItemList requestList;
    [Export] Button requestButton;
    [Export] ItemList sendList;
    [Export] ItemList playerList;
    [Export] Button sendButton;

    public override void _Ready() {
        UpdatePlayerList();
        UpdateRequestableItems();

        GameManager.Instance.ItemsChanged += UpdateSendableItems;

        requestButton.Pressed += OnRequest;
        sendButton.Pressed += OnSend;
    }

    private void UpdateRequestableItems() {
        System.Array allItems = Enum.GetValues(typeof(Item));
        foreach (Item item in allItems) {
            requestList.AddItem(item.ToString());
        }
    }

    private void UpdateSendableItems() {
        sendList.Clear();
        List<Item> myItems = GameManager.Instance.players.GetValueOrDefault(Multiplayer.GetUniqueId()).items;
        foreach (Item item in myItems) {
            sendList.AddItem(item.ToString());
        }
    }

    private void UpdatePlayerList() {
        List<PlayerInfo> infoList = GameManager.Instance.players.Values.ToList();
        foreach (PlayerInfo info in infoList) {
            if (info.id != Multiplayer.GetUniqueId()) playerList.AddItem(info.id.ToString());
        }
    }

    public void OnRequest() {
        Array<Item> requestedItems = GetSelectedItemsFromList(requestList);
        GameManager.Instance.RequestItem(Multiplayer.GetUniqueId(), requestedItems);
    }

    public void OnSend() {
        int sendToPlayerId = playerList.GetItemText(playerList.GetSelectedItems()[0]).ToInt();
        Array<Item> sendItems = GetSelectedItemsFromList(sendList);
        GD.Print("On Send, Sending To ", sendToPlayerId, " ", sendItems.Count, " items.");
        GameManager.Instance.GiveItem(Multiplayer.GetUniqueId(), sendToPlayerId, sendItems);
    }

    private Array<Item> GetSelectedItemsFromList(ItemList list) {
        Array<Item> items = new();

        foreach (int i in list.GetSelectedItems()) {
            string text = list.GetItemText(i);
            Item item = (Item)Enum.Parse(typeof(Item), text);
            items.Add(item);
        }

        return items;
    }
}
