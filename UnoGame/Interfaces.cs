namespace UnoGame;

public interface ICard
{
    CardColor CardColor
    {
        get;
        set;
    }
    CardValue CardValue
    {
        get;
        set;
    }
    bool IsWild { get; }

}

public interface IPlayer
{
    int PlayerId
    {
        get;
        set;
    }

    string PlayerName
    {
        get;
        set;
    }
}
