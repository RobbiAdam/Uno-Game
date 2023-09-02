using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();

    static void Main(string[] args)
    {
        _numberOfPlayer = InsertNumberOfPlayers();
        ShowPlayerList();

        Card drawnCard = gameController.DrawCard();
        Console.WriteLine($"Drew a card: {drawnCard.CardColor} - {drawnCard.CardValue}");
    }

    static int InsertNumberOfPlayers()
    {
        int _numberOfPlayers = 0;
        bool validInput = false;

        while (!validInput)
        {
            Console.WriteLine("Enter the numbers of players: ");
            if (int.TryParse(Console.ReadLine(), out _numberOfPlayers) && _numberOfPlayers > 1 && _numberOfPlayers <= 4)
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 2 and 4.");
            }
        }
        return _numberOfPlayers;
    }

    static void ShowPlayerList()
    {
        for (int playerId = 1; playerId <= _numberOfPlayer; playerId++)
        {
            Console.WriteLine($"Enter the name for Player {playerId}");
            string playerName = Console.ReadLine();

            gameController.AddPlayer(new Player(playerId, playerName));

            foreach (IPlayer player in gameController.Players)
            {
                Console.WriteLine($"Player ID: {player.PlayerId}, Player Name: {player.PlayerName}");
            }
        }
    }

}
