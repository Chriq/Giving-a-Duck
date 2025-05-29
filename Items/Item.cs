using Godot;
using System;

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
}


public enum Item {
    DOUBLE_JUMP,
    WALL_JUMP,
    DASH
}

enum ItemFlag
{
    DOUBLE_JUMP = 0x01 <<  0,
    SPRING      = 0x01 <<  1,
    WALL_JUMP   = 0x01 <<  2,
    CLIMB       = 0x01 <<  3,
    DASH        = 0x01 <<  4,
    BOMB        = 0x01 <<  5,
    PLATFORMS   = 0x01 <<  6,
    SIZE        = 0x01 <<  7,
    LANTERN     = 0x01 <<  8,
    TORCH       = 0x01 <<  9,
    ICE_BOOTS   = 0x01 << 10,
    Goggles     = 0x01 << 11,
}
