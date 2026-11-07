#nullable enable
using System;
using Godot;

public partial class BossController : CharacterBody2D
{
    [Export]
    public NodePath HealthComponentPath { get; set; } = new("HealthComponent");

    [Export]
    public NodePath AnimatedSpritePath { get; set; } = new("AnimatedSprite2D");

    [Export]
    public NodePath BossHitboxPath { get; set; } = new("BossHitbox");

    [Export]
    public float MoveSpeed { get; set; } = 90.0f;

    [Export]
    public float Gravity { get; set; } = 980.0f;

    [Export]
    public float AttackRange { get; set; } = 100.0f;

    [Export]
    public float AttackCooldown { get; set; } = 1.4f;

    [Export]
    public float AttackWindup { get; set; } = 0.45f;

    [Export]
    public float AttackHitboxDuration { get; set; } = 0.2f;

    [Export]
    public int AttackDamage { get; set; } = 1;

    private HealthComponent _health = null!;
    private AnimatedSprite2D _animatedSprite = null!;
    private Area2D _bossHitbox = null!;
    private KnightController? _player;
    private double _attackCooldownRemaining;
    private double _attackWindupRemaining;
    private double _attackHitboxRemaining;
    private bool _isAttacking;
    private bool _isHitboxActive;
    private bool _attackHasHitPlayer;
    private bool _isDead;
    private bool _deathSequenceFinished;

    public override void _Ready()
    {
        AddToGroup("Boss");
        _health = GetNode<HealthComponent>(HealthComponentPath);
        _animatedSprite = GetNode<AnimatedSprite2D>(AnimatedSpritePath);
        _bossHitbox = GetNode<Area2D>(BossHitboxPath);
        _bossHitbox.CollisionLayer = 0;
        _bossHitbox.CollisionMask = 1;

        _health.Died += OnDied;
        _animatedSprite.AnimationFinished += OnAnimationFinished;
        _bossHitbox.BodyEntered += OnBossHitboxBodyEntered;
        SetHitboxActive(false);
        if (!HasAnimation("idle") || !HasAnimation("attack") || !HasAnimation("death"))
        {
            GD.PushWarning(
                $"'{Name}' should have non-looping 'attack' and 'death' animations, "
                + "plus an 'idle' animation, on its AnimatedSprite2D.");
        }

        PlayAnimation("idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
        {
            return;
        }

        if (_player is null || !GodotObject.IsInstanceValid(_player))
        {
            _player = GetTree().GetFirstNodeInGroup("Player") as KnightController;
        }

        if (_player is null)
        {
            PlatformerMotion.ApplyGravity(this, Gravity, delta);
            MoveAndSlide();
            return;
        }

        Vector2 toPlayer = _player.GlobalPosition - GlobalPosition;
        _animatedSprite.FlipH = toPlayer.X < 0.0f;
        PlatformerMotion.ApplyGravity(this, Gravity, delta);

        if (Mathf.Abs(toPlayer.X) > AttackRange)
        {
            CancelAttack();
            Velocity = new Vector2(Mathf.Sign(toPlayer.X) * MoveSpeed, Velocity.Y);
            PlayAnimation(HasAnimation("walk") ? "walk" : "idle");
            MoveAndSlide();
            return;
        }

        Velocity = new Vector2(0.0f, Velocity.Y);
        MoveAndSlide();

        if (_isAttacking)
        {
            UpdateAttack(delta);
            return;
        }

        _attackCooldownRemaining = Math.Max(0.0, _attackCooldownRemaining - delta);
        if (_attackCooldownRemaining <= 0.0)
        {
            StartAttack();
        }
        else
        {
            PlayAnimation("idle");
        }
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _isHitboxActive = false;
        _attackWindupRemaining = Math.Max(0.0, AttackWindup);
        _attackHitboxRemaining = Math.Max(0.0, AttackHitboxDuration);
        _attackCooldownRemaining = Math.Max(0.0, AttackCooldown);
        _attackHasHitPlayer = false;
        if (_player is not null)
        {
            float direction = Mathf.Sign(_player.GlobalPosition.X - GlobalPosition.X);
            _bossHitbox.Position = new Vector2(direction * AttackRange * 0.5f, -10.0f);
        }

        SetHitboxActive(false);
        PlayAnimation("attack");
    }

    private void UpdateAttack(double delta)
    {
        if (!_isHitboxActive)
        {
            _attackWindupRemaining -= delta;
            if (_attackWindupRemaining <= 0.0)
            {
                _isHitboxActive = true;
                SetHitboxActive(true);
            }

            return;
        }

        foreach (Node2D body in _bossHitbox.GetOverlappingBodies())
        {
            TryDamagePlayer(body);
        }

        _attackHitboxRemaining -= delta;
        if (_attackHitboxRemaining <= 0.0)
        {
            SetHitboxActive(false);
            _isHitboxActive = false;
            _isAttacking = false;
            PlayAnimation("idle");
        }
    }

    private void CancelAttack()
    {
        SetHitboxActive(false);
        _isHitboxActive = false;
        _isAttacking = false;
    }

    private void OnBossHitboxBodyEntered(Node2D body)
    {
        TryDamagePlayer(body);
    }

    private void TryDamagePlayer(Node2D body)
    {
        if (_isDead || !_isAttacking || !_isHitboxActive || _attackHasHitPlayer)
        {
            return;
        }

        if (body.IsInGroup("Player") && body is KnightController player)
        {
            _attackHasHitPlayer = true;
            player.TakeDamage(AttackDamage);
        }
    }

    private void OnDied()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        Velocity = Vector2.Zero;
        CancelAttack();
        SetPhysicsProcess(false);
        CollisionDisabler.DisableBody(this);
        if (!HasAnimation("death"))
        {
            CompleteDeathSequence();
            return;
        }

        _animatedSprite.SpriteFrames!.SetAnimationLoopMode(
            new StringName("death"),
            SpriteFrames.LoopMode.None);
        PlayAnimation("death");
    }

    private void OnAnimationFinished()
    {
        if (_isDead && _animatedSprite.Animation == new StringName("death"))
        {
            CompleteDeathSequence();
        }
    }

    private void CompleteDeathSequence()
    {
        if (_deathSequenceFinished)
        {
            return;
        }

        _deathSequenceFinished = true;
        GameOverScreen? gameOverScreen = GameOverScreen.Find(GetTree());
        if (gameOverScreen is not null)
        {
            gameOverScreen.ShowVictory();
        }
        else
        {
            GD.PushError("No GameOverScreen was found after the boss was defeated.");
        }

        QueueFree();
    }

    private bool HasAnimation(string animationName)
    {
        return _animatedSprite.SpriteFrames?.HasAnimation(new StringName(animationName)) ?? false;
    }

    private void PlayAnimation(string animationName)
    {
        if (!HasAnimation(animationName))
        {
            return;
        }

        if (_animatedSprite.Animation != new StringName(animationName))
        {
            _animatedSprite.Play(animationName);
        }
    }

    private void SetHitboxActive(bool active)
    {
        _bossHitbox.SetDeferred(Area2D.PropertyName.Monitoring, active);
    }
}
