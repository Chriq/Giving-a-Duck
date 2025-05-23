using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node {
    public static GameManager Instance;
    public System.Collections.Generic.Dictionary<long, PlayerInfo> players = new();

    [Signal]
    public delegate void ItemsChangedEventHandler();

    public override void _Ready() {
        Instance = this;
    }

    public void InitializeItems() {
        int index = 0;
        foreach (PlayerInfo p in players.Values) {
            if (index == 0) p.items.Add(Item.DOUBLE_JUMP);
            else p.items.Add(Item.WALL_JUMP);

            index++;
        }

        EmitSignal(SignalName.ItemsChanged);
    }

    public void RequestItem(long requestPlayerId, Array<Item> items) {
        // TODO: send to chat
    }

    public void GiveItem(long fromPlayerId, long toPlayerId, Array<Item> items) {
        if (!Multiplayer.IsServer()) {
            RpcId(1, MethodName.ExecuteSend, fromPlayerId, toPlayerId, items);
        } else {
            ExecuteSend(fromPlayerId, toPlayerId, items);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ExecuteRequest(long requestPlayerId, Item item) {
        // TODO, send to chat
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ExecuteSend(long fromPlayerId, long toPlayerId, Array<Item> items) {
        List<Item> toPlayerItemList = players.GetValueOrDefault(toPlayerId).items;
        List<Item> fromPlayerItemList = players.GetValueOrDefault(fromPlayerId).items;

        // Validate that the sending player has all items to send
        if (!items.All(fromPlayerItemList.Contains)) return;

        fromPlayerItemList.RemoveAll(items.Contains);
        toPlayerItemList.AddRange(items);

        Rpc(MethodName.SyncItems, fromPlayerId, toPlayerId, items);

        EmitSignal(SignalName.ItemsChanged);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SyncItems(long fromPlayerId, long toPlayerId, Array<Item> items) {
        List<Item> toPlayerItemList = players.GetValueOrDefault(toPlayerId).items;
        List<Item> fromPlayerItemList = players.GetValueOrDefault(fromPlayerId).items;

        fromPlayerItemList.RemoveAll(items.Contains);
        toPlayerItemList.AddRange(items);
    }
}