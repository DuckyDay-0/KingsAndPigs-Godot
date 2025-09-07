using Godot;
using System;

public partial class EnemyKingPigScript : CharacterBody2D
{

    public int health = 100;
    AnimatedSprite2D animatedSprite;
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D-Enemy");
        animatedSprite.Play("idleKingPig");
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animatedSprite.Play("hitKingPig");
        GD.Print(health);
    }
    public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;
        //handles gravity
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        Velocity = velocity;
        MoveAndSlide();
    }
}
