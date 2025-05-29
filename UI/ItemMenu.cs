using Godot;
using System;

public partial class ItemMenu : Node
{
    [Export] private Control itemIconButtonContainer;
    [Export] private Control itemPopupcontainer;
    private Godot.Collections.Array<Node> itemIconButtons;
    
    public int selected;

    public override void _Ready() {
        itemIconButtons = itemIconButtonContainer.GetChildren();
        foreach (TextureButton tb in itemIconButtons)
        {
            tb.Pressed += () => Select(tb);
        }
    }

    public void Select(Node selectedButton)
    {
        selected = itemIconButtons.IndexOf(selectedButton);
        GD.Print($"ItemMenu: Selected Item: {selected}");

        // TODO: Icon Selector Image / Effect

        OpenItemDescription(selected);
        
    }

    public void OpenItemDescription(int selected)
    {
        itemPopupcontainer.Show();

        Label labelName = itemPopupcontainer.GetChild<Label>(0); // Set string
        labelName.Text = ItemClass.ItemNames[selected];
        Label labelDescription = itemPopupcontainer.GetChild<Label>(1);
        labelDescription.Text = ItemClass.ItemDescriptions[selected];

        // TODO Check if item is currently held
        // Disable Request if already held
    }
}
