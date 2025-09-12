using Godot;
using System;

public partial class EnemyAnimationHandler : AnimatedSprite2D
{
	[Export]
    private string AnimationPrefix { get; set; } = "";

    //public AnimatedSprite2D AnimatedSprite { get; set; }
 
    public string CurrentAnimation
    {
        get { return Animation; }
    }



	public void IdleAnimation()
	{
        Play("idle" + AnimationPrefix + "Pig");
    }

    public void PatrolAnimation()
    {
        Play("patrol" + AnimationPrefix + "Pig");
    }

    public void RunAnimation()
    {
        Play("run" + AnimationPrefix + "Pig");
    }

    public void AttackAnimation()
    {
        Play("attack" + AnimationPrefix + "Pig");
    }

    public void DieAnimation()
	{
		Play("dead" + AnimationPrefix + "Pig");

	}

    public void HitAnimation()
    {
        Play("hit" + AnimationPrefix + "Pig");
    }
}
