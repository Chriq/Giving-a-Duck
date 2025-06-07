using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node {
    public static GameManager Instance;
    public System.Collections.Generic.Dictionary<long, PlayerInfo> players = new();


    public Array<string> discoveredBeacons = new();
    private Array<Item> itemPool = new();

    [Signal]
    public delegate void ItemsChangedEventHandler();

    public override void _Ready() {
        Instance = this;
    }

    public void InitializeItems() {

        // if (!Multiplayer.IsServer()) {
        //     RpcId(1, MethodName.InitializeRemoteItems, , (int)item);
        // } else {
        //     InitializeRemoteItems(fromPlayerId, toPlayerId, item);
        // }


        foreach (Item item in Enum.GetValues(typeof(Item))) {
            itemPool.Add(item);
        }

        if (Multiplayer.IsServer()) {
            itemPool.Shuffle();

            foreach (PlayerInfo p in players.Values) {
                Item item = itemPool[0];
                Rpc(MethodName.InitializeRemoteItems, p.id, (int)item);
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void InitializeRemoteItems(long playerId, Item item) {
        players[playerId].items.Add(item);
        itemPool.Remove(item);
        EmitSignal(SignalName.ItemsChanged);
    }

    public void GiveItem(long fromPlayerId, long toPlayerId, Item item) {
        if (!Multiplayer.IsServer()) {
            RpcId(1, MethodName.ExecuteSend, fromPlayerId, toPlayerId, (int)item);
        } else {
            ExecuteSend(fromPlayerId, toPlayerId, item);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ExecuteSend(long fromPlayerId, long toPlayerId, Item item) {
        Rpc(MethodName.SyncItems, fromPlayerId, toPlayerId, (int)item);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void SyncItems(long fromPlayerId, long toPlayerId, Item item) {
        List<Item> toPlayerItemList = players[toPlayerId].items;
        List<Item> fromPlayerItemList = players[fromPlayerId].items;

        // Validate that the sending player has all items to send
        if (!fromPlayerItemList.Contains(item)) {
            GD.Print("ERROR! Sending player does not have item!");
            return;
        }

        fromPlayerItemList.Remove(item);
        toPlayerItemList.Add(item);

        GD.Print($"Send ({toPlayerId}, {item}) was completed by ({fromPlayerId})");
        EmitSignal(SignalName.ItemsChanged);
    }

    public void CheckBeacons() {
        if (discoveredBeacons.Count == Consts.NUM_TOTAL_BEACONS) {
            GD.Print("Go To Ending");
            if (Multiplayer.IsServer()) {
                Rpc(MethodName.ResetGame);
            } else {
                ResetGame();
            }

        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void ResetGame() {
        CallDeferred(MethodName.DeferredResetGame);
    }

    private void DeferredResetGame() {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");

        ClearAllData();

        Multiplayer.MultiplayerPeer.Close();
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
    }

    public void ClearAllData() {
        MapManager.Instance.ResetMap();
        discoveredBeacons.Clear();
        players.Clear();
    }

    public bool ClientHasItem(Item i) {
        return players[Multiplayer.GetUniqueId()].items.Contains(i);
    }
}