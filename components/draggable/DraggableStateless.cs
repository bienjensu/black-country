using Godot;
using Stateless;

namespace blackcountry.components.draggable;

[GlobalClass]
public partial class DraggableStateless : Node {
    [Export] private Control                      draggable;
    private          StateMachine<State, Trigger> dragState;
    private          Vector2                      moveTarget;
    [Export] private Node2D                       target;

    private void OnDragStart() {
        var tween = CreateTween();
        tween.TweenProperty(target, "scale", new Vector2(1.1f, 1.1f), 0.1f);
    }

    private void OnDragEnd() {
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(target, "scale",    new Vector2(1, 1), 0.1f);
        tween.TweenProperty(target, "skew",     0.0f,              0.1f);
        tween.TweenProperty(target, "rotation", 0.0f,              0.1f);
    }

    public override void _Ready() {
        base._Ready();

        dragState = new StateMachine<State, Trigger>(State.Released);

        dragState.Configure(State.Released)
                 .Permit(Trigger.Drag, State.Dragged)
                 .Ignore(Trigger.Release);

        dragState.Configure(State.Dragged)
                 .Permit(Trigger.Release, State.Released)
                 .OnEntry(OnDragStart)
                 // Reset move target
                 .OnExit(_ => moveTarget = Vector2.Zero)
                 .OnExit(_ => OnDragEnd())
                 .Ignore(Trigger.Drag);

        dragState.OnTransitioned(t => GD.Print(target.Name, " Transitioned[", t.Source,
                                               "->",
                                               t.Destination, "] by ", t.Trigger, "."));

        if (draggable != null)
            draggable.GuiInput += OnGuiInput;
    }

    public override void _Process(double delta) {
        base._Process(delta);

        if (dragState.IsInState(State.Dragged)) {
            var difference = target.Position -
                             target.Position.Lerp(target.GetPosition() + moveTarget,
                                                  (float)delta * 10f);

            target.Skew = -difference.X / 100f;

            target.Rotation = -difference.X / 100f;

            target.Translate(-difference);

            moveTarget += difference;
        }
    }

    private void OnGuiInput(InputEvent @event) {
        if (dragState.IsInState(State.Dragged))
            if (@event is InputEventMouseMotion e)
                moveTarget += target.Transform.BasisXform(e.Relative);

        switch (@event) {
            case InputEventMouseButton {
                     ButtonMask: MouseButtonMask.Left,
                     Pressed   : true
                 }:
                dragState.Fire(Trigger.Drag);
                break;
            case InputEventMouseButton {
                     //ButtonMask: MouseButtonMask.Left,
                     Pressed : false
                 }:
                dragState.Fire(Trigger.Release);
                break;
        }
    }

    private enum State {
        Dragged,
        Released
    }

    private enum Trigger {
        Drag,
        Release
    }
}