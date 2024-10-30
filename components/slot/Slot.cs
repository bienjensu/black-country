using blackcountry.components.item;
using Godot;

namespace blackcountry.components.slot;

[Tool]
[GlobalClass]
public partial class Slot : Area2D {
    [Export] private SlotType slotType;

    public bool AcceptsItem(Item item) {
        var accepted = (item, slotType) switch {
                           (_, SlotType.All)       => true,
                           (Money, SlotType.Money) => true,
                           (_, _)                  => false
                       };
        return accepted;
    }

    private enum SlotType {
        Money,
        All
    }
}