namespace UnoGame.test;


public class UnitTest1
{
    [Fact]
    public void CheckPlayerName()
    {
        // Arrange
        IPlayer player = new Player(1, "Alice");
        // Act
        int playerId = player.PlayerId;
        string playerName = player.PlayerName;

        // Assert
        Assert.Equal(1, playerId);
        Assert.Equal("Alice", playerName);
    }
    [Fact]
    public void CheckCardType()
    {
        // Arrange
        ICard newCard = new Card();
        newCard.CardValue = CardValue.Wild;
        newCard.CardColor = CardColor.Blank;

        ICard newCard2 = new Card();
        newCard2.CardValue = CardValue.Zero;

        GameController gameController = new GameController();

        // Act
        bool isWildCard = gameController.IsWildCard(newCard);
        bool isWildCard2= gameController.IsWildCard(newCard2);

        // Assert
        Assert.True(isWildCard);
        Assert.False(isWildCard2);

    }



}