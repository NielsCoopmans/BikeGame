

public static class GameStateManager 
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Nightmare
    }

    public static int currentLevel = 1;
    public static Difficulty difficulty = Difficulty.Easy;
}
