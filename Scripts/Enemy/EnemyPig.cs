using Godot;
using System;

public partial class EnemyPig : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	EnemyAnimationHandler animationHandler;

    public override void _Ready()
    {
		animationHandler = GetNode<EnemyAnimationHandler>("AnimationHandler");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}


		animationHandler.IdleAnimation();
		Velocity = velocity;
		MoveAndSlide();
	}
}
