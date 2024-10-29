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
            var difference = target.Position -
                             target.Position.Lerp(target.GetPosition() + moveTarget,
                                                  (float)delta * 10f);

            target.Skew = -difference.X / 100f;

            target.Rotation = -difference.X / 100f;

            target.Translate(-difference);

            moveTarget += difference;
        }
    }

    private void on_gui_input(InputEvent @event) {
        (dragState, var changed) = (@event, dragState) switch {
                                       (InputEventMouseButton {
                                            ButtonIndex: MouseButton.Left,
                                            Pressed    : true
                                        }, DragState.Released) => (DragState.Dragging,
                                                                   true),

                                       (InputEventMouseButton {
                                            ButtonIndex: MouseButton.Left,
                                            Pressed    : false
                                        }, DragState.Dragging) => (DragState.Released,
                                                                   true),

                                       (_, _) => (dragState, false)
                                   };

        if (@event is InputEventMouseMotion motion)
            moveTarget += target.Transform.BasisXform(motion.Relative);

        @event.Dispose();

        if (dragState is DragState.Released) moveTarget = Vector2.Zero;

        if (!changed) return;
        if (dragState is DragState.Released) {
            moveTarget = Vector2.Zero;
            var tween = CreateTween();
            tween.SetParallel();
            tween.TweenProperty(target, "skew",     0f,          0.1);
            tween.TweenProperty(target, "rotation", 0f,          0.1);
            tween.TweenProperty(target, "scale",    Vector2.One, 0.1);
        }
        else {
            target.MoveToFront();
            var tween = CreateTween();
            tween.TweenProperty(target, "scale", Vector2.One * 1.1f, 0.1);
        }
    }

    private enum DragState {
        Dragging,
        Released
    }
}