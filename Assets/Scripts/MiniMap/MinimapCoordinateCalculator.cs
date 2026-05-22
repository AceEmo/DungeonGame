using UnityEngine;

public class MinimapCoordinateCalculator
{
    private readonly float worldRoomSize;
    private readonly Vector2Int invalidGridPosition;

    public MinimapCoordinateCalculator(float worldRoomSize, Vector2Int invalidGridPosition)
    {
        this.worldRoomSize = worldRoomSize;
        this.invalidGridPosition = invalidGridPosition;
    }

    public Vector2Int GetPlayerGridPosition(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return invalidGridPosition;
        }

        int playerGridX = Mathf.RoundToInt(playerTransform.position.x / worldRoomSize);
        int playerGridY = Mathf.RoundToInt(playerTransform.position.y / worldRoomSize);
        
        return new Vector2Int(playerGridX, playerGridY);
    }
}