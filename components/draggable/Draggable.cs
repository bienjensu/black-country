using Godot;

public partial class Draggable : Node {
    [Export] private Control   draggable;
    private          DragState dragState = DragState.Released;
    private          Vector2   moveTarget;
    [Export] private Node2D    target;

    public override void _Ready() {
        base._Ready();

        draggable.GuiInput += on_gui_input;
    }

    public override void _Process(double delta) {
        base._Process(delta);

        if (dragState is DragState.Dragging) {
            target.Scale = new Vector2(1.1f, 1.1f);
            var difference = target.Position -
                             target.Position.Lerp(target.GetPosition() + moveTarget,
                                                  (float)delta * 10f);
            target.Translate(-difference);

            moveTarget += difference;
        }
        else {
            target.Scale = Vector2.One;
        }
    }

    private void on_gui_input(InputEvent @event) {
        dragState = (@event, dragState) switch {
                        (InputEventMouseButton {
                             ButtonIndex: MouseButton.Left,
                             Pressed    : true
                         }, DragState.Released) => DragState.Dragging,

                        (InputEventMouseButton {
                             ButtonIndex: MouseButton.Left,
                             Pressed    : false
                         }, DragState.Dragging) => DragState.Released,

                        (_, _) => dragState
                    };

        if (@event is InputEventMouseMotion motion)
            moveTarget += target.Transform.BasisXform(motion.Relative);

        if (dragState is DragState.Released) moveTarget = Vector2.Zero;
    }

    private enum DragState {
        Dragging,
        Released
    }
}