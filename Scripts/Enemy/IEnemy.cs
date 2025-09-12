using Godot;
using System;

public interface IEnemy
{
    void Attack(IPlayer player);

    void TakeDamage(int damage);

    void Die();

    Vector2 GetPosition();
}
