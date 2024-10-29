using System.Collections.Generic;
using Godot;
using Godot.Collections;

[Tool]
public partial class Collapsible : Node {
    [Export] private Control        collapsible  = new();
    [Export] private Array<Control> collapsibles = new();

    private CollapseState state = CollapseState.Collapsed;

    [Export]
    private CollapseState collapseState {
        get => state;
        set {
            state = value;
            update_collapsibles();
            GD.Print("Collapse State: " + state);
        }
    }

    public override void _Ready() {
        base._Ready();

        collapsible.GuiInput += on_gui_input;

        update_collapsibles();
    }

    public override string[] _GetConfigurationWarnings() {
        List<string> warnings = [];

        if (collapsibles.Count == 0) warnings.Add("No collapsibles found.");
        if (collapsible is null) warnings.Add("No collapsible found.");

        return warnings.ToArray();
    }

    private void on_gui_input(InputEvent @event) {
        (state, var changed) = (@event, state) switch {
                                   (InputEventMouseButton {
                                        ButtonMask : MouseButtonMask.Left,
                                        DoubleClick: true
                                    }, CollapseState.Collapsed) =>
                                       (CollapseState.Expanded, true),
                                   (InputEventMouseButton {
                                        ButtonMask : MouseButtonMask.Left,
                                        DoubleClick: true
                                    }, CollapseState.Expanded) =>
                                       (CollapseState.Collapsed, true),
                                   (_, _) => (state, false)
                               };
        @event.Dispose();
        if (changed) collapseState = state;
    }

    private void update_collapsibles() {
        if (collapseState is CollapseState.Collapsed)
            foreach (var child in collapsibles)
                child.Hide();
        else
            foreach (var child in collapsibles)
                child.Show();
    }

    internal enum CollapseState {
        Collapsed,
        Expanded
    }
}