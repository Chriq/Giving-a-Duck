using Godot;
using System;
using System.Collections.Generic;

public partial class ItemMenu : Node {
    [Export] private Control itemIconButtonContainer;
    [Export] private Control itemPopupcontainer;
    [Export] private PackedScene itemButton;
    [Export] private Button requestButton;
    //private Godot.Collections.Array<Node> itemIconButtons = new();
    private Dictionary<Item, TextureButton> itemIconButtons = new();

    public Item selected;

    public override void _Ready() {
        foreach (Item item in Enum.GetValues(typeof(Item))) {
            TextureButton b = itemButton.Instantiate<TextureButton>();

            b.Pressed += () => SelectItem(item);
            b.MouseEntered += () => ShowItemLabels(item);
            b.MouseExited += HideItemLabels;

            itemIconButtonContainer.AddChild(b);
            itemIconButtons.Add(item, b);

            TextureRect icon = b.GetChild<TextureRect>(0);
            icon.Texture = ResourceLoader.Load<Texture2D>("res://UI/" + Enum.GetName(item) + ".png");
        }
        requestButton.Pressed += HideItemLabels;

        GameManager.Instance.ItemsChanged += UpdateItems;
    }

    public void SelectItem(Item item) {
        if (!GameManager.Instance.ClientHasItem(item)) {
            selected = item;
            requestButton.Disabled = false;
            ShowItemLabels(item);
        } else {
            requestButton.Disabled = true;
        }
    }

    private void UpdateItems() {
        foreach ((Item i, TextureButton b) in itemIconButtons) {
            if (GameManager.Instance.ClientHasItem(i)) {
                b.Disabled = true;
            } else {
                b.Disabled = false;
            }
        }
    }

    private void ShowItemLabels(Item item) {
        Label labelName = itemPopupcontainer.GetChild<Label>(0); // Set string
        Label labelDescription = itemPopupcontainer.GetChild<Label>(1);

        labelName.Text = Enum.GetName(item).Replace("_", " ");
        labelDescription.Text = ItemClass.descriptions[item];

        if (GameManager.Instance.ClientHasItem(item)) {
            labelDescription.Text += "\n*You Have this ability.*";
        }
    }

    public void HideItemLabels() {
        if (requestButton.Disabled) {
            Label labelName = itemPopupcontainer.GetChild<Label>(0); // Set string
            Label labelDescription = itemPopupcontainer.GetChild<Label>(1);

            labelName.Text = "";
            labelDescription.Text = "";
        }
    }
}
