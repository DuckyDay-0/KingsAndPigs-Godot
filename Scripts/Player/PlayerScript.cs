using Godot;
using System;

public partial class PlayerScript : CharacterBody2D,  IPlayer
{
    private const int speed = 400;
    private const int jumpVelocity = -300;
    AnimatedSprite2D animatedSprite;
    CollisionShape2D collisionShape;
    CollisionShape2D collisionShapeAttack;
    Area2D area;
    public bool isAttacking = false;
    private string currentAnimation = "";

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D_Player");
        animatedSprite.AnimationFinished += OnAnimationFinished;
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        collisionShapeAttack = GetNode<CollisionShape2D>("AttackArea2D/CollisionShape2D-AttackCollision");
        area = GetNode<Area2D>("AttackArea2D");
        area.BodyEntered += OnAttackHit;

        Global.player = this;

    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        var direction = Input.GetAxis("left", "right");

        //handles attack 
        if (Input.IsActionJustPressed("attack") && !isAttacking)
        {
            Attack();
        }
        //handles gravity
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        if (!isAttacking)
        {
            if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            {
                velocity.Y = jumpVelocity;
                animatedSprite.Play("jumpKingHuman");
            }

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
        //handles sprite flip and hitbox
        if (direction > 0)
        {
            animatedSprite.FlipH = false;
            collisionShape.Position = new Vector2(0, 0);
            collisionShapeAttack.Position = new Vector2(33, -6);
        }
        else if (direction < 0)
        {
            animatedSprite.FlipH = true;
            collisionShape.Position = new Vector2(9, 0);
            collisionShapeAttack.Position = new Vector2(-23, -6);
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

    private void Attack()
    {
        GD.Print("attack");
        isAttacking = true;
        animatedSprite.Play("attackKingHuman");
        collisionShapeAttack.Disabled = false;
        //add timer for anti spaming the attack
    }

    private void OnAnimationFinished()
    {
        if (animatedSprite.Animation == "attackKingHuman")
        { 
            isAttacking = false;
            collisionShapeAttack.Disabled = true;
        }
    }
    public void TakeDamage()
    {
        GD.Print("Ouch");
    }

    private void OnAttackHit(Node body)
    {
        if (body is EnemyKingPigScript enemy)
        {
            enemy.TakeDamage(50);
        }
    }
}
