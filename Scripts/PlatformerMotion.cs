using Godot;

public static class PlatformerMotion
{
    public static void ApplyGravity(CharacterBody2D body, float gravity, double delta)
    {
        if (!body.IsOnFloor())
        {
            body.Velocity += Vector2.Down * gravity * (float)delta;
        }
        else if (body.Velocity.Y > 0.0f)
        {
            body.Velocity = new Vector2(body.Velocity.X, 0.0f);
        }
    }
}
