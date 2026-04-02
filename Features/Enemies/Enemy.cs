using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export(PropertyHint.Range, "10,1000,1,suffix:px/s")]
    public float MoveSpeed { get; set; } = 120.0f;

    [Export(PropertyHint.Range, "0,32,1,suffix:px")]
    public float StopDistance { get; set; } = 6.0f;

    [Export(PropertyHint.Range, "1,50,1")]
    public int ContactDamage { get; set; } = 1;

    [Export(PropertyHint.Range, "0.05,2.0,0.01,suffix:s")]
    public float ContactDamageInterval { get; set; } = 0.5f;

    [Export(PropertyHint.Range, "1,999,1")]
    public int MaxHealth { get; set; } = 3;

    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private Node2D _player = null!;
    private float _contactDamageCooldown;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        _player = FindPlayer();
    }

    public override void _PhysicsProcess(double delta)
    {
        _contactDamageCooldown = Mathf.Max(0.0f, _contactDamageCooldown - (float)delta);

        if (!GodotObject.IsInstanceValid(_player))
        {
            _player = FindPlayer();
        }

        Vector2 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y += _gravity * (float)delta;
        }

        float directionX = 0.0f;
        if (GodotObject.IsInstanceValid(_player))
        {
            float dx = _player.GlobalPosition.X - GlobalPosition.X;
            if (Mathf.Abs(dx) > StopDistance)
            {
                directionX = Mathf.Sign(dx);
            }
        }

        velocity.X = directionX * MoveSpeed;

        Velocity = velocity;
        MoveAndSlide();

        TryDamagePlayerByContact();
    }

    private void TryDamagePlayerByContact()
    {
        if (_contactDamageCooldown > 0.0f)
        {
            return;
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            Node collider = collision.GetCollider() as Node;
            if (collider == null || !collider.IsInGroup("player"))
            {
                continue;
            }

            if (collider is Player player)
            {
                player.TakeDamage(ContactDamage);
            }
            else
            {
                collider.Call("TakeDamage", ContactDamage);
            }

            _contactDamageCooldown = ContactDamageInterval;
            return;
        }
    }

    private Node2D FindPlayer()
    {
        Node fromGroup = GetTree().GetFirstNodeInGroup("player");
        if (fromGroup is Node2D fromGroupNode)
        {
            return fromGroupNode;
        }

        Node currentScene = GetTree().CurrentScene;
        if (GodotObject.IsInstanceValid(currentScene))
        {
            Node fromName = currentScene.FindChild("Player", true, false);
            if (fromName is Node2D fromNameNode)
            {
                return fromNameNode;
            }
        }

        return null!;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        GD.Print($"Enemy HP: {CurrentHealth}/{MaxHealth}");

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        QueueFree();
    }
}
