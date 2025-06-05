using Godot;
using System;

public partial class ItemMenu : Node {
    [Export] private Control itemIconButtonContainer;
    [Export] private Control itemPopupcontainer;
    [Export] private PackedScene itemButton;
    private Godot.Collections.Array<Node> itemIconButtons = new();

    public Item selected;

    public override void _Ready() {
        // itemIconButtons = itemIconButtonContainer.GetChildren();
        // foreach (TextureButton tb in itemIconButtons)
        // {
        //     tb.Pressed += () => Select(tb);
        // }

        foreach (Item item in Enum.GetValues(typeof(Item))) {
            TextureButton b = itemButton.Instantiate<TextureButton>();
            b.Pressed += () => SelectItem(item);
            itemIconButtonContainer.AddChild(b);
            itemIconButtons.Add(b);

            TextureRect icon = b.GetChild<TextureRect>(0);
            icon.Texture = ResourceLoader.Load<Texture2D>("res://UI/" + Enum.GetName(item) + ".png");
        }
    }

    public void Select(Node selectedButton) {
        //selected = itemIconButtons.IndexOf(selectedButton);
        GD.Print($"ItemMenu: Selected Item: {selected}");

        // TODO: Icon Selector Image / Effect

        //OpenItemDescription(selected);

    }

    public void OpenItemDescription(int selected) {
        itemPopupcontainer.Show();

        Label labelName = itemPopupcontainer.GetChild<Label>(0); // Set string
        labelName.Text = ItemClass.ItemNames[selected];
        Label labelDescription = itemPopupcontainer.GetChild<Label>(1);
        labelDescription.Text = ItemClass.ItemDescriptions[selected];

        // TODO Check if item is currently held
        // Disable Request if already held
    }

    public void SelectItem(Item item) {
        selected = item;
        itemPopupcontainer.Show();

        Label labelName = itemPopupcontainer.GetChild<Label>(0); // Set string
        Label labelDescription = itemPopupcontainer.GetChild<Label>(1);

        labelName.Text = Enum.GetName(item).Replace("_", " ");
        labelDescription.Text = ItemClass.descriptions[item];

        // TODO Check if item is currently held
        // Disable Request if already held
    }
}
