using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public struct itemRequestInfo {
    public itemRequestInfo(int playerId, int item) {
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

    [Export] Button requestButton;

    [Export] ItemMenu itemMenu;

    System.Collections.Generic.Dictionary<itemRequestInfo, Control> itemRequestList;

    public override void _Ready() {
        itemRequestList = new System.Collections.Generic.Dictionary<itemRequestInfo, Control>();
        // GameManager.Instance.ItemsChanged += UpdateSendableItems;

        requestButton.Pressed += OnRequest;
    }

    public void OnRequest() {
        // TODO : Use player name instead of uniqueID
        int chunk = 0; // TODO : Retrieve chunk location of current player
        Item item = itemMenu.selected;

        Rpc(MethodName.ExecuteRequest, Multiplayer.GetUniqueId(), chunk, (int)item);
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void ExecuteRequest(int playerId, int chunk, int item) {

        Control node;

        if (playerId == Multiplayer.GetUniqueId()) {
            node = chatLogPrefab.Instantiate<Control>();
            Label rq_label = node.GetNode<Label>("RequestLabel");

            rq_label.Text = $"You requested {item} from chunk {chunk}.";
        } else {
            node = chatRequestPrefab.Instantiate<Control>();

            Label rq_label = node.GetNode<Label>("RequestLabel");
            string nameDisplay = GameManager.Instance.players[playerId].name;
            GD.Print(nameDisplay);
            rq_label.Text = $"{(!string.IsNullOrEmpty(nameDisplay) ? nameDisplay : "Player " + playerId)} request from chunk {chunk}: {item}.";

            Button rq_button = node.GetNode<Button>("SendButton");
            rq_button.Pressed += () => {
                OnSend(playerId, item);
                CallDeferred(MethodName.RemoveCompletedRequest, node);
            };
        }
        itemRequestList.Add(new itemRequestInfo(playerId, item), node);
        chatRequestTarget.AddChild(node);
    }

    public void OnSend(int playerId, int item) {
        if (Multiplayer.IsServer()) {
            ExecuteSend(Multiplayer.GetUniqueId(), playerId, item);
        } else {
            RpcId(1, MethodName.ExecuteSend, Multiplayer.GetUniqueId(), playerId, item);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void ExecuteSend(int completerID, int playerId, int item) {
        itemRequestInfo rq = new itemRequestInfo(playerId, item);

        if (itemRequestList.ContainsKey(rq)) {
            Rpc(MethodName.CompleteSend, completerID, playerId, item);
        }
        itemRequestList.Remove(rq);
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void CompleteSend(int completerID, int playerId, Item item) {
        GD.Print($"Send ({playerId}, {item}) was completed by ({completerID})");
        GameManager.Instance.GiveItem(Multiplayer.GetUniqueId(), playerId, item);

        // TODO: Remove From chat window

        // TODO: Update Item Lists

    }

    private void RemoveCompletedRequest(Control control) {
        control.QueueFree();
    }
}
