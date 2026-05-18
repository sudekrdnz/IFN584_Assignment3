namespace BoardGameFramework.Core.Players;

public abstract class Player
{
    public string Name { get; set; }
    public int PlayerNumber { get; set; }
    public bool IsHuman { get; protected set; }

    protected Player(string name, int playerNumber)
    {
        Name = name;
        PlayerNumber = playerNumber;
    }
}
