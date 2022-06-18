namespace _Game.Scripts
{
    public static class GameLayers
    {
        public const int
            OBJECT_LAYER = 6,
            PLAYER_LAYER = 7,
            DRAGZONE_LAYER = 10,
            COLLISION_LISTENER_LAYER = 11,
            ANIMAL_LAYER = 13,
            BUY_LAYER = 14;

        public const int
            OBJECT_MASK = 1 << OBJECT_LAYER,
            DRAGZONE_MASK = 1 << DRAGZONE_LAYER,
            COLLISION_LISTENER_MASK = 1 << COLLISION_LISTENER_LAYER,
            ANIMAL_MASK = 1 << ANIMAL_LAYER,
            BUY_MASK = 1 << BUY_LAYER;
    }
}