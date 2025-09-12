using Godot;
using System;

public partial class EnemyKingPigScript : CharacterBody2D, IEnemy
{

    public int health = 100;
    public float patrolSpeed = 100;
    public float chaseSpeed = 200;
    public float visionRange = 300;
    public float attackRange = 50;
    public bool isPlayerInVisionRange = false;
    public bool canAttack = false;
    public float attackCooldown = 1.0f;

    public Vector2 patrolDirection = Vector2.Left;

    private EnemyStateMachine fsm;
    //private AnimatedSprite2D animatedSprite;
    private Area2D attackArea;
    private CollisionShape2D collisionAttackShape;
    private Area2D visionArea;
    private CollisionShape2D visionShape;
    private CollisionShape2D collisionShapeEnemy;

    EnemyAnimationHandler animationHandler;

    

    public override void _Ready()
    {
        //animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D-Enemy");

        attackArea = GetNode<Area2D>("AttackArea-Enemy");
        collisionAttackShape = GetNode<CollisionShape2D>("AttackArea-Enemy/CollisionAttackShape2D-Enemy");
        collisionShapeEnemy = GetNode<CollisionShape2D>("CollisionShape2D-Enemy");
        attackArea.BodyEntered += OnAttackAreaEntered;

        visionArea = GetNode<Area2D>("VisionArea-Enemy");
        visionShape = GetNode<CollisionShape2D>("VisionArea-Enemy/VisionShape-Enemy");
        visionArea.BodyEntered += OnVisionEntered;
        visionArea.BodyExited += OnVisionExited;

        fsm = new EnemyStateMachine();
        animationHandler = GetNode<EnemyAnimationHandler>("AnimationHandler");
        //animationHandler.AnimationFinished += OnAnimationFinished;
        animationHandler.AnimationFinished += OnAnimationFinished;
    }


    public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;
        //handles gravity
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        fsm.Update((float)delta);

        switch (fsm.CurrentState)
        {
            case EnemyStateMachine.EnemyState.Idle:
                velocity.X = 0;
                animationHandler.IdleAnimation();
                if (fsm.IsStateFinished())
                {
                   fsm.ChangeState(EnemyStateMachine.EnemyState.Patrol, 2f);
                }
                break;

            case EnemyStateMachine.EnemyState.Patrol:
                velocity.X = patrolDirection.X * patrolSpeed;

                animationHandler.PatrolAnimation();

                animationHandler.FlipH = patrolDirection.X > 0;
                if (patrolDirection == Vector2.Right)
                {
                    animationHandler.FlipH = true;
                    collisionAttackShape.Position = new Vector2(15, -1);
                }
                else
                {
                    animationHandler.FlipH = false;
                    collisionAttackShape.Position = new Vector2(-15, -1);
                }

                if (fsm.IsStateFinished())
                {
                    patrolDirection.X *= -1;
                    fsm.ChangeState(EnemyStateMachine.EnemyState.Idle, 5f);
                }
                break;

            case EnemyStateMachine.EnemyState.Chase:

                if (Global.player != null && isPlayerInVisionRange)
                {
                    float distance = GlobalPosition.DistanceTo(Global.player.GlobalPosition);


                    if (distance < attackRange)
                    {
                        canAttack = true;
                        fsm.ChangeState(EnemyStateMachine.EnemyState.Attack, 0.5f);
                    }
                    else
                    {
                        float chaseDirection = Math.Sign(Global.player.GlobalPosition.X - GlobalPosition.X);

                        velocity.X = chaseDirection * chaseSpeed;

                        animationHandler.RunAnimation();
                        animationHandler.FlipH = chaseDirection > 0;

                        collisionAttackShape.Position = new Vector2(chaseDirection > 0 ? 15 : -15, -1);

                    }

                }
                else
                {
                    fsm.ChangeState(EnemyStateMachine.EnemyState.Patrol, 2f);
                }
                break;


            case EnemyStateMachine.EnemyState.Attack:

                if (canAttack)
                {
                    canAttack = false;

                    animationHandler.AttackAnimation();
                    GD.Print("About to attack");
                    collisionAttackShape.SetDeferred("disabled", false);
                }


                if (fsm.IsStateFinished())
                {
                    collisionAttackShape.SetDeferred("disabled", true);
                    fsm.ChangeState(EnemyStateMachine.EnemyState.Cooldown, attackCooldown);
                }
                break;

            case EnemyStateMachine.EnemyState.Cooldown:
                velocity.X = 0;
                //animatedSprite.Play("idleKingPig");

                animationHandler.IdleAnimation();
                if (fsm.IsStateFinished())
                {
                    fsm.ChangeState(isPlayerInVisionRange ? EnemyStateMachine.EnemyState.Chase : EnemyStateMachine.EnemyState.Idle, 1f);
                }


                break;


        }

        if (health <= 0)
        {
            Die();    
        }
        Velocity = velocity;

        MoveAndSlide();
    }


    public void Die()
    {
        animationHandler.DieAnimation();

    }

    public void OnAnimationFinished()
    {
        GD.Print("Animation Finished");
        if (animationHandler.Animation == "deadKingPig")
        {
            GD.Print("Animation Finished 2");

            QueueFree();
        }
    }

    public void TakeDamage(int damage)
    {        
        health -= damage;
        animationHandler.HitAnimation();
        GD.Print(health);        
    }

    private void OnAttackAreaEntered(Node2D body)
    {
        if (body is IPlayer player && !collisionAttackShape.Disabled)
        {
            Attack(player);
            GD.Print("OnAttackAreaEntered");
        }
    }

    private void OnVisionEntered(Node2D body)
    {
        if (body is PlayerScript player)
        {
            isPlayerInVisionRange = true;
            GD.Print("Chase/Spotted");
            fsm.ChangeState(EnemyStateMachine.EnemyState.Chase);
        }
    }

    public void Attack(IPlayer player)
    {
        player.TakeDamage();
    }

    private void OnVisionExited(Node2D body) 
    {
        if (body is PlayerScript player)
        {
            isPlayerInVisionRange = false;
            GD.Print("Out of sight");
            fsm.ChangeState(EnemyStateMachine.EnemyState.Idle, 3f);
            collisionAttackShape.SetDeferred("disabled", true);
        }
    }
}
