using Godot;
using System;

public partial class EnemyKingPigScript : CharacterBody2D
{

 

    public enum EnemyState
    { 
        Idle,
        Patrol,
        Chase,
        Attack,
        Cooldown
    }
    public int health = 100;
    public EnemyState state = EnemyState.Idle;
    public float stateTimer = 0f;

    public float patrolSpeed = 100;
    public float chaseSpeed = 200;
    public float visionRange = 300;
    public float attackRange = 50;
    public bool isPlayerInVisionRange = false;
    public bool canAttack = false;
    public float attackCooldown = 1.0f;

    public Vector2 patrolDirection = Vector2.Left;

    AnimatedSprite2D animatedSprite;
    Area2D attackArea;
    CollisionShape2D collisionAttackShape;
    Area2D visionArea;
    CollisionShape2D visionShape;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D-Enemy");

        attackArea = GetNode<Area2D>("AttackArea-Enemy");
        collisionAttackShape = GetNode<CollisionShape2D>("AttackArea-Enemy/CollisionAttackShape2D-Enemy");
        attackArea.BodyEntered += OnAttackAreaEntered;


        visionArea = GetNode<Area2D>("VisionArea-Enemy");
        visionShape = GetNode<CollisionShape2D>("VisionArea-Enemy/VisionShape-Enemy");
        visionArea.BodyEntered += OnVisionEntered;
        visionArea.BodyExited += OnVisionExited;
    }


    public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;
        //handles gravity
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        stateTimer -= (float)delta;

        switch (state)
        {
            case EnemyState.Idle:
                velocity.X = 0;
                animatedSprite.Play("idleKingPig");
                if (stateTimer < 0)
                {
                    ChangeState(EnemyState.Patrol, 2f);
                }
                break;

            case EnemyState.Patrol:
                velocity.X = patrolDirection.X * patrolSpeed;
                animatedSprite.Play("patrolKingPig");

                animatedSprite.FlipH = patrolDirection.X > 0;

                if (patrolDirection == Vector2.Right)
                {
                    animatedSprite.FlipH = true;
                    collisionAttackShape.Position = new Vector2(15, -1);
                }
                else
                { 
                    animatedSprite.FlipH = false;
                    collisionAttackShape.Position = new Vector2(-15, -1);
                }

                if (stateTimer < 0)
                {
                    patrolDirection.X *= -1;
                    ChangeState(EnemyState.Idle, 5f);
                }
                break;

            case EnemyState.Chase:

                if (Global.player != null && isPlayerInVisionRange)
                {
                    float distance = GlobalPosition.DistanceTo(Global.player.GlobalPosition);


                    if (distance < attackRange)
                    {
                        canAttack = true;
                        ChangeState(EnemyState.Attack, 0.5f);
                    }
                    else
                    {
                        float chaseDirection = Math.Sign(Global.player.GlobalPosition.X - GlobalPosition.X);

                        velocity.X = chaseDirection * chaseSpeed;
                        animatedSprite.Play("runKingPig");
                        animatedSprite.FlipH = chaseDirection > 0;

                        collisionAttackShape.Position = new Vector2(chaseDirection > 0 ? 15 : -15, -1);

                    }

                }
                else
                {
                    ChangeState(EnemyState.Patrol, 2f);
                }
                break;


            case EnemyState.Attack:

                if (canAttack)
                {
                    canAttack = false;
                    animatedSprite.Play("attackKingPig");
                    GD.Print("About to attack");
                    collisionAttackShape.SetDeferred("disabled", false);
                }


                if (stateTimer < 0)
                {
                    collisionAttackShape.SetDeferred("disabled", true);
                    ChangeState(EnemyState.Cooldown, attackCooldown);
                }
                break;

            case EnemyState.Cooldown:
                velocity.X = 0;
                animatedSprite.Play("idleKingPig");
                if (stateTimer < 0)
                {
                    ChangeState(isPlayerInVisionRange ? EnemyState.Chase : EnemyState.Idle, 1f);
                }


                break;


        }
        Velocity = velocity;

        MoveAndSlide();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animatedSprite.Play("hitKingPig");
        GD.Print(health);
    }

    private void ChangeState(EnemyState newState, float duration = 0)
    {
        state = newState;
        stateTimer = duration;
    }

    private void OnAttackAreaEntered(Node2D body)
    {
        if (body is PlayerScript player && !collisionAttackShape.Disabled)
        {
            player.TakeDamage();
            GD.Print("OnAttackAreaEntered");
        }
    }

    private void OnVisionEntered(Node2D body)
    {
        if (body is PlayerScript player)
        {
            isPlayerInVisionRange = true;
            GD.Print("Chase/Spotted");
            ChangeState(EnemyState.Chase);
        }
    }

    private void OnVisionExited(Node2D body) 
    {
        if (body is PlayerScript player)
        {
            isPlayerInVisionRange = false;
            GD.Print("Out of sight");
            ChangeState(EnemyState.Idle, 3f);
            collisionAttackShape.SetDeferred("disabled", true);
        }
    }
}
