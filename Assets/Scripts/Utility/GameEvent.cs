using System;
using System.Collections.Generic;
using System.Linq;

public static class GameEvent{
    public static bool canMove = true;
    public static bool isInLobby = true;
    public static bool ifFirstTime = true;

    private static bool isPaused = false;
    public static bool IsPaused
    { 
        get => isPaused;

        set 
        {
            if(!canMove && value) return;

            isPaused = value;
            canMove=!value;

            Messenger.Broadcast(value ? GAME_PAUSED : GAME_RESUMED);
        }
    }

    public const string PLAYER_HIT = "PLAYER_HIT";
    public const string ENEMY_HIT = "ENEMY_HIT";
    public const string ITEM_PICKED = "ITEM_PICKED";
    public const string GEAR_PICKED = "GEAR_PICKED";
    public const string BULLET_SHOOT = "BULLET_SHOOT";

    public const string GAME_PAUSED = "GAME_PAUSED";
    public const string GAME_RESUMED = "GAME_RESUMED";

}