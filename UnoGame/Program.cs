using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();
    static bool _gameStatus = false;

    static void Main()
    {
        _numberOfPlayer = InsertNumberOfPlayers();
        ShowPlayerList();
        gameController.DealStartingHands();
        gameController.SetDiscardPile();
        DisplayTopCardOnDiscardPile();
        DisplayPlayerHands();

        // Game loop
        while (!gameController.IsGameOver() && !_gameStatus)
        {
            TakeAction();

            if (gameController.IsGameOver() || _gameStatus)
            {

                Console.WriteLine("Game over!");

            }
            gameController.NextPlayerTurn();
        }
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
            string playerName;
            bool isNameTaken;

            do
            {
                Console.WriteLine($"Enter the name for Player {playerId}");
                playerName = Console.ReadLine();

                isNameTaken = gameController.IsPlayerNameTaken(playerName);

                if (isNameTaken)
                {
                    Console.WriteLine("Player name is already taken. Please choose a different name.");
                }

            } while (isNameTaken);

            gameController.AddPlayer(new Player(playerId, playerName));
        }

        foreach (IPlayer player in gameController.Players)
        {
            Console.WriteLine($"Player ID: {player.PlayerId}, Player Name: {player.PlayerName}");
        }
    }

    static void DisplayPlayerHands()
    {
        Console.WriteLine("Players and their hands:");
        foreach (IPlayer player in gameController.Players)
        {
            PlayerData playerData = gameController.GetPlayerData(player);
            if (playerData != null)
            {
                Console.WriteLine($"{player.PlayerName} (ID: {player.PlayerId}):");
                foreach (ICard card in playerData.HandCard)
                {
                    if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
                    {
                        Console.WriteLine($"  {card.CardValue}");
                    }
                    else
                    {
                        Console.WriteLine($"  {card.CardColor} {card.CardValue}");
                    }
                }
            }
        }
    }

    static void DisplayTopCardOnDiscardPile()
    {
        if (gameController.DiscardedPile.Count > 0)
        {
            ICard topCard = gameController.DiscardedPile.Last();
            Console.WriteLine($"Top Card on Discard Pile: {topCard.CardColor} {topCard.CardValue}");
        }
        else
        {
            Console.WriteLine("Discard pile is empty.");
        }
    }
    static void DisplayCurrentPlayerHand(string playerName, List<ICard> HandCard)
    {
        Console.WriteLine($"{playerName}'s Hand:");

        for (int i = 0; i < HandCard.Count; i++)
        {
            ICard card = HandCard[i];
            string cardDescription = GetCardDescription(card);
            Console.WriteLine($"{i + 1}. {cardDescription}");
        }
    }
    static string GetCardDescription(ICard card)
    {
        if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
        {
            return card.CardValue.ToString();
        }
        else
        {
            return $"{card.CardColor} {card.CardValue}";
        }
    }
    static void TakeAction()
    {
        IPlayer currentPlayer = gameController.GetPlayerTurn();

        if (currentPlayer != null)
        {
            PlayerData currentPlayerData = gameController.GetPlayerData(currentPlayer);

            if (currentPlayerData != null)
            {
                Console.WriteLine("");
                Console.WriteLine($"{currentPlayer.PlayerName}'s turn.");
                DisplayCurrentPlayerHand(currentPlayer.PlayerName, currentPlayerData.HandCard);
                Console.WriteLine("");

                bool _continueTakingAction = true;

                while (_continueTakingAction)
                {
                    DisplayTopCardOnDiscardPile();
                    Console.WriteLine("");
                    Console.WriteLine($"{currentPlayer.PlayerName} Choose an action:");
                    Console.WriteLine("1. Draw a card");
                    Console.WriteLine("2. Discard a card");
                    Console.WriteLine("3. End turn");
                    Console.WriteLine("4. End the game"); //force end
                    Console.WriteLine("");
                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                gameController.DrawCardToPlayerHand(currentPlayer);
                                Console.WriteLine("");
                                Console.WriteLine($"{currentPlayer.PlayerName} drew a card.");
                                DisplayCurrentPlayerHand(currentPlayer.PlayerName, currentPlayerData.HandCard);
                                break;
                            case 2:
                                if (ChooseCardToDiscard(currentPlayer))
                                {
                                    _continueTakingAction = false;
                                }
                                break;

                            case 3:
                                _continueTakingAction = false;
                                break;
                            case 4:
                                _gameStatus = true; // End Game
                                _continueTakingAction = false;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please choose a valid action.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a valid action number.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No current player data.");
            }
        }
        else
        {
            Console.WriteLine("No current player.");
        }
    }
    static bool ChooseCardToDiscard(IPlayer player)
    {
        Console.WriteLine("");
        Console.WriteLine($"{player.PlayerName}, choose a card to discard:");
        DisplayTopCardOnDiscardPile();

        PlayerData playerData = gameController.GetPlayerData(player);
        if (playerData != null)
        {
            List<ICard> handCards = playerData.HandCard;

            for (int i = 0; i < handCards.Count; i++)
            {
                ICard card = handCards[i];
                string cardDescription = GetCardDescription(card);
                Console.WriteLine($"{i + 1}. {cardDescription}");
            }

            Console.Write("Enter the number of the card to discard: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= handCards.Count)
            {
                ICard selectedCard = handCards[choice - 1];

                if (gameController.IsCardValidToDiscard(selectedCard))
                {
                    gameController.DiscardCard(player, selectedCard);
                    Console.WriteLine($"{player.PlayerName} discarded: {GetCardDescription(selectedCard)}");

                    // Check if the discarded card is an action card
                    if (gameController.IsActionCard(selectedCard))
                    {
                        Console.WriteLine($"{player.PlayerName} discarded an action card!");
                        DisplayActionCardMessage(selectedCard, player.PlayerName);

                    }
                    else
                    if (gameController.IsWildCard(selectedCard))
                    {
                        DisplayWildCardAction(selectedCard, player.PlayerName);
                    }


                    return true;
                }
                else
                {
                    Console.WriteLine("You can't discard that card.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid card number.");
            }
        }
        else
        {
            Console.WriteLine("No current player data.");
        }

        return false;
    }


    static void DisplayActionCardMessage(ICard card, string playerName)
    {
        switch (card.CardValue)
        {
            case CardValue.Skip:
                Console.WriteLine($"Next Player Turn is skipped.");
                break;
            case CardValue.Reverse:
                Console.WriteLine($"Player {playerName} reverses the turn order.");
                break;
            case CardValue.DrawTwo:
                Console.WriteLine($"Next Player draws two cards and their turn is skipped.");
                break;
            default:

                Console.WriteLine($"Action card played by {playerName}: {card.CardValue}");
                break;
        }
    }

    static void DisplayWildCardAction(ICard card, string playerName)
    {
        if (card.CardValue == CardValue.Wild)
        {
            Console.WriteLine($"{playerName} played a Wild card.");
            CardColor chosenColor = ChooseWildCardColor();
            Console.WriteLine($"{playerName} chose the color: {chosenColor}");
            // Here you can handle the Wild card with the chosen color.
        }
        else if (card.CardValue == CardValue.WildDrawFour)
        {
            Console.WriteLine($"{playerName} played a Wild Draw Four card.");
            CardColor chosenColor = ChooseWildCardColor();
            Console.WriteLine($"{playerName} chose the color: {chosenColor}");
            // Here you can handle the Wild Draw Four card with the chosen color.
        }
    }
    static CardColor ChooseWildCardColor()
    {
        Console.WriteLine("Choose a color for the Wild card:");
        Console.WriteLine("1. Red");
        Console.WriteLine("2. Blue");
        Console.WriteLine("3. Green");
        Console.WriteLine("4. Yellow");

        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
        {
            // Convert the player's choice to a CardColor enum value
            return (CardColor)(choice - 1);
        }
        else
        {
            Console.WriteLine("Invalid choice. Please choose a valid color.");
            return ChooseWildCardColor(); // Recursive call to ensure a valid choice
        }
    }
}
