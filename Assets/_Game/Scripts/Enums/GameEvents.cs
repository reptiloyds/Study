namespace _Game.Scripts.Enums
{
    public enum GameEvents
    {
        //События рекламы
        RewardRequest,
        RewardShowed,
        RewardClicked,
        RewardCanceled,
        RewardErrorLoaded,
        RewardErrorDisplay,
        
        InterRequest,
        InterShowed,
        InterErrorLoaded,
        InterErrorDisplay,
        
        BannerShowed,
        BannerHidden,
        
        //События покупок
        FirstInAppPurchased,
        InAppPurchased,
        
        //События игрового процесса
        LevelStart,
        LevelFinish,
        SaveLoaded,
        CameraMoved,
        TutorialTimer,
        BuyPoint,
        ItemSpawned,
        BoatStopped,
        TaskCompleted,
    }
}