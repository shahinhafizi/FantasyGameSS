using Godot;

public static class CollisionDisabler
{
    public static void DisableBody(CollisionObject2D body)
    {
        body.SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0);
        body.SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0);
        DisableRecursively(body);
    }

    private static void DisableRecursively(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is CollisionShape2D collisionShape)
            {
                collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            }
            else if (child is CollisionPolygon2D collisionPolygon)
            {
                collisionPolygon.SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
            }
            else if (child is Area2D area)
            {
                area.SetDeferred(Area2D.PropertyName.Monitoring, false);
                area.SetDeferred(Area2D.PropertyName.Monitorable, false);
                area.SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0);
                area.SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0);
            }

            DisableRecursively(child);
        }
    }
}
