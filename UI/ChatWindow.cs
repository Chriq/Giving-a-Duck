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

    [Export] Label doorText;

    System.Collections.Generic.Dictionary<itemRequestInfo, Control> itemRequestList;

    public override void _Ready() {
        requestButton.Disabled = true;
        itemRequestList = new System.Collections.Generic.Dictionary<itemRequestInfo, Control>();

        requestButton.Pressed += OnRequest;

        GameManager.Instance.AllBeaconsFound += ShowDoorText;
    }

    private void ShowDoorText() {
        doorText.Show();
    }


    public void OnRequest() {
        requestButton.Disabled = true;
        int chunk = MapManager.Instance.currentChunk;
        Item item = (Item)itemMenu.selected;

        Rpc(MethodName.ExecuteRequest, Multiplayer.GetUniqueId(), chunk, (int)item);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void ExecuteRequest(int playerId, int chunk, int item) {
        Control node;

        if (playerId == Multiplayer.GetUniqueId()) {
            node = chatLogPrefab.Instantiate<Control>();
            Label rq_label = node.GetNode<Label>("RequestLabel");

            rq_label.Text = $"You requested {ItemClass.GetItemName((Item)item)} from chunk {chunk}.";
        } else {
            node = chatRequestPrefab.Instantiate<Control>();

            Label rq_label = node.GetNode<Label>("RequestLabel");
            string nameDisplay = GameManager.Instance.players[playerId].name;
            rq_label.Text = $"Will you give {(!string.IsNullOrEmpty(nameDisplay) ? nameDisplay : "Player " + playerId)} in Chunk {chunk} a {ItemClass.GetItemName((Item)item)}?";

            Button send_button = node.GetNode<Button>("SendButton");
            if (GameManager.Instance.ClientHasItem((Item)item)) {
                send_button.Pressed += () => {
                    OnSend(playerId, item);
                    CallDeferred(MethodName.RemoveCompletedRequest, node);
                };
            } else {
                send_button.Disabled = true;
            }
        }
        itemRequestList.Add(new itemRequestInfo(playerId, item), node);
        chatRequestTarget.AddChild(node);
    }

    public void OnSend(int toPlayerId, int item) {
        if (Multiplayer.IsServer()) {
            GD.Print(Multiplayer.GetUniqueId() + " sending item to " + toPlayerId);
            ExecuteSend(Multiplayer.GetUniqueId(), toPlayerId, item);
        } else {
            RpcId(1, MethodName.ExecuteSend, Multiplayer.GetUniqueId(), toPlayerId, item);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void ExecuteSend(int fromPlayerId, int toPlayerId, int item) {
        itemRequestInfo rq = new itemRequestInfo(toPlayerId, item);

        if (itemRequestList.ContainsKey(rq)) {
            Rpc(MethodName.CompleteSend, fromPlayerId, toPlayerId, item);
        }
        itemRequestList.Remove(rq);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CompleteSend(int fromPlayerId, int toPlayerId, Item item) {
        GameManager.Instance.GiveItem(fromPlayerId, toPlayerId, item);
    }

    private void RemoveCompletedRequest(Control control) {
        control.QueueFree();
    }
}
