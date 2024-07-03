using Godot;
using System;
using System.Collections.Generic;

public partial class Weapon : Resource
{
    [Export] public string name { get; set; }
    [Export] public Texture2D texture { get; set; }
    [Export] public Texture2D icon { get; set; }
    [Export] public float damage { get; set; }
    [Export] public float firerate { get; set; }
    [Export] public string playerState { get; set; }
    [Export] public string weaponAnimation { get; set; }
    [Export] public int[] projectiles { get; set; }
    [Export] public int[] properties { get; set; }
}
