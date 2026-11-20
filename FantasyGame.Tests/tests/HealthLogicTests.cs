using System;
using Xunit;

namespace FantasyGame.Tests;

public class HealthLogicTests
{
    [Fact]
    public void Constructor_SetsInitialHealthToMaxHealth()
    {
        // Arrange & Act
        var health = new HealthLogic(10);

        // Assert
        Assert.Equal(10, health.MaxHealth);
        Assert.Equal(10, health.CurrentHealth);
        Assert.False(health.IsDead);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_InvalidMaxHealth_ThrowsArgumentOutOfRangeException(int invalidMaxHealth)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new HealthLogic(invalidMaxHealth));
    }

    [Theory]
    [InlineData(10, 3, 7, false)]
    [InlineData(10, 10, 0, true)]
    [InlineData(10, 25, 0, true)] // Clamping: får aldrig gå under 0
    public void TakeDamage_UpdatesHealthAndDeathStateCorrectly(
        int maxHealth,
        int damage,
        int expectedHealth,
        bool expectedIsDead)
    {
        // Arrange
        var health = new HealthLogic(maxHealth);

        // Act
        health.TakeDamage(damage);

        // Assert
        Assert.Equal(expectedHealth, health.CurrentHealth);
        Assert.Equal(expectedIsDead, health.IsDead);
    }

    [Fact]
    public void TakeDamage_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var health = new HealthLogic(10);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => health.TakeDamage(-1));
    }

    [Fact]
    public void TakeDamage_ZeroAmount_DoesNotRaiseHealthChangedEvent()
    {
        // Arrange
        var health = new HealthLogic(10);
        bool eventRaised = false;
        health.HealthChanged += (_, _) => eventRaised = true;

        // Act
        health.TakeDamage(0);

        // Assert
        Assert.False(eventRaised);
        Assert.Equal(10, health.CurrentHealth);
    }

    [Fact]
    public void TakeDamage_FatalDamage_RaisesDiedEventExactlyOnce()
    {
        // Arrange
        var health = new HealthLogic(10);
        int diedCount = 0;
        health.Died += () => diedCount++;

        // Act
        health.TakeDamage(10);
        health.TakeDamage(5); // Extra skada efter döden ska ignoreras

        // Assert
        Assert.True(health.IsDead);
        Assert.Equal(1, diedCount);
    }
}