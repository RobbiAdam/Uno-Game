namespace UnoGame;

public interface ICard
{
    CardColor CardColor
    {
        get;
    }
    CardValue CardValue
    {
        get;
    }
    bool IsWild { get; }

}

public interface IPlayer
{
    int PlayerId
    {
        get;
    }

    string PlayerName
    {
        get;
    }
}
