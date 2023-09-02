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

    public ICard DrawCard()
    {
        Random random = new Random();
        
        CardValue randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

        if (randomValue == CardValue.Wild || randomValue == CardValue.WildDrawFour)
        {
            return new Card { CardValue = randomValue, IsWild = true };
        }
        else
        {
            CardColor randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
            return new Card { CardColor = randomColor, CardValue = randomValue };
        }
    }

    public List<IPlayer> Players
    {
        get { return PlayerList; }
    }
}
