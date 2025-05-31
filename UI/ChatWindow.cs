using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public struct itemRequestInfo
{
    public itemRequestInfo(int playerId, int item)
    {
        this.playerId = playerId;
        this.item = item;
    }

    public int playerId;
    public int item;
}

public partial class ChatWindow : Node {
    /* Chat Request UI Elements */
    [Export] Control chatRequestTarget;
    [Export] PackedScene chatRequestPrefab;
    [Export] PackedScene chatLogPrefab;

    // [Export] ItemList requestList;
    [Export] Button requestButton;

    [Export] ItemMenu itemMenu;

    System.Collections.Generic.Dictionary<itemRequestInfo, Control> itemRequestList;
    // [Export] ItemList sendList;
    // [Export] ItemList playerList;
    // [Export] Button sendButton;

    public override void _Ready() {
        itemRequestList = new System.Collections.Generic.Dictionary<itemRequestInfo, Control>();
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
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    public void ExecuteRequest(int playerId, int chunk, int item) {
        
        Control node;

        if (playerId == Multiplayer.GetUniqueId())
        {
            node = chatLogPrefab.Instantiate<Control>();
            Label rq_label = node.GetNode<Label>("RequestLabel");

            rq_label.Text = $"You requested {item} from chunk {chunk}.";
        }
        else
        {
            node = chatRequestPrefab.Instantiate<Control>();

            Label rq_label = node.GetNode<Label>("RequestLabel");
            rq_label.Text = $"Player {playerId} request from chunk {chunk}: {item}.";
        
            Button rq_button = node.GetNode<Button>("SendButton");
            rq_button.Pressed += () => OnSend(playerId, item);
        }
        itemRequestList.Add( new itemRequestInfo(playerId, item), node);
        chatRequestTarget.AddChild(node);
    }

    public void OnSend(int playerId, int item) {
        if (Multiplayer.IsServer())
        {
            ExecuteSend(Multiplayer.GetUniqueId(), playerId, item);
        }
        else
        {
            RpcId(1, MethodName.ExecuteSend, Multiplayer.GetUniqueId(), playerId, item);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void ExecuteSend(int completerID, int playerId, int item ) {
        itemRequestInfo rq = new itemRequestInfo(playerId, item);

        if (itemRequestList.ContainsKey(rq))
        {
            Rpc(MethodName.CompleteSend, completerID, playerId, item);
        }
        itemRequestList.Remove(rq);
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    public void CompleteSend(int completerID, int playerId, int item) {
        GD.Print($"Send ({playerId}, {item}) was completed by ({completerID})");
        // int sendToPlayerId = playerList.GetItemText(playerList.GetSelectedItems()[0]).ToInt();
        // Array<Item> sendItems = GetSelectedItemsFromList(sendList);
        // GD.Print("On Send, Sending To ", sendToPlayerId, " ", sendItems.Count, " items.");
        // GameManager.Instance.GiveItem(Multiplayer.GetUniqueId(), sendToPlayerId, sendItems);

        // TODO: Remove From chat window

        // TODO: Update Item Lists

    }
}
