namespace BoardGameFramework.Core.Players;

public abstract class Player
{
    public string Name { get; private set; }
    public int PlayerNumber { get; private set; }
    public bool IsHuman { get; protected set; }

    protected Player(string name, int playerNumber)
    {
        Name = name;
        PlayerNumber = playerNumber;
    }
}