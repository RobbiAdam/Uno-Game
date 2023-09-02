namespace UnoGame;

public class GameController
{
    private List<IPlayer> PlayerList;
    public GameController()
    {
        PlayerList = new List<IPlayer>();
    }

    public void AddPlayer(IPlayer player)
    {
        PlayerList.Add(player);
    }

    public Card DrawCard()
    {
        Random random = new Random();

        CardColor randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
        CardValue randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

        return new Card { CardColor = randomColor, CardValue = randomValue };
    }
    public List<IPlayer> Players
    {
        get { return PlayerList; }
    }
}
