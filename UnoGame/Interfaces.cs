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
