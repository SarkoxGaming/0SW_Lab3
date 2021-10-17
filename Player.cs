using Godot;
using System;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Vector2 Up = new Vector2(0,-1);

    const int GRAVITY = 20;
    const int MAX_FALL_SPEED = 200;
    const int MAX_SPEED = 80;
    const int JUMP_FORCE = 500;
    const int Acceleration = 10;

    bool facing_right = true;

    Sprite currentSprite;
    AnimationPlayer animationPlayer;
    private AnimationTree animationTree = null;
	private AnimationNodeStateMachinePlayback animationState = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationTree = GetNode<AnimationTree>("AnimationTree");
		animationState = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");
        animationState.Travel("Idle");
        
    }
    Vector2 Velocity = new Vector2();

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        var input_vector = Vector2.Zero;
        input_vector.x = Input.GetActionStrength("ui_right") 
                            - Input.GetActionStrength("ui_left");
        input_vector.y = Input.GetActionStrength("ui_down") 
                            - Input.GetActionStrength("ui_up");
        
        input_vector = input_vector.Normalized();

        Velocity.y += GRAVITY;
        if (Velocity.y > MAX_FALL_SPEED) {
            Velocity.y = MAX_FALL_SPEED;
        }

        Velocity.x = Velocity.Clamped(MAX_SPEED).x;


        if (input_vector != Vector2.Zero) {
            animationTree.Set("parameters/Idle/blend_position", input_vector);
		    animationTree.Set("parameters/Running/blend_position", input_vector);
            animationTree.Set("parameters/Attacking/blend_position", input_vector);
            animationTree.Set("parameters/Jump/blend_position", input_vector);
        
        }
        
        if (Input.GetActionStrength("ui_attack") > 0) {
            animationState.Travel("Attacking");
        }
        if (input_vector.x == 1 && IsOnFloor()) {
            Velocity.x += Acceleration;
            facing_right = true;
            animationState.Travel("Running");
            
        } else if (input_vector.x == -1 && IsOnFloor()) {
            Velocity.x -= Acceleration;
            facing_right = false;
            animationState.Travel("Running");
        } else {
            Velocity = Velocity.LinearInterpolate(Vector2.Zero, 0.2f);
        }
        if (Input.GetActionStrength("ui_left") == 0 && Input.GetActionStrength("ui_right") == 0 && Input.GetActionStrength("ui_attack") == 0 && IsOnFloor()) {
            animationState.Travel("Idle");
        }
        if (IsOnFloor()) {
            if (Input.IsActionJustPressed("ui_jump")) {
                animationState.Travel("Jump");
                Velocity.y = -JUMP_FORCE;
            }
        }
        /*
        if (!IsOnFloor()) {
            if (Velocity.y < 0) {
                animationPlayer.Play("Jumping");
            } else {
                animationPlayer.Play("Falling");  
            }
        }
        */
        Velocity = MoveAndSlide(Velocity, Up);
    }
}
