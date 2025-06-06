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
        foreach (Item i in Enum.GetValues(typeof(Item))) {
            itemPool.Add(i);
        }

        foreach (PlayerInfo p in players.Values) {
            Item item = itemPool.PickRandom();
            p.items.Add(item);
            itemPool.Remove(item);
        }

        //EmitSignal(SignalName.ItemsChanged);
    }

    // public void RequestItem(long requestPlayerId, Item item) {
    //     int chunk = 0;
    //     Rpc(MethodName.ExecuteRequest, Multiplayer.GetUniqueId(), chunk, (int)item);
    // }


    // [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    // private void ExecuteRequest(long requestPlayerId, int chunk, int item) {
    //     // TODO, send to chat
    // }

    public void GiveItem(long fromPlayerId, long toPlayerId, Item item) {
        if (!Multiplayer.IsServer()) {
            RpcId(1, MethodName.ExecuteSend, fromPlayerId, toPlayerId, (int)item);
        } else {
            ExecuteSend(fromPlayerId, toPlayerId, item);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ExecuteSend(long fromPlayerId, long toPlayerId, Item item) {
        List<Item> toPlayerItemList = players.GetValueOrDefault(toPlayerId).items;
        List<Item> fromPlayerItemList = players.GetValueOrDefault(fromPlayerId).items;

        // Validate that the sending player has all items to send
        if (!fromPlayerItemList.Contains(item)) return;

        fromPlayerItemList.Remove(item);
        toPlayerItemList.Add(item);

        Rpc(MethodName.SyncItems, fromPlayerId, toPlayerId, (int)item);

        EmitSignal(SignalName.ItemsChanged);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SyncItems(long fromPlayerId, long toPlayerId, Item item) {
        List<Item> toPlayerItemList = players.GetValueOrDefault(toPlayerId).items;
        List<Item> fromPlayerItemList = players.GetValueOrDefault(fromPlayerId).items;

        fromPlayerItemList.Remove(item);
        toPlayerItemList.Add(item);
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
}