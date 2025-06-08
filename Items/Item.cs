using Godot;
using System;
using System.Collections.Generic;

public static class ItemClass {
    public static string[] ItemNames = new string[] {
        "A5",
        "B5",
        "c5",
        "d5",
        "e5",
        "f5",
        "g5",
        "h5",
        "j5",
        "k5",
        "k5",
        ";5",
    };
    public static string[] ItemDescriptions = new string[] {
        "A",
        "B",
        "c",
        "d",
        "e",
        "f",
        "g",
        "h",
        "j",
        "k",
        "k",
        ";",
    };

    public static Dictionary<Item, string> descriptions = new() {
        {Item.DOUBLE_JUMP, "Jump again while mid-air to go higher and farther [SPACE]"},
        {Item.DASH, "Gain an instant burst of speed in the direction you are moving [SHIFT]"},
        {Item.WALL_JUMP, "Jump off a wall to gain a burst of speed up and away from it [SPACE]"},
        {Item.CLIMB, "Climb up a sheer surface, but get tired after 8 tiles [W]"}
    };

    public static string GetItemName(Item item) {
        return Enum.GetName(item).Replace("_", " ");
    }
}


public enum Item {
    DOUBLE_JUMP,
    WALL_JUMP,
    DASH,
    CLIMB
}

enum ItemFlag {
    DOUBLE_JUMP = 0x01 << 0,
    SPRING = 0x01 << 1,
    WALL_JUMP = 0x01 << 2,
    CLIMB = 0x01 << 3,
    DASH = 0x01 << 4,
    BOMB = 0x01 << 5,
    PLATFORMS = 0x01 << 6,
    SIZE = 0x01 << 7,
    LANTERN = 0x01 << 8,
    TORCH = 0x01 << 9,
    ICE_BOOTS = 0x01 << 10,
    Goggles = 0x01 << 11,
}
