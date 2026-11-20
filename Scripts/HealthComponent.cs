#nullable enable
using Godot;

public partial class HealthComponent : Node
{
    [Signal]
    public delegate void HealthChangedEventHandler(int currentHealth, int maximumHealth);

    [Signal]
    public delegate void DiedEventHandler();

    [Export]
    public int MaxHealth { get; set; } = 10;

    private HealthLogic? _logic;

    private HealthLogic Logic => _logic ??= CreateLogic();

    public int CurrentHealth => Logic.CurrentHealth;
    public bool IsDead => Logic.IsDead;

    public override void _Ready()
    {
        _ = Logic;
    }

    public void TakeDamage(int amount)
    {
        Logic.TakeDamage(amount);
    }

    private HealthLogic CreateLogic()
    {
        HealthLogic logic = new(MaxHealth);
        logic.HealthChanged += OnHealthChanged;
        logic.Died += OnDied;
        return logic;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        EmitSignal(SignalName.HealthChanged, currentHealth, maxHealth);
    }

    private void OnDied()
    {
        EmitSignal(SignalName.Died);
    }

    public override void _ExitTree()
    {
        if (_logic is not null)
        {
            _logic.HealthChanged -= OnHealthChanged;
            _logic.Died -= OnDied;
        }
    }
}