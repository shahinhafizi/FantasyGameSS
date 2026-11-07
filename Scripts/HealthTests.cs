using Godot;

public partial class HealthTests : Node
{
    public override void _Ready()
    {
        GD.Print("--- KÖR UNIT TESTS FÖR HEALTHCOMPONENT ---");
        TestInitialHealth();
        TestTakeDamage();
        TestDeathState();
        GD.Print("--- TESTER KLARA ---");
    }

    private void TestInitialHealth()
    {
        // Arrange
        HealthComponent testHealth = new HealthComponent { MaxHealth = 10 };
        // Act
        testHealth._Ready(); 
        // Assert
        if (testHealth.CurrentHealth == 10) GD.Print("[GODKÄNT] Test 1: Livet startar korrekt på MaxHealth.");
        else GD.PrintErr("[UNDERKÄNT] Test 1");
    }

    private void TestTakeDamage()
    {
        // Arrange
        HealthComponent testHealth = new HealthComponent { MaxHealth = 10 };
        testHealth._Ready();
        // Act
        testHealth.TakeDamage(3);
        // Assert
        if (testHealth.CurrentHealth == 7) GD.Print("[GODKÄNT] Test 2: TakeDamage drar av rätt mängd.");
        else GD.PrintErr("[UNDERKÄNT] Test 2");
    }

    private void TestDeathState()
    {
        // Arrange
        HealthComponent testHealth = new HealthComponent { MaxHealth = 10 };
        testHealth._Ready();
        // Act
        testHealth.TakeDamage(10);
        // Assert
        if (testHealth.IsDead && testHealth.CurrentHealth == 0) GD.Print("[GODKÄNT] Test 3: Karaktären dör.");
        else GD.PrintErr("[UNDERKÄNT] Test 3");
    }
}