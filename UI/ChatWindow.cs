using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public partial class ChatWindow : Node {
    /* Chat Request UI Elements */
    [Export] Control chatRequestTarget;
    [Export] PackedScene chatRequestPrefab;

    // [Export] ItemList requestList;
    [Export] Button requestButton;

    [Export] ItemMenu itemMenu;
    // [Export] ItemList sendList;
    // [Export] ItemList playerList;
    // [Export] Button sendButton;

    public override void _Ready() {
        // UpdatePlayerList();
        // UpdateRequestableItems();

        // GameManager.Instance.ItemsChanged += UpdateSendableItems;

        requestButton.Pressed += OnRequest;
        // sendButton.Pressed += OnSend;
    }

    private void UpdateRequestableItems() {
        // System.Array allItems = Enum.GetValues(typeof(Item));
        // foreach (Item item in allItems) {
        //     requestList.AddItem(item.ToString());
        // }
    }

    private void UpdateSendableItems() {
        // sendList.Clear();
        // List<Item> myItems = GameManager.Instance.players.GetValueOrDefault(Multiplayer.GetUniqueId()).items;
        // foreach (Item item in myItems) {
        //     sendList.AddItem(item.ToString());
        // }
    }

    private void UpdatePlayerList() {
        // List<PlayerInfo> infoList = GameManager.Instance.players.Values.ToList();
        // foreach (PlayerInfo info in infoList) {
        //     if (info.id != Multiplayer.GetUniqueId()) playerList.AddItem(info.id.ToString());
        // }
    }

    public void OnRequest() {
        // TODO : Use player name instead of uniqueID
        int chunk = 0; // TODO : Retrieve chunk location of current player
        int item = itemMenu.selected;
        
        Rpc(MethodName.ExecuteRequest, Multiplayer.GetUniqueId(), chunk, item);

        ExecuteLocalRequest(Multiplayer.GetUniqueId(), chunk, item);
    }

    public void ExecuteLocalRequest(long playerId, long chunk, long item) {
        Control node = chatRequestPrefab.Instantiate<Control>();
        Label rq_label = node.GetNode<Label>("RequestLabel");

        rq_label.Text = $"You requested {item} from chunk {chunk}.";
        
        chatRequestTarget.AddChild(node);
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void ExecuteRequest(long playerId, long chunk, long item) {
        Control node = chatRequestPrefab.Instantiate<Control>();
        Label rq_label = node.GetNode<Label>("RequestLabel");

        rq_label.Text = $"Player {Multiplayer.GetUniqueId()} request from chunk {chunk}: {item}.";
        
        chatRequestTarget.AddChild(node);
    }

    public void OnSend() {
        // int sendToPlayerId = playerList.GetItemText(playerList.GetSelectedItems()[0]).ToInt();
        // Array<Item> sendItems = GetSelectedItemsFromList(sendList);
        // GD.Print("On Send, Sending To ", sendToPlayerId, " ", sendItems.Count, " items.");
        // GameManager.Instance.GiveItem(Multiplayer.GetUniqueId(), sendToPlayerId, sendItems);
    }

    // private Array<Item> GetSelectedItemsFromList(ItemList list) {
    //     // Array<Item> items = new();

    //     // foreach (int i in list.GetSelectedItems()) {
    //     //     string text = list.GetItemText(i);
    //     //     Item item = (Item)Enum.Parse(typeof(Item), text);
    //     //     items.Add(item);
    //     // }

    //     // return items;
    // }
}
