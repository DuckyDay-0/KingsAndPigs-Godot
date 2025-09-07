using Godot;
using System;

public partial class PlayerScript : CharacterBody2D
{
    private const int speed = 400;
    private const int jumpVelocity = -300;
    AnimatedSprite2D animatedSprite;
    public bool isAttacking = false;
    private string currentAnimation = "";
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D_Player");
        animatedSprite.AnimationFinished += OnAnimationFinished;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        var direction = Input.GetAxis("left", "right");

        //handles attack 
        if (Input.IsActionJustPressed("attack") && !isAttacking)
        {
            GD.Print("attack");
            isAttacking = true;
            animatedSprite.Play("attackKingHuman");
        }
        if (!isAttacking)
        {
            //handles gravity
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            {
                velocity.Y = jumpVelocity;
                if (!isAttacking)
                {
                    animatedSprite.Play("jumpKingHuman");
                }
            }

            //handles run animation 
            if (IsOnFloor())
            {
                if (direction == 0)
                {
                    animatedSprite.Play("idleKingHuman");
                }
                else
                {
                    animatedSprite.Play("runKingHuman");
                }
            }
        }
        //handles sprite flip
        if (direction > 0)
        {
            animatedSprite.FlipH = false;
        }
        else if (direction < 0)
        {
            animatedSprite.FlipH = true;
        }


        //handles movement 
        if (direction != 0)
        {
            velocity.X = direction * speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, speed); //fix declarationasdasd
        }




        Velocity = velocity;
        MoveAndSlide();
    }

    private void OnAnimationFinished()
    {
        if (animatedSprite.Animation == "attackKingHuman")
        { 
            isAttacking = false;
        }

    }
}
