using Godot;
using System;
using System.Collections.Generic;

public partial class ItemDisplay : Label {
    public override void _Process(double delta) {
        List<Item> items = GameManager.Instance.players.GetValueOrDefault(Multiplayer.GetUniqueId()).items;
        if (items.Count > 0) {
            string display = "";
            foreach (Item i in items) {
                display += i + " ";
            }
            Text = display;
        } else {
            Text = "";
        }
    }

}
