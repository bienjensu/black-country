using Godot;

namespace blackcountry.components.item;

[Tool]
[GlobalClass]
public partial class Money : Item {
    [Export] private int amount;
}